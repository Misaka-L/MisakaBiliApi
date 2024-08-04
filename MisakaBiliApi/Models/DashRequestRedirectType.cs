namespace MisakaBiliApi.Models;

/// <summary>
/// 获取视频 DASH URL 重定向类型
/// </summary>
public enum DashRequestRedirectType
{
    /// <summary>
    /// 重定向到视频 DASH URL
    /// </summary>
    Video,
    /// <summary>
    /// 重定向到音频 DASH URL
    /// </summary>
    Audio,
    /// <summary>
    /// 不重定向
    /// </summary>
    None
}
