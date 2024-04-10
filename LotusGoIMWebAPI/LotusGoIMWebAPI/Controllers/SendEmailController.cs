using LotusGoIMWebAPI.Common;
using LotusGoIMWebAPI.Common.ResultModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace LotusGoIMWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailController : ControllerBase
    {
        private readonly EmailHelper _emailHelper;
        private readonly RedisHelper _redisHelper;

        public SendEmailController(EmailHelper emailHelper, RedisHelper redisHelper)
        {
            this._emailHelper = emailHelper;
            this._redisHelper = redisHelper;
        }

        [HttpGet("GetVerificationCode")]
        [AllowAnonymous]
        public ResultModel<Object> GetVerificationCode(string emailAddress)
        {
            var verificationCode = new Random().Next(100000, 999999);
            var subject = "LotusGoIM 用户注册验证码";
            var body = $"您的验证码是：{verificationCode}，有效期5分钟。";
            _emailHelper.SendMessage(emailAddress, subject, body);

            _redisHelper.GetDatabase().StringSet(emailAddress, verificationCode, TimeSpan.FromMinutes(5));

            return ResultModelFactory.ResultModelSusccess<object>();
        }
    }
}
