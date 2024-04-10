<template>
    <el-text>AI消息管理</el-text><br/>
    <el-input style="width: 20%;" v-model="searchFilter.userId" placeholder="用户ID">
      <template #prepend>
        用户ID
      </template>
    </el-input>
    <el-button type="primary" plain @click="findMessages">查找</el-button>
    <el-button plain @click="resetForm">重置</el-button>
    <el-table stripe :data="messages">
      <el-table-column prop="chatGptMessageId" label="ID"/>
      <el-table-column prop="userId" label="用户ID"/>
      <el-table-column prop="email" label="用户邮箱"/>
      <el-table-column prop="role" label="角色"/>
      <el-table-column prop="content" label="内容"/>
      <el-table-column prop="sendTime" label="发送时间"  width="250px"/>
      <el-table-column label="操作">
        <template #default="scope">
          <el-button type="danger" plain @click="deleteMessage(scope.row.chatGptMessageId)"><el-icon><Delete/></el-icon></el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination @current-change="changePage" background layout="prev, pager, next" :default-page-size="searchFilter.pageSize" :total="pageCount" />
</template>
<script setup lang="ts" name="ChatGPTMessagePage">
import { ref, getCurrentInstance } from 'vue'
import useCurrentUserStore from '../../stores/currentUser'
import useChatGPTMessageApi from '../../api/chatGptMessageApi'
import { ChatGptMessageWithUserModel, ChatGptMessageSearchFilter } from '../../types'
import { ElMessage, ElMessageBox } from 'element-plus'

let that: any = getCurrentInstance()!.appContext.config.globalProperties
const currentUserStore = useCurrentUserStore()
that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token

const chatGptMessageApi = useChatGPTMessageApi(that)
const searchFilter = ref({
    userId: undefined,
    pageIndex: 1,
    pageSize: 8,
    isDeleted: false
} as ChatGptMessageSearchFilter)

const messages = ref<ChatGptMessageWithUserModel[]>([])
const pageCount = ref(1)
const changePage = (res: number) => {
    searchFilter.value.pageIndex = res
  findMessages()
}
const resetForm = () => {
    searchFilter.value.userId = undefined
}
const findMessages = () => {
    chatGptMessageApi.getPageAsync(searchFilter.value).then(res => {
        messages.value = res.data
        pageCount.value = res.totalCount
    })
}
findMessages()

const deleteMessage = (id: number) => {
    ElMessageBox.confirm(
        '确定删除该消息？',
        '提示',
        {
            distinguishCancelAndClose: true,
            confirmButtonText: '是',
            cancelButtonText: '取消',
        }
    ).then(
        () => {
            chatGptMessageApi.deleteAsync(id).then(
                res => {
                    if(res.statusCode==200){
                        ElMessage.success("删除成功！")
                        findMessages()
                    }
                }
            )
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