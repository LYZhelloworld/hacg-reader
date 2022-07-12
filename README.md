# HACG-Reader
琉璃神社阅读器（非官方），采用 RSS Feed 拉取并解析最新发表的文章，并附带解析磁链功能。

文章内容版权归琉璃神社所有。

## 基本功能
- 自动解析神社域名
- 拉取文章列表
- 获取当前文章相关信息（如标题、分类、评论等）
- 自动解析文章正文中的磁力链接

## 开发中/计划中的功能
- 预览正文内容（考虑到可能在公共场所使用，可折叠）
- 应用程序配置（用于在解析不到域名时手动提供）
- 移动端 UI

## 使用的开源库
- [coverlet.collector](https://www.nuget.org/packages/coverlet.collector)：提供单元测试覆盖率支持
- [FluentAssertions](https://www.nuget.org/packages/FluentAssertions)：用于单元测试
- [HtmlAgilityPack](https://www.nuget.org/packages/HtmlAgilityPack)：用于解析 HTML 文档
- [Microsoft.Toolkit.Mvvm](https://www.nuget.org/packages/Microsoft.Toolkit.Mvvm)：用于提供 MVVM 支持
- [Moq](https://www.nuget.org/packages/Moq)：用于单元测试
