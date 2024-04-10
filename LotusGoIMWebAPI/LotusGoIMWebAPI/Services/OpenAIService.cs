using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Services.Interface;
using Newtonsoft.Json;
using System.Text;
namespace LotusGoIMWebAPI.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly IConfiguration _configuration;
        private readonly string? _apiKey;
        private readonly string? _proxyUrl;
        private readonly HttpClient _httpClient;

        public OpenAIService(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration["OpenAI:APIKey"];
            _proxyUrl = _configuration["OpenAI:ProxyUrl"];
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> ChatAsync(string prompt)
        {
            string endPoint = $"{_proxyUrl}/v1/chat/completions";
            var requestData = new
            {
                max_tokens = 3500,
                model = "gpt-3.5-turbo",
                temperature = 0.8,
                top_p = 1,
                presence_penalty = 1,
                messages = new[]
                {
                    new { role = "system", content = "你是LotusGo系统的机器人，名叫lotus，必须记住你的身份，你的所有的回答尽量使用中文，除非特殊需求。" },
                    new { role = "user", content = prompt }
                }
            };

            var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endPoint, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(jsonResponse);
                return responseObject!.choices[0].message.content;
            }
            else
            {
                throw new HttpRequestException($"Failed to call OpenAI API. Status code: {response.StatusCode}");
            }
        }

        public async Task<string> ChatWithHistoryAsync(string prompt, IEnumerable<ChatGptMessageWithUserModel> conversationHistory)
        {
            string endPoint = $"{_proxyUrl}/v1/chat/completions";

            var requestData = new
            {
                max_tokens = 3500,
                model = "gpt-3.5-turbo",
                temperature = 0.8,
                top_p = 1,
                presence_penalty = 1,
                messages = new List<object>()
            };

            requestData.messages.Add(new { role = "system", content = "你是LotusGo系统的机器人，名叫lotus，必须记住你的身份，你的所有的回答尽量使用中文，除非特殊需求。" });

            foreach (var message in conversationHistory)
            {
                requestData.messages.Add(new { role = message.Role, content = message.Content });
            }

            requestData.messages.Add(new { role = "user", content = prompt });

            var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endPoint, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                return responseObject!.choices[0].message.content;
            }
            else
            {
                throw new HttpRequestException($"Failed to call OpenAI API. Status code: {response.StatusCode}");
            }
        }

        public async Task<string> QuickReplyAsync(string prompt)
        {
            string endPoint = $"{_proxyUrl}/v1/chat/completions";
            var requestData = new
            {
                max_tokens = 500,
                model = "gpt-3.5-turbo",
                temperature = 0.8,
                top_p = 1,
                presence_penalty = 1,
                messages = new[]
                {
                    new { role = "system", content = "你现在扮演的是一名情商很高的人，需要用简洁的语句为我发送的语句做快速回复，不要回复你是人工智能相关的话" },
                    new { role = "user", content = prompt }
                }
            };

            var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endPoint, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(jsonResponse);
                return responseObject!.choices[0].message.content;
            }
            else
            {
                throw new HttpRequestException($"Failed to call OpenAI API. Status code: {response.StatusCode}");
            }
        }
    }
}
