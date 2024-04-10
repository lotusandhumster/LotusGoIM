using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LotusGoIMWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "user")]
    public class FriendController
    {
        private readonly IFriendService _friendService;
        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }
        [HttpPost("Add")]
        public async Task<ResultModel<bool>> AddAsync([FromBody] Friend friend)
        {
            return await _friendService.AddAsync(friend);
        }
        [HttpDelete("Delete")]
        public async Task<bool> DeleteAsync(int id)
        {
            return await _friendService.DeleteAsync(id);
        }
        [HttpGet("GetCount")]
        public async Task<int> GetCountAsync(int userId)
        {
            return await _friendService.GetCountAsync(userId);
        }
        [HttpGet("Get")]
        public async Task<ResultModel<FriendModel>> GetFriendAsync(int id)
        {
            var result = await _friendService.GetAsync(id);
            return result;
        }
        [HttpGet("GetList")]
        public async Task<ResultModel<List<FriendModel>>> GetListAsync([FromQuery] FriendSearchFilter filter)
        {
            return await _friendService.GetListAsync(filter);
        }

        [HttpPut("Update")]
        public async Task<ResultModel<bool>> UpdateAsync([FromBody] Friend friend)
        {
            return await _friendService.UpdateAsync(friend);
        }
    }
}
