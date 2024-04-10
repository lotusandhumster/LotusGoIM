using LotusGoIMWebAPI.Common;
using LotusGoIMWebAPI.Common.Enums;
using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace LotusGoIMWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "user")]
    public class OpenAIController : ControllerBase
    {
        private readonly IOpenAIService _openAiService;
        private readonly IChatGptMessageService _chatGptMessageService;
        private readonly RedisHelper _redisHelper;

        public OpenAIController(IOpenAIService openAiService, IChatGptMessageService chatGptMessageService, RedisHelper redisHelper)
        {
            _openAiService = openAiService;
            _chatGptMessageService = chatGptMessageService;
            _redisHelper = redisHelper;
        }

        [HttpPost("Chat")]
        public async Task<ResultModel<string>> ChatAsync([FromBody] string prompt)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var json = _redisHelper.GetDatabase().StringGet(token);
            var user = JsonConvert.DeserializeObject<ClientUser>(json.ToString());
            var result = await _openAiService.ChatAsync(prompt);

            var userMessage = new ChatGptMessage();
            userMessage.SendTime = DateTime.Now;
            userMessage.UserId = user!.UserId;
            userMessage.Content = prompt;
            userMessage.Role = ChatGptMessageRoleType.User;
            await _chatGptMessageService.AddAsync(userMessage);

            if (result == null)
            {
                return ResultModelFactory.ResultModelInternalServerError<string>();
            }

            var systemMessage = new ChatGptMessage();
            systemMessage.UserId = user!.UserId;
            systemMessage.Content = result;
            systemMessage.SendTime = DateTime.Now;
            systemMessage.Role = ChatGptMessageRoleType.System;
            await _chatGptMessageService.AddAsync(systemMessage);

            return ResultModelFactory.ResultModelSusccess(result);
        }

        [HttpPost("ChatWithHistory")]
        public async Task<ResultModel<string>> ChatWithHistoryAsync([FromBody] string prompt)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var json = _redisHelper.GetDatabase().StringGet(token);
            var user = JsonConvert.DeserializeObject<ClientUser>(json.ToString());

            var filter = new ChatGptMessageSearchFilter();

            var conversationHistory = await _chatGptMessageService.GetLast20ListAsync(user!.UserId);

            var userMessage = new ChatGptMessage();
            userMessage.SendTime = DateTime.Now;
            userMessage.UserId = user!.UserId;
            userMessage.Content = prompt;
            userMessage.Role = ChatGptMessageRoleType.User;
            await _chatGptMessageService.AddAsync(userMessage);

            var result = await _openAiService.ChatWithHistoryAsync(prompt, conversationHistory);

            if (result == null)
            {
                return ResultModelFactory.ResultModelInternalServerError<string>();
            }

            var systemMessage = new ChatGptMessage();
            systemMessage.UserId = user!.UserId;
            systemMessage.Content = result;
            systemMessage.SendTime = DateTime.Now;
            systemMessage.Role = ChatGptMessageRoleType.System;
            await _chatGptMessageService.AddAsync(systemMessage);

            return ResultModelFactory.ResultModelSusccess(result);
        }

        [HttpPost("QuickReply")]
        public async Task<ResultModel<string>> QuickReplyAsync([FromBody] string prompt)
        {
            var result = await _openAiService.QuickReplyAsync(prompt);

            return ResultModelFactory.ResultModelSusccess<string>(result);
        }
    }
}
