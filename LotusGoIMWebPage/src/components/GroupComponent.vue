<template>
    <el-card class="group-info-div" shadow="hover" :body-style="{padding: 0}" @click="openMessage">
        <el-row>
            <el-col :span="6">
                <div class="avatar-div">
                    <el-avatar shape="square" class="group-avatar" :src="group.avatarUrl"/>
                </div>
            </el-col>
            <el-col :span="16">
                <div class="info-div">
                    <div>
                        <el-text class="friend-name" truncated>
                            &nbsp;&nbsp;<b>{{ group.name }}</b>
                        </el-text>
                    </div>
                    <div>
                        <el-popover
                            placement="right"
                            :title="group.name"
                            :width="200"
                            trigger="hover"
                            :content="group.description"
                        >
                            <template #reference>
                                <el-text class="group-description" truncated>{{ group.description }}</el-text>
                            </template>
                        </el-popover>
                    </div>
                </div>
            </el-col>
            <el-col :span="2">
                <div>
                    <el-badge :value="group.unread" :hidden="group.unread==0" max></el-badge>
                </div>
            </el-col>
        </el-row>
    </el-card>
</template>

<script lang="ts" setup>
import { ref, defineProps, getCurrentInstance, onMounted } from 'vue'
import { Group } from '../types'
import useCurrentUserStore from '../stores/currentUser'
import { GroupMessage as GM } from '../types'

let that: any = getCurrentInstance()!.appContext.config.globalProperties
const currentUserStore = useCurrentUserStore()
that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token

const props = defineProps<{
    group: Group
}>()

const group = ref(props.group)

const openMessage = () => {
    currentUserStore.currentGroup = group.value
    group.value.unread = 0
    for(let i = 0; i < currentUserStore.groupList.length; i++){
        if(currentUserStore.groupList[i].groupId == group.value.groupId){
            currentUserStore.groupList[i].unread = 0
        }
    }
    that.$router.push('/groupMessage')
}

currentUserStore.connection.on("ReceiveGroupMessage", (message: GM) => {
    if ((message.groupId != currentUserStore.currentGroup.groupId || currentUserStore.messageMenu!='1')
        && message.groupId == group.value.groupId) {
        group.value.unread += 1
        for(let i = 0; i < currentUserStore.groupList.length; i++){
            if(currentUserStore.groupList[i].groupId == group.value.groupId){
                currentUserStore.groupList[i].unread = group.value.unread
            }
        }
    }
});

onMounted(() => {
    for(let i = 0; i < currentUserStore.groupList.length; i++){
        if(currentUserStore.groupList[i].groupId == group.value.groupId){
            group.value.unread = currentUserStore.groupList[i].unread
        }
    }
})

</script>

<style scoped>
.group-avatar {
    float: left;
}
.info-avatar {
    float: left;
    cursor: pointer;
}

.group-description {
    width: 7vw;
    margin-left: 10px;
    margin-top: -20px;
}
.group-info-div {
    display: flex;
    border-bottom: 1px solid var(--el-color-info-light-8);
    cursor: pointer;
    padding: 3px;
    padding-bottom: 0px;
}
.show-info-avatar {
    width: 100px;
    height: 100px;
}

.avatar-div {
    margin-top: 5px;
}
</style>