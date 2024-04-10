using LotusGoIMWebAPI.Common.Enums;

namespace LotusGoIMWebAPI.Common.ResultModel
{
    public class ResultModel<T>
    {
        public ResultStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
