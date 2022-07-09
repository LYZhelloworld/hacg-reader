using System;

namespace HAcgReader.Services;

/// <summary>
/// 提供与神社域名相关的功能
/// </summary>
public interface IDomainService : IDisposable
{
    /// <summary>
    /// 获取神社使用的域名
    /// </summary>
    /// <returns>使用的域名，如果获取失败则返回空字符串</returns>
    string GetDomain();
}