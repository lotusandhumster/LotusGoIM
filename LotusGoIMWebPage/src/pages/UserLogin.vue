<template>
    <el-row>
        <el-col>
            <div class="lotusgo-door">
                &nbsp;&nbsp;<b>LotusGo 智能聊天网</b>
            </div>
            <div style="margin-left: 48%;opacity: 20%; font-size: 30px;">可以很简单。</div>
        </el-col>
        <el-col>
            <el-card v-loading="loading" class="login-card">
                <!-- login -->
                <el-row>
                    <el-col :span="24">
                        <el-text class="login-title">登录</el-text>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <el-divider></el-divider>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="5">
                        <el-avatar shape="square" class="login-avatar" :src="currentUserStore.currentUser.avatarUrl??userAvatar"/>
                    </el-col>
                    <el-col :span="5">
                        <div  class="account-input-lable">
                            <el-text class="account-input-lable-text">账号:</el-text>
                        </div>
                        <div  class="account-input-lable">
                            <el-text class="account-input-lable-text">密码:</el-text>
                        </div>    
                    </el-col>
                    <el-col :span="14">
                        <div>
                            <el-input v-model="loginModel.email"
                                placeholder="请输入账号"
                                class="account-input"
                            >
                            </el-input>
                            <el-input v-model="loginModel.password" 
                                type="password"
                                placeholder="请输入密码"
                                class="account-input"
                                @keyup.enter="toLogin"
                                >
                            </el-input>
                        </div>
                    </el-col>
                </el-row>
                <el-row>
                    <el-checkbox v-model="rememberPassword">记住密码</el-checkbox>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <el-divider></el-divider>
                    </el-col>
                </el-row>   
                <el-row>
                    <el-col :span="24">
                        <el-button @click="toLogin" class="login-button">登录</el-button>
                    </el-col>
                </el-row>      
                <el-row>
                    <el-col :span="24">
                        <ul class="register-and-forget-ul">
                            <li><el-button type="primary" link @click="$router.push('/register')">注册</el-button></li>
                            <li><el-button type="primary" link @click="$router.push('/forgetPassword')">忘记密码</el-button></li>
                        </ul>
                    </el-col>
                </el-row>
                
                <br/>
                <el-row>
                    <el-col :span="4">
                        <el-text>主题: </el-text>
                    </el-col>
                    <el-col :span="20">
                        <el-icon @click="toggleDark()" size="30px">
                            <Sunny v-if="!isDark" class="theme-sunny"/>
                            <Moon v-else/>
                        </el-icon>
                    </el-col>
                </el-row>        
            </el-card>
        </el-col>
    </el-row>
</template>

<script setup lang="ts" name="UserLogin">
import { ref, getCurrentInstance } from 'vue'
import { useDark, useToggle } from '@vueuse/core'
import useLoginApi from '../api/loginApi'
import { FriendModel, LoginModel } from '../types'
import useCurrentUserStore from '../stores/currentUser'
import { ElMessage } from 'element-plus'

let that: any = getCurrentInstance()!.appContext.config.globalProperties
const currentUserStore = useCurrentUserStore()

const userAvatar = new URL('../assets/images/robot.jpg', import.meta.url).href
const loginApi = useLoginApi(that)

const isDark = useDark()
const toggleDark = useToggle(isDark)

const loading = ref(false)

const loginModel = ref<LoginModel>({
    email: '',
    password: ''
})

const rememberPassword = ref(false)
rememberPassword.value = currentUserStore.rememberPassword

if(rememberPassword.value){
    loginModel.value = {
        ...currentUserStore.loginModel
    }
}

async function toLogin() {
    loading.value = true
    currentUserStore.currentFriend = {} as FriendModel
    try {
        const response = await loginApi.loginAsync(loginModel.value);
        if (response.statusCode == 200) {
            currentUserStore.updateToken(response.data)
            that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + response.data
            if(rememberPassword.value){
                currentUserStore.setLoginModel(loginModel.value)
                currentUserStore.rememberPassword = true
            }else{
                currentUserStore.clearLoginModel()
                currentUserStore.rememberPassword = false
            }
            ElMessage.success('登录成功')
            that.$router.push('/home')
        } else {
            ElMessage.error(`登录失败:账号或密码错误`)
        }
    } catch (error) {
        console.log(error)
        ElMessage.error(`登录失败:账号或密码错误`)
    }
    loading.value = false
}
</script>

<style scoped>
.login-card {
    width: 30%;
    height: 97vh;
    right: 10px;
    top: 10px;
    position: fixed;
}
.login-title {
    font-size: 40px;
    font-weight: bold;
    display: flex;
    justify-content: center;
}

.login-button {
    width: 100%;
    height: 50px;
    font-size: 25px;
}

.register-and-forget-ul {
    list-style: none;
    margin-top: 22px;
}

.register-and-forget-ul li {
    text-align: right;
}

.login-avatar {
    position: absolute;
    width: 80px;
    height: 80px;
}

.theme-sunny {
    color: #f1c40f;
}

.account-input {
    margin-top: 5px;
    margin-bottom: 5px;
}

.account-input-lable{
    margin-top: 10px;
    margin-bottom: 20px;
}

.account-input-lable-text{
    font-size: 15px;
}

.lotusgo-door {
    font-size: 12vh;
    opacity: 10%;
    margin-top: 30vh;
}


</style>