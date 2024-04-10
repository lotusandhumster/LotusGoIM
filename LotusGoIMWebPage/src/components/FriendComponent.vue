<template>
    <el-card class="friend-info-div" shadow="hover" :body-style="{padding: 0}">
        <el-row>
            <el-col :span="6">
                <div class="avatar-div" @click="showFirendInfo=true">
                    <el-avatar shape="square" class="friend-avatar" :src="friend.avatarUrl"/>
                </div>
            </el-col>
            <el-col :span="16">
                <div class="info-div" @click="openMessage">
                    <div>
                        <el-text class="friend-name" truncated>
                            <el-icon v-if="friend.state===0" style="color:green"><Bell /></el-icon>
                            <el-icon v-if="friend.state===1" style="color:orange"><WarningFilled /></el-icon>
                            <el-icon v-if="friend.state===2" style="color:cadetblue"><Clock /></el-icon>
                            <el-icon v-if="friend.state==3"><CircleCloseFilled /></el-icon>
                            <b>{{ friend.remark!=''?friend.remark??friend.nickName : friend.nickName }}</b>
                        </el-text>
                    </div>
                    <div>
                        <el-popover
                            placement="right"
                            :title="friend.nickName"
                            :width="200"
                            trigger="hover"
                            :content="friend.description"
                        >
                            <template #reference>
                                <el-text class="friend-description" truncated>{{ friend.description }}</el-text>
                            </template>
                        </el-popover>
                    </div>
                </div>
            </el-col>
            <el-col :span="2">
                <div>
                    <el-badge :value="friend.unread" :hidden="friend.unread==0" max></el-badge>
                </div>
                <div style="margin-left: -20px;margin-top: -7px;">
                    <el-tooltip
                        content="隐藏对话"
                        effect="light"
                    >
                        <el-button text plain type="primary" @click="hiddenFriend">
                            <el-icon size="20"><View /></el-icon>
                        </el-button>
                    </el-tooltip>
                </div>
            </el-col>
        </el-row>
    </el-card>
    <el-dialog
        title="好友信息"
        v-model="showFirendInfo"
        width="30%"
        :before-close="() => showFirendInfo=false"
    >
        <el-row>
            <el-col :span="8">
                <el-avatar shape="square" class="show-info-avatar" :src="friend.avatarUrl"/>
                <br/>
                <el-text v-if="friend.state===0" style="color:green"><el-icon><Bell /></el-icon>在线</el-text>
                <el-text v-if="friend.state===1" style="color:orange"><el-icon><WarningFilled /></el-icon>繁忙</el-text>
                <el-text v-if="friend.state===2" style="color:cadetblue"><el-icon><Clock /></el-icon>离开</el-text>
                <el-text v-if="friend.state==3"><el-icon><CircleCloseFilled /></el-icon>离线</el-text><br/>
            </el-col>
            <el-col :span="16">
                <el-row>
                    <el-col :span="4">昵称：</el-col>
                    <el-col :span="16">{{ friend.nickName }}</el-col>
                </el-row>
                <el-row>
                    <el-col :span="4">备注：</el-col>
                    <el-col :span="16">{{ friend.remark }}
                    &nbsp;<el-icon color="blue" style="cursor: pointer;" @click="showEdit"><Edit></Edit></el-icon></el-col>
                </el-row>
                <el-row>
                    <el-col :span="4">性别：</el-col>
                    <el-col :span="16">
                        <el-icon v-if="friend.gender===1" color="blue"><Male /></el-icon>
                        <el-icon v-else-if="friend.gender===2" color="pink"><Female /></el-icon>
                        <el-icon v-else color="green">?</el-icon>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="4">关系：</el-col>
                    <el-col :span="16">
                        <el-radio-group v-model="friend.type" @change="changeType">
                            <el-radio :label="1">朋友</el-radio>
                            <el-radio :label="0">特别关心</el-radio>
                            <el-radio :label="2">黑名单</el-radio>
                        </el-radio-group>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="4">描述：</el-col>
                    <el-col :span="16">{{ friend.description }}</el-col>
                </el-row>
            </el-col>
        </el-row>   
    </el-dialog>
</template>

<script lang="ts" setup>
import { ref, defineProps, getCurrentInstance } from 'vue'
import { FriendModel } from '../types'
import useCurrentUserStore from '../stores/currentUser'
import useFriendApi from '../api/friendApi';
import { ElMessageBox, ElMessage } from 'element-plus'
import { PrivateMessageSearchFilter, FriendSearchFilter } from '../types';
import usePrivateMessageApi from '../api/privateMessageApi';

let that: any = getCurrentInstance()!.appContext.config.globalProperties
const currentUserStore = useCurrentUserStore()
that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token
const friendApi = useFriendApi(that)
const privateMessageApi = usePrivateMessageApi(that)
const searchFilter: PrivateMessageSearchFilter = {} as PrivateMessageSearchFilter

const props = defineProps<{
    friend: FriendModel
}>()

const friend = ref(props.friend)

const showFirendInfo = ref(false)

const openMessage = () => {
    currentUserStore.currentFriend = friend.value
    friend.value.unread = 0
    for(let i = 0; i < currentUserStore.friends.length; i++){
        if(currentUserStore.friends[i].friendId == friend.value.friendId){
            currentUserStore.friends[i].unread = 0
        }
    }
    getMessages()
    that.$router.push('/privateMessage')
}

const getMessages = async () => {
    searchFilter.senderId = currentUserStore.currentUser.userId;
    searchFilter.receiverId = currentUserStore.currentFriend.friendUserId;

    try {
        const res = await privateMessageApi.getPageAsync(searchFilter);
        if (res.statusCode == 200) {
            currentUserStore.setPrivateMessages(res.data.reverse());
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

currentUserStore.connection.on("ReceivePrivateMessage", (message) => {
    if ((message.senderId != currentUserStore.currentFriend.friendUserId || currentUserStore.messageMenu!='0')
        && message.senderId == friend.value.friendUserId) {
        friend.value.unread += 1
        for (let i=0; i<currentUserStore.friends.length; i++){
            if (currentUserStore.friends[i].friendId == friend.value.friendId){
                currentUserStore.friends[i].unread = friend.value.unread
            }
        }
    }
})

const showEdit = () => {
    ElMessageBox.prompt('请输入备注', '修改备注', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
    }).then(({ value }) => {
        friend.value.remark = value
        friendApi.updateAsync(friend.value).then((res) => {
            if (res.statusCode === 200) {
                ElMessage.success('修改备注成功')
            } else {
                ElMessage.error(res.message)
            }
        })
    }).catch(() => {
        ElMessage.info('取消修改备注')
    })
}

const changeType = () => {
    friendApi.updateAsync(friend.value).then((res) => {
        if (res.statusCode === 200) {
            ElMessage.success('修改关系成功')
            for (let i=0; i<currentUserStore.friends.length; i++){
                if (currentUserStore.friends[i].friendUserId === friend.value.friendUserId){
                    currentUserStore.friends[i].type = friend.value.type
                }
            }
            currentUserStore.followFriends= currentUserStore.friends.filter(f => f.type === 0)
            currentUserStore.commonFriends = currentUserStore.friends.filter(f => f.type === 1)
            currentUserStore.blacklistFriends = currentUserStore.friends.filter(f => f.type === 2)
        } else {
            ElMessage.error(res.message)
        }
    })
}

const hiddenFriend = () => {
    ElMessageBox.confirm(
        '确定隐藏此对话？',
        '提示',
        {
            distinguishCancelAndClose: true,
            confirmButtonText: '是',
            cancelButtonText: '取消',
        }
    )
    .then(() => {
        friendApi.deleteAsync(friend.value.friendId).then(
          () => loadFriends()
        )
        that.$router.push("/home")
    })
}

const loadFriends = () => {
    const filter = ref<FriendSearchFilter>({
      remark: ''
    } as FriendSearchFilter)
    filter.value.userId = currentUserStore.currentUser.userId
    friendApi.getListAsync(filter.value).then((res) => {
        if (res.statusCode === 200) {
            currentUserStore.friends = res.data
            for (let i=0; i<currentUserStore.friends.length; i++){
                currentUserStore.friends[i].unread = 0
            }
            currentUserStore.followFriends= currentUserStore.friends.filter(f => f.type === 0)
            currentUserStore.commonFriends = currentUserStore.friends.filter(f => f.type === 1)
            currentUserStore.blacklistFriends = currentUserStore.friends.filter(f => f.type === 2)
        }else{
            ElMessage.error('好友列表获取失败！')
        }
    })
}

</script>

<style scoped>
.friend-avatar {
    float: left;
}
.info-avatar {
    float: left;
    cursor: pointer;
}

.friend-description {
    width: 7vw;
    margin-left: 10px;
    margin-top: -20px;
}
.friend-info-div {
    display: flex;
    border-bottom: 1px solid var(--el-color-info-light-8);
    cursor: pointer;
    padding: 3px;
    padding-bottom: 0px;
}
.show-info-avatar {
    width: 100px;
    height: 100px;
}

.avatar-div {
    margin-top: 5px;
}
</style>