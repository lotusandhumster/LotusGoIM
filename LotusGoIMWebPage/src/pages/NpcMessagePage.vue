<template>
    <el-row>
        <el-col :span="19">
            <el-container class="message-container">
                <el-header style="height: 30px;">
                    <el-row>
                        <el-col :span="1">
                            <el-avatar fit="cover" shape="square" :size="40" :src="userAvatar"></el-avatar>
                        </el-col>
                        <el-col :span="20">
                            <el-text style="font-size:30px;">
                                &nbsp;&nbsp;AI助手Lotus
                            </el-text>
                        </el-col>
                    </el-row>
                </el-header>
                <el-main>
                    <el-scrollbar ref="scrollbarRef" @scroll="handleScroll">
                        <NpcMessage v-for="message in [...currentUserStore.chatGptMessages]" :message="message" :key="message.chatGptMessageId"/>
                        
                    </el-scrollbar>
                </el-main>
                <el-footer>
                    <el-row>
                        <el-col :span="22">
                            <el-input :disabled="loading" @keyup.enter="sendMessage" resize="none" type="textarea" maxlength="1000" show-word-limit :rows="2" v-model="prompt">
                            </el-input>
                        </el-col>
                        <el-col :span="2">
                            <el-button v-loading="loading" :disabled="loading" plain type="primary" style="height: 100%;width: 100%;" @click="sendMessage"><el-icon size="30px"><Position /></el-icon></el-button>
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
                        <el-avatar shape="square" :size="150" fit="cover" style="width: 150px;" :src="userAvatar" class="avatar" />
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="8"></el-col>
                    <el-col :span="16"><el-text>AI助手Lotus</el-text></el-col>
                </el-row>
                <br/>
                <el-row>
                    <el-col :span="6"><el-text>邮箱：</el-text></el-col>
                    <el-col :span="16"><el-text>1529185705@qq.com</el-text></el-col>
                </el-row>
                <br/>
                <el-row>
                    <el-col :span="6"><el-text>描述：</el-text></el-col>
                    <el-col :span="16"><el-text>有什么需要帮助的都可以找我！</el-text></el-col>
                </el-row>
                <br/>
                <el-row>
                    <el-col :span="6"></el-col>
                    <el-col :span="16"><el-button plain @click="clearAllMessage">清除所有对话</el-button></el-col>
                </el-row>
            </el-card>
        </el-col>
    </el-row>
    <div class="back-bottom" @click="toBottom" v-show="showUnread">
        <el-button type="primary" plain>
            <el-icon size="30"><ArrowDownBold /></el-icon>
        </el-button>
    </div>
</template>

<script setup lang="ts" name="NpcMessagePage">
import useCurrentUserStore from '../stores/currentUser';
import { getCurrentInstance, onMounted, ref, watch } from 'vue'
import { ElMessageBox, ElMessage, ElScrollbar } from 'element-plus'
import NpcMessage from '../components/NpcMessage.vue'
import { ChatGptMessageSearchFilter, ChatGptMessageWithUserModel } from '../types'
import useChatGptMessageApi from '../api/chatGptMessageApi'
import useOpenAIApi from '../api/openaiApi';

let that: any = getCurrentInstance()!.appContext.config.globalProperties
const currentUserStore = useCurrentUserStore()
that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token
const chatGptMessageApi = useChatGptMessageApi(that)
const openaiApi = useOpenAIApi(that)

const loading = ref(false)

const userAvatar = new URL('../assets/images/robot.jpg', import.meta.url).href
const showUnread = ref(false)
let messageCount = 20
let chatGptMessageSearchFilter = {
    pageIndex: 1,
    userId: currentUserStore.currentUser.userId,
    isDeleted: false,
} as ChatGptMessageSearchFilter

const scrollbarRef = ref<InstanceType<typeof ElScrollbar>>()

const unreadCount = ref(0)

const prompt = ref('')

const sendMessage = () => {
    prompt.value = prompt.value.trim()
    if (prompt.value === '') {
        ElMessage.error('消息不能为空')
        prompt.value
        return
    }
    const userMessage: ChatGptMessageWithUserModel = {
        nickName: currentUserStore.currentUser.nickName,
        email: currentUserStore.currentUser.email,
        avatarUrl: currentUserStore.currentUser.avatarUrl,
        userId: currentUserStore.currentUser.userId,
        role: 'user',
        sendTime: new Date(),
        content: prompt.value
    } as ChatGptMessageWithUserModel
    currentUserStore.chatGptMessages.push(userMessage)
    loading.value = true
    openaiApi.chatWithHistoryAsync(prompt.value).then((res) => {
        const systemMessage: ChatGptMessageWithUserModel = {
            nickName: 'AI助手Lotus',
            email: '1529185705@qq.com',
            avatarUrl: userAvatar,
            role: 'system',
            sendTime: new Date(),
            content: res.data
        } as ChatGptMessageWithUserModel
        currentUserStore.chatGptMessages.push(systemMessage)
        loading.value = false
        prompt.value = ''
    })
    prompt.value = 'AI助手正在作答'
}

onMounted(
    () => {
        setTimeout(() => scrollbarRef.value!.setScrollTop(1e16), 100)
    }
)

const clearAllMessage = () => {
    ElMessageBox.confirm('确定清除所有对话吗？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
    }).then(() => {
        currentUserStore.chatGptMessages = []
        chatGptMessageApi.clearAllMessageAsync(currentUserStore.currentUser.userId).then(() => {
            ElMessage.success('清除成功')
        })
    }).catch(() => {
        ElMessage.info('已取消清除')
    })
}

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
            if (currentUserStore.chatGptMessages.length >= 15 &&
                messageCount - 15*chatGptMessageSearchFilter.pageIndex > 0
                ){
                chatGptMessageSearchFilter.pageIndex += 1
                chatGptMessageApi.getPageAsync(chatGptMessageSearchFilter).then(res => {
                    messageCount = res.totalCount
                    currentUserStore.chatGptMessages.unshift(...res.data.reverse())
                })
            }
        }
    }
}

const toBottom = () => {
    scrollbarRef.value!.setScrollTop(1e16)
    unreadCount.value = 0
}

watch(() => currentUserStore.chatGptMessages.length,
 () => {
    setTimeout(() => scrollbarRef.value!.setScrollTop(1e16), 200)
 }
)

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