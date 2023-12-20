using Newtonsoft.Json;

namespace TwoAPI.Sample.AnnotateMaster
{
    /// <summary>
    /// ConfigHelper类用于操作和获取应用程序的配置信息。
    /// </summary>
    internal class ConfigHelper
    {
        /// <summary>
        /// 获取应用程序配置信息的静态方法。
        /// </summary>
        /// <returns>返回ConfigDto类型的配置信息对象。如果读取失败，则返回一个新实例。</returns>
        internal static ConfigDto GetConfig()
        {
            try
            {
                // 获取当前应用程序的目录路径。
                string currentDirectory = Directory.GetCurrentDirectory();

                // 构造配置文件的完整路径。
                string configFilePath = Path.Combine(currentDirectory, "appsettings.json");

                // 检查配置文件是否存在，不存在则抛出异常。
                if (!File.Exists(configFilePath))
                {
                    throw new FileNotFoundException("Configuration file not found.");
                }

                // 读取配置文件的内容为字符串格式。
                string json = File.ReadAllText(configFilePath);

                // 将JSON字符串反序列化为ConfigDto对象。
                var config = JsonConvert.DeserializeObject<ConfigDto>(json);
                // 如果解析结果为null，则创建一个新的ConfigDto实例。
                if (config == null)
                    return new ConfigDto();

                // 返回解析成功的ConfigDto对象。
                return config;
            }
            catch (Exception)
            {
                // 在发生任何异常的情况下，返回一个新的ConfigDto实例。
                return new ConfigDto();
            }
        }
    }
}
