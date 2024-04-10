<template>
    <el-card class="register-main-card">
        <el-row>
            <el-col :span="1">
                <el-icon class="back-button" @click="$router.back()"><CaretLeft /></el-icon>
            </el-col>
            <el-col :span="23">
                <el-text class="register-title">注册</el-text>
            </el-col>
            <el-divider></el-divider>
        </el-row>
            <el-row>
                <el-col :span="9"></el-col>
                <el-col :span="1">头像</el-col>
                <el-col :span="10">
                    <el-upload
                        class="avatar-uploader"
                        action="/api/UploadFile"
                        :show-file-list="false"
                        :before-upload="beforeAvatarUpload"
                        :accept="['.jpg','.jpeg','.png','.gif','.bmp']"
                        :on-success="handleAvatarSuccess"
                        >
                        <img v-if="registerForm.avatarUrl!=null" :src="registerForm.avatarUrl" class="avatar" />
                        <el-icon v-else class="avatar-uploader-icon" ><Plus /></el-icon>
                    </el-upload>
                </el-col>
            </el-row>
            <el-form
                ref="registerFormRef"
                :model="registerForm"
                :rules="registerFormRules"
                label-width="120px"
                style="max-width: 500px; margin: auto;"
            >
                <el-form-item label="昵称" prop="nickName">
                    <el-input v-model="registerForm.nickName"></el-input>
                </el-form-item>
                <el-form-item label="邮箱" prop="email">
                    <el-input v-model="registerForm.email" type="email"></el-input>
                </el-form-item>
                <el-form-item label="密码" prop="password">
                    <el-input v-model="registerForm.password" type="password"></el-input>
                </el-form-item>
                <el-form-item label="确认密码" prop="confirmPassword">
                    <el-input v-model="registerForm.confirmPassword" type="password"></el-input>
                </el-form-item>
                <el-form-item label="性别" prop="gender">
                    <el-radio-group v-model="registerForm.gender">
                        <el-radio :label="1">男</el-radio>
                        <el-radio :label="2">女</el-radio>
                        <el-radio :label="3">未知</el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="描述" prop="description">
                    <el-input v-model="registerForm.description" type="textarea" show-word-limit maxlength="50"></el-input>
                </el-form-item>
                <el-form-item label="验证码" prop="verificationCode">
                <el-input v-model="registerForm.verificationCode">
                    <template #append>
                        <el-button type="primary" @click="sendVerificationCode">发送验证码</el-button>
                    </template>
                </el-input>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="submitForm" plain>提交</el-button>
                    <el-button @click="resetForm">重置</el-button>
                </el-form-item>
            </el-form>
    </el-card>
</template>

<script setup lang="ts" name="RegisterPage">
import type { UploadProps, FormInstance } from 'element-plus'
import { ElMessage } from 'element-plus'
import { ref, getCurrentInstance } from 'vue'
import useRegisterApi from '../api/registerApi';
import { RegisterClientUserModel } from '../types';
import useSendEmailApi from '../api/sendEmailApi'

let that: any = getCurrentInstance()!.appContext.config.globalProperties
var registerApi = useRegisterApi(that);
var sendEmailApi = useSendEmailApi(that);

var registerForm = ref<RegisterClientUserModel>({} as RegisterClientUserModel)

registerForm.value.gender = 3

const registerFormRules = {
    nickName: [
        { required: true, message: '请输入昵称', trigger: 'blur' },
        { min: 1, max: 20, message: '长度在 1 到 20 个字符', trigger: 'blur' }
    ],
    email: [
        { required: true, message: '请输入邮箱', trigger: 'blur' },
        { type: 'email', message: '请输入正确的邮箱地址', trigger: ['blur', 'change'] }
    ],
    password: [
        { required: true, message: '请输入密码', trigger: 'blur' },
        { min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' }
    ],
    confirmPassword: [
        { required: true, message: '请再次输入密码', trigger: 'blur' },
        { validator: (rule: any, value: any, callback: any) => {
            if (value === '') {
                callback(new Error('请再次输入密码'))
            } else if (value !== registerForm.value.password) {
                callback(new Error('两次输入密码不一致!'))
            } else {
                callback()
            }
            rule
        }, trigger: 'blur' }
    ],
    verificationCode: [
        { required: true, message: '请输入验证码', trigger: 'blur' }
    ]
}

const registerFormRef = ref<FormInstance>()

const sendVerificationCodeDisabled = ref(false)


const handleAvatarSuccess: UploadProps['onSuccess'] = (response) => {
    registerForm.value.avatarUrl = response
    ElMessage.success('上传头像成功!')
}

const submitForm = () => {
    registerFormRef.value!.validate((valid: boolean) => {
        if (valid) {
            registerApi.registerAsync(registerForm.value).then((response) => {
                if (response.statusCode == 200) {
                    ElMessage.success('注册成功!')
                    that.$router.push('/')
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

const resetForm = () => {
    registerForm.value = {} as RegisterClientUserModel
}

const sendVerificationCode = () => {
    if (sendVerificationCodeDisabled.value) {
        ElMessage.info('请勿在一分钟内重复发送!')
        return
    }
    if (registerForm.value.email == null || registerForm.value.email == '') {
        ElMessage.info('请输入邮箱!')
        return
    }
    sendEmailApi.getVerificationCode(registerForm.value.email).then((response) => {
        if (response.statusCode == 200) {
            sendVerificationCodeDisabled.value = true
            setTimeout(() => {
                sendVerificationCodeDisabled.value = false
            }, 60000)
            ElMessage.success('已发送验证码!')
        } else {
            ElMessage.error('验证码发送失败!')
        }
    })
}

</script>

<style scoped>
.register-main-card {
    width: 100%;
    margin: 0 auto;
    height: 97vh;
}

.back-button {
    font-size: 30px;
    cursor: pointer;
}
.register-title {
    font-size: 22px;
    font-weight: bold;
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