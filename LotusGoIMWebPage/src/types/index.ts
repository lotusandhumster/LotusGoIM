export interface ClientUser {
    userId: number;
    nickName: string;
    email: string;
    password: string;
    avatarUrl?: string;
    gender: number;
    description: string;
    state: number;
}

export interface Friend {
    friendId: number;
    userId: number;
    friendUserId: number;
    remark: string;
    type: number;
    isDeleted: boolean;
}

export interface Group {
    groupId: number;
    name: string;
    avatarUrl?: string;
    owner: number;
    unread: number;
    description: string;
    isDeleted: boolean;
}

export interface GroupMember {
    groupMemberId: number;
    groupId: number;
    memberId: number;
    memberName: string;
    isDeleted: boolean;
}

export interface GroupMessage {
    groupMessageId: number;
    groupId: number;
    userId: number;
    type: number;
    content: string;
    sendTime: Date;
    fileUrl?: string;
    isDeleted: boolean;
}

export interface PrivateMessage {
    privateMessageId: number;
    senderId: number;
    receiverId: number;
    type: number;
    content: string;
    sendTime: Date;
    fileUrl?: string;
    isDeleted: boolean;
}

export interface ForgetPasswordModel {
    email: string;
    password: string;
    confirmPassword: string;
    verificationCode: string;
}

export interface FriendModel {
    friendId: number;
    userId: number;
    friendUserId: number;
    remark: string;
    type: number;
    isDeleted: boolean;
    nickName: string;
    email: string;
    avatarUrl?: string;
    gender: number;
    description: string;
    state: number;
    unread: number;
}

export interface GroupMemberModel {
    groupMemberId: number;
    groupId: number;
    memberId: number;
    memberName: string;
    isDeleted: boolean;
    userId: number;
    nickName: string;
    email: string;
    avatarUrl?: string;
    gender: number;
    description: string;
    state: number;
}

export interface GroupMessageModel {
    groupMessageId: number;
    groupId: number;
    userId: number;
    type: number;
    content: string;
    sendTime: Date;
    fileUrl?: string;
    isDeleted: boolean;
    nickName: string;
    email: string;
    avatarUrl?: string;
    gender: number;
    description: string;
}

export interface LoginModel {
    email: string;
    password: string;
}

export interface PrivateMessageModel {
    privateMessageId: number;
    senderId: number;
    receiverId: number;
    type: number;
    content: string;
    sendTime: Date;
    fileUrl?: string;
    isDeleted: boolean;
    nickName: string;
    email: string;
    avatarUrl?: string;
    gender: number;
    description: string;
}

export interface RegisterClientUserModel {
    nickName: string;
    email: string;
    password: string;
    avatarUrl?: string;
    gender: number;
    description: string;
    state: number;
    confirmPassword: string;
    verificationCode: string;
}

export interface ResultModel<T> {
    statusCode: number;
    message: string;
    data: T;
}

export interface PageResultModel<T> {
    statusCode: number;
    message: string;
    data: T;
    totalCount: number;
}

export interface PageParam {
    pageIndex: number;
    pageSize: number;
}

export interface ClientUserSearchFilter extends PageParam {
    nickName: string;
    email: string;
    gender: number;
    state: number;
    isDeleted: boolean;
}

export interface FriendSearchFilter extends PageParam {
    nickName: string;
    email: string;
    classifyId: number;
    remark: string;
    userId: number;
    state: number;
    type: number;
    isDeleted: boolean;
}

export interface GroupMemberSearchFilter extends PageParam {
    groupId: number;
    memberId: number;
    memberName: string;
    genderType: number;
    isDeleted: boolean;
}

export interface GroupMessageSearchFilter extends PageParam {
    groupId?: number;
    clientUserId?: number;
    content?: string;
    type?: number;
}

export interface GroupSearchFilter extends PageParam {
    name: string;
    owner: number;
    isDeleted: boolean;
    memberId: number;
}

export interface PrivateMessageSearchFilter extends PageParam {
    senderId?: number;
    receiverId?: number;
    content?: string;
    type?: number;
}

export interface ChatGptMessage {
    chatGptMessageId: number;
    userId: number;
    role: string;
    content: string;
    isDeleted: boolean;
    sendTime: Date;
}

export interface ChatGptMessageWithUserModel extends ChatGptMessage {
    nickName: string;
    email: string;
    avatarUrl?: string;
    gender: number;
    description: string;
}

export interface ChatGptMessageSearchFilter extends PageParam {
    userId?: number;
    role?: string;
    isDeleted?: boolean;
}