<template>
   <el-container class="container">
      <el-header class="container-header">
        <el-row>
          <el-col :span="2">
            <el-text>Lotus Go</el-text>
          </el-col>
          <el-col :span="2">
            <el-button text @click="showSearchUser=true" plain><el-icon size="20"><Search /></el-icon>&nbsp;查找好友</el-button>
          </el-col>
          <el-col :span="3">
            <el-button text @click="showCreateGroup=true" plain><el-icon size="20"><ChatDotRound /></el-icon>&nbsp;创建群聊</el-button>
          </el-col>
          <el-col :span="3">
            <el-button text @click="toAI" plain><el-icon size="20"><Reading /></el-icon>&nbsp;AI助手</el-button>
          </el-col>
          <el-col :span="3">
            <el-button text @click="toAdmin" plain v-if="currentUserStore.currentUser.userId==1"><el-icon size="20"><Postcard /></el-icon>&nbsp;管理员模式</el-button>
          </el-col>
          <el-col :span="8"></el-col>
          <el-col :span="2">
            <div style="float: left; margin-top: 10px;">
                <el-icon @click="toggleDark()" size="30px">
                    <Sunny v-if="!isDark" class="theme-sunny"/>
                    <Moon v-else/>
                </el-icon>
            </div>&nbsp;
            <el-text size="large" truncated>
                {{ currentUser.nickName }}
            </el-text>
          </el-col>
          <el-col :span="1">
            <el-dropdown>
              <div class="header-avatar">
                <el-avatar shape="square" :size="50" :src="currentUser.avatarUrl" />
              </div>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item @click="infoDrawer=true">个人信息</el-dropdown-item>
                  <el-dropdown-item @click="showUpdatePassword=true">修改密码</el-dropdown-item>
                  <el-dropdown-item divided @click="logout()">退出登录</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </el-col>
        </el-row>
      </el-header>
      <el-container>
        <el-aside width="200px" class="container-aside">
          <div>
            <el-menu
              :default-active="currentUserStore.messageMenu"
              mode="horizontal"
              :ellipsis="false"
              style="height: 4vh;"
              @select="changeSessageMenu"
            >
              <el-menu-item index="0" style="width: 50%;">
                好友
              </el-menu-item>
              <el-menu-item index="1" style="width: 50%">
                群聊
              </el-menu-item>
            </el-menu>
            <component :is="currentUserStore.messageMenu == '0' ? FriendList : GroupList" />
          </div>
        </el-aside>
        <el-main>
            <router-view />
        </el-main>
      </el-container>
    </el-container>
    <el-drawer v-model="infoDrawer" title="个人信息" :with-header="false">
      <el-row>
          <el-col :span="10"></el-col>
          <el-col :span="5">
            <el-text>个人信息</el-text>
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
                  <img v-if="updateClientUser.avatarUrl!=null" :src="updateClientUser.avatarUrl" class="avatar" />
                  <el-icon v-else class="avatar-uploader-icon" ><Plus /></el-icon>
              </el-upload>
          </el-col>
      </el-row>
      <el-form
          ref="updateClientUserRef"
          :model="updateClientUser"
          :rules="updateClientUserRules"
          label-width="120px"
          style="max-width: 500px; margin: auto;"
      >
          <el-form-item label="昵称" prop="nickName">
              <el-input v-model="updateClientUser.nickName"></el-input>
          </el-form-item>
          <el-form-item label="邮箱" prop="email">
              {{ updateClientUser.email }}
          </el-form-item>
          <el-form-item label="性别" prop="gender">
              <el-radio-group v-model="updateClientUser.gender">
                  <el-radio :label="1">男</el-radio>
                  <el-radio :label="2">女</el-radio>
                  <el-radio :label="3">未知</el-radio>
              </el-radio-group>
          </el-form-item>
          <el-form-item label="描述" prop="description">
              <el-input v-model="updateClientUser.description" type="textarea" show-word-limit maxlength="50"></el-input>
          </el-form-item>
          <el-form-item>
              <el-button type="primary" @click="submitForm" plain>修改</el-button>
              <el-button plain @click="resetForm">重置</el-button>
          </el-form-item>
      </el-form>
    </el-drawer>
    <el-dialog title="查找好友" v-model="showSearchUser" width="1000px">
      <el-row>
        <el-input style="width: 40%;" v-model="clientUserFilter.nickName" placeholder="昵称">
      <template #prepend>
        昵称
      </template>
    </el-input>
    <el-input style="width: 40%;" v-model="clientUserFilter.email" placeholder="邮箱">
      <template #prepend>
        邮箱
      </template>
    </el-input>&nbsp;&nbsp;
    <el-button type="primary" plain @click="findUsers">查找</el-button>
    <el-button plain @click="resetFindUsers">重置</el-button>
        <el-scrollbar height="400px">
         <el-table style="width: 1000px" stripe :data="clientUserList">
            <el-table-column label="头像" width="100px">
              <template #default="scope">
                <el-avatar shape="square" :size="40" :src="scope.row.avatarUrl" />
              </template>
            </el-table-column>
            <el-table-column prop="nickName" label="昵称" width="180px"/>
            <el-table-column prop="email" label="邮箱" width="200px"/>
            <el-table-column label="性别" >
              <template #default="scope">
                <el-icon size="30" v-if="scope.row.gender===1" color="blue"><Male /></el-icon>
                <el-icon size="30" v-else-if="scope.row.gender===2" color="pink"><Female /></el-icon>
                <el-icon size="30" v-else color="green">?</el-icon>
              </template>
            </el-table-column>
            <el-table-column prop="description" label="描述" width="200px">
              <template #default="scope">
                <el-text truncated>{{ scope.row.description }}</el-text>
              </template>
            </el-table-column>
            <el-table-column label="操作">
              <template #default="scope">
                <el-button v-if="scope.row.userId != currentUser.userId" type="primary" plain @click="toSendMessage(scope.row.userId)">去聊天</el-button>
              </template>
            </el-table-column>
          </el-table>
          <el-pagination @current-change="changePage" background layout="prev, pager, next" :default-page-size="clientUserFilter.pageSize" :total="pageCount" />
        </el-scrollbar>
      </el-row>
    </el-dialog>
    <el-dialog title="修改密码" v-model="showUpdatePassword">
      <el-form label-width="auto">
        <el-form-item label="旧密码">
          <el-input v-model="oldPassword" show-password></el-input>
        </el-form-item>
        <el-form-item label="新密码">
          <el-input v-model="newPassword" show-password></el-input>
        </el-form-item>
        <el-form-item label="确认密码">
          <el-input v-model="confirmPassword" show-password></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="showUpdatePassword = false">取 消</el-button>
        <el-button type="primary" plain @click="updatePassword">确 定</el-button>
      </div>
    </el-dialog>

    <el-dialog v-model="showCreateGroup" title="新建群" width="600px">
      <el-row>
          <el-col :span="5"></el-col>
          <el-col :span="5"><el-text>头像</el-text></el-col>
          <el-col :span="10">
              <el-upload
                  class="avatar-uploader"
                  action="/api/UploadFile"
                  :show-file-list="false"
                  :before-upload="beforeAddGroupAvatarUpload"
                  :accept="['.jpg','.jpeg','.png','.gif','.bmp']"
                  :on-success="handleAddGroupAvatarSuccess"
                  >
                  <img v-if="addGroup.avatarUrl!=null" :src="addGroup.avatarUrl" class="avatar" />
                  <el-icon v-else class="avatar-uploader-icon" ><Plus /></el-icon>
              </el-upload>
          </el-col>
      </el-row>
      <el-form
          ref="addGroupRef"
          :model="addGroup"
          :rules="addGroupRules"
          label-width="120px"
          style="max-width: 500px; margin: auto;"
      >
          <el-form-item label="群名称" prop="name">
              <el-input v-model="addGroup.name"></el-input>
          </el-form-item>
          <el-form-item label="描述" prop="description">
              <el-input v-model="addGroup.description" type="textarea" show-word-limit maxlength="100"></el-input>
          </el-form-item>
          <el-form-item>
              <el-button type="primary" @click="submitAddGroupForm" plain>新建</el-button>
              <el-button plain @click="resetAddGroupForm">重置</el-button>
          </el-form-item>
      </el-form>
    </el-dialog>
</template>

<script setup lang="ts" name="HomePage">
import useCurrentUserStore from '../stores/currentUser'
import useLoginApi from '../api/loginApi'
import { useDark, useToggle } from '@vueuse/core'
import { ref, getCurrentInstance } from 'vue'
import useClientUserApi from '../api/clientUserApi'
import { ElMessage, ElNotification } from 'element-plus'
import { ChatGptMessageSearchFilter, Group, GroupSearchFilter, GroupMessageModel, GroupMessage as GM, ClientUser, FriendSearchFilter, PrivateMessage as PM, PrivateMessageModel, Friend, ClientUserSearchFilter, PrivateMessageSearchFilter } from '../types'
import type { UploadProps, FormInstance } from 'element-plus'
import FriendList from '../components/FriendListComponent.vue'
import GroupList from '../components/GroupListComponent.vue'
import * as signalR from "@microsoft/signalr";
import useFriendApi from '../api/friendApi'
import usePrivateMessageApi from '../api/privateMessageApi'
import useGroupApi from '../api/groupApi'
import useChatGPTMessageApi from '../api/chatGptMessageApi'

const isDark = useDark()
const toggleDark = useToggle(isDark)

const currentUserStore = useCurrentUserStore()
const currentUser = ref<ClientUser>({} as ClientUser)


const infoDrawer = ref(false)

const showSearchUser = ref(false)
const clientUserList = ref<ClientUser[]>([])

const clientUserFilter = ref({
  nickName: '',
  email: '' ,
  pageSize: 5
} as ClientUserSearchFilter)

const groupFilter = ref<GroupSearchFilter>({
  name: ''
} as GroupSearchFilter)

const showCreateGroup = ref(false)

const updateClientUser = ref<ClientUser>(
  {...currentUser.value} as ClientUser
)

const updateClientUserRef = ref<FormInstance>()


const that = getCurrentInstance()!.appContext.config.globalProperties
if (currentUserStore.token == ''){
  ElMessage.info('请先登录')
  that.$router.push("/")
}
that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token
const loginApi = useLoginApi(that)
const clientUserApi = useClientUserApi(that)
const friendApi = useFriendApi(that)
const privateMessageApi = usePrivateMessageApi(that)
const groupApi = useGroupApi(that)
const chatGptMessageApi = useChatGPTMessageApi(that)

const showUpdatePassword = ref(false)
const oldPassword = ref('')
const newPassword = ref('')
const confirmPassword = ref('')

const addGroup = ref({
} as Group)
const addGroupRef = ref()

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub", {
        accessTokenFactory: () => currentUserStore.token
    })
    .withAutomaticReconnect()
    .build();

currentUserStore.connection = connection

const toAdmin = () => {
  that.$router.push('/admin')
}
    
clientUserApi.getUserByJwtAsync().then(res => {
    currentUserStore.updateUserInfo(res.data)
    currentUser.value = currentUserStore.currentUser
    updateClientUser.value = {...currentUser.value} as ClientUser
})


async function updatePassword(){
  if (!oldPassword.value || !newPassword.value || !confirmPassword.value) {
    ElMessage.error('请填写完整信息')
    return
  }
  if (newPassword.value.length > 20 || newPassword.value.length < 6) {
    ElMessage.error('密码长度应在6-20位之间')
    return
  }
  if (newPassword.value !== confirmPassword.value) {
    ElMessage.error('两次输入的密码不一致')
    return
  }
  var result = await clientUserApi.updatePasswordAsync(currentUser.value.userId,oldPassword.value, newPassword.value)
  if (result.statusCode == 200){
    ElMessage.success('修改成功')
    showUpdatePassword.value = false
  } else {
    ElMessage.error(result.message)
    return
  }
  logout()
}

const logout = () => {
  loginApi.logoutAsync()
  currentUserStore.logout()
  that.$router.push('/')
}

const toAI = () => {
    that.$router.push('/npcMessage')
    let chatGptMessageSearchFilter = {
        pageIndex: 1,
        isDeleted: false,
        userId: currentUserStore.currentUser.userId
    } as ChatGptMessageSearchFilter
    chatGptMessageApi.getPageAsync(chatGptMessageSearchFilter).then(res => {
        currentUserStore.chatGptMessages = res.data.reverse()
    })
}

const toSendMessage = (userId: number) => {
  let isExist = false
  for (let i=0; i<currentUserStore.friends.length; i++){
    if (currentUserStore.friends[i].friendUserId == userId){
      isExist = true
      currentUserStore.friends[i].unread = 0
      currentUserStore.currentFriend = currentUserStore.friends[i]
      getMessages()
      that.$router.push('/privateMessage')
      showSearchUser.value = false
    }
  }
  if (!isExist){
    let newFriend: Friend = {
      userId : currentUser.value.userId,
      friendUserId : userId,
      type : 1
    } as Friend

    friendApi.addAsync(newFriend).then(()=>{
      loadFriends()
      setTimeout(() => {
        for (let i=0; i<currentUserStore.friends.length; i++){
          if (currentUserStore.friends[i].friendUserId == userId){
            currentUserStore.friends[i].unread = 0
            currentUserStore.currentFriend = currentUserStore.friends[i]
            getMessages()
            that.$router.push('/privateMessage')
            showSearchUser.value = false
          }
        }
      }, 100)
    })
    showSearchUser.value = false
    
  }
}

const getMessages = async () => {
    const searchFilter: PrivateMessageSearchFilter = {} as PrivateMessageSearchFilter
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

const resetForm = () => {
  updateClientUser.value = {...currentUser.value} as ClientUser
}

const updateClientUserRules = {
  nickName: [
    { required: true, message: '请输入昵称', trigger: 'blur' },
    { min: 1, max: 20, message: '长度在 1 到 20 个字符', trigger: 'blur' }
  ],
  description: [
    { max: 50, message: '长度在 0 到 50 个字符', trigger: 'blur' }
  ]
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
  updateClientUser.value.avatarUrl = response
  ElMessage.success('上传头像成功!')
}

const submitForm = () => {
  updateClientUserRef.value!.validate((valid: boolean) => {
    if (valid) {
      clientUserApi.updateAsync(updateClientUser.value).then((response) => {
        if (response.statusCode == 200) {
          ElMessage.success('修改成功!')
          currentUserStore.updateUserInfo(updateClientUser.value)
          currentUser.value = currentUserStore.currentUser
          infoDrawer.value = false
        } else {
          ElMessage.error(response.message)
        }
      })
    } else {
      ElMessage.info('请检查输入!')
      return false
    }
  })
}

const changeSessageMenu = (index: string) => {
  currentUserStore.messageMenu = index
}

currentUserStore.connection.on("ReceivePrivateMessage", (message: PM) => {
  const privateMessageModel = {} as PrivateMessageModel
  privateMessageModel.content = message.content
  privateMessageModel.senderId = message.senderId
  privateMessageModel.receiverId = message.receiverId
  privateMessageModel.type = message.type
  privateMessageModel.sendTime = new Date()
  privateMessageModel.avatarUrl = currentUserStore.currentFriend.avatarUrl
  privateMessageModel.nickName = currentUserStore.currentFriend.nickName
  
  if (message.senderId === currentUserStore.currentFriend.friendUserId) {
  } else {
      let isExist = false
      for (let i=0; i < currentUserStore.friends.length; i++){
        if (currentUserStore.friends[i].friendUserId == message.senderId && currentUserStore.friends[i].type != 2){
          isExist = true
          if (currentUserStore.friends[i].type == 0 || currentUserStore.messageMenu != '0'){
            ElNotification({
              title: currentUserStore.friends[i].remark!=''?currentUserStore.friends[i].remark??currentUserStore.friends[i].nickName : currentUserStore.friends[i].nickName,
              message: privateMessageModel.type==1 ? privateMessageModel.content :
                privateMessageModel.type==2 ? '[图片]' : '[文件]' + privateMessageModel.content
            })
          }
        }
      }
      if (!isExist){
        let newFriend: Friend = {
          userId : currentUser.value.userId,
          friendUserId : message.senderId,
          type : 1
        } as Friend

        friendApi.addAsync(newFriend).then(()=>{
          loadFriends()
        })
      }
  }
});

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

const resetFindUsers = () => {
  clientUserFilter.value.email = ''
  clientUserFilter.value.nickName = ''
}
const pageCount = ref(0)
const findUsers = () => {
  clientUserApi.getPageAsync(clientUserFilter.value).then((res) => {
    if (res.statusCode === 200) {
      clientUserList.value = res.data
      pageCount.value = res.totalCount
    } else {
      ElMessage.error('查找失败！')
    }
  })
}
findUsers()

const changePage = (res: number) => {
  clientUserFilter.value.pageIndex = res
  findUsers()
}

currentUserStore.connection.start()
  .then(() => {
      loadGroups()
  })
  .catch((error) => {
    console.error("Error establishing connection:", error)
});

const loadGroups = () => {
    const filter = ref<GroupSearchFilter>({
        name: ''
    } as GroupSearchFilter)
    filter.value.memberId = currentUserStore.currentUser.userId
    groupApi.getListAsync(filter.value).then((res) => {
        if (res.statusCode === 200) {
            currentUserStore.groupList = res.data
            for (let i=0; i<currentUserStore.groupList.length; i++){
                currentUserStore.groupList[i].unread = 0
                connection.invoke("JoinGroup", currentUserStore.groupList[i].groupId)
            }
        }
    })    
}

currentUserStore.connection.on("ReceiveGroupMessage", (message: GM) => {
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
  })
});


const beforeAddGroupAvatarUpload = (file: any) => {
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

const handleAddGroupAvatarSuccess: UploadProps['onSuccess'] = (response) => {
  addGroup.value.avatarUrl = response
  ElMessage.success('上传头像成功!')
}

const addGroupRules = {
  name: [
    { required: true, message: '请输入群名称', trigger: 'blur' },
    { min: 1, max: 20, message: '长度在 1 到 20 个字符', trigger: 'blur' }
  ],
  description: [
    { max: 100, message: '不得超过100个字符', trigger: 'blur' }
  ]
}

const resetAddGroupForm = () => {
  addGroupRef.value.resetFields()
}

const submitAddGroupForm = () => {
  addGroupRef.value.validate((valid: boolean) => {
    if (valid) {
      addGroup.value.owner = currentUserStore.currentUser.userId
      groupApi.addAsync(addGroup.value).then(() => {
        ElMessage.success('新建成功!')
        showCreateGroup.value = false
        loadNewGroups()
        addGroup.value = {} as Group
      })
    } else {
      ElMessage.error('请检查输入是否正确!')
      return false
    }
  })
}


const loadNewGroups = () => {
    groupFilter.value.memberId = currentUserStore.currentUser.userId
    let groups = [...currentUserStore.groupList]
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
            for (let i=0; i<currentUserStore.groupList.length; i++){
                if (!groups.find(g => g.groupId === currentUserStore.groupList[i].groupId)){
                    connection.invoke("JoinGroup", currentUserStore.groupList[i].groupId)
                    currentUserStore.currentGroup = currentUserStore.groupList[i]
                }
            }
            currentUserStore.joinGroupList= currentUserStore.groupList.filter(g => g.owner !== currentUserStore.currentUser.userId)
            currentUserStore.createdGroupList = currentUserStore.groupList.filter(g => g.owner === currentUserStore.currentUser.userId)
        }else{
            ElMessage.error('群列表获取失败！')
        }
    })
}

currentUserStore.connection.on("ReceiveInviteGroupMember", () => {
    loadNewGroups()
});

currentUserStore.connection.on("ReceiveRemoveGroup", (groupId: number) => {
    for (let i=0; i<currentUserStore.groupList.length; i++){
      if (currentUserStore.groupList[i].groupId === groupId){
        ElMessage.info(`群[${currentUserStore.groupList[i].name}]被解散！`)
        if (currentUserStore.groupList[i].groupId === currentUserStore.currentGroup.groupId){
          that.$router.push("/home")
        }
        break
      }
    }
    loadNewGroups()
});


</script>

<style scoped>
.container {
  display: flex;
  flex-direction: column;
}
.container-header {
  line-height: 50px;
  height: 60px;
  border-bottom: 1px solid var(--el-border-color);
}
.header-avatar{
    margin-left: 65%;
    cursor: pointer;
}
.dark-button{
    height: 50px;
    line-height: 50px;
}
.theme-sunny {
    color: #f1c40f;
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