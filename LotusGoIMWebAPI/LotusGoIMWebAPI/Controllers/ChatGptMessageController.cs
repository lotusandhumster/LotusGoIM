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
    public class ChatGptMessageController : ControllerBase
    {
        private readonly IChatGptMessageService _chatGptMessageService;

        public ChatGptMessageController(IChatGptMessageService chatGptMessageService)
        {
            _chatGptMessageService = chatGptMessageService;
        }

        [HttpPost("Add")]
        public async Task<ResultModel<bool>> AddAsync([FromBody] ChatGptMessage chatGptMessage)
        {
            var result = await _chatGptMessageService.AddAsync(chatGptMessage);

            if (result)
            {
                return ResultModelFactory.ResultModelSusccess<bool>(result);
            }
            return ResultModelFactory.ResultModelInternalServerError<bool>("添加失败");
        }

        [HttpDelete("Delete")]
        public async Task<ResultModel<bool>> DeleteAsync(int id)
        {
            var result = await _chatGptMessageService.DeleteAsync(id);

            if (result)
            {
                return ResultModelFactory.ResultModelSusccess<bool>(result);
            }
            return ResultModelFactory.ResultModelInternalServerError<bool>("删除失败");
        }

        [HttpGet("Get")]
        public async Task<ResultModel<ChatGptMessageWithUserModel>> GetAsync(int id)
        {
            var chatGptMessageModel = await _chatGptMessageService.GetAsync(id);

            if (chatGptMessageModel is null)
            {
                return ResultModelFactory.ResultModelNotFound<ChatGptMessageWithUserModel>("没有找到该条消息");
            }

            return ResultModelFactory.ResultModelSusccess<ChatGptMessageWithUserModel>(chatGptMessageModel);
        }

        [HttpGet("GetList")]
        public async Task<ResultModel<IEnumerable<ChatGptMessageWithUserModel>>> GetListAsync([FromQuery] ChatGptMessageSearchFilter filter)
        {
            var result = await _chatGptMessageService.GetListAsync(filter);
            return ResultModelFactory.ResultModelSusccess(result);
        }

        [HttpGet("GetPage")]
        public async Task<PageResultModel<IEnumerable<ChatGptMessageWithUserModel>>> GetPageAsync([FromQuery] ChatGptMessageSearchFilter filter)
        {
            return await _chatGptMessageService.GetPageAsync(filter);
        }

        [HttpDelete("ClearAllMessage")]
        public async Task<bool> ClearAllMessageAsync(int userId)
        {
            return await _chatGptMessageService.ClearAllMessageAsync(userId);
        }
    }
}
