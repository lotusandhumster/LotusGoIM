import { ResultModel } from '../types/index';

let _that: any;

async function chatAsync(prompt: string): Promise<ResultModel<string>> {
    return (await _that.$axios.post('/OpenAI/Chat', prompt)).data;
}

async function chatWithHistoryAsync(prompt: string): Promise<ResultModel<string>> {
    return (await _that.$axios.post('/OpenAI/ChatWithHistory', prompt)).data;
}

async function QuickReplyAsync(prompt: string): Promise<ResultModel<string>> {
    return (await _that.$axios.post('/OpenAI/QuickReply', prompt)).data;
}

export default function useOpenAIApi(that: any) {
    _that = that;
    return {
        chatAsync,
        chatWithHistoryAsync,
        QuickReplyAsync
    };
}
