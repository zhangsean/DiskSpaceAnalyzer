using System;
using System.Collections;
using System.Windows.Forms;

#region 进行字符串排序。
/// <summary>
/// 进行字符串排序。
/// </summary>
public class ListViewItemComparerByString : IComparer
{
    private bool Asc;
    private int col;
    public ListViewItemComparerByString()
    {
        Asc = true;
        col = 0;
    }
    public ListViewItemComparerByString(int column, bool AscOrder)
    {
        Asc = AscOrder;
        col = column;
    }
    public int Compare(object x, object y)
    {
        int ret = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
        if (ret == 1)
            return Asc ? 1 : 0;
        else
            return Asc ? 0 : 1;
    }
}
#endregion

#region  进行文件大小排序。
/// <summary>
/// 进行文件大小排序。
/// </summary>
public class ListViewItemComparerBySize : IComparer
{
    private bool Asc;
    private int col;
    public ListViewItemComparerBySize()
    {
        Asc = true;
        col = 0;
    }
    public ListViewItemComparerBySize(int column, bool AscOrder)
    {
        Asc = AscOrder;
        col = column;
    }
    public int Compare(object x, object y)
    {
        long lvX = 0;
        long lvY = 0;
        try
        {
            lvX = long.Parse(((ListViewItem)x).SubItems[col].Tag.ToString());
            lvY = long.Parse(((ListViewItem)y).SubItems[col].Tag.ToString());
        }
        catch
        {
            return -1;
        }
        if (lvX > lvY)
            return Asc ? 1 : 0;
        else
            return Asc ? 0 : 1;
    }
}
#endregion

#region  进行整型数排序。
/// <summary>
/// 进行整型数排序。
/// </summary>
public class ListViewItemComparerByInt : IComparer
{
    private bool Asc;
    private int col;
    public ListViewItemComparerByInt()
    {
        Asc = true;
        col = 0;
    }
    public ListViewItemComparerByInt(int column, bool AscOrder)
    {
        Asc = AscOrder;
        col = column;
    }
    public int Compare(object x, object y)
    {
        long lvX = 0;
        long lvY = 0;
        try
        {
            lvX = long.Parse(((ListViewItem)x).SubItems[col].Text);
            lvY = long.Parse(((ListViewItem)y).SubItems[col].Text);
        }
        catch
        {
            return -1;
        }
        if (lvX > lvY)
            return Asc ? 1 : 0;
        else
            return Asc ? 0 : 1;
    }
}
#endregion
