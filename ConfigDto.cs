// 2023-12-20 下午 11:42:10
// 修改并优化ConfigDto.cs文件中的注释，用于SDK文档的导出
namespace TwoAPI.Sample.AnnotateMaster
{
    /// <summary>
    /// ConfigDto类用于封装和管理API配置信息。
    /// 此类提供了对API配置参数的访问，例如AI主机地址、AI密钥、模型信息等。
    /// </summary>
    internal class ConfigDto
    {

        /// <summary>
        /// 获取或设置Steam服务的启用状态。
        /// 当值为true时，表示启用Steam服务；反之则为禁用。
        /// 此属性通常用于决定是否与Steam服务集成。
        /// </summary>
        public bool? Steam { get; set; }

        /// <summary>
        /// 获取或设置AI服务的主机地址。
        /// 此属性存储用于连接AI服务的网络地址。
        /// </summary>
        public string? AIHost { get; set; }

        /// <summary>
        /// 获取或设置用于访问AI服务的密钥。
        /// 此属性存储用于身份验证的API密钥，以确保安全访问。
        /// </summary>        
        public string? AIKey { get; set; }

        /// <summary>
        /// 获取或设置当前使用的AI模型的标识。
        /// 此属性指明了AI服务中所使用的具体模型。
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// 获取或设置用于AI服务的默认提示信息。
        /// 此属性存储在请求AI服务时使用的初始提示文本。
        /// </summary>
        public string? Prompt { get; set; }

        /// <summary>
        /// 获取或设置支持的文件类型列表。
        /// </summary>
        public string[]? SupportFiles { get; set; }
    }
}