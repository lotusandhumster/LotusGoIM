using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LotusGoIMWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "user")]
    public class GroupMemberController : ControllerBase
    {
        private readonly IGroupMemberService _groupMemberService;

        public GroupMemberController(IGroupMemberService groupMemberService)
        {
            _groupMemberService = groupMemberService;
        }

        [HttpPost("Add")]
        public async Task<ResultModel<bool>> AddAsync(GroupMember groupMember)
        {
            return ResultModelFactory.ResultModelSusccess(await _groupMemberService.AddAsync(groupMember));
        }

        [HttpDelete("Delete")]
        public async Task<ResultModel<bool>> DeleteAsync(int id)
        {
            return ResultModelFactory.ResultModelSusccess(await _groupMemberService.DeleteAsync(id));
        }

        [HttpPut("Update")]
        public async Task<ResultModel<bool>> UpdateAsync(GroupMember groupMember)
        {
            return ResultModelFactory.ResultModelSusccess(await _groupMemberService.UpdateAsync(groupMember));
        }

        [HttpGet("Get")]
        public async Task<ResultModel<GroupMemberModel>> GetAsync(int id)
        {
            return ResultModelFactory.ResultModelSusccess(await _groupMemberService.GetAsync(id));
        }

        [HttpGet("GetList")]
        public async Task<ResultModel<List<GroupMemberModel>>> GetListAsync([FromQuery] GroupMemberSearchFilter filter)
        {
            return ResultModelFactory.ResultModelSusccess(await _groupMemberService.GetListAsync(filter));
        }

        [HttpGet("GetCount")]
        public async Task<ResultModel<int>> GetCountAsync(int groupId)
        {
            return ResultModelFactory.ResultModelSusccess(await _groupMemberService.GetCountAsync(groupId));
        }
    }
}
