import { PrivateMessage, ResultModel, PrivateMessageModel, PrivateMessageSearchFilter, PageResultModel } from '../types/index';

let _that: any;

async function addAsync(privateMessage: PrivateMessage): Promise<ResultModel<boolean>> {
    return (await _that.$axios.post('/PrivateMessage/Add', privateMessage)).data;
}

async function deleteAsync(id: number): Promise<ResultModel<boolean>> {
    return (await _that.$axios.delete(`/PrivateMessage/Delete?id=${id}`)).data;
}

async function getAsync(id: number): Promise<ResultModel<PrivateMessageModel>> {
   return (await _that.$axios.get(`/PrivateMessage/Get?id=${id}`)).data;
}

async function getListAsync(filter: PrivateMessageSearchFilter): Promise<ResultModel<PrivateMessageModel[]>> {
    return (await _that.$axios.get('/PrivateMessage/GetList', { params: filter })).data;
}

async function getPageAsync(filter: PrivateMessageSearchFilter): Promise<PageResultModel<PrivateMessageModel[]>> {
    return (await _that.$axios.get('/PrivateMessage/GetPage', { params: filter })).data;
}

export default function usePrivateMessageApi(that: any) {
    _that = that;
    return {
        addAsync,
        deleteAsync,
        getAsync,
        getListAsync,
        getPageAsync
    };
}
