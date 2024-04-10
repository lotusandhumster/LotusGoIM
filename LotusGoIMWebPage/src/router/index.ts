import {createRouter, createWebHashHistory} from 'vue-router'
import UserLogin from '../pages/UserLogin.vue'
import HomePage from '../pages/HomePage.vue'
import RegisterPage from '../pages/RegisterPage.vue'
import ForgetPasswordPage from '../pages/ForgetPasswordPage.vue'
import PrivateMessagePage from '../pages/PrivateMessagePage.vue'
import GroupMessagePage from '../pages/GroupMessagePage.vue'
import NpcMessagePage from '../pages/NpcMessagePage.vue'
import AdminManagerPage from '../pages/ManagePage/AdminManagePage.vue'
import ClientUserManagePage from '../pages/ManagePage/ClientUserManagePage.vue'
import ChatGPTMessageManagePage from '../pages/ManagePage/ChatGPTMessageManagePage.vue'
import GroupManagePage from '../pages/ManagePage/GroupManagePage.vue'
import GroupMessageManagePage from '../pages/ManagePage/GroupMessageManagePage.vue'
import PrivateMessageManagePage from '../pages/ManagePage/PrivateMessageManagePage.vue'

const router = createRouter({
  history: createWebHashHistory(),
  routes: [
    {
      path: '/',
      name: 'UserLogin',
      component: UserLogin
    },
    {
      path: '/admin',
      name: 'Admin',
      component: AdminManagerPage,
      children: [
        {
          name: 'ClientUserManage',
          path: '/clientUserManage',
          component: ClientUserManagePage
        },
        {
          name: 'ChatGPTMessageManage',
          path: '/chatpgtMessageManage',
          component: ChatGPTMessageManagePage
        },
        {
          name: 'GroupManage',
          path: '/groupManage',
          component: GroupManagePage
        },
        {
          name: 'GroupMessageManage',
          path: '/groupMessageManage',
          component: GroupMessageManagePage
        },
        {
          name: 'PrivateMessagePageManage',
          path: '/privateMessageManage',
          component: PrivateMessageManagePage
        }
      ]
    },
    {
      name: 'HomePage',
      path: '/home',
      component: HomePage,
      children: [
        {
          name: 'PrivateMessagePage',
          path: '/privateMessage',
          component: PrivateMessagePage,
        },
        {
          name: 'GroupMessagePage',
          path: '/groupMessage',
          component: GroupMessagePage,
        },
        {
          name: 'NpcMessagePage',
          path: '/npcMessage',
          component: NpcMessagePage,
        }
      ]
    },
    {
      name: 'RegisterPage',
      path: '/register',
      component: RegisterPage,
    },
    {
      name: 'ForgetPasswordPage',
      path: '/forgetPassword',
      component: ForgetPasswordPage,
    }
  ],
})

export default router