using LotusGoIMWebAPI.Common;
using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LotusGoIMWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IClientUserService _clientUserService;
        private readonly JwtHelper _jwtHelper;
        private readonly RedisHelper _redisHelper;

        public LoginController(IClientUserService clientUserService, JwtHelper jwtHelper, RedisHelper redisHelper)
        {
            this._clientUserService = clientUserService;
            this._jwtHelper = jwtHelper;
            this._redisHelper = redisHelper;
        }

        [HttpPost("Login")]
        public async Task<ResultModel<string>> LoginAsync([FromBody] LoginModel loginModel)
        {
            loginModel.Password = EncryptHelper.EncryptPassword(loginModel.Password);
            var user = await _clientUserService.LoginAsync(loginModel);
            if (user == null)
            {
                return ResultModelFactory.ResultModelNotFound<string>();
            }

            var token = _jwtHelper.GenerateToken(user);
            var userJson = JsonConvert.SerializeObject(user);
            _redisHelper.GetDatabase().StringSet(token, userJson, TimeSpan.FromDays(1));

            return ResultModelFactory.ResultModelSusccess(token);
        }

        [HttpPost("LoginOut")]
        [Authorize(Roles = "user")]
        public ResultModel<object> LoginOut()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            _redisHelper.GetDatabase().KeyDelete(token);
            return ResultModelFactory.ResultModelSusccess<object>();
        }

    }
}
