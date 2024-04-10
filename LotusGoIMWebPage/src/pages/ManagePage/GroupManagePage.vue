<template>
    <el-text>群管理</el-text><br/>
    <el-input style="width: 20%;" v-model="groupSearchFilter.name" placeholder="群名称">
      <template #prepend>
        群名称
      </template>
    </el-input>&nbsp;&nbsp;
    <el-button type="primary" plain @click="findGroups">查找</el-button>
    <el-button plain @click="resetFindGroup">重置</el-button>
    <el-table stripe :data="groupList">
      <el-table-column prop="groupId" label="ID"/>
      <el-table-column label="头像" width="100px">
        <template #default="scope">
          <el-avatar shape="square" :size="40" :src="scope.row.avatarUrl" />
        </template>
      </el-table-column>
      <el-table-column prop="name" label="群名称" width="180px"/>
      <el-table-column prop="owner" label="所有者ID"/>
      <el-table-column prop="description" label="描述" width="500px">
        <template #default="scope">
          <el-text truncated>{{ scope.row.description }}</el-text>
        </template>
      </el-table-column>
      <el-table-column label="操作">
        <template #default="scope">
          <el-button plain @click="update(scope.row)"><el-icon><Edit/></el-icon></el-button>
          <el-button type="danger" plain @click="deleteGroup(scope.row.groupId)"><el-icon><Delete/></el-icon></el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination @current-change="changePage" background layout="prev, pager, next" :default-page-size="groupSearchFilter.pageSize" :total="pageCount" />
    <el-drawer v-model="infoDrawer" title="管理群" :with-header="false">
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
              <el-button plain type="danger" @click="deleteGroup(currentGroup.groupId)">解散群聊</el-button>
          </el-form-item>
      </el-form>
    </el-drawer>
</template>
<script setup lang="ts" name="GroupManagePage">
import { ref, getCurrentInstance } from 'vue'
import useCurrentUserStore from '../../stores/currentUser'
import useGroupApi from '../../api/groupApi'
import { Group, GroupSearchFilter } from '../../types'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { UploadProps } from 'element-plus'

let that: any = getCurrentInstance()!.appContext.config.globalProperties
const currentUserStore = useCurrentUserStore()
that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token

const groupApi = useGroupApi(that)
const groupSearchFilter = ref({
    name: '',
    pageIndex: 1,
    pageSize: 8,
    isDeleted: false
} as GroupSearchFilter)
const currentGroup = ref({} as Group)
const updateGroup = ref({
} as Group)

const updateGroupRef = ref()
const resetForm = () => {
   updateGroup.value = {...currentGroup.value}
}
const showSettingGroup = ref(false)

const groupList = ref<Group[]>([])
const pageCount = ref(1)
const infoDrawer = ref(false)
const changePage = (res: number) => {
  groupSearchFilter.value.pageIndex = res
  findGroups()
}

const findGroups = () => {
    groupApi.getPageAsync(groupSearchFilter.value).then(res => {
        groupList.value = res.data
        pageCount.value = res.totalCount
    })
}
findGroups()

const updateGroupRules = {
  name: [
    { required: true, message: '请输入群名称', trigger: 'blur' },
    { min: 1, max: 20, message: '长度在 1 到 20 个字符', trigger: 'blur' }
  ],
  description: [
    { max: 100, message: '不得超过100个字符', trigger: 'blur' }
  ]
}

const resetFindGroup = () => {
    groupSearchFilter.value.name = ''
}

const update = (group: Group) => {
    currentGroup.value = group
    updateGroup.value = {...group}
    infoDrawer.value = true
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
const submitForm = () => {
  updateGroupRef.value.validate((valid: boolean) => {
    if (valid) {
      groupApi.updateAsync(updateGroup.value).then(() => {
        currentGroup.value.avatarUrl = updateGroup.value.avatarUrl
        currentGroup.value.name = updateGroup.value.name
        currentGroup.value.description = updateGroup.value.description
        showSettingGroup.value = false
        ElMessage.success('修改成功!')
      })
    } else {
      ElMessage.error('请检查输入是否正确!')
      return false
    }
  })
}

 const deleteGroup = (id: number) => {
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
            groupApi.deleteAsync(id).then(res => {
                if(res.statusCode === 200){
                    ElMessage.success("解散成功")
                    findGroups()
                }
            })
        }
    )
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