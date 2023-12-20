namespace TwoAPI.Sample.AnnotateMaster
{
    /// <summary>
    /// 主程序类，负责处理文件夹内程序代码的注释添加和优化。
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// 程序的主入口点。
        /// </summary>
        /// <param name="args">命令行参数。</param>
        static void Main(string[] args)
        {
            string? folderPath = null;
            string? outputPath = null;

            // 检查 f 参数，获取文件夹路径
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "f" && i + 1 < args.Length)
                {
                    folderPath = args[i + 1];
                    break;
                }
            }

            // 如果未指定 f 参数，则提示输入文件夹地址
            while (string.IsNullOrEmpty(folderPath))
            {
                Console.WriteLine("请输入文件夹地址 (f 参数):");
                folderPath = Console.ReadLine();
            }

            // 检查文件夹是否存在
            // 如果不存在，则要求重新输入文件夹地址，直到输入正确为止。
            // 这里没有对路径进行验证，只是简单地检查了目录是否存在。
            // 可以根据实际需求添加更多的验证逻辑。 
            while (!Directory.Exists(folderPath))
            {
                Console.WriteLine("文件夹不存在，请重新输入文件夹地址:");
                folderPath = Console.ReadLine();
            }

            // 检查 o 参数
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "o" && i + 1 < args.Length)
                {
                    outputPath = args[i + 1];
                    break;
                }
            }

            // 如果未指定 o 参数，则在 f 地址后添加 "_bak" 作为默认输出地址
            // 这里使用了一个自定义的方法 GetUniqueOutputPath 来生成唯一的输出文件夹路径。
            // 可以根据实际需求修改该方法的实现。

            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = FileHelper.GetUniqueOutputPath(folderPath);
            }

            try
            {
                // 创建输出文件夹
                Directory.CreateDirectory(outputPath);
                // 遍历文件夹中的文件，并复制到输出文件夹 
                TraverseFolder(folderPath, outputPath, folderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("操作完成，请按任意键退出程序...");
            Console.ReadKey();
        }

        /// <summary>
        /// 遍历指定文件夹，复制文件到输出路径，并进行文件内容处理。
        /// </summary>
        /// <param name="folderPath">要遍历的文件夹路径。</param>
        /// <param name="outputPath">文件处理输出路径。</param>
        /// <param name="orginalPath">原始文件夹路径，用于生成相对路径。</param>
        static void TraverseFolder(string folderPath, string outputPath, string orginalPath)
        {
            foreach (string filePath in Directory.GetFiles(folderPath))
            {
                var fileName = Path.GetFileName(filePath);
                var subPath = filePath.TrimEnd(fileName.ToCharArray()).Replace(orginalPath, "").TrimStart("\\".ToCharArray());
                var outputFolderPath = Path.Combine(outputPath, subPath);

                // 创建输出文件夹
                Directory.CreateDirectory(outputFolderPath);

                var outputFilePath = Path.Combine(outputPath, subPath, fileName);


                // 复制文件
                File.Copy(filePath, outputFilePath);
                Console.WriteLine("复制文件: " + outputFilePath);

                // 检查文件类型和大小
                // 使用 FileHelper 类的 IsTextFile 和 IsSmallFile 方法来判断文件是否为文本文件且小于指定大小。

                if (FileHelper.IsTextFile(filePath) && FileHelper.IsSmallFile(filePath, 7 * 1024))
                {
                    Console.WriteLine("开始AI优化文件: " + fileName);

                    // 打开文本文件并输出文件名和内容
                    var fileContent = File.ReadAllText(filePath);

                    if (!string.IsNullOrWhiteSpace(fileContent))
                    {
                        var newContent = string.Empty;
                        try
                        {
                            newContent = NetworkHelper.PostAsync(fileContent, fileName).GetAwaiter().GetResult();
                            Console.WriteLine("优化后文件: " + outputFilePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("优化文件失败: " + outputFilePath);
                            Console.WriteLine(ex.Message);
                        }


                        if (!string.IsNullOrWhiteSpace(newContent))
                        {
                            FileAttributes attributes = File.GetAttributes(outputFilePath);

                            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            {
                                // 取消只读属性
                                attributes &= ~FileAttributes.ReadOnly;
                                File.SetAttributes(outputFilePath, attributes);
                            }

                            // 执行写入操作 
                            File.WriteAllText(outputFilePath, newContent);
                        }
                    }
                } 
            }

            foreach (string subFolderPath in Directory.GetDirectories(folderPath))
            {
                TraverseFolder(subFolderPath, outputPath, orginalPath); // 递归调用遍历子文件夹
            }
        }
    }
}
