using System.Text.RegularExpressions;

namespace TwoAPI.Sample.AnnotateMaster
{
    /// <summary>
    /// MarkdownHelper类是一个帮助处理Markdown格式代码的类，它主要用于提取Markdown格式内容中的代码块。
    /// </summary>
    internal partial class MarkdownHelper
    {

        /// <summary>
        /// ExtractCode方法用于从Markdown格式的内容中提取代码块，并返回提取到的代码字符串。
        /// 如果传入的内容为空或者不符合Markdown格式，该方法将直接返回原始内容。
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal static string ExtractCode(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return "";
            }

            // 检查内容是否符合Markdown格式，如果是则进行提取操作，否则直接返回原始输入。
            if (IsMarkdownFormat(content))
            {
                // 从Markdown内容中提取第一个代码块并返回
                return ExtractCodeFromMarkdown(content);
            }
            else
            {
                // 返回原始内容（不包含任何修改）
                return content;
            }
        }

        /// <summary>
        /// IsMarkdownFormat方法用于判断给定的内容是否符合Markdown格式。
        /// 判断的标准是内容是否以三个反引号和语言指示符开头。
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal static bool IsMarkdownFormat(string content)
        {
            // 使用正则表达式检查内容是否以三个反引号开头，并且后面跟着语言指示器（即判断是否是代码块）
            return IsMarkdownRegex().IsMatch(content);
        }

        /// <summary>
        /// ExtractCodeFromMarkdown方法用于从给定的Markdown内容中提取第一个代码块。
        /// 该方法会尝试找到Markdown内容中的第一个代码块，如果找到，就返回该代码块的内容；如果没有找到，就返回空字符串。
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        internal static string ExtractCodeFromMarkdown(string content)
        {
            // 在Markdown内容中查找第一个代码块
            Match match = GetCodeRegex().Match(content);

            if (match.Success)
            {
                // 从匹配组中提取出代码 
                return match.Groups[1].Value;
            }
            else
            {
                // 没有找到任何代码块, 返回空字符串或者根据需要处理错误
                return string.Empty;
            }
        }

        /// <summary>
        /// GetCodeRegex方法是获取一个匹配Markdown格式代码块的正则表达式对象的方法。
        /// 该方法返回一个Regex对象，该对象用于匹配以三个反引号开头，且后面跟着某种语言指示器的代码块。
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(@"```.*\n([\s\S]*?)\n```")]
        internal static partial Regex GetCodeRegex();

        /// <summary>
        /// IsMarkdownRegex方法是获取一个判断是否为Markdown格式的正则表达式对象的方法。
        /// 该方法返回一个Regex对象，该对象用于判断内容是否以三个反引号和语言指示器开头。
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex(@"^```[\w\s]*\n")]
        internal static partial Regex IsMarkdownRegex();
    }
}
