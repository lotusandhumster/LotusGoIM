using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
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
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;

        public GroupController(IGroupService groupService, IGroupMemberService groupMemberService)
        {
            _groupService = groupService;
            _groupMemberService = groupMemberService;
        }

        [HttpGet("Get")]
        public async Task<ResultModel<Group>> GetAsync(int id)
        {
            var result = await _groupService.GetAsync(id);

            return ResultModelFactory.ResultModelSusccess(result);
        }

        [HttpPost("Add")]
        public async Task<ResultModel<bool>> AddAsync([FromBody] Group group)
        {
            if (group.Name.Length > 20 || group.Name.Length < 1)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("群名长度不正确");
            }
            if (group.Description.Length > 100)
            {
                return ResultModelFactory.PageResultModelInternalServerError<bool>("群描述长度不正确");
            }

            var result = await _groupService.AddAsync(group);

            var filter = new GroupSearchFilter();
            filter.Owner = group.Owner;

            var newGroup = (await _groupService.GetListAsync(filter)).LastOrDefault();
            var member = new GroupMember();
            member.MemberId = group.Owner;
            member.GroupId = newGroup!.GroupId;
            await _groupMemberService.AddAsync(member);

            return ResultModelFactory.ResultModelSusccess(result);
        }

        [HttpPut("Update")]
        public async Task<ResultModel<bool>> UpdateAsync([FromBody] Group group)
        {
            if (group.Name.Length > 20 || group.Name.Length < 1)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("群名长度不正确");
            }
            if (group.Description.Length > 100)
            {
                return ResultModelFactory.PageResultModelInternalServerError<bool>("群描述长度不正确");
            }

            var result = await _groupService.UpdateAsync(group);

            return ResultModelFactory.ResultModelSusccess(result);
        }

        [HttpDelete("Delete")]
        public async Task<ResultModel<bool>> DeleteAsync(int id)
        {
            var result = await _groupService.DeleteAsync(id);

            return ResultModelFactory.ResultModelSusccess(result);
        }

        [HttpGet("GetList")]
        public async Task<ResultModel<IEnumerable<Group>>> GetListAsync([FromQuery] GroupSearchFilter filter)
        {
            var result = await _groupService.GetListAsync(filter);

            return ResultModelFactory.ResultModelSusccess(result);
        }

        [HttpGet("GetPage")]
        public async Task<PageResultModel<IEnumerable<Group>>> GetPageAsync([FromQuery] GroupSearchFilter filter)
        {
            var result = await _groupService.GetPageAsync(filter);

            return result;
        }
    }
}
