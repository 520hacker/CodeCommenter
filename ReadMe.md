# 一键给文件夹中所有的代码添加注释的应用
![Build Status](https://img.shields.io/badge/build-passing-brightgreen) ![License](https://img.shields.io/badge/license-MIT-blue) ![.NET8](https://img.shields.io/badge/.NET-8-blueviolet)

这是一个使用 .NET 8 开发的开源项目，旨在方便开发者为项目中的所有代码一键添加注释。本项目默认使用 GPT 进行注释的添加，操作简便，用户只需运行程序，然后输入需要添加注释的代码的文件夹路径即可。

- 当前项目的源代码注释写的特别的详细，其实<u>这些注释就是使用本项目自己生成的</u>。
- 是的，你可以通过修改 appsettings.json 的prompt来把添加注释的动作变成别的，比如**直接优化压缩代码**，**直接实现代码**，自动编写你注释说明的代码，但是你可能需要多花一些心思来解决AI幻觉造成的bug。

## 特性

- **安全性**：本程序会在添加注释前，先将当前项目内容复制到一个新的 `_bak` 文件夹中，并在这个文件夹内完成添加注释的工作，保证了源代码的安全性。

- **灵活性**：本项目支持使用不同版本的 GPT 进行注释操作，推荐使用 `gpt-4-32k`。如果预算有限，也可以选择使用 `gpt-3.5-turbo-16k`。

- **免费**：本项目使用了关联项目 https://github.com/520hacker/two-api 提供的key（见 appsettings.json ）,额度有限，请勿浪费。

## 如何使用

1. 克隆或下载本项目到本地
2. 打开终端，运行程序 TwoAPI.Sample.AnnotateMaster.exe
3. 当提示输入文件夹路径时，输入需要添加注释的代码的文件夹路径
4. 程序执行完成之后，请使用合适的代码对比工具来同步注释，比如 BCompare

## 效果截图

![img](https://memos2504.oss-accelerate.aliyuncs.com/memos2504/assets/2023/12/20/1703078165_mmexport1703077689361.jpg)

![img](https://memos2504.oss-accelerate.aliyuncs.com/memos2504/assets/2023/12/20/1703078174_mmexport1703077691016.jpg)

## 注意事项

由于 ChatGPT 的幻觉问题依然十分严重，所以我们建议大家使用 `gpt-4-32k` 来进行注释操作。如果预算有限，可以选择使用 `gpt-3.5-turbo-16k` 进行替代。
警告！！！请勿将此项目用于贵单位、组织、个人的商业或需要保护权益的项目，因此造成的泄密和带来的各种风险和损失，开发者不承担任何相关责任。

## 许可证

本项目采用 MIT 许可证，欢迎任何人对我们的项目进行改进并分享。

## 联系我们
有任何想法，欢迎点击 http://qr61.cn/oRUvxf/qyT8mJT 打开二维码扫码加入微信群