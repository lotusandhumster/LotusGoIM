namespace LotusGoIMWebAPI.Common.ResultModel
{
    public class PageResultModel<T>: ResultModel<T>
    {
        public int TotalCount { get; set; }
    }
}
