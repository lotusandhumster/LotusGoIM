<template>
    <el-scrollbar height="85vh">
        <el-input v-model="filter.name" placeholder="查找" clearable @input="toSearch">
        </el-input>
        <GroupComponent v-for="group in currentUserStore.groupList" :group="group" v-if="filter.name!=''" :key="group.groupId"/>
        <el-collapse v-else v-model="defaultOpen">
            <el-collapse-item title="我创建的群" name="0">
                <GroupComponent v-for="group in currentUserStore.createdGroupList" :group="group" :key="group.groupId"/>
            </el-collapse-item>
            <el-collapse-item title="我加入的群" name="1">
                <GroupComponent v-for="group in currentUserStore.joinGroupList" :group="group" :key="group.groupId" />
            </el-collapse-item>
        </el-collapse>
    </el-scrollbar> 
</template>
<script lang="ts" setup>
import GroupComponent from './GroupComponent.vue';
import { getCurrentInstance, ref, onMounted } from 'vue'
import useGroupApi from '../api/groupApi';
import useCurrentUserStore from '../stores/currentUser';
import { GroupSearchFilter } from '../types';
import { ElMessage } from 'element-plus';

const defaultOpen = ref<string[]>(['0', '1'])

let that: any = getCurrentInstance()!.appContext.config.globalProperties
const currentUserStore = useCurrentUserStore()
that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token

const groupApi = useGroupApi(that)

const filter = ref<GroupSearchFilter>({
    name: ''
} as GroupSearchFilter)

const loadGroups = () => {
    filter.value.memberId = currentUserStore.currentUser.userId
    groupApi.getListAsync(filter.value).then((res) => {
        if (res.statusCode === 200) {
            interface GroupUnreadDict {
                [key: string]: number
            }
            let groupUnreadDict: GroupUnreadDict = {}
            for (let i=0; i<currentUserStore.groupList.length; i++){
                groupUnreadDict[currentUserStore.groupList[i].groupId] = currentUserStore.groupList[i].unread
            }
            currentUserStore.groupList = res.data
            for (let i=0; i<currentUserStore.groupList.length; i++){
                currentUserStore.groupList[i].unread = groupUnreadDict[currentUserStore.groupList[i].groupId] || 0
            }
            currentUserStore.joinGroupList= currentUserStore.groupList.filter(g => g.owner !== currentUserStore.currentUser.userId)
            currentUserStore.createdGroupList = currentUserStore.groupList.filter(g => g.owner === currentUserStore.currentUser.userId)
        }else{
            ElMessage.error('群列表获取失败！')
        }
    })
}

onMounted(() => {
    setTimeout(() => {
        loadGroups()
    }, 20)
})

const toSearch = () => {
    currentUserStore.groupList = []
    loadGroups()
}

</script>
