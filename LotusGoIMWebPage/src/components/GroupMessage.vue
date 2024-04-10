<template>
    <div v-if="message.userId==currentUserStore.currentUser.userId">
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
                    <el-card v-if="message.type==1" :body-style="{padding: 0}" style="padding: 15px;">
                        <el-text>{{ message.content }}</el-text>
                    </el-card>
                    <el-image :previewSrcList="[message.content]" v-else-if="message.type==2" :src="message.content" style="width: 200px;"></el-image>
                    <el-button v-else-if="message.type==3" @click="downloadFile"  type="primary" plain>{{ message.content }}<el-icon><Download /></el-icon></el-button>
                </div>
            </el-col>
            <el-col :span="1"></el-col>
        </el-row>
    </div>
    <div v-else>
        <el-row>
            <el-col :span="1">
                <el-avatar shape="square" :size="35" :src="message.avatarUrl" />
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
                    <el-card v-if="message.type==1" :body-style="{padding: 0}" style="padding: 15px;">
                        <el-text>{{ message.content }}</el-text>
                    </el-card>
                    <el-image :previewSrcList="[message.content]" v-else-if="message.type==2" :src="message.content" style="width: 200px;"></el-image>
                    <el-button v-else-if="message.type==3" @click="downloadFile"  type="primary" plain>{{ message.content }}<el-icon><Download /></el-icon></el-button>
                </div>
            </el-col>
        </el-row>
    </div>
</template>

<script lang="ts" setup>
import { ref, defineProps } from 'vue'
import { GroupMessageModel } from '../types';
import useCurrentUserStore from '../stores/currentUser';

const currentUserStore = useCurrentUserStore()

const props = defineProps<{
    message: GroupMessageModel
}>()

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

const downloadFile = () => {
    window.open(message.value.fileUrl)
}


</script>

<style scoped>
</style>