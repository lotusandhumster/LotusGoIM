<template>
    <el-row>
        <el-col :span="19">
            <el-container class="message-container">
                <el-header style="height: 30px;">
                    <el-row>
                        <el-col :span="1">
                            <el-avatar fit="cover" shape="square" :size="40" :src="currentUserStore.currentGroup.avatarUrl"></el-avatar>
                        </el-col>
                        <el-col :span="22">
                            <el-text style="font-size:30px;" truncated>
                                {{ currentUserStore.currentGroup.name }}
                            </el-text>
                        </el-col>
                        <el-col :span="1" v-if="currentUserStore.currentGroup.owner===currentUserStore.currentUser.userId">
                            <el-tooltip
                                effect="light"
                                content="管理群"
                                placement="top-start"
                            >
                                <el-button type="text" @click="toSettingGroup">
                                    <el-icon size="40"><Setting /></el-icon>
                                </el-button>
                            </el-tooltip>
                        </el-col>
                    </el-row>
                </el-header>
                <el-main>
                    <el-scrollbar ref="scrollbarRef" @scroll="handleScroll">
                        <GroupMessage v-for="message in [...currentUserStore.groupMessages]" :message="message" :key="message.groupMessageId"/>
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
                            <el-input :disabled="loading" @keyup.enter="sendMessage" resize="none" type="textarea" maxlength="1000" show-word-limit :rows="2" v-model="groupMessage.content">
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
            <el-card class="group-info-card">
                <el-row>
                    <el-col :span="4"></el-col>
                    <el-col :span="16">
                        <el-avatar shape="square" :size="150" fit="cover" style="width: 150px;" v-if="currentUserStore.currentGroup.avatarUrl!=null" :src="currentUserStore.currentGroup.avatarUrl" />
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="6"><el-text>名称：</el-text></el-col>
                    <el-col :span="16"><el-text>{{ currentUserStore.currentGroup.name }}</el-text></el-col>
                </el-row>
                <el-row>
                    <el-col :span="6"><el-text>描述：</el-text></el-col>
                    <el-col :span="16"><el-text>{{ currentUserStore.currentGroup.description }}</el-text></el-col>
                </el-row>
                <el-row>
                    <GroupMemberComponent />
                </el-row>
                <el-row>
                    <el-col :span="15">
                    </el-col>
                    <el-col :span="5" v-if="currentUserStore.currentGroup.owner!==currentUserStore.currentUser.userId">
                        <el-button text @click="toQuitGroup">退出群聊</el-button>
                    </el-col>
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

    <el-drawer v-model="showSettingGroup" title="管理群" :with-header="false">
      <el-row>
          <el-col :span="10"></el-col>
          <el-col :span="5">
            <el-text>管理群</el-text>
          </el-col>
      </el-row>
      <el-divider></el-divider>
      <el-row>
          <el-col :span="5"></el-col>
          <el-col :span="5"><el-text>头像</el-text></el-col>
          <el-col :span="10">
              <el-upload
                  class="avatar-uploader"
                  action="/api/UploadFile"
                  :show-file-list="false"
                  :before-upload="beforeAvatarUpload"
                  :accept="['.jpg','.jpeg','.png','.gif','.bmp']"
                  :on-success="handleAvatarSuccess"
                  >
                  <img v-if="updateGroup.avatarUrl!=null" :src="updateGroup.avatarUrl" class="avatar" />
                  <el-icon v-else class="avatar-uploader-icon" ><Plus /></el-icon>
              </el-upload>
          </el-col>
      </el-row>
      <el-form
          ref="updateGroupRef"
          :model="updateGroup"
          :rules="updateGroupRules"
          label-width="120px"
          style="max-width: 500px; margin: auto;"
      >
          <el-form-item label="群名称" prop="name">
              <el-input v-model="updateGroup.name"></el-input>
          </el-form-item>
          <el-form-item label="描述" prop="description">
              <el-input v-model="updateGroup.description" type="textarea" show-word-limit maxlength="100"></el-input>
          </el-form-item>
          <el-form-item>
              <el-button type="primary" @click="submitForm" plain>修改</el-button>
              <el-button plain @click="resetForm">重置</el-button>
              <el-button plain type="danger" @click="toRemoveForm">解散群聊</el-button>
          </el-form-item>
      </el-form>
    </el-drawer>
</template>

<script setup lang="ts" name="GroupMessagePage">
import useCurrentUserStore from '../stores/currentUser'
import { getCurrentInstance, onMounted, ref, watch } from 'vue'
import { ElMessage, ElScrollbar, ElMessageBox } from 'element-plus'
import GroupMessage from '../components/GroupMessage.vue'
import { GroupMessage as GM, Group, GroupMessageModel, GroupMessageSearchFilter, GroupSearchFilter } from '../types'
import type { UploadProps } from 'element-plus'
import useGroupMessageApi from '../api/groupMessageApi'
import useClientUserApi from '../api/clientUserApi'
import GroupMemberComponent from '../components/GroupMemberComponent.vue'
import useGroupApi from '../api/groupApi'
import useOpenAIApi from '../api/openaiApi'

let that: any = getCurrentInstance()!.appContext.config.globalProperties
const currentUserStore = useCurrentUserStore()
that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token
const groupMessageApi = useGroupMessageApi(that)
const clientUserApi = useClientUserApi(that)
const groupApi = useGroupApi(that)
const openaiApi = useOpenAIApi(that)

let messageCount = 20
let groupMessageSearchFilter = {
    pageIndex: 1
} as GroupMessageSearchFilter

const scrollbarRef = ref<InstanceType<typeof ElScrollbar>>()

const showSettingGroup = ref(false)
const updateGroup = ref({
} as Group)
const updateGroupRef = ref()

const unreadCount = ref(0)

const groupMessage = ref({
    content: ''
} as GM)

const searchFilter: GroupMessageSearchFilter = {} as GroupMessageSearchFilter

const loading = ref(false)

const quickReplay = () => {
    const content = currentUserStore.groupMessages.findLast(m => m.userId != currentUserStore.currentUser.userId)?.content
    if (content){
        loading.value = true
        groupMessage.value.content = '正在生成智能回复'
        openaiApi.QuickReplyAsync(content).then(res => {
            groupMessage.value.content = res.message
            loading.value = false
        })
    }else{
        ElMessage.info("暂无可回复消息！")
    }    
}

const sendMessage = () => {
    groupMessage.value.content = groupMessage.value.content.trim()
    if (groupMessage.value.content === undefined || groupMessage.value.content === '') {
        ElMessage.error('消息不能为空')
        groupMessage.value.content = ''
        return
    }
    groupMessage.value.userId = currentUserStore.currentUser.userId
    groupMessage.value.groupId = currentUserStore.currentGroup.groupId
    groupMessage.value.type = 1
    
    currentUserStore.connection.invoke('SendGroupMessage', groupMessage.value).then(() => { 
        const groupMessageModel = {} as GroupMessageModel
        groupMessageModel.content = groupMessage.value.content
        groupMessageModel.userId = currentUserStore.currentUser.userId
        groupMessageModel.groupId = currentUserStore.currentGroup.groupId
        groupMessageModel.type = 1
        groupMessageModel.sendTime = new Date()
        groupMessageModel.avatarUrl = currentUserStore.currentUser.avatarUrl
        groupMessageModel.nickName = currentUserStore.currentUser.nickName
        currentUserStore.groupMessages.push(groupMessageModel)

        groupMessage.value.content = ''
        setTimeout(() => {
            scrollbarRef.value!.setScrollTop(1e16)
        }, 10)
    });
}

const handleSuccess: UploadProps['onSuccess'] = (response) => {
    groupMessage.value.userId = currentUserStore.currentUser.userId
    groupMessage.value.groupId = currentUserStore.currentGroup.groupId
    groupMessage.value.type = 2
    groupMessage.value.content = response
    
    currentUserStore.connection.invoke('SendGroupMessage', groupMessage.value).then(() => { 
        const groupMessageModel = {} as GroupMessageModel
        groupMessageModel.content = groupMessage.value.content
        groupMessageModel.userId = currentUserStore.currentUser.userId
        groupMessageModel.groupId = currentUserStore.currentGroup.groupId
        groupMessageModel.type = 2
        groupMessageModel.sendTime = new Date()
        groupMessageModel.avatarUrl = currentUserStore.currentUser.avatarUrl
        groupMessageModel.nickName = currentUserStore.currentUser.nickName
        currentUserStore.groupMessages.push(groupMessageModel)

        groupMessage.value.content = ''
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
    groupMessage.value.userId = currentUserStore.currentUser.userId
    groupMessage.value.groupId = currentUserStore.currentGroup.groupId
    groupMessage.value.type = 3
    groupMessage.value.fileUrl = response
    
    currentUserStore.connection.invoke('SendGroupMessage', groupMessage.value).then(() => { 
        const groupMessageModel = {} as GroupMessageModel
        groupMessageModel.content = groupMessage.value.content
        groupMessageModel.userId = currentUserStore.currentUser.userId
        groupMessageModel.groupId = currentUserStore.currentGroup.groupId
        groupMessageModel.type = 3
        groupMessageModel.sendTime = new Date()
        groupMessageModel.avatarUrl = currentUserStore.currentUser.avatarUrl
        groupMessageModel.nickName = currentUserStore.currentUser.nickName
        groupMessageModel.fileUrl = groupMessage.value.fileUrl
        console.log(groupMessageModel)
        currentUserStore.groupMessages.push(groupMessageModel)

        groupMessage.value.content = ''
        setTimeout(() => {
            scrollbarRef.value!.setScrollTop(1e16)
        }, 10)
    });
}

const beforeFileUpload = (file: File) => {
    groupMessage.value.content = file.name
    const isLt20M = file.size / 1024 / 1024 < 20
    if (!isLt20M) {
        ElMessage.error('上传文件大小不能超过 20MB!')
    }
    return isLt20M
}


watch(() => currentUserStore.currentGroup.groupId,
    () => {
        setTimeout(() => scrollbarRef.value!.setScrollTop(1e16), 100)
        getMessages()
    }
)

const getMessages = async () => {
    searchFilter.groupId = currentUserStore.currentGroup.groupId;

    try {
        const res = await groupMessageApi.getPageAsync(searchFilter);
        if (res.statusCode == 200) {
            currentUserStore.setGroupMessages(res.data.reverse());
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

onMounted(
    () => setTimeout(() => 
        scrollbarRef.value!.setScrollTop(1e16), 50)
)

currentUserStore.connection.on("ReceiveGroupMessage", (message: GM) => {
    if (scrollbarRef.value) {
        const scrollContainer = scrollbarRef.value.$el.querySelector('.el-scrollbar__wrap');
        const isBottom = scrollContainer.scrollHeight - scrollContainer.scrollTop <= scrollContainer.clientHeight;
        const groupMessageModel = {} as GroupMessageModel
        groupMessageModel.content = message.content
        groupMessageModel.userId = message.userId
        groupMessageModel.groupId = message.groupId
        groupMessageModel.type = message.type
        groupMessageModel.sendTime = new Date()
        groupMessageModel.fileUrl = message.fileUrl
        clientUserApi.getAsync(message.userId).then((res) => {
            groupMessageModel.avatarUrl = res.data.avatarUrl
            groupMessageModel.nickName = res.data.nickName
            groupMessageModel.email = res.data.email
            groupMessageModel.gender = res.data.gender
            groupMessageModel.description = res.data.description
            if (message.groupId === currentUserStore.currentGroup.groupId && message.userId !== currentUserStore.currentUser.userId){
                currentUserStore.groupMessages.push(groupMessageModel)
                if (isBottom) {
                    setTimeout(() => {
                            scrollbarRef.value!.setScrollTop(1e16)
                    }, 10)
                }else{
                    unreadCount.value ++
                }
            }
        })
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
            if (loadFlag && currentUserStore.groupMessages.length >= 15 &&
                messageCount - 15*groupMessageSearchFilter.pageIndex > 0
                ){
                loadFlag = false
                groupMessageSearchFilter.pageIndex += 1
                groupMessageApi.getPageAsync(groupMessageSearchFilter).then(res => {
                    messageCount = res.totalCount
                    currentUserStore.groupMessages.unshift(...res.data.reverse())
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

const toSettingGroup = () => {
    updateGroup.value = {...currentUserStore.currentGroup}
    showSettingGroup.value = true
}

const beforeAvatarUpload = (file: any) => {
  const isJPG = file.type === 'image/jpeg' || file.type === 'image/png' || file.type === 'image/gif' || file.type === 'image/bmp'
  if (!isJPG) {
    ElMessage.error('上传头像图片只能是 JPG、PNG、GIF、BMP 格式!')
  }
  const isLt2M = file.size / 1024 / 1024 < 2
  if (!isLt2M) {
    ElMessage.error('上传头像图片大小不能超过 2MB!')
  }
  return isJPG && isLt2M
}

const handleAvatarSuccess: UploadProps['onSuccess'] = (response) => {
  updateGroup.value.avatarUrl = response
  ElMessage.success('上传头像成功!')
}

const updateGroupRules = {
  name: [
    { required: true, message: '请输入群名称', trigger: 'blur' },
    { min: 1, max: 20, message: '长度在 1 到 20 个字符', trigger: 'blur' }
  ],
  description: [
    { max: 100, message: '不得超过100个字符', trigger: 'blur' }
  ]
}

const resetForm = () => {
  updateGroupRef.value.resetFields()
}

const submitForm = () => {
  updateGroupRef.value.validate((valid: boolean) => {
    if (valid) {
      groupApi.updateAsync(updateGroup.value).then(() => {
        currentUserStore.currentGroup.avatarUrl = updateGroup.value.avatarUrl
        currentUserStore.currentGroup.name = updateGroup.value.name
        currentUserStore.currentGroup.description = updateGroup.value.description
        showSettingGroup.value = false
        ElMessage.success('修改成功!')
      })
    } else {
      ElMessage.error('请检查输入是否正确!')
      return false
    }
  })
}

const toQuitGroup = () => {
    ElMessageBox.confirm(
        '确定退出该群聊？',
        '提示',
        {
            distinguishCancelAndClose: true,
            confirmButtonText: '是',
            cancelButtonText: '取消',
        }
    ).then(
        () => {
            currentUserStore.connection.invoke("QuitGroup", currentUserStore.currentGroup.groupId).then(
                () => {
                    ElMessage.success("退出成功")
                    currentUserStore.connection.invoke("LeaveGroup", currentUserStore.currentGroup.groupId)
                    loadNewGroups()
                    that.$router.push("/home")
                }
            )
        }
    )
}

const toRemoveForm = () => {
    ElMessageBox.confirm(
        '确定解散该群聊？',
        '提示',
        {
            distinguishCancelAndClose: true,
            confirmButtonText: '是',
            cancelButtonText: '取消',
        }
    ).then(
        () => {
            currentUserStore.connection.invoke("RemoveGroup", currentUserStore.currentGroup.groupId).then(
                () => {
                    ElMessage.success("解散成功")
                    currentUserStore.connection.invoke("LeaveGroup", currentUserStore.currentGroup.groupId)
                    loadNewGroups()
                    that.$router.push("/home")
                }
            )
        }
    )
}


const loadNewGroups = () => {
    const groupFilter = ref<GroupSearchFilter>({
        name: ''
    } as GroupSearchFilter)
    groupFilter.value.memberId = currentUserStore.currentUser.userId
    groupApi.getListAsync(groupFilter.value).then((res) => {
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

</script>

<style scoped>
.message-container {
height: 82vh;
}
.group-info-card {
height: 84vh;
}

.back-bottom {
    display: flex;
    position: fixed;
    bottom: 120px;
    right: 340px;
    opacity: 0.5;
}

.avatar-uploader .el-upload {
  border: 1px dashed var(--el-border-color);
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: var(--el-transition-duration-fast);
}
.avatar-uploader .el-upload:hover {
  border-color: var(--el-color-primary);
}
.el-icon.avatar-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 180px;
  height: 180px;
  text-align: center;
  border: 1px;
}
.avatar {
  width: 180px;
  height: 180px;
  display: block;
}

</style>