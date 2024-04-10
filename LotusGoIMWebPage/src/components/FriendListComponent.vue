<template>
    <el-scrollbar height="85vh">
        <el-input v-model="filter.remark" placeholder="查找" clearable @input="toSearch">
        </el-input>
        <FriendComponent v-for="friend in currentUserStore.friends" :friend="friend" v-if="filter.remark!=''" :key="friend.friendId"/>
        <el-collapse v-else v-model="defaultOpen">
            <el-collapse-item title="特别关心" name="0">
                <FriendComponent v-for="friend in currentUserStore.followFriends" :friend="friend" :key="friend.friendId" />
            </el-collapse-item>
            <el-collapse-item title="好友" name="1">
                <FriendComponent v-for="friend in currentUserStore.commonFriends" :friend="friend" :key="friend.friendId" />
            </el-collapse-item>
            <el-collapse-item title="黑名单" name="2">
                <FriendComponent v-for="friend in currentUserStore.blacklistFriends" :friend="friend" :key="friend.friendId" />
            </el-collapse-item>
        </el-collapse>
    </el-scrollbar> 
</template>
<script lang="ts" setup>
import FriendComponent from './FriendComponent.vue';
import { getCurrentInstance, ref, onMounted } from 'vue'
import useFriendApi from '../api/friendApi';
import useCurrentUserStore from '../stores/currentUser';
import { FriendSearchFilter } from '../types';
import { ElMessage } from 'element-plus';

const defaultOpen = ref<string[]>(['0', '1'])

let that: any = getCurrentInstance()!.appContext.config.globalProperties
const currentUserStore = useCurrentUserStore()
that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token
const friendApi = useFriendApi(that)

const filter = ref<FriendSearchFilter>({
    remark: ''
} as FriendSearchFilter)

const loadFriends = () => {
    filter.value.userId = currentUserStore.currentUser.userId
    friendApi.getListAsync(filter.value).then((res) => {
        if (res.statusCode === 200) {
            interface FriendUnreadDict {
                [key: string]: number
            }
            let friendUnreadDict: FriendUnreadDict = {}
            for (let i=0; i<currentUserStore.friends.length; i++){
                friendUnreadDict[currentUserStore.friends[i].friendId] = currentUserStore.friends[i].unread
            }
            currentUserStore.friends = res.data
            for (let i=0; i<currentUserStore.friends.length; i++){
                currentUserStore.friends[i].unread = friendUnreadDict[currentUserStore.friends[i].friendId] || 0
            }
            currentUserStore.followFriends= currentUserStore.friends.filter(f => f.type === 0)
            currentUserStore.commonFriends = currentUserStore.friends.filter(f => f.type === 1)
            currentUserStore.blacklistFriends = currentUserStore.friends.filter(f => f.type === 2)
        }else{
            ElMessage.error('好友列表获取失败！')
        }
    })
}

onMounted(() => {
    setTimeout(() => {
        loadFriends()
    }, 20)
})

const toSearch = () => {
    currentUserStore.friends = []
    loadFriends()
}

</script>
