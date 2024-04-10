import { defineStore } from 'pinia'
import { ClientUser, FriendModel, LoginModel, PrivateMessageModel, Group, ChatGptMessageWithUserModel, GroupMessageModel } from '../types'
import { HubConnection } from '@microsoft/signalr'

export const useCurrentUserStore = defineStore("",{
  persist: true,
  state () {
    return {
      currentUser: {} as ClientUser,
      token: '' as string ,
      loginModel: {} as LoginModel,
      rememberPassword: false as boolean,
      currentFriend: {} as FriendModel,
      privateMessages: [] as PrivateMessageModel[],
      connection: {} as HubConnection,
      friends: [] as FriendModel[],
      commonFriends: [] as FriendModel[],
      followFriends: [] as FriendModel[],
      blacklistFriends: [] as FriendModel[],
      groupList: [] as Group[],
      createdGroupList: [] as Group[],
      joinGroupList: [] as Group[],
      currentGroup: {} as Group,
      groupMessages: [] as GroupMessageModel[],
      messageMenu: '0' as string,
      chatGptMessages: [] as ChatGptMessageWithUserModel[]
    }
  },
  getters: {
    isLogin: (state) => state.currentUser !== null,
    getToken: (state) => state.token,
    getCurrentUser: (state) => state.currentUser,
    getLoginModel: (state) => state.loginModel,
    getPrivateMessages: (state) => state.privateMessages,
    getGroupMessages: (state) => state.groupMessages,
    getChatGptMessages: (state) => state.chatGptMessages
  },
  actions: {
    login (user: ClientUser, token: string) {
      this.currentUser = user
      this.token = token
    },
    logout () {
      this.token = ''
      this.currentFriend = {} as FriendModel
      this.connection.stop()
      this.friends = [] as FriendModel[]
      this.commonFriends = [] as FriendModel[]
      this.followFriends = [] as FriendModel[]
      this.blacklistFriends = [] as FriendModel[]
      this.groupList = [] as Group[]
      this.joinGroupList = [] as Group[]
      this.createdGroupList = [] as Group[]
      this.currentGroup = {} as Group
      this.groupMessages = [] as GroupMessageModel[]
      this.chatGptMessages = [] as ChatGptMessageWithUserModel[]
    },
    updateUserInfo (user: ClientUser) {
      this.currentUser = user
    },
    updateToken (token: string) {
      this.token = token
    },
    setLoginModel (loginModel: LoginModel) {
      this.loginModel = loginModel
    },
    clearLoginModel () {
      this.loginModel = {} as LoginModel
    },
    setPrivateMessages(messages : PrivateMessageModel[]) {
      this.privateMessages = [...messages]
    },
    setGroupMessages(messages : GroupMessageModel[]) {
      this.groupMessages = [...messages]
    },
  }
})
export default useCurrentUserStore