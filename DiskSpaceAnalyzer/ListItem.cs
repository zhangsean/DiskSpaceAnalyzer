
public class ListItem
{
    /// <summary>
    /// 文件类型。
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// 文件名称。
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 文件路径。
    /// </summary>
    public string Path { get; set; }
    /// <summary>
    /// 文件大小（数字）。
    /// </summary>
    public long Length { get; set; }
    /// <summary>
    /// 文件大小（文本描述）。
    /// </summary>
    public string Size
    {
        get
        {
            return GetLength(Length);
        }
    }
    /// <summary>
    /// 最后修改日期。
    /// </summary>
    public string LastWriteTime { get; set; }
    /// <summary>
    /// 创建日期。
    /// </summary>
    public string CreationTime { get; set; }

    #region string GetLength(long Bytes)  将字节大小转换为最适合的大小。
    /// <summary>
    /// 将字节大小转换为最适合的大小。
    /// </summary>
    /// <param name="Bytes"></param>
    /// <returns></returns>
    public string GetLength(long Bytes)
    {
        double B = (double)Bytes;
        B /= 1024;
        if (B < 1024)
            return B.ToString("F2") + " KB";
        else
        {
            B /= 1024;
            if (B < 1024)
                return B.ToString("F2") + " MB";
            else
            {
                B /= 1024;
                return B.ToString("F2") + " GB";
            }
        }
    }
    #endregion

}
