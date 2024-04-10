import { Friend, ResultModel, FriendModel, FriendSearchFilter } from '../types/index';

let _that: any;

async function addAsync(friend: Friend): Promise<ResultModel<boolean>> {
    return (await _that.$axios.post('/Friend/Add', friend)).data;
}

async function deleteAsync(id: number): Promise<boolean> {
    return (await _that.$axios.delete(`/Friend/Delete?id=${id}`)).data;
}

async function getCountAsync(userId: number): Promise<number> {
    return (await _that.$axios.get(`/Friend/GetCount?userId=${userId}`)).data;
}

async function getAsync(id: number): Promise<ResultModel<FriendModel>> {
    return (await _that.$axios.get(`/Friend/Get?id=${id}`)).data;
}

async function getListAsync(filter: FriendSearchFilter): Promise<ResultModel<FriendModel[]>> {
    return (await _that.$axios.get('/Friend/GetList', { params: filter })).data;
}

async function updateAsync(friend: Friend): Promise<ResultModel<boolean>> {
    return (await _that.$axios.put('/Friend/Update', friend)).data;
}

export default function useFriendApi(that: any) {
    _that = that;
    return {
        addAsync,
        deleteAsync,
        getCountAsync,
        getAsync,
        getListAsync,
        updateAsync
    };
}
