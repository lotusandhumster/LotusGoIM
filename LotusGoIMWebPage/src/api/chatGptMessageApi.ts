import { ChatGptMessage, ChatGptMessageWithUserModel, ChatGptMessageSearchFilter, ResultModel, PageResultModel } from '../types/index';

let _that: any;
async function addAsync(chatGptMessage: ChatGptMessage): Promise<ResultModel<boolean>> {
    return (await _that.$axios.post('/ChatGptMessage/Add', chatGptMessage)).data;
}

async function deleteAsync(id: number): Promise<ResultModel<boolean>> {
    return (await _that.$axios.delete(`/ChatGptMessage/Delete?id=${id}`)).data;
}

async function getAsync(id: number): Promise<ResultModel<ChatGptMessageWithUserModel>> {
   return (await _that.$axios.get(`/ChatGptMessage/Get?id=${id}`)).data;
}

async function getListAsync(filter: ChatGptMessageSearchFilter): Promise<ResultModel<ChatGptMessageWithUserModel[]>> {
    return (await _that.$axios.get('/ChatGptMessage/GetList', { params: filter })).data;
}

async function getPageAsync(filter: ChatGptMessageSearchFilter): Promise<PageResultModel<ChatGptMessageWithUserModel[]>> {
    return (await _that.$axios.get('/ChatGptMessage/GetPage', { params: filter })).data;
}

async function clearAllMessageAsync(id: number): Promise<ResultModel<boolean>> {
    return (await _that.$axios.delete(`/ChatGptMessage/ClearAllMessage?userId=${id}`)).data;
}

export default function useChatGPTMessageApi(that: any) {
    _that = that;
    return {
        addAsync,
        deleteAsync,
        getAsync,
        getListAsync,
        getPageAsync,
        clearAllMessageAsync
    };
}
