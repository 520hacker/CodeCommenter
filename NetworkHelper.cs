using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using TwoAPI.DataContracts.Consts;
using TwoAPI.DataContracts.Response.Vendors.OpenAI;

namespace TwoAPI.Sample.AnnotateMaster
{
    /// <summary>
    /// 定义NetworkHelper内部局部类，辅助网络请求操作
    /// </summary>
    internal partial class NetworkHelper
    {
        private const int DefaultTimeoutMs = 600_000; // 默认超时时间为600秒
        /// <summary>
        /// 异步发送POST请求到AI服务。
        /// </summary>
        /// <param name="code">需要发送的代码。</param>
        /// <param name="fileName">文件名，用于辅助AI服务理解上下文。</param>
        /// <param name="timeoutMs">请求超时时间，以毫秒为单位，默认为600000毫秒。</param>
        /// <param name="cancellationToken">取消操作的令牌。</param>
        /// <returns>AI服务的响应字符串。</returns>
        internal static async Task<string?> PostAsync(string code, string fileName, int? timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
        {
            var config = ConfigHelper.GetConfig(); // 获取配置信息对象
            string baseUrl = config.AIHost ?? ""; // 获取AI主机地址配置项值
            string apiKey = config.AIKey ?? ""; // 获取AI密钥配置项值

            var question = OpenAIHelper.GetPrompt(code, fileName);
            var requestModel = OpenAIHelper.GetCompletionsRequest(question);

            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None
                /* 
                 设置HttpClientHandler对象的属性：
                 - ServerCertificateCustomValidationCallback：设置服务器证书验证回调函数，始终返回true表示信任所有服务器证书；
                 - AutomaticDecompression：设置自动解压缩方法，支持GZip、Deflate和None三种解压缩方式。
                */
            };

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = OpenAIV1.Host;
            }
            else
            {
                var uri = new Uri(baseUrl);
                baseUrl = uri.GetLeftPart(UriPartial.Authority);
            }

            var _httpClient = new HttpClient(httpClientHandler)
            {
                Timeout = TimeSpan.FromMilliseconds(timeoutMs ?? DefaultTimeoutMs),
                BaseAddress = new Uri(baseUrl)
            };
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }
            else
            {
                try
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            var path = OpenAIHelper.GetChatCompletionsPath(baseUrl);
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(requestModel, settings);
            var content = new StringContent(json);
            using var request = CreatePostJsonRequest(path, content);
            var response = await SendRequestAsync(_httpClient, request, cancellationToken);
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var reader = new StreamReader(stream);
            if (!(config.Steam ?? false))
            {
                var lines = await reader.ReadToEndAsync(cancellationToken);
                var obj = JsonConvert.DeserializeObject<CompletionCreateResponse>(lines);
                return MarkdownHelper.ExtractCode(obj?.Choices?[0].Message?.Content);
            }
            else
            {
                var contents = string.Empty;
                while (!reader.EndOfStream)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var line = await reader.ReadLineAsync(cancellationToken);
                    if (line == "data: " || line == null)
                    {
                        continue;
                    }

                    if (line.StartsWith("[DONE]") || line.Equals("data: [DONE]"))
                    {
                        break;
                    }

                    contents = OpenAIHelper.GetContentByLine(contents, line);
                }

                return contents;
            }
        }

        /// <summary>
        /// 创建一个以JSON格式发送的POST请求。
        /// </summary>
        /// <param name="uri">请求的URI。</param>
        /// <param name="content">请求的内容。</param>
        /// <returns>配置好的HttpRequestMessage实例。</returns>
        private static HttpRequestMessage CreatePostJsonRequest(string uri, HttpContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json")
            {
                CharSet = "utf-8"
            };

            request.Content = content;
            return request;
        }

        /// <summary>
        /// 使用指定的HttpClient异步发送请求。
        /// </summary>
        /// <param name="_httpClient">用于发送请求的HttpClient实例。</param>
        /// <param name="request">需要发送的HttpRequestMessage实例。</param>
        /// <param name="cancellationToken">取消操作的令牌。</param>
        /// <returns>服务器响应的HttpResponseMessage实例。</returns>
        private static async Task<HttpResponseMessage> SendRequestAsync(HttpClient _httpClient, HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            return response;
        }
    }
}
