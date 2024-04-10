import { RegisterClientUserModel, ForgetPasswordModel, ResultModel } from '../types/index';

let _that: any;

async function registerAsync(model: RegisterClientUserModel): Promise<ResultModel<boolean>> {
    return (await _that.$axios.post('/Register/Register', model)).data;
}

async function forgetPasswordAsync(model: ForgetPasswordModel): Promise<ResultModel<boolean>> {
    return (await _that.$axios.put('/Register/ForgetPassword', model)).data;
}

export default function useRegisterApi(that: any) {
    _that = that;
    return {
        registerAsync,
        forgetPasswordAsync
    };
}
