namespace LotusGoIMWebAPI.Common.ResultModel
{
    public static class ResultModelFactory
    {
        public static ResultModel<T> ResultModelSusccess<T>(string message = "Success")
        {
            return new ResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.Success,
                Message = message
            };
        }

        public static ResultModel<T> ResultModelSusccess<T>(T? data, string message = "Success")
        {
            return new ResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.Success,
                Message = message,
                Data = data
            };
        }
        public static ResultModel<T> ResultModelNotFound<T>(string message = "Not Found")
        {
            return new ResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.NotFound,
                Message = message
            };
        }

        public static ResultModel<T> ResultModelBadRequest<T>(string message = "Bad Request")
        {
            return new ResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.BadRequest,
                Message = message
            };
        }

        public static ResultModel<T> ResultModelForbidden<T>(string message = "Forbidden")
        {
            return new ResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.Forbidden,
                Message = message
            };
        }

        public static ResultModel<T> ResultModelUnauthorized<T>(string message = "Unauthorized")
        {
            return new ResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.Unauthorized,
                Message = message
            };
        }

        public static ResultModel<T> ResultModelInternalServerError<T>(string message = "Internal Server Error")
        {
            return new ResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.InternalServerError,
                Message = message
            };
        }

        public static PageResultModel<T> PageResultModelSuccess<T>(T? data, int totalCount, string message = "Success")
        {
            return new PageResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.Success,
                Message = message,
                Data = data,
                TotalCount = totalCount
            };
        }

        public static PageResultModel<T> PageResultModelNotFound<T>(string message = "Not Found")
        {
            return new PageResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.NotFound,
                Message = message
            };
        }

        public static PageResultModel<T> PageResultModelBadRequest<T>(string message = "Bad Request")
        {
            return new PageResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.BadRequest,
                Message = message
            };
        }

        public static PageResultModel<T> PageResultModelInternalServerError<T>(string message = "Internal Server Error")
        {
            return new PageResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.InternalServerError,
                Message = message
            };
        }

        public static PageResultModel<T> PageResultModelForbidden<T>(string message = "Forbidden")
        {
            return new PageResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.Forbidden,
                Message = message
            };
        }

        public static PageResultModel<T> PageResultModelUnauthorized<T>(string message = "Unauthorized")
        {
            return new PageResultModel<T>
            {
                StatusCode = Enums.ResultStatusCode.Unauthorized,
                Message = message
            };
        }
    }
}
