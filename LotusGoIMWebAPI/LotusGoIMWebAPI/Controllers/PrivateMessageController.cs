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
    public class PrivateMessageController : ControllerBase
    {
        private readonly IPrivateMessageService _privateMessageService;

        public PrivateMessageController(IPrivateMessageService privateMessageService)
        {
            _privateMessageService = privateMessageService;
        }

        [HttpPost("Add")]
        public async Task<ResultModel<bool>> AddAsync([FromBody] PrivateMessage privateMessage)
        {
            var result = await _privateMessageService.AddAsync(privateMessage);

            if(result)
            {
                return ResultModelFactory.ResultModelSusccess<bool>(result);
            }
            return ResultModelFactory.ResultModelInternalServerError<bool>("添加失败");
        }

        [HttpDelete("Delete")]
        public async Task<ResultModel<bool>> DeleteAsync(int id)
        {
            var result = await _privateMessageService.DeleteAsync(id);

            if(result)
            {
                return ResultModelFactory.ResultModelSusccess<bool>(result);
            }
            return ResultModelFactory.ResultModelInternalServerError<bool>("删除失败");
        }

        [HttpGet("Get")]
        public async Task<ResultModel<PrivateMessageModel>> GetAsync(int id)
        {
            var privateMessageModel = await _privateMessageService.GetAsync(id);

            if(privateMessageModel is null)
            {
                return ResultModelFactory.ResultModelNotFound<PrivateMessageModel>("没有找到该条消息");
            }

            return ResultModelFactory.ResultModelSusccess<PrivateMessageModel>(privateMessageModel);
        }

        [HttpGet("GetList")]
        public async Task<ResultModel<IEnumerable<PrivateMessageModel>>> GetListAsync([FromQuery] PrivateMessageSearchFilter filter)
        {
            var result = await _privateMessageService.GetListAsync(filter);
            return ResultModelFactory.ResultModelSusccess(result);
        }

        [HttpGet("GetPage")]
        public async Task<PageResultModel<IEnumerable<PrivateMessageModel>>> GetPageAsync([FromQuery] PrivateMessageSearchFilter filter)
        {
            return await _privateMessageService.GetPageAsync(filter);
        }

    }
}
