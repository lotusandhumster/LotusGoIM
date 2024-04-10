import { ResultModel } from '../types/index';

let _that: any;

async function getVerificationCode(emailAddress: string): Promise<ResultModel<Object>> {
    return (await _that.$axios.get(`/SendEmail/GetVerificationCode?emailAddress=${emailAddress}`)).data;
}

export default function useSendEmailApi(that: any) {
    _that = that;
    return {
        getVerificationCode
    };
}
