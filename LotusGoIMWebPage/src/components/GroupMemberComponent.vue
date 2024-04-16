<template>
    <div>
        <el-text>群成员列表&nbsp;
          <el-tooltip
              effect="light"
              content="邀请成员"
              placement="top-start"
          >
            <el-icon color="skyblue" style="cursor: pointer;" @click="showInviteMember=true"><CirclePlus /></el-icon>
          </el-tooltip>&nbsp;
          <el-tooltip
            effect="light"
            content="修改群昵称"
            placement="top-start"
          >
            <el-icon style="color:skyblue; cursor: pointer;" @click="showEditMemberName=true"><Edit/></el-icon>
          </el-tooltip>
        </el-text>
    </div>
    <el-card style="width: 220px;" shadow="never" :body-style="{padding: 0}"  >
        <el-scrollbar height="350px">
            <el-card @click="showMemberInfo(member)" class="member-info" :body-style="{padding: 0}" shadow="hover" v-for="member in groupMemberList" :key="member.memberId">
                <el-row  v-if="currentUserStore.currentGroup.owner===member.memberId" >
                    <el-col :span="2">
                        <el-icon color="orange"><Avatar /></el-icon>
                    </el-col>
                    <el-col :span="3">
                        <el-avatar :size="20" :src="member.avatarUrl"></el-avatar>
                    </el-col>
                    <el-col :span="2">
                            <el-icon v-if="member.gender===1" color="blue"><Male /></el-icon>
                            <el-icon v-else-if="member.gender===2" color="pink"><Female /></el-icon>
                            <el-icon v-else color="green">?</el-icon>
                    </el-col>
                    <el-col :span="14">
                        <el-text>{{member.memberName ?? member.memberName!='' ? member.memberName : member.nickName }}
                        </el-text>
                    </el-col>
                    <el-col :span="2">
                        <el-icon v-if="member.state===0" style="color:green"><Bell /></el-icon>
                        <el-icon v-if="member.state===1" style="color:orange"><WarningFilled /></el-icon>
                        <el-icon v-if="member.state===2" style="color:cadetblue"><Clock /></el-icon>
                        <el-icon v-if="member.state===3"><CircleCloseFilled /></el-icon>
                    </el-col>
                </el-row>
            </el-card>
            <el-card class="member-info" :body-style="{padding: 0}" shadow="hover" v-for="member in groupMemberList" :key="member.memberId">
                <el-row  v-if="currentUserStore.currentGroup.owner!==member.memberId" >
                    <el-col :span="2" @click="showMemberInfo(member)">
                        <el-icon color="skyblue"><Avatar /></el-icon>
                    </el-col>
                    <el-col :span="3" @click="showMemberInfo(member)">
                        <el-avatar :size="20" :src="member.avatarUrl"></el-avatar>
                    </el-col>
                    <el-col :span="2" @click="showMemberInfo(member)">
                            <el-icon v-if="member.gender===1" color="blue"><Male /></el-icon>
                            <el-icon v-else-if="member.gender===2" color="pink"><Female /></el-icon>
                            <el-icon v-else color="green">?</el-icon>
                    </el-col>
                    <el-col :span="12" @click="showMemberInfo(member)">
                        <el-text>{{member.memberName ?? member.memberName!='' ? member.memberName : member.nickName }}
                        </el-text>
                    </el-col>
                    <el-col :span="2" v-if="currentUserStore.currentGroup.owner!==currentUserStore.currentUser.userId"></el-col>
                    <el-col :span="2">
                        <el-icon v-if="member.state===0" style="color:green"><Bell /></el-icon>
                        <el-icon v-if="member.state===1" style="color:orange"><WarningFilled /></el-icon>
                        <el-icon v-if="member.state===2" style="color:cadetblue"><Clock /></el-icon>
                        <el-icon v-if="member.state===3"><CircleCloseFilled /></el-icon>
                    </el-col>
                    <el-col :span="2" v-if="currentUserStore.currentGroup.owner===currentUserStore.currentUser.userId">
                        <el-tooltip
                            effect="light"
                            content="移除群聊"
                            placement="top-start"
                        >
                            <el-icon color="orange" @click="removeMember(member)"><RemoveFilled /></el-icon>
                        </el-tooltip>
                    </el-col>
                </el-row>
            </el-card>
        </el-scrollbar>
    </el-card>
    <el-dialog title="成员信息" v-model="showMemberInfoFlag" width="30%">
        <el-row>
            <el-col :span="6">
                <el-avatar :size="80" shape="square" :src="currentMember.avatarUrl"></el-avatar>
            </el-col>
            <el-col :span="18">
                <el-row>
                    <el-col :span="5"><el-text>群昵称：</el-text></el-col>
                    <el-col :span="19"><el-text>{{currentMember.memberName}}</el-text></el-col>
                </el-row>
                <el-row>
                    <el-col :span="5"><el-text>昵称：</el-text></el-col>
                    <el-col :span="19"><el-text>{{currentMember.nickName}}</el-text></el-col>
                </el-row>
                <el-row>
                    <el-col :span="5"><el-text>性别：</el-text></el-col>
                    <el-col :span="19">
                            <el-icon size="20" v-if="currentMember.gender===1" color="blue"><Male /></el-icon>
                            <el-icon size="20"  v-else-if="currentMember.gender===2" color="pink"><Female /></el-icon>
                            <el-icon size="20"  v-else color="green">?</el-icon>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="5"><el-text>状态：</el-text></el-col>
                    <el-col :span="19">
                        <el-icon size="20" v-if="currentMember.state===0" style="color:green"><Bell /></el-icon>
                        <el-icon size="20" v-if="currentMember.state===1" style="color:orange"><WarningFilled /></el-icon>
                        <el-icon size="20" v-if="currentMember.state===2" style="color:cadetblue"><Clock /></el-icon>
                        <el-icon size="20" v-if="currentMember.state===3"><CircleCloseFilled /></el-icon>
                    </el-col>
                </el-row>
            </el-col>
        </el-row>
        <el-button v-if="currentMember.userId !== currentUserStore.currentUser.userId" plain @click="toSendMessage(currentMember.userId)">去聊天</el-button>
    </el-dialog>
    <el-dialog title="邀请进群" v-model="showInviteMember" width="1000px">
      <el-row>
        <el-col :span="2"><el-text style="font-size: 18px;">昵称：</el-text></el-col>
        <el-col :span="6">
          <el-input v-model="clientUserFilter.nickName" placeholder="昵称"></el-input>
        </el-col>
        <el-col :span="1"></el-col>
        <el-col :span="2"><el-text style="font-size: 18px;">邮箱：</el-text></el-col>
        <el-col :span="7">
          <el-input v-model="clientUserFilter.email" placeholder="邮箱"></el-input>
        </el-col>
        <el-col :span="1"></el-col>
        <el-col :span="5">
          <el-button type="primary" plain @click="FindUsers">查找</el-button>
          <el-button plain @click="resetFindUsers">重置</el-button>
        </el-col>
        <el-scrollbar height="400px">
          <el-table stripe :data="clientUserList" style="width: 950px">
            <el-table-column label="头像" width="100px">
              <template #default="scope">
                <el-avatar shape="square" :size="50" :src="scope.row.avatarUrl" />
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
            <el-table-column label="状态" >
              <template #default="scope">
                <el-icon size="30" v-if="scope.row.state===0" style="color:green"><Bell /></el-icon>
                <el-icon size="30" v-if="scope.row.state===1" style="color:orange"><WarningFilled /></el-icon>
                <el-icon size="30" v-if="scope.row.state===2" style="color:cadetblue"><Clock /></el-icon>
                <el-icon size="30" v-if="scope.row.state==3"><CircleCloseFilled /></el-icon>
              </template>
            </el-table-column>
            <el-table-column label="操作">
              <template #default="scope">
                <el-button v-if=" !groupMemberList.find(gm => gm.userId === scope.row.userId)" type="primary" plain @click="toInviteMember(scope.row.userId)">邀请</el-button>
                <el-text v-else>已在群中</el-text>
              </template>
            </el-table-column>
          </el-table>
        </el-scrollbar>
      </el-row>
    </el-dialog>
    <el-dialog title="修改群昵称" v-model="showEditMemberName" width="30%">
        <el-input maxlength="20" show-word-limit v-model="newMemberName" @keyup.enter="toEditMember">
          <template #append>
            <el-button plain @click="toEditMember">修改</el-button>
          </template>
        </el-input>
    </el-dialog>
</template>

<script lang="ts" setup>
import { getCurrentInstance, onMounted, ref, watch } from 'vue'
import useCurrentUserStore from '../stores/currentUser'
import useGroupMemberApi from '../api/groupMemberApi'
import { GroupMember, ClientUser, GroupMemberSearchFilter, GroupMemberModel, PrivateMessageSearchFilter, Friend, FriendSearchFilter, ClientUserSearchFilter } from '../types'
import { ElMessage, ElMessageBox } from 'element-plus'
import usePrivateMessageApi from '../api/privateMessageApi'
import useFriendApi from '../api/friendApi'
import useClientUserApi from '../api/clientUserApi'

let that: any = getCurrentInstance()!.appContext.config.globalProperties
const currentUserStore = useCurrentUserStore()
that.$axios.defaults.headers.common['Authorization'] = 'Bearer ' + currentUserStore.token

const groupMemberApi = useGroupMemberApi(that)
const privateMessageApi = usePrivateMessageApi(that)
const friendApi = useFriendApi(that)
const clientUserApi = useClientUserApi(that)

const groupMemberList = ref<GroupMemberModel[]>([])

const showEditMemberName = ref(false)

const showMemberInfoFlag = ref(false)
const currentMember = ref<GroupMemberModel>({} as GroupMemberModel)

const newMemberName = ref('')

const filter = ref<GroupMemberSearchFilter>({
    groupId: currentUserStore.currentGroup.groupId,
    memberName: ''
} as GroupMemberSearchFilter)

const clientUserFilter = ref<ClientUserSearchFilter>({
    nickName: '',
    email: ''  
} as ClientUserSearchFilter)
const clientUserList = ref<ClientUser[]>([])

const showInviteMember = ref(false)

const getGroupMemberList = () => {
    groupMemberApi.getListAsync(filter.value).then(res => {
        groupMemberList.value = res.data
    })
}

const FindUsers = () => {
  clientUserApi.getListAsync(clientUserFilter.value).then((res) => {
    if (res.statusCode === 200) {
      clientUserList.value = res.data
    } else {
      ElMessage.error('查找失败！')
    }
  })
}
const resetFindUsers = () => {
  clientUserFilter.value.email = ''
  clientUserFilter.value.nickName = ''
}

const toInviteMember = (userId: number) => {
    currentUserStore.connection.invoke("InviteGroupMember", currentUserStore.currentGroup.groupId, userId).then(() => {
        ElMessage.success('邀请成功！')
        showInviteMember.value = false
        getGroupMemberList()
    }).catch(() => {
        ElMessage.error('邀请失败！')
    })
}

onMounted(() => {
    getGroupMemberList()
})

const showMemberInfo = (member: GroupMemberModel) => {
    showMemberInfoFlag.value = true
    currentMember.value = member
}

watch(() => currentUserStore.currentGroup.groupId,
 () => {
    filter.value.groupId = currentUserStore.currentGroup.groupId
    filter.value.memberName = ''
    getGroupMemberList()
 }
)

const toEditMember = () => {
  let newMember: GroupMember = {} as GroupMember
  for(let i=0; i<groupMemberList.value.length; i++){
    if(groupMemberList.value[i].memberId == currentUserStore.currentUser.userId){
      newMember = groupMemberList.value[i]
      newMember.memberName = newMemberName.value
      break
    }
  }
  groupMemberApi.updateAsync(newMember).then(() => {
    ElMessage.success('修改成功！')
    showEditMemberName.value = false
    getGroupMemberList()
  }).catch(() => {
    ElMessage.error('修改失败！')
  })
 
}

const removeMember = (member: GroupMemberModel) => {
  ElMessageBox.confirm('确定移除该成员吗？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
    }).then(() => {
        currentUserStore.chatGptMessages = []
        currentUserStore.connection.invoke("RemoveGroupMember", member.groupMemberId).then(() => {
            ElMessage.success('移除成功')
            getGroupMemberList()
        })
    }).catch(() => {
        ElMessage.info('已取消')
    })
  
}

currentUserStore.connection.on("ReceiveRemoveGroupMember", () => {
  getGroupMemberList()
})

const getMessages = async () => {
    const searchFilter: PrivateMessageSearchFilter = {} as PrivateMessageSearchFilter
    searchFilter.senderId = currentUserStore.currentUser.userId;
    searchFilter.receiverId = currentUserStore.currentFriend.friendUserId;

    try {
        const res = await privateMessageApi.getPageAsync(searchFilter);
        if (res.statusCode == 200) {
            currentUserStore.setPrivateMessages(res.data.reverse());
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

const toSendMessage = (userId: number) => {
  let isExist = false
  currentUserStore.messageMenu = '0'
  for (let i=0; i<currentUserStore.friends.length; i++){
    if (currentUserStore.friends[i].friendUserId == userId){
      isExist = true
      currentUserStore.friends[i].unread = 0
      currentUserStore.currentFriend = currentUserStore.friends[i]
      getMessages()
      that.$router.push('/privateMessage')
    }
  }
  if (!isExist){
    let newFriend: Friend = {
      userId : currentUserStore.currentUser.userId,
      friendUserId : userId,
      type : 1
    } as Friend

    friendApi.addAsync(newFriend).then(()=>{
      loadFriends()
      setTimeout(() => {
        for (let i=0; i<currentUserStore.friends.length; i++){
          if (currentUserStore.friends[i].friendUserId == userId){
            currentUserStore.friends[i].unread = 0
            currentUserStore.currentFriend = currentUserStore.friends[i]
            getMessages()
            that.$router.push('/privateMessage')
          }
        }
      }, 100)
    }) 

  }
}


const loadFriends = () => {
    const filter = ref<FriendSearchFilter>({
      remark: ''
    } as FriendSearchFilter)
    filter.value.userId = currentUserStore.currentUser.userId
    friendApi.getListAsync(filter.value).then((res) => {
        if (res.statusCode === 200) {
            currentUserStore.friends = res.data
            for (let i=0; i<currentUserStore.friends.length; i++){
                currentUserStore.friends[i].unread = 0
            }
            currentUserStore.followFriends= currentUserStore.friends.filter(f => f.type === 0)
            currentUserStore.commonFriends = currentUserStore.friends.filter(f => f.type === 1)
            currentUserStore.blacklistFriends = currentUserStore.friends.filter(f => f.type === 2)
        }else{
            ElMessage.error('好友列表获取失败！')
        }
    })
}

currentUserStore.connection.on("ReceiveInviteGroupMember", () => {
    getGroupMemberList()
});
currentUserStore.connection.on("ReceiveQuitGroup", () => {
    getGroupMemberList()
});
</script>
<style scoped>
.member-info {
    cursor: pointer;
}
</style>
