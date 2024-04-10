using LotusGoIMWebAPI.Common;
using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Services;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LotusGoIMWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "user")]
    public class ClientUserController : ControllerBase
    {
        private readonly IClientUserService _clientUserService;
        private readonly RedisHelper _redisHelper;

        public ClientUserController(IClientUserService clientUserService, RedisHelper redisHelper)
        {
            this._clientUserService = clientUserService;
            this._redisHelper = redisHelper;
        }

        [HttpPost("Add")]
        public async Task<ResultModel<bool>> AddAsync([FromBody] ClientUser user)
        {
            if (user == null)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>();
            }
            var result = await _clientUserService.AddAsync(user);
            return result;
        }

        [HttpGet("Get")]
        public async Task<ResultModel<ClientUser>> GetAsync(int id)
        {
            var user = await _clientUserService.GetAsync(id);
            if (user == null)
            {
                return ResultModelFactory.ResultModelNotFound<ClientUser>();
            }
            return ResultModelFactory.ResultModelSusccess(user);
        }


        [HttpPut("Update")]
        public async Task<ResultModel<bool>> UpdateAsync([FromBody] ClientUser user)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            if (user == null)
            {
                return ResultModelFactory.ResultModelNotFound<bool>();
            }
            var result = await _clientUserService.UpdateAsync(user);
            var json = _redisHelper.GetDatabase().StringGet(token);
            var currentUser = JsonConvert.DeserializeObject<ClientUser>(json.ToString());

            if (currentUser != null && user.UserId == currentUser.UserId)
            {
                _redisHelper.GetDatabase().StringSet(token, JsonConvert.SerializeObject(user), TimeSpan.FromDays(1));
            }
            return result;
        }

        [HttpPut("UpdatePassword")]
        public async Task<ResultModel<bool>> UpdatePasswordAsync(int id, string orderPassword, string password)
        {
            var clientUser = await _clientUserService.GetAsync(id);

            if (clientUser is null)
            {
                return ResultModelFactory.ResultModelNotFound<bool>("没有找到用户");
            }

            if (clientUser!.Password != EncryptHelper.EncryptPassword(orderPassword))
            {
                return ResultModelFactory.PageResultModelInternalServerError<bool>("旧密码错误");
            }

            var result = await _clientUserService.UpdatePasswordAsync(id, password);
            return result;
        }

        [HttpPut("UpdatePasswordAdmin")]
        public async Task<ResultModel<bool>> UpdatePasswordAdminAsync(int id, string password)
        {
            var clientUser = await _clientUserService.GetAsync(id);
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var json = _redisHelper.GetDatabase().StringGet(token);
            var user = JsonConvert.DeserializeObject<ClientUser>(json.ToString());
            if (user == null)
            {
                ResultModelFactory.ResultModelNotFound<ClientUser>();
            }
            if (user!.UserId != 1)
            {
                ResultModelFactory.ResultModelUnauthorized<bool>();
            }
            if (clientUser is null)
            {
                return ResultModelFactory.ResultModelNotFound<bool>("没有找到用户");
            }

            var result = await _clientUserService.UpdatePasswordAsync(id, password);
            return result;
        }

        [HttpDelete("Delete")]
        public async Task<ResultModel<bool>> DeleteAsync(int id)
        {
            if (id == 1)
            {
                ResultModelFactory.ResultModelUnauthorized<bool>();
            }
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var json = _redisHelper.GetDatabase().StringGet(token);
            var user = JsonConvert.DeserializeObject<ClientUser>(json.ToString());
            if (user == null)
            {
                ResultModelFactory.ResultModelNotFound<ClientUser>();
            }
            if (user!.UserId != 1)
            {
                ResultModelFactory.ResultModelUnauthorized<bool>();
            }
            var result = await _clientUserService.DeleteAsync(id);
            if (result)
            {
                return ResultModelFactory.ResultModelSusccess<bool>(result);
            }
            return ResultModelFactory.ResultModelNotFound<bool>();
        }

        [HttpGet("GetList")]
        public async Task<ResultModel<List<ClientUser>>> GetListAsync([FromQuery] ClientUserSearchFilter filter)
        {
            var users = await _clientUserService.GetListAsync(filter);
            if (users == null)
            {
                return ResultModelFactory.ResultModelNotFound<List<ClientUser>>();
            }
            return ResultModelFactory.ResultModelSusccess(users);
        }

        [HttpGet("GetPage")]
        public async Task<PageResultModel<List<ClientUser>>> GetPageAsync([FromQuery] ClientUserSearchFilter filter)
        {
            var users = await _clientUserService.GetPageAsync(filter);
            if (users == null)
            {
                return ResultModelFactory.PageResultModelNotFound<List<ClientUser>>();
            }
            var total = await _clientUserService.GetCountAsync(filter);
            return ResultModelFactory.PageResultModelSuccess(users, total);
        }

        [HttpGet("GetUserByJwt")]
        public ResultModel<ClientUser> GetUserByJwt()
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var json = _redisHelper.GetDatabase().StringGet(token);
            var user = JsonConvert.DeserializeObject<ClientUser>(json.ToString());
            if (user == null)
            {
                ResultModelFactory.ResultModelNotFound<ClientUser>();
            }
            return ResultModelFactory.ResultModelSusccess<ClientUser>(user);
        }

    }
}
