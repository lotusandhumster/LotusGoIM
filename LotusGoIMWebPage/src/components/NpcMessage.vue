<template>
    <div v-if="message.role==='user'">
        <el-row>
            <el-col :span="19"></el-col>
            <el-col :span="4">
                <div style="margin-top: 5px;margin-right: 10px; text-align: right;">
                    <el-text>{{ formatDate(message.sendTime) }}</el-text>
                </div>
            </el-col>
            <el-col :span="1">
                <el-avatar shape="square" :size="35" :src="message.avatarUrl" />
            </el-col>
        </el-row>
        <el-row>
            <el-col :span="23" style="text-align: right;">
                <div style="display: inline-block;">
                    <el-card :body-style="{padding: 0}" style="padding: 15px;">
                        <el-text>{{ message.content }}</el-text>
                    </el-card>
                </div>
            </el-col>
            <el-col :span="1"></el-col>
        </el-row>
    </div>
    <div v-else>
        <el-row>
            <el-col :span="1">
                <el-avatar shape="square" :size="35" :src="userAvatar" />
            </el-col>
            <el-col :span="23">
                <div style="margin-top: 5px;">
                    <el-text>{{ formatDate(message.sendTime) }}</el-text>
                </div>
            </el-col>
        </el-row>
        <el-row>
            <el-col :span="1"></el-col>
            <el-col :span="23">
                <div style="display: inline-block;">
                    <el-card :body-style="{padding: 0}" style="padding: 15px;">
                        <el-text>{{ message.content }}</el-text>
                    </el-card>
                </div>
            </el-col>
        </el-row>
    </div>
</template>

<script lang="ts" setup>
import { ref, defineProps } from 'vue'
import { ChatGptMessageWithUserModel } from '../types';

const props = defineProps<{
    message: ChatGptMessageWithUserModel
}>()
const userAvatar = new URL('../assets/images/robot.jpg', import.meta.url).href
const message = ref(props.message)

function formatDate(sendTime: Date): string {
    sendTime = new Date(sendTime)
    const year = sendTime.getFullYear()
    const month = String(sendTime.getMonth() + 1).padStart(2, '0');
    const day = String(sendTime.getDate()).padStart(2, '0');
    const hours = String(sendTime.getHours()).padStart(2, '0');
    const minutes = String(sendTime.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day} ${hours}:${minutes}`;
}
</script>

<style scoped>
</style>