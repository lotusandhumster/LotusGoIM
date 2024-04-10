using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LotusGoIMWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "user")]
    public class GroupMessageController : ControllerBase
    {
        private readonly IGroupMessageService _groupMessageService;

        public GroupMessageController(IGroupMessageService groupMessageService)
        {
            _groupMessageService = groupMessageService;
        }

        [HttpPost("Add")]
        public async Task<ResultModel<bool>> AddAsync([FromBody] GroupMessage groupMessage)
        {
            var result = await _groupMessageService.AddAsync(groupMessage);

            if (result)
            {
                return ResultModelFactory.ResultModelSusccess<bool>(result);
            }
            return ResultModelFactory.ResultModelInternalServerError<bool>("添加失败");
        }

        [HttpDelete("Delete")]
        public async Task<ResultModel<bool>> DeleteAsync(int id)
        {
            var result = await _groupMessageService.DeleteAsync(id);

            if (result)
            {
                return ResultModelFactory.ResultModelSusccess<bool>(result);
            }
            return ResultModelFactory.ResultModelInternalServerError<bool>("删除失败");
        }

        [HttpGet("Get")]
        public async Task<ResultModel<GroupMessageModel>> GetAsync(int id)
        {
            var groupMessageModel = await _groupMessageService.GetAsync(id);

            if (groupMessageModel is null)
            {
                return ResultModelFactory.ResultModelNotFound<GroupMessageModel>("没有找到该条消息");
            }

            return ResultModelFactory.ResultModelSusccess<GroupMessageModel>(groupMessageModel);
        }

        [HttpGet("GetList")]
        public async Task<ResultModel<IEnumerable<GroupMessageModel>>> GetListAsync([FromQuery] GroupMessageSearchFilter filter)
        {
            var result = await _groupMessageService.GetListAsync(filter);
            return ResultModelFactory.ResultModelSusccess(result);
        }

        [HttpGet("GetPage")]
        public async Task<PageResultModel<IEnumerable<GroupMessageModel>>> GetPageAsync([FromQuery] GroupMessageSearchFilter filter)
        {
            return await _groupMessageService.GetPageAsync(filter);
        }
    }
}
