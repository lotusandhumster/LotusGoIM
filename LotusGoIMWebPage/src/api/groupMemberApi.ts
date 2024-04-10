import { GroupMember, ResultModel, GroupMemberModel, GroupMemberSearchFilter } from '../types/index';

let _that: any;

async function addAsync(groupMember: GroupMember): Promise<ResultModel<boolean>> {
    return (await _that.$axios.post('/GroupMember/Add', groupMember)).data;
}

async function deleteAsync(id: number): Promise<ResultModel<boolean>> {
    return (await _that.$axios.delete(`/GroupMember/Delete?id=${id}`)).data;
}

async function updateAsync(groupMember: GroupMember): Promise<ResultModel<boolean>> {
    return (await _that.$axios.put('/GroupMember/Update', groupMember)).data;
}

async function getAsync(id: number): Promise<ResultModel<GroupMemberModel>> {
    return (await _that.$axios.get(`/GroupMember/Get?id=${id}`)).data;
}

async function getListAsync(filter: GroupMemberSearchFilter): Promise<ResultModel<GroupMemberModel[]>> {
    return (await _that.$axios.get('/GroupMember/GetList', { params: filter })).data;
}

async function getCountAsync(groupId: number): Promise<ResultModel<number>> {
    return (await _that.$axios.get(`/GroupMember/GetCount?groupId=${groupId}`)).data;
}

export default function useGroupMemberApi(that: any) {
    _that = that;
    return {
        addAsync,
        deleteAsync,
        updateAsync,
        getAsync,
        getListAsync,
        getCountAsync
    };
}
