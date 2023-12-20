namespace TwoAPI.Sample.AnnotateMaster
{
    internal class FileHelper
    {
        /// <summary>
        /// 获取唯一的输出路径。若指定的输出路径已存在，则通过在路径后附加特定后缀（例如 "_bak1"）来生成唯一路径。
        /// </summary>
        /// <param name="outputPath">原始输出路径</param>
        /// <returns>如果原始路径已存在，返回修改后的唯一路径；否则返回原始路径</returns>
        internal static string GetUniqueOutputPath(string outputPath)
        {
            int count = 1;
            string uniqueOutputPath = outputPath;

            // 如果输出路径已存在，则在路径后添加"_bak{count}"来生成唯一的输出路径
            while (Directory.Exists(uniqueOutputPath))
            {
                uniqueOutputPath = $"{outputPath}_bak{count}";
                count++;
            }

            return uniqueOutputPath;
        }

        /// <summary>
        /// 判断指定文件是否为文本文件。此方法基于文件扩展名进行判断。
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>如果文件是支持的文本文件类型，则返回true；否则返回false</returns>
        internal static bool IsTextFile(string filePath)
        {
            var config = ConfigHelper.GetConfig();
            
            // 获取文件扩展名，并转换为小写字母形式
            var extension = Path.GetExtension(filePath).ToLower();

            // 获取支持的文本文件扩展名列表
            var textExtensions = config.SupportFiles ?? [".cs"];

            // 判断文件扩展名是否在支持的文本文件扩展名列表中，如果是则返回true，否则返回false
            return textExtensions.Contains(extension);
        }

        /// <summary>
        /// 判断指定文件是否为小型文件。通过比较文件大小与指定的最大大小来决定。
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="maxSize">定义小型文件的最大大小（单位：字节）</param>
        /// <returns>如果文件大小小于指定的最大大小，则返回true；否则返回false</returns>
        internal static bool IsSmallFile(string filePath, int maxSize)
        {
            FileInfo fileInfo = new(filePath);
            return fileInfo.Length < maxSize;
        }
    }
}
