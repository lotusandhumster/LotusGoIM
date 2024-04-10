<template>
    <el-text>用户管理</el-text><br/>
    <el-input style="width: 20%;" v-model="clientUserFilter.nickName" placeholder="昵称">
      <template #prepend>
        昵称
      </template>
    </el-input>
    <el-input style="width: 20%;" v-model="clientUserFilter.email" placeholder="邮箱">
      <template #prepend>
        邮箱
      </template>
    </el-input>&nbsp;&nbsp;
    <el-button type="primary" plain @click="findUsers">查找</el-button>
    <el-button plain @click="resetFindUsers">重置</el-button>
    <el-table stripe :data="clientUserList">
      <el-table-column prop="userId" label="ID" />
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
      <el-table-column prop="description" label="描述" >
        <template #default="scope">
          <el-text truncated>{{ scope.row.description }}</el-text>
        </template>
      </el-table-column>
      <el-table-column label="状态" >
        <template #default="scope">
          <el-icon size="30" v-if="scope.row.state===0" style="color:green"><Bell /></el-icon>
          <el-icon size="30" v-if="scope.row.state===1" style="color:orange"><WarningFilled /></el-icon>
          <el-icon size="30" v-if="scope.row.state===2" style="color:cadetblue"><Clock /></el-icon>
          <el-icon size="30" v-if="scope.row.state===3"><CircleCloseFilled /></el-icon>
        </template>
      </el-table-column>
      <el-table-column label="操作">
        <template #default="scope">
          <el-button v-if="scope.row.userId != currentUserStore.currentUser.userId" plain @click="update(scope.row)"><el-icon><Edit/></el-icon></el-button>
          <el-button v-if="scope.row.userId != currentUserStore.currentUser.userId" type="danger" plain @click="deleteUser(scope.row.userId)"><el-icon><Delete/></el-icon></el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination @current-change="changePage" background layout="prev, pager, next" :default-page-size="clientUserFilter.pageSize" :total="pageCount" />
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
              <el-button  plain @click="showUpdatePassword=true" type="warning">修改密码</el-button>
           </el-form-item>
       </el-form>
     </el-drawer>
     <el-dialog title="修改密码" v-model="showUpdatePassword">
       <el-form label-width="auto">
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
<script setup lang="ts" name="AdminManagerPage">
import { ref, getCurrentInstance } from 'vue'
import useCurrentUserStore from '../../stores/currentUser';
import useClientUserApi from '../../api/clientUserApi'
import { ClientUserSearchFilter, ClientUser } from '../../types';
import { ElMessage, ElMessageBox } from 'element-plus'
import type { UploadProps, FormInstance } from 'element-plus'

let that: any = getCurrentInstance()!.appContext.config.globalProperties
const currentUserStore = useCurrentUserStore()
that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token

const clientUserApi = useClientUserApi(that)
const clientUserFilter = ref({
  nickName: '',
  email: '',
  pageIndex: 1,
  pageSize: 8
} as ClientUserSearchFilter)

const updateClientUserRules = {
   nickName: [
     { required: true, message: '请输入昵称', trigger: 'blur' },
     { min: 1, max: 20, message: '长度在 1 到 20 个字符', trigger: 'blur' }
   ],
   description: [
     { max: 50, message: '长度在 0 到 50 个字符', trigger: 'blur' }
   ]
 }

const showUpdatePassword = ref(false)
const newPassword = ref('')
const confirmPassword = ref('')

const clientUserList = ref<ClientUser[]>([])
const pageCount = ref(1)
const infoDrawer = ref(false)
const changePage = (res: number) => {
  clientUserFilter.value.pageIndex = res
  findUsers()
}
const updateClientUserRef = ref<FormInstance>()



const currentUser = ref<ClientUser>(
  {} as ClientUser
)
const updateClientUser = ref<ClientUser>(
  {} as ClientUser
)
const update = (user: ClientUser) => {
  currentUser.value = user
  infoDrawer.value = true
  updateClientUser.value = {...currentUser.value}
}
 
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
const resetForm = () => {
   updateClientUser.value = {...currentUser.value} as ClientUser
 }

 const deleteUser = (id: number) => {
  ElMessageBox.confirm(
        '确定删除此用户？',
        '提示',
        {
            distinguishCancelAndClose: true,
            confirmButtonText: '是',
            cancelButtonText: '取消',
        }
    )
    .then(() => {
        clientUserApi.deleteAsync(id).then(
          () => findUsers
        )
    })
 }

async function updatePassword(){
   if (!newPassword.value || !confirmPassword.value) {
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
   var result = await clientUserApi.updatePasswordAdminAsync(currentUser.value.userId,newPassword.value)
   if (result.statusCode == 200){
     ElMessage.success('修改成功')
     showUpdatePassword.value = false
   } else {
     ElMessage.error(result.message)
     return
   }
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
           infoDrawer.value = false
           findUsers()
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

const resetFindUsers = () => {
  clientUserFilter.value.email = ''
  clientUserFilter.value.nickName = ''
}
</script>
<style scoped>
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