import { LoginModel, ResultModel } from '../types/index';

let _that: any;

async function loginAsync(loginModel: LoginModel): Promise<ResultModel<string>> {
    return (await _that.$axios.post('/Login/Login', loginModel)).data;
}

async function logoutAsync(): Promise<ResultModel<any>> {
    return (await _that.$axios.post('/Login/LoginOut')).data;
}

export default function useLoginApi(that: any) {
    _that = that;
    return {
        loginAsync,
        logoutAsync
    };
}
