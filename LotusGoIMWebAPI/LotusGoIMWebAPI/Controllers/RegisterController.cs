using LotusGoIMWebAPI.Common;
using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LotusGoIMWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IClientUserService _clientUserService;
        private readonly RedisHelper _redisHelper;

        public RegisterController(IClientUserService clientUserService, RedisHelper redisHelper)
        {
            _clientUserService = clientUserService;
            _redisHelper = redisHelper;
        }

        [HttpPost("Register")]
        public async Task<ResultModel<bool>> RegisterAsync([FromBody]RegisterClientUserModel model)
        {
            if(model.VerificationCode != _redisHelper.GetDatabase().StringGet(model.Email))
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("验证码错误");
            }
            if (model.Password != model.ConfirmPassword)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("两次密码不一致");
            }
            var user = new ClientUser
            {
                Email = model.Email,
                Password = model.Password,
                NickName = model.NickName,
                AvatarUrl = model.AvatarUrl,
                Gender = model.Gender,
                Description = model.Description,
                IsDeleted = false
            };
            return await _clientUserService.AddAsync(user);

        }

        [HttpPut("ForgetPassword")]
        public async Task<ResultModel<bool>> ForgetPasswordAsync([FromBody]ForgetPasswordModel model)
        {
            if (model.VerificationCode != _redisHelper.GetDatabase().StringGet(model.Email))
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("验证码错误");
            }
            var user = await _clientUserService.GetByEmailAsync(model.Email);
            if (user == null)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("用户不存在");
            }
            return await _clientUserService.UpdatePasswordAsync(user.UserId, model.Password);
        }
    }
}
