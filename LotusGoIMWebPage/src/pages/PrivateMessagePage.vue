<template>
    <el-row>
        <el-col :span="19">
            <el-container class="message-container">
                <el-header style="height: 30px;">
                    <el-row>
                        <el-col :span="1">
                            <el-avatar fit="cover" shape="square" :size="40" :src="currentUserStore.currentFriend.avatarUrl"></el-avatar>
                        </el-col>
                        <el-col :span="20">
                            <el-text style="font-size:30px;" truncated v-if="currentUserStore.currentFriend.remark">
                                {{ currentUserStore.currentFriend.remark }} ({{ currentUserStore.currentFriend.nickName }})
                            </el-text>
                            <el-text style="font-size:30px;" truncated v-else>
                                {{ currentUserStore.currentFriend.nickName }}
                            </el-text>
                        </el-col>
                    </el-row>
                </el-header>
                <el-main>
                    <el-scrollbar ref="scrollbarRef" @scroll="handleScroll">
                        <PrivateMessage v-for="message in [...currentUserStore.privateMessages]" :message="message" :key="message.privateMessageId"/>
                    </el-scrollbar>
                </el-main>
                <el-footer>
                    <el-row>
                        <el-col :span="1">
                            <el-upload
                                action="/api/UploadFile"
                                :show-file-list="false"
                                :before-upload="beforeUpload"
                                :accept="['.jpg','.jpeg','.png','.gif','.bmp']"
                                :on-success="handleSuccess"
                                >
                                <template #trigger>
                                <el-button type="success" size="small" plain><el-icon><Picture /></el-icon></el-button>
                                </template>
                            </el-upload>
                        </el-col>
                        <el-col :span="1">
                            <el-upload
                                action="/api/UploadFile"
                                :show-file-list="false"
                                :before-upload="beforeFileUpload"
                                :on-success="handleFileSuccess"
                                >
                                <template #trigger>
                                <el-button type="primary" plain size="small"><el-icon><Document /></el-icon></el-button>
                                </template>
                            </el-upload>
                        </el-col>
                        <el-col :span="2">
                            <el-tooltip
                                effect="light"
                                content="智能回复"
                                placement="top"
                            >
                                <el-button @click="quickReplay" style="height:24px; width: 35px; margin-top:-1px;" plain><el-icon><ChatDotSquare /></el-icon></el-button>
                            </el-tooltip>
                        </el-col>
                        <el-col :span="20"></el-col>
                    </el-row>
                    <el-row>
                        <el-col :span="22">
                            <el-input :disabled="loading" @keyup.enter="sendMessage" resize="none" type="textarea" maxlength="1000" show-word-limit :rows="2" v-model="privateMessage.content">
                            </el-input>
                        </el-col>
                        <el-col :span="2">
                            <el-button v-loading="loading" :disabled="loading"  plain type="primary" style="height: 100%;width: 100%;" @click="sendMessage"><el-icon size="30px"><Position /></el-icon></el-button>
                        </el-col>
                    </el-row>
                </el-footer>
            </el-container>
        </el-col>
        <el-col :span="5">
            <el-card class="friend-info-card">
                <el-row>
                    <el-col :span="4"></el-col>
                    <el-col :span="16">
                        <el-avatar shape="square" :size="150" fit="cover" style="width: 150px;" v-if="currentUserStore.currentFriend.avatarUrl!=null" :src="currentUserStore.currentFriend.avatarUrl" class="avatar" />
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="6"><el-text>昵称：</el-text></el-col>
                    <el-col :span="16"><el-text>{{ currentUserStore.currentFriend.nickName }}</el-text></el-col>
                </el-row>
                <el-row>
                    <el-col :span="6"><el-text>备注：</el-text></el-col>
                    <el-col :span="16"><el-text>{{ currentUserStore.currentFriend.remark }}</el-text>
                    &nbsp;<el-icon color="blue" style="cursor: pointer;" @click="showEdit"><Edit></Edit></el-icon></el-col>
                </el-row>
                <el-row>
                    <el-col :span="6"><el-text>邮箱：</el-text></el-col>
                    <el-col :span="16"><el-text>{{ currentUserStore.currentFriend.email }}</el-text></el-col>
                </el-row>
                <el-row>
                    <el-col :span="6"><el-text>性别：</el-text></el-col>
                    <el-col :span="16">
                        <el-icon v-if="currentUserStore.currentFriend.gender===1" color="blue"><Male /></el-icon>
                        <el-icon v-else-if="currentUserStore.currentFriend.gender===2" color="pink"><Female /></el-icon>
                        <el-icon v-else color="green">?</el-icon>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="6"><el-text>关系：</el-text></el-col>
                    <el-col :span="16">
                        <el-radio-group v-model="currentUserStore.currentFriend.type" @change="changeType">
                            <el-radio :label="1">朋友</el-radio>
                            <el-radio :label="0">特别关心</el-radio>
                            <el-radio :label="2">黑名单</el-radio>
                        </el-radio-group>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="6"><el-text>描述：</el-text></el-col>
                    <el-col :span="16"><el-text>{{ currentUserStore.currentFriend.description }}</el-text></el-col>
                </el-row>
            </el-card>
        </el-col>
    </el-row>

    <div class="back-bottom" @click="toBottom" v-show="showUnread">
        <el-badge :value="unreadCount" :hidden="unreadCount==0" max>
            <el-button type="primary" plain>
                <el-icon size="30"><ArrowDownBold /></el-icon>
            </el-button>
        </el-badge>
    </div>
</template>

<script setup lang="ts" name="PrivateMessagePage">
import useCurrentUserStore from '../stores/currentUser';
import { getCurrentInstance, onMounted, ref, watch } from 'vue'
import useFriendApi from '../api/friendApi';
import { ElMessageBox, ElMessage, ElScrollbar } from 'element-plus'
import PrivateMessage from '../components/PrivateMessage.vue'
import { PrivateMessage as PM, PrivateMessageModel, PrivateMessageSearchFilter } from '../types';
import type { UploadProps } from 'element-plus'
import usePrivateMessageApi from '../api/privateMessageApi';
import useOpenAIApi from '../api/openaiApi';

let that: any = getCurrentInstance()!.appContext.config.globalProperties
const currentUserStore = useCurrentUserStore()
that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token
const friendApi = useFriendApi(that)
const privateMessageApi = usePrivateMessageApi(that)
const openaiApi = useOpenAIApi(that)

let messageCount = 20
let privateMessageSearchFilter = {
    pageIndex: 1
} as PrivateMessageSearchFilter

const loading = ref(false)

const scrollbarRef = ref<InstanceType<typeof ElScrollbar>>()

const unreadCount = ref(0)

const privateMessage = ref({
    content: ''
} as PM)

const showEdit = () => {
    ElMessageBox.prompt('请输入备注', '修改备注', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
    }).then(({ value }) => {
        currentUserStore.currentFriend.remark = value
        friendApi.updateAsync(currentUserStore.currentFriend).then((res) => {
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

const quickReplay = () => {
    const content = currentUserStore.privateMessages.findLast(m => m.senderId != currentUserStore.currentUser.userId)?.content
    if (content){
        loading.value = true
        privateMessage.value.content = '正在生成智能回复'
        openaiApi.QuickReplyAsync(content).then(res => {
            privateMessage.value.content = res.message
            loading.value = false
        })
    }else{
        ElMessage.info("暂无可回复消息！")
    }    
}

const changeType = () => {
    friendApi.updateAsync(currentUserStore.currentFriend).then((res) => {
        if (res.statusCode === 200) {
            ElMessage.success('修改关系成功')
            for (let i=0; i<currentUserStore.friends.length; i++){
                if (currentUserStore.friends[i].friendUserId === currentUserStore.currentFriend.friendUserId){
                    currentUserStore.friends[i].type = currentUserStore.currentFriend.type
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

const sendMessage = () => {
    privateMessage.value.content = privateMessage.value.content.trim()
    if (privateMessage.value.content === undefined || privateMessage.value.content === '') {
        ElMessage.error('消息不能为空')
        privateMessage.value.content = ''
        return
    }
    privateMessage.value.senderId = currentUserStore.currentUser.userId
    privateMessage.value.receiverId = currentUserStore.currentFriend.friendUserId
    privateMessage.value.type = 1
    
    currentUserStore.connection.invoke('SendPrivateMessage', privateMessage.value).then(() => { 
        const privateMessageModel = {} as PrivateMessageModel
        privateMessageModel.content = privateMessage.value.content
        privateMessageModel.senderId = currentUserStore.currentUser.userId
        privateMessageModel.receiverId = currentUserStore.currentFriend.friendUserId
        privateMessageModel.type = 1
        privateMessageModel.sendTime = new Date()
        privateMessageModel.avatarUrl = currentUserStore.currentUser.avatarUrl
        privateMessageModel.nickName = currentUserStore.currentUser.nickName
        currentUserStore.privateMessages.push(privateMessageModel)

        privateMessage.value.content = ''
        setTimeout(() => {
            scrollbarRef.value!.setScrollTop(1e16)
        }, 10)
    });
}

const handleSuccess: UploadProps['onSuccess'] = (response) => {
    privateMessage.value.senderId = currentUserStore.currentUser.userId
    privateMessage.value.receiverId = currentUserStore.currentFriend.friendUserId
    privateMessage.value.type = 2
    privateMessage.value.content = response
    
    currentUserStore.connection.invoke('SendPrivateMessage', privateMessage.value).then(() => { 
        const privateMessageModel = {} as PrivateMessageModel
        privateMessageModel.content = privateMessage.value.content
        privateMessageModel.senderId = currentUserStore.currentUser.userId
        privateMessageModel.receiverId = currentUserStore.currentFriend.friendUserId
        privateMessageModel.type = 2
        privateMessageModel.sendTime = new Date()
        privateMessageModel.avatarUrl = currentUserStore.currentUser.avatarUrl
        privateMessageModel.nickName = currentUserStore.currentUser.nickName
        currentUserStore.privateMessages.push(privateMessageModel)

        privateMessage.value.content = ''
        setTimeout(() => {
            scrollbarRef.value!.setScrollTop(1e16)
        }, 10)
    });
}

const beforeUpload = (file: any) => {
  const isJPG = file.type === 'image/jpeg' || file.type === 'image/png' || file.type === 'image/gif' || file.type === 'image/bmp'
  if (!isJPG) {
    ElMessage.error('上传图片只能是 JPG、PNG、GIF、BMP 格式!')
  }
  const isLt2M = file.size / 1024 / 1024 < 2
  if (!isLt2M) {
    ElMessage.error('上传图片大小不能超过 2MB!')
  }
  return isJPG && isLt2M
}

const showUnread = ref(false)

const handleFileSuccess: UploadProps['onSuccess'] = (response) => {
    privateMessage.value.senderId = currentUserStore.currentUser.userId
    privateMessage.value.receiverId = currentUserStore.currentFriend.friendUserId
    privateMessage.value.type = 3
    privateMessage.value.fileUrl = response
    
    currentUserStore.connection.invoke('SendPrivateMessage', privateMessage.value).then(() => { 
        const privateMessageModel = {} as PrivateMessageModel
        privateMessageModel.content = privateMessage.value.content
        privateMessageModel.senderId = currentUserStore.currentUser.userId
        privateMessageModel.receiverId = currentUserStore.currentFriend.friendUserId
        privateMessageModel.type = 3
        privateMessageModel.sendTime = new Date()
        privateMessageModel.avatarUrl = currentUserStore.currentUser.avatarUrl
        privateMessageModel.nickName = currentUserStore.currentUser.nickName
        privateMessageModel.fileUrl = privateMessage.value.fileUrl
        console.log(privateMessageModel)
        currentUserStore.privateMessages.push(privateMessageModel)

        privateMessage.value.content = ''
        setTimeout(() => {
            scrollbarRef.value!.setScrollTop(1e16)
        }, 10)
    });
}

const beforeFileUpload = (file: File) => {
    privateMessage.value.content = file.name
    const isLt20M = file.size / 1024 / 1024 < 20
    if (!isLt20M) {
        ElMessage.error('上传文件大小不能超过 20MB!')
    }
    return isLt20M
}


watch(() => currentUserStore.currentFriend.friendUserId,
 () => {
    setTimeout(() => scrollbarRef.value!.setScrollTop(1e16), 200)
 }
)

onMounted(
    () => setTimeout(() => 
        scrollbarRef.value!.setScrollTop(1e16), 100)
)

currentUserStore.connection.on("ReceivePrivateMessage", (message) => {
    if (scrollbarRef.value) {
        const scrollContainer = scrollbarRef.value.$el.querySelector('.el-scrollbar__wrap');
        const isBottom = scrollContainer.scrollHeight - scrollContainer.scrollTop <= scrollContainer.clientHeight;
        const privateMessageModel = {} as PrivateMessageModel
        if (message.senderId === currentUserStore.currentFriend.friendUserId) {
            privateMessageModel.content = message.content
            privateMessageModel.senderId = message.senderId
            privateMessageModel.receiverId = message.receiverId
            privateMessageModel.type = message.type
            privateMessageModel.sendTime = new Date()
            privateMessageModel.avatarUrl = currentUserStore.currentFriend.avatarUrl
            privateMessageModel.nickName = currentUserStore.currentFriend.nickName
        } else {

        }
        if (isBottom) {
            setTimeout(() => {
                    scrollbarRef.value!.setScrollTop(1e16)
            }, 10)
        }else{
            unreadCount.value ++
        }
        if (message.senderId === currentUserStore.currentFriend.friendUserId){
            currentUserStore.privateMessages.push(privateMessageModel)
        }
    }
});

let loadFlag = true
const handleScroll = () => {
    if (scrollbarRef.value) {
        const scrollContainer = scrollbarRef.value.$el.querySelector('.el-scrollbar__wrap');
        const isBottom = scrollContainer.scrollHeight - scrollContainer.scrollTop <= scrollContainer.clientHeight;
        if (!isBottom){
            showUnread.value = true
        }else{
            showUnread.value = false
            unreadCount.value = 0
        }

        const isTop = scrollContainer.scrollTop < 100
        if (isTop){
            if (loadFlag && currentUserStore.privateMessages.length >= 15 &&
                messageCount - 15*privateMessageSearchFilter.pageIndex > 0
                ){
                loadFlag = false
                privateMessageSearchFilter.pageIndex += 1
                privateMessageApi.getPageAsync(privateMessageSearchFilter).then(res => {
                    messageCount = res.totalCount
                    currentUserStore.privateMessages.unshift(...res.data.reverse())
                    loadFlag = true
                })
            }
        }
    }
}

const toBottom = () => {
    scrollbarRef.value!.setScrollTop(1e16)
    unreadCount.value = 0
}

</script>

<style scoped>
.message-container {
height: 82vh;
}
.friend-info-card {
height: 84vh;
}

.back-bottom {
    display: flex;
    position: fixed;
    bottom: 120px;
    right: 340px;
    opacity: 0.5;
}

</style>