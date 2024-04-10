import { ClientUser, ResultModel, PageResultModel, ClientUserSearchFilter } from '../types/index';

let _that: any;

async function addAsync(clientUser: ClientUser): Promise<ResultModel<boolean>> {
    return (await _that.$axios.post('/ClientUser/Add', clientUser)).data;
}

async function deleteAsync(id: number): Promise<ResultModel<boolean>> {
    return (await _that.$axios.delete(`/ClientUser/Delete?id=${id}`)).data;
}

async function updateAsync(clientUser: ClientUser): Promise<ResultModel<boolean>> {
    return (await _that.$axios.put('/ClientUser/Update', clientUser)).data;
}

async function updatePasswordAsync(id: number, orderPassword:string, password: string): Promise<ResultModel<boolean>> {
    return (await _that.$axios.put(`/ClientUser/UpdatePassword?id=${id}&orderPassword=${orderPassword}&password=${password}`)).data;
}

async function updatePasswordAdminAsync(id: number, password: string): Promise<ResultModel<boolean>> {
    return (await _that.$axios.put(`/ClientUser/UpdatePasswordAdmin?id=${id}&password=${password}`)).data;
}

async function getAsync(id: number): Promise<ResultModel<ClientUser>> {
    return (await _that.$axios.get(`/ClientUser/Get?id=${id}`)).data;
}

async function getListAsync(filter: ClientUserSearchFilter): Promise<ResultModel<ClientUser[]>> {
    return (await _that.$axios.get('/ClientUser/GetList', { params: filter })).data;
}

async function getPageAsync(filter: ClientUserSearchFilter): Promise<PageResultModel<ClientUser[]>> {
    return (await _that.$axios.get('/ClientUser/GetPage', { params: filter })).data;
}

async function getUserByJwtAsync(): Promise<ResultModel<ClientUser>> {
    return (await _that.$axios.get('/ClientUser/GetUserByJwt')).data;
}

export default function useClientUserApi(that: any) {
    _that = that;
    return {
        addAsync,
        deleteAsync,
        updateAsync,
        updatePasswordAsync,
        getAsync,
        getListAsync,
        getPageAsync,
        getUserByJwtAsync,
        updatePasswordAdminAsync
    };
}
