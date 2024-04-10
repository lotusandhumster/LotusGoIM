<template>
    <el-container class="container">
       <el-header class="container-header">
         <el-row>
           <el-col :span="4">
             <el-text>Lotus Go 管理员</el-text>
           </el-col>
           <el-col :span="3">
             <el-button text @click="$router.push('/home')" plain><el-icon size="20"><User /></el-icon>&nbsp;普通模式</el-button>
           </el-col>
           <el-col :span="6"></el-col>
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
         <el-aside width="150px" class="container-aside">
           <div>
             <el-menu
               :ellipsis="false"
               @select="changeSessageMenu"
               :default-active="currentUserStore.messageMenu"
             >
               <el-menu-item index="0">
                <el-icon><User /></el-icon>用户管理
               </el-menu-item>
               <el-menu-item index="1">
                <el-icon><UserFilled /></el-icon>群管理
               </el-menu-item>
               <el-menu-item index="2">
                <el-icon><Comment /></el-icon>AI消息管理
               </el-menu-item>
               <el-menu-item index="3">
                <el-icon><ChatLineRound /></el-icon>私聊管理
               </el-menu-item>
               <el-menu-item index="4">
                <el-icon><ChatLineSquare /></el-icon>群消息管理
               </el-menu-item>
             </el-menu>
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

 </template>
 
 <script setup lang="ts" name="AdminManagePage">
 import useCurrentUserStore from '../../stores/currentUser'
 import { useDark, useToggle } from '@vueuse/core'
 import { ref, getCurrentInstance } from 'vue'
 import useClientUserApi from '../../api/clientUserApi'
 import { ElMessage } from 'element-plus'
 import { ClientUser } from '../../types'
 import type { UploadProps, FormInstance } from 'element-plus'
 import useLoginApi from '../../api/loginApi'

 const isDark = useDark()
 const toggleDark = useToggle(isDark)
 
 const currentUserStore = useCurrentUserStore()

 const currentUser = ref<ClientUser>({} as ClientUser)
 const infoDrawer = ref(false)

 
 const updateClientUser = ref<ClientUser>(
   {...currentUser.value} as ClientUser
 )
 
 const updateClientUserRef = ref<FormInstance>()
 
 
 const that = getCurrentInstance()!.appContext.config.globalProperties
 if (currentUserStore.token == ''){
    ElMessage.info('请先登录')
    that.$router.push("/")
}
 if (currentUserStore.currentUser.userId != 1){
    that.$router.push('/home')
 }
 that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token
 const clientUserApi = useClientUserApi(that)
 const loginApi = useLoginApi(that)
 
 const showUpdatePassword = ref(false)
 const oldPassword = ref('')
 const newPassword = ref('')
 const confirmPassword = ref('')
 
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
   if(index=='0') that.$router.push('/clientUserManage')
   if(index=='1') that.$router.push('/groupManage')
   if(index=='2') that.$router.push('/chatpgtMessageManage')
   if(index=='3') that.$router.push('/privateMessageManage')
   if(index=='4') that.$router.push('/groupMessageManage')
 }
 
 changeSessageMenu(currentUserStore.messageMenu)

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