import { GroupMessage, ResultModel, GroupMessageModel, GroupMessageSearchFilter, PageResultModel } from '../types/index';

let _that: any;

async function addAsync(groupMessage: GroupMessage): Promise<ResultModel<boolean>> {
    return (await _that.$axios.post('/GroupMessage/Add', groupMessage)).data;
}

async function deleteAsync(id: number): Promise<ResultModel<boolean>> {
    return (await _that.$axios.delete(`/GroupMessage/Delete?id=${id}`)).data;
}

async function getAsync(id: number): Promise<ResultModel<GroupMessageModel>> {
    return (await _that.$axios.get(`/GroupMessage/Get?id=${id}`)).data;
}

async function getListAsync(filter: GroupMessageSearchFilter): Promise<ResultModel<GroupMessageModel[]>> {
    return (await _that.$axios.get('/GroupMessage/GetList', { params: filter })).data;
}

async function getPageAsync(filter: GroupMessageSearchFilter): Promise<PageResultModel<GroupMessageModel[]>> {
    return (await _that.$axios.get('/GroupMessage/GetPage', { params: filter })).data;
}

export default function useGroupMessageApi(that: any) {
    _that = that;
    return {
        addAsync,
        deleteAsync,
        getAsync,
        getListAsync,
        getPageAsync
    };
}
