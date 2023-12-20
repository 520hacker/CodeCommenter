using Newtonsoft.Json;
using TwoAPI.DataContracts.Consts; // 引用常量定义
using TwoAPI.DataContracts.Node.Vendor.OpenAI; // 引用OpenAI节点数据契约
using TwoAPI.DataContracts.Request.Vendors.OpenAI;
using TwoAPI.DataContracts.Response.Vendors.OpenAI; // 引用OpenAI请求数据契约

namespace TwoAPI.Sample.AnnotateMaster
{
    /// <summary>
    /// OpenAIHelper类提供与OpenAI服务交互的辅助方法。
    /// </summary>
    internal class OpenAIHelper
    {
        /// <summary>
        /// GetResponse方法用于将JSON格式的字符串解析为CompletionCreateResponse对象。
        /// 若输入字符串为空或为"[DONE]"，则返回null。
        /// </summary>
        /// <param name="line">代表JSON格式的响应字符串。</param>
        /// <returns>解析后的CompletionCreateResponse对象，解析失败时返回空对象。</returns>
        internal static CompletionCreateResponse? GetResponse(string line)
        {
            if (string.IsNullOrEmpty(line) || line.Equals("[DONE]")) return null;

            try
            { 
                return JsonConvert.DeserializeObject<CompletionCreateResponse>(line);
            }
            catch (Exception)
            {
                return new CompletionCreateResponse();
            }
        }

        /// <summary>
        /// GetContentByLine方法用于从OpenAI的响应中提取并累加内容字符串。
        /// </summary>
        /// <param name="contents">之前累加的内容字符串。</param>
        /// <param name="line">当前处理的响应字符串。</param>
        /// <returns>更新后的内容字符串。</returns>
        internal static string GetContentByLine(string contents, string line)
        {
            try
            {
                if (line.StartsWith("data: "))
                {
                    line = line["data: ".Length..];
                }

                //line = line.Trim();
                if (string.IsNullOrWhiteSpace(line))
                    return contents;

                var obj = GetResponse(line);

                var str = string.Empty;
                if (obj != null && !string.IsNullOrWhiteSpace(obj?.Choices?[0].Delta?.Content))
                    str = obj.Choices[0].Delta.Content;

                if (obj != null && !string.IsNullOrWhiteSpace(obj?.Choices?[0].Message?.Content))
                    str = obj.Choices[0].Message.Content;

                Console.Write(str);
                contents += str;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return contents;
        }

        /// <summary>
        /// GetCompletionsRequest方法用于创建并返回一个CompletionsRequest对象，
        ///该对象包含了向OpenAI发送请求所需的参数信息。
        /// </summary>
        /// <param name="question">需要向OpenAI提交的问题字符串。</param>
        /// <returns>配置好的CompletionsRequest对象。</returns>
        internal static CompletionsRequest GetCompletionsRequest(string question)
        {
            var config = ConfigHelper.GetConfig(); // 获取配置信息

            var request = new CompletionsRequest()
            {
                FrequencyPenalty = 1, // 设置频率惩罚，以减少重复内容的生成概率
                MaxTokens = 8000, // 设置生成文本的最大令牌数
                Messages =
                    [
                        new ChatCompletionMessageNode() // 初始化聊天消息节点
                        {
                            Role = "user", // 指定发送者角色为用户
                            Content = question // 设置消息内容为传入的问题
                        }
                    ],
                Model = config.Model, // 设置模型名称，使用配置文件中指定的模型
                Stream = config.Steam ?? false // 设置是否以流的形式接收响应，此处默认设为非流式
            };

            return request;
        }

        /// <summary>
        /// GetPrompt方法用于获取一个提示字符串，该字符串用于在问题和文件名中插入到配置的提示模板中。
        /// </summary>
        /// <param name="question">问题字符串。</param>
        /// <param name="fileName">文件名。</param>
        /// <returns>构建的提示字符串。</returns>
        internal static string GetPrompt(string question, string fileName)
        {
            var tmpQuestion = GetNeedOnlinePrompt(question, fileName); // 获取在线处理所需的提示信息
            return tmpQuestion; // 返回构建的提示字符串
        }

        /// <summary>
        /// GetNeedOnlinePrompt方法根据给定的问题和文件名，使用配置中的提示模板生成一个需要在线处理的提示字符串。
        /// </summary>
        /// <param name="question"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static string GetNeedOnlinePrompt(string question, string fileName)
        {
            var config = ConfigHelper.GetConfig(); // 获取配置信息
            // 返回格式化后的提示字符串，包含文件名、问题内容和当前时间信息
            return string.Format(config.Prompt ?? "请对{0}的内容添加注释，内容如下{1}，在内容顶部添加一个注释更新信息{2}", fileName, question, DateTime.Now.ToString());
        }

        /// <summary>
        /// GetChatCompletionsPath方法根据给定的baseUrl，返回与聊天完成相关接口路径。
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        internal static string GetChatCompletionsPath(string baseUrl)
        {
            // 如果baseUrl为空或空白，则直接使用默认的ChatCompletions路径，否则构建完整路径
            return string.IsNullOrWhiteSpace(baseUrl) ? OpenAIV1.ChatCompletions : $"{new Uri(baseUrl).AbsolutePath.TrimStart('/')}/{OpenAIV1.ChatCompletions}".Trim('/');
        }
    }
}
