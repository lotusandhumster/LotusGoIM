<template>
    <el-card class="forget-password-card">
        <el-row>
            <el-col :span="1">
                <el-icon class="back-button" @click="$router.back()"><CaretLeft /></el-icon>
            </el-col>
            <el-col :span="23">
                <el-text class="forget-password-title">&nbsp;&nbsp;找回密码</el-text>
            </el-col>
            <el-divider></el-divider>
            <el-form
                ref="forgetPasswordFormRef"
                :model="forgetPasswordModel"
                :rules="forgetPasswordModelRules"
                label-width="120px"
                style="max-width: 500px; margin: auto;"
            >
                <el-form-item label="邮箱" prop="email">
                    <el-input v-model="forgetPasswordModel.email" type="email" placeholder="请输入邮箱"></el-input>
                </el-form-item>
                <el-form-item label="验证码" prop="verificationCode">
                    <el-input v-model="forgetPasswordModel.verificationCode">
                        <template #append>
                            <el-button @click="sendVerificationCode">发送验证码</el-button>
                        </template>
                    </el-input>
                </el-form-item>
                <el-form-item label="新密码" prop="password">
                    <el-input v-model="forgetPasswordModel.password" type="password" placeholder="请输入新密码"></el-input>
                </el-form-item>
                <el-form-item label="确认密码" prop="confirmPassword">
                    <el-input v-model="forgetPasswordModel.confirmPassword" type="password" placeholder="请再次输入新密码"></el-input>
                </el-form-item>
                <el-form-item>
                    <el-button type='primary' plain @click="toForgetPassword">找回密码</el-button>
                </el-form-item>
            </el-form>
        </el-row>
    </el-card>
</template>

<script setup lang="ts" name="ForgetPassword">
import useRegisterApi from '../api/registerApi';
import {ref, getCurrentInstance} from 'vue';
import { ForgetPasswordModel } from '../types';
import { ElMessage } from 'element-plus';
import useSendEmailApi from '../api/sendEmailApi';
import type { FormInstance } from 'element-plus'

let that: any = getCurrentInstance()!.appContext.config.globalProperties
var registerApi = useRegisterApi(that);
var sendEmailApi = useSendEmailApi(that);

var forgetPasswordModel = ref<ForgetPasswordModel>({} as ForgetPasswordModel)
var forgetPasswordFormRef = ref<FormInstance>()

var sendVerificationCodeDisabled = ref(false)

const forgetPasswordModelRules = {
    email: [
        { required: true, message: '请输入邮箱', trigger: 'blur' },
        { type: 'email', message: '请输入正确的邮箱地址', trigger: ['blur', 'change'] }
    ],
    verificationCode: [
        { required: true, message: '请输入验证码', trigger: 'blur' }
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
            } else if (value !== forgetPasswordModel.value.password) {
                callback(new Error('两次输入密码不一致!'))
            } else {
                callback()
            }
            rule
        }, trigger: 'blur' }
    ],
}


const sendVerificationCode = () => {
    if (sendVerificationCodeDisabled.value) {
        ElMessage.info('请勿在一分钟内重复发送!')
        return
    }
    if (forgetPasswordModel.value.email == null || forgetPasswordModel.value.email == '') {
        ElMessage.info('请输入邮箱!')
        return
    }
    sendEmailApi.getVerificationCode(forgetPasswordModel.value.email).then((response) => {
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

const toForgetPassword = () => {
    forgetPasswordFormRef.value!.validate((valid: boolean) => {
        if (valid) {
            registerApi.forgetPasswordAsync(forgetPasswordModel.value).then((response) => {
                if (response.statusCode == 200) {
                    ElMessage.success('修改密码成功!')
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

</script>

<style scoped>
.forget-password-card {
    max-width: 500px;
    margin: auto;
    margin-top: 50px;
    padding: 20px;
    border-radius: 10px;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
}
.back-button {
    font-size: 30px;
    cursor: pointer;
}

.forget-password-title {
    font-size: 22px;
    font-weight: bold;
}
</style>