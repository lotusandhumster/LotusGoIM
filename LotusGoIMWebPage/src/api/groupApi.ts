import { Group, ResultModel, GroupSearchFilter, PageResultModel } from '../types/index';

let _that: any;

async function getAsync(id: number): Promise<ResultModel<Group>> {
    return (await _that.$axios.get(`/Group/Get?id=${id}`)).data;
}

async function addAsync(group: Group): Promise<ResultModel<boolean>> {
    return (await _that.$axios.post('/Group/Add', group)).data;
}

async function updateAsync(group: Group): Promise<ResultModel<boolean>> {
    return (await _that.$axios.put('/Group/Update', group)).data;
}

async function deleteAsync(id: number): Promise<ResultModel<boolean>> {
    return (await _that.$axios.delete(`/Group/Delete?id=${id}`)).data;
}

async function getListAsync(filter: GroupSearchFilter): Promise<ResultModel<Group[]>> {
    return (await _that.$axios.get('/Group/GetList', { params: filter })).data;
}

async function getPageAsync(filter: GroupSearchFilter): Promise<PageResultModel<Group[]>> {
    return (await _that.$axios.get('/Group/GetPage', { params: filter })).data;
}

export default function useGroupApi(that: any) {
    _that = that;
    return {
        getAsync,
        addAsync,
        updateAsync,
        deleteAsync,
        getListAsync,
        getPageAsync
    };
}
