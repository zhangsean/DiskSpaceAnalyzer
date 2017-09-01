using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace DiskSpaceAnalyzer
{
    public partial class FormMain : Form
    {
        #region Parameters
        /// <summary>
        /// 定时器次数。
        /// </summary>
        private static int iTimes = 0;
        /// <summary>
        /// 文件夹大小，字节数。
        /// </summary>
        private static long lFloderLength = 0;
        /// <summary>
        /// 順序排列的箭頭。
        /// </summary>
        private static string ASC = ((char)0x25b2).ToString().PadLeft(2, ' ');
        /// <summary>
        /// 倒序排列的箭頭。
        /// </summary>
        private static string DESC = ((char)0x25bc).ToString().PadLeft(2, ' ');
        /// <summary>
        /// 当前状态。
        /// </summary>
        private static string sStatus = "";
        /// <summary>
        /// 当前扫描的文件夹路径。
        /// </summary>
        private static string sDirPath = "";
        /// <summary>
        /// 文件夹个数。
        /// </summary>
        public static int iDirCount = 0;
        /// <summary>
        /// 文件个数。
        /// </summary>
        public static int iFileCount = 0;
        /// <summary>
        /// 待添加到列表中的项次集合。
        /// </summary>
        public List<ListItem> ListItems = new List<ListItem>();
        /// <summary>
        /// ListView的总宽度。
        /// </summary>
        private int iBoxWidth = 640;
        /// <summary>
        /// 栏位宽度列表。
        /// </summary>
        private int[] iColWidth = { 50, 110, 150, 90, 120, 120 };
        /// <summary>
        /// 分析磁盘的线程。
        /// </summary>
        private Thread thread;
        /// <summary>
        /// 删除任务是否完成。
        /// </summary>
        private bool bDeleteOK;
        /// <summary>
        /// 待删除的目录和文件。
        /// </summary>
        private List<string> DeletePath = new List<string>();
        /// <summary>
        /// 是否删除目录自身。
        /// </summary>
        private bool bDeleteSelf;
        /// <summary>
        /// 是否正在执行删除任务。
        /// </summary>
        private bool bDeleting;
        /// <summary>
        /// 是否正在执行分析任务。
        /// </summary>
        private bool bAnalysing;
        /// <summary>
        /// 外部程序传递的参数。
        /// </summary>
        private string[] args = null;
        /// <summary>
        /// 当前选中的文件夹路径。
        /// </summary>
        private static string sSelectedDirPath = "";
        /// <summary>
        /// 排序的列。
        /// </summary>
        private int iSortColumn;
        /// <summary>
        /// 是否正序排列。
        /// </summary>
        private bool bAscOrder;
        #endregion

        #region FormMain
        public FormMain(string[] args)
        {
            InitializeComponent();
            this.args = args;
        }
        #endregion

        #region string GetNodeFullPath(TreeNode eNode)  获得指定节点的完整路径。
        /// <summary>
        /// 获得指定节点的完整路径。
        /// </summary>
        /// <param name="eNode">要分析的节点。</param>
        /// <returns>指定节点的完整路径。</returns>
        private string GetNodeFullPath(TreeNode eNode)
        {
            TreeNode myNode = eNode;
            string myDir = myNode.Tag.ToString();
            while (myNode.Parent != null && myNode.Parent.Text != "我的电脑")
            {
                myNode = myNode.Parent;
                string tmpText = myNode.Text;
                if (tmpText.Contains(":)"))
                    tmpText = tmpText.Substring(tmpText.IndexOf(":)") - 1, 2);
                if (!tmpText.EndsWith("\\"))
                    tmpText += '\\';
                myDir = myDir.Insert(0, tmpText);
            }
            return myDir;
        }
        #endregion

        #region ShowTreeViewNodes(TreeNode NodeDir)  向TreeView控件添加节点。
        /// <summary>
        /// 向TreeView控件添加节点。
        /// </summary>
        /// <param name="NodeDir"></param>
        private void ShowTreeViewNodes(TreeNode NodeDir)
        {
            try
            {
                NodeDir.Nodes.Clear();
                if (NodeDir.Parent == null)
                {
                    foreach (DriveInfo drv in DriveInfo.GetDrives())
                    {
                        if (drv.DriveType == DriveType.Fixed || drv.DriveType == DriveType.Removable)
                        {
                            string sDrvName = string.Format("{0} ({1}) {2} / {3}", drv.VolumeLabel, drv.Name.Remove(2), GetLength(drv.TotalFreeSpace), GetLength(drv.TotalSize));
                            TreeNode aNode = new TreeNode(sDrvName);
                            aNode.Tag = drv.Name;
                            NodeDir.Nodes.Add(aNode);
                        }
                    }
                }
                else
                {
                    foreach (string DirName in Directory.GetDirectories(GetNodeFullPath(NodeDir)))
                    {
                        string aDirName = DirName.Substring(DirName.LastIndexOf('\\') + 1);
                        TreeNode newNode = new TreeNode(aDirName);
                        newNode.Tag = aDirName;
                        NodeDir.Nodes.Add(newNode);
                    }
                }
            }
            catch
            { }
        }
        #endregion

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
                return B.ToString("F2") + "KB";
            else
            {
                B /= 1024;
                if (B < 1024)
                    return B.ToString("F2") + "MB";
                else
                {
                    B /= 1024;
                    return B.ToString("F2") + "GB";
                }
            }
        }
        #endregion

        #region showState(string text)  显示系统状态。
        /// <summary>
        /// 显示系统状态。
        /// </summary>
        /// <param name="text">当前状态描述。</param>
        private void showState(string text)
        {
            iTimes = 0;
            sStatus = text;
            tssLabStatus.Text = text;
        }
        #endregion

        #region ExplorerFloder(TextBox myTextBox)  浏览目录。
        /// <summary>
        /// 浏览目录。
        /// </summary>
        /// <param name="myTextBox">指定接收返回结果的文本框(TextBox)。</param>
        public void ExplorerFloder(TextBox myTextBox)
        {
            folderBrowserDialog1.SelectedPath = "";
            folderBrowserDialog1.ShowDialog();
            myTextBox.Text = folderBrowserDialog1.SelectedPath != "" ? folderBrowserDialog1.SelectedPath : myTextBox.Text;
        }
        #endregion

        #region AnalyseDirectory(string myDir, ListView lvResult)  搜索文件或文件夹。
        /// <summary>
        /// 分析文件夹及其内文件的磁盘开销情况。
        /// </summary>
        /// <param name="sourceFloder">指定文件夹。</param>
        /// <param name="lvResult">接收分析结果的ListView控件。</param>
        public void AnalyseDirectory(object myDir)
        {
            DirectoryInfo dir = new DirectoryInfo(myDir.ToString());
            if (dir.Exists)
            {
                bAnalysing = true;
                try
                {
                    long lengthTotal = 0;
                    foreach (DirectoryInfo di in dir.GetDirectories())
                    {
                        sStatus = "正在分析目录：" + di.Name;
                        lFloderLength = 0;
                        ListItems.Add(new ListItem()
                        {
                            Type = "文件夹",
                            Name = di.Name,
                            Path = di.FullName,
                            Length = GetFloderLength(di.FullName, ref lFloderLength),
                            LastWriteTime = di.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss"),
                            CreationTime = di.CreationTime.ToString("yyyy/MM/dd HH:mm:ss")
                        });
                        lengthTotal += lFloderLength;
                        iDirCount++;
                    }
                    foreach (FileInfo fi in dir.GetFiles())
                    {
                        sStatus = "正在分析文件：" + fi.Name;
                        ListItems.Add(new ListItem()
                        {
                            Type = fi.Extension,
                            Name = fi.Name,
                            Path = fi.FullName,
                            Length = fi.Length,
                            LastWriteTime = fi.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss"),
                            CreationTime = fi.CreationTime.ToString("yyyy/MM/dd HH:mm:ss")
                        });
                        lengthTotal += fi.Length;
                        iFileCount++;
                    }
                    ListItems.Add(new ListItem()
                    {
                        Type = "文件夹",
                        Name = dir.Name,
                        Path = dir.FullName,
                        Length = lengthTotal,
                        LastWriteTime = dir.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss"),
                        CreationTime = dir.CreationTime.ToString("yyyy/MM/dd HH:mm:ss")
                    });
                    sStatus = "目录分析完毕，正在组织列表";
                }
                catch (Exception Err)
                {
                    if (Err.Message == "正在中止线程。")
                    {
                        sStatus = "用户取消分析任务。";
                    }
                    else
                        sStatus = "分析指定目录时出错：" + Err.Message;
                }
            }
            else
            {
                StopTimer2("指定的目录不存在。");
            }
        }
        #endregion

        #region long GetFloderLength(string strFloder, ref long flen)  计算文件夹所占磁盘大小。
        /// <summary>
        /// 计算文件夹所占磁盘大小。
        /// </summary>
        /// <param name="strFloder">文件夹路径。</param>
        /// <param name="flen">所有文件大小总和。</param>
        /// <returns>文件夹的大小。</returns>
        public long GetFloderLength(string strFloder, ref long flen)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(strFloder);
                if (dir.Exists)
                {
                    foreach (FileSystemInfo fsi in dir.GetFileSystemInfos())
                    {
                        if (fsi is DirectoryInfo)
                        {
                            GetFloderLength(fsi.FullName, ref flen);
                        }
                        else
                        {
                            FileInfo fi = new FileInfo(fsi.FullName);
                            flen += fi.Length;
                        }
                    }
                    return flen;
                }
                else
                    return 0;
            }
            catch
            {
                return -1;
            }
        }
        #endregion

        #region bool CopyDirectory(string sourcePath, string aimPath)  複製目錄。
        /// <summary>
        /// 複製目錄。
        /// </summary>
        /// <param name="sourcePath">源目錄</param>
        /// <param name="aimPath">目標目錄</param>
        private bool CopyDirectory(string sourcePath, string aimPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(sourcePath);
                FileSystemInfo[] fsis = dir.GetFileSystemInfos();
                foreach (FileSystemInfo fsi in fsis)
                {
                    if (fsi is DirectoryInfo)
                    {
                        Directory.CreateDirectory(aimPath + "\\" + fsi.Name);
                        CopyDirectory(sourcePath + "\\" + fsi.Name, aimPath + "\\" + fsi.Name);
                    }
                    else
                    {
                        if (!File.Exists(aimPath + "\\" + fsi.Name))
                        {
                        DoCopy:
                            try
                            {
                                File.Copy(sourcePath + "\\" + fsi.Name, aimPath + "\\" + fsi.Name);
                            }
                            catch (Exception err)
                            {
                                string msg = "複製文件時出現以下異常：\n\n";
                                msg += err.Message;
                                msg += "\n\n你是否要繼續？";
                                DialogResult dr = MessageBox.Show(msg, "複製文件異常", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question);
                                switch (dr)
                                {
                                    case DialogResult.Retry:
                                        goto DoCopy;
                                    case DialogResult.Ignore:
                                        continue;
                                    case DialogResult.Abort:
                                        return false;
                                }
                            }
                        }
                        else
                        {
                            string msg = "文件已存在。\n\n";
                            FileInfo fi = new FileInfo(aimPath + "\\" + fsi.Name);
                            msg += "原文件：" + fi.FullName + "\n修改時間：" + fi.LastWriteTime;
                            msg += "\n\n替換為\n\n";
                            fi = new FileInfo(sourcePath + "\\" + fsi.Name);
                            msg += "新文件：" + fi.FullName + "\n修改時間：" + fi.LastWriteTime;
                            msg += "\n\n您確認要替換嗎？";
                            DialogResult dr = MessageBox.Show(msg, "確認文件替換", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            switch (dr)
                            {
                                case DialogResult.Yes:
                                    File.Copy(sourcePath + "\\" + fsi.Name, aimPath + "\\" + fsi.Name, true);
                                    break;
                                case DialogResult.No:
                                    continue;
                                case DialogResult.Cancel:
                                    return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch
            { }
            return true;
        }
        #endregion

        #region bool CutDirectory(string sourcePath, string aimPath)  剪切目錄。
        /// <summary>
        /// 剪切目錄。
        /// </summary>
        /// <param name="sourcePath">源目錄</param>
        /// <param name="aimPath">目標目錄</param>
        private bool CutDirectory(string sourcePath, string aimPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(sourcePath);
                FileSystemInfo[] fsis = dir.GetFileSystemInfos();
                foreach (FileSystemInfo fsi in fsis)
                {
                    if (fsi is DirectoryInfo)
                    {
                        Directory.CreateDirectory(aimPath + "\\" + fsi.Name);
                        CopyDirectory(sourcePath + "\\" + fsi.Name, aimPath + "\\" + fsi.Name);
                    }
                    else
                    {
                        if (!File.Exists(aimPath + "\\" + fsi.Name))
                        {
                        DoCopy:
                            try
                            {
                                File.Copy(sourcePath + "\\" + fsi.Name, aimPath + "" + fsi.Name);
                            }
                            catch (Exception err)
                            {
                                string msg = "複製文件時出現以下異常：\n\n";
                                msg += err.Message;
                                msg += "\n\n你是否要繼續？";
                                DialogResult dr = MessageBox.Show(msg, "剪切文件異常", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question);
                                switch (dr)
                                {
                                    case DialogResult.Retry:
                                        goto DoCopy;
                                    case DialogResult.Ignore:
                                        continue;
                                    case DialogResult.Abort:
                                        return false;
                                }
                            }
                        }
                        else
                        {
                            string msg = "文件已存在。\n\n";
                            FileInfo fi = new FileInfo(aimPath + "\\" + fsi.Name);
                            msg += "原文件：" + fi.FullName + "\n修改時間：" + fi.LastWriteTime;
                            msg += "\n\n替換為\n\n";
                            fi = new FileInfo(sourcePath + "\\" + fsi.Name);
                            msg += "新文件：" + fi.FullName + "\n修改時間：" + fi.LastWriteTime;
                            msg += "\n\n您確認要替換嗎？";
                            DialogResult dr = MessageBox.Show(msg, "確認文件替換", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            switch (dr)
                            {
                                case DialogResult.Yes:
                                    File.Copy(sourcePath + "\\" + fsi.Name, aimPath + "\\" + fsi.Name, true);
                                    break;
                                case DialogResult.No:
                                    continue;
                                case DialogResult.Cancel:
                                    return false;
                            }
                        }
                    }
                }
                Directory.Delete(sourcePath, true);
                return true;
            }
            catch
            { }
            return true;
        }
        #endregion

        #region bool CopyFile(string sourceFile, string aimFile)  複製文件。
        /// <summary>
        /// 複製文件。
        /// </summary>
        /// <param name="sourceFile">源文件。</param>
        /// <param name="aimFile">目標文件。</param>
        /// <returns>複製是否成功。</returns>
        public bool CopyFile(string sourceFile, string aimFile)
        {
            try
            {
                if (File.Exists(aimFile))
                {
                    string msg = "目标文件已经存在。\n\n";
                    FileInfo fi = new FileInfo(aimFile);
                    msg += "原文件：" + fi.FullName + "\n修改時間：" + fi.LastWriteTime;
                    msg += "\n\n将被替換為\n\n";
                    fi = new FileInfo(sourceFile);
                    msg += "新文件：" + fi.FullName + "\n修改時間：" + fi.LastWriteTime;
                    msg += "\n\n您確認要替換嗎？";
                    DialogResult dr = MessageBox.Show(msg, "確認文件替換", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    switch (dr)
                    {
                        case DialogResult.Yes:
                            File.Copy(sourceFile, aimFile, true);
                            break;
                        case DialogResult.No:
                            return false;
                    }
                }
                else
                    File.Copy(sourceFile, aimFile);
                return true;
            }
            catch(Exception Err)
            {
                string msg = "系统返回如下错误：\n";
                msg += Err.Message;
                msg += "\n\n您要继续尝试复制吗？";
                DialogResult dr = MessageBox.Show(msg, "复制文件时出错", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                switch (dr)
                {
                    case DialogResult.Yes:
                        File.Copy(sourceFile, aimFile);
                        break;
                    case DialogResult.No:
                        return false;
                }
                return true;
            }
        }
        #endregion

        #region bool CutFile(string sourceFile, string aimFile)  剪切文件。
        /// <summary>
        /// 剪切文件。
        /// </summary>
        /// <param name="sourceFile">源文件。</param>
        /// <param name="aimFile">目標文件。</param>
        /// <returns>剪切是否成功。</returns>
        public bool CutFile(string sourceFile, string aimFile)
        {
            try
            {
                if (File.Exists(aimFile))
                {
                    string msg = "目标文件已经存在。\n\n";
                    FileInfo fi = new FileInfo(aimFile);
                    msg += "原文件：" + fi.FullName + "\n修改時間：" + fi.LastWriteTime;
                    msg += "\n\n将被替換為\n\n";
                    fi = new FileInfo(sourceFile);
                    msg += "新文件：" + fi.FullName + "\n修改時間：" + fi.LastWriteTime;
                    msg += "\n\n您確認要替換嗎？";
                    DialogResult dr = MessageBox.Show(msg, "確認文件替換", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    switch (dr)
                    {
                        case DialogResult.Yes:
                            {
                                File.Delete(aimFile);
                                File.Move(sourceFile, aimFile);
                                break;
                            }
                        case DialogResult.No:
                            return false;
                    }
                }
                else
                    File.Move(sourceFile, aimFile);
                return true;
            }
            catch (Exception Err)
            {
                string msg = "系统返回如下错误：\n";
                msg += Err.Message;
                msg += "\n\n您要继续尝试吗？";
                DialogResult dr = MessageBox.Show(msg, "剪切文件时出错", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                switch (dr)
                {
                    case DialogResult.Yes:
                        File.Move(sourceFile, aimFile);
                        break;
                    case DialogResult.No:
                        return false;
                }
                return true;
            }
        }
        #endregion

        #region bool DeleteFile(string aimFile)  删除文件。
        /// <summary>
        /// 删除文件。
        /// </summary>
        /// <param name="aimFile">要删除的文件。</param>
        /// <returns></returns>
        public bool DeleteFile(string aimFile)
        {
            try
            {
                FileInfo fi = new FileInfo(aimFile);
                if (fi.IsReadOnly)
                    fi.IsReadOnly = false;
                fi.Delete();
                return true;
            }
            catch (Exception Err)
            {
                string msg = "系统返回如下错误：\n";
                msg += Err.Message;
                msg += "\n\n您要继续尝试吗？";
                DialogResult dr = MessageBox.Show(msg, "删除文件时出错", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                switch (dr)
                {
                    case DialogResult.Yes:
                        return DeleteFile(aimFile);
                    case DialogResult.No:
                        return false;
                    default:
                        return false;
                }
            }
        }
        #endregion

        #region DoClearDirectory
        /// <summary>
        /// 执行清空目录动作。
        /// </summary>
        private void DoClearDirectory()
        { 
            try
            {
                sStatus = "正在执行删除任务...";
                bDeleting = true;
                bDeleteOK = false;
                bool Succ = true;
                foreach (string Floder in DeletePath)
                {
                    if (!string.IsNullOrEmpty(Floder))
                        Succ = ClearDirectory(Floder, bDeleteSelf);
                    else
                        break;
                    if (!Succ)
                        break;
                }
                bDeleteOK = Succ;
                if (Succ)
                {
                    sStatus = "删除完成，正在清理列表...";
                }
                else
                    sStatus = "删除目录任务被中止。";
            }
            catch(Exception ex)
            {
                sStatus = "准备清空目录时出错：" + ex.Message;
            }
        }
        #endregion

        #region bool ClearDirectory(string aimFloder, bool DeleteSelf)  清空或者删除目录。
        /// <summary>
        /// 清空或者删除目录。
        /// </summary>
        /// <param name="aimPath">想要清空或删除的目录或者文件。</param>
        /// <param name="DeleteSelf">是否删除文件夹自身。</param>
        /// <returns>是否成功清空目录。</returns>
        public bool ClearDirectory(string aimPath, bool DeleteSelf)
        {
            string CurFloder = "";
            try
            {
                sStatus = "正在删除：" + aimPath;
                if (File.Exists(aimPath))
                    return DeleteFile(aimPath);
                DirectoryInfo dir = new DirectoryInfo(aimPath);
                foreach (FileSystemInfo fsi in dir.GetFileSystemInfos())
                {
                    sStatus = "正在删除：" + fsi.FullName;
                    CurFloder = fsi.FullName;
                    fsi.Delete();
                }
                if (DeleteSelf)
                    dir.Delete();
                return true;
            }
            catch (Exception Err)
            {
                bool Succ = true;
                if (Err.Message.Contains("不是空的"))
                {
                    Succ = ClearDirectory(CurFloder, DeleteSelf);
                    if (Succ)
                        Succ = ClearDirectory(aimPath, DeleteSelf);
                }
                else if (Err.Message.Contains("目錄名稱不正確"))
                {
                    Succ = DeleteFile(aimPath);
                }
                else if (Err.Message.Contains("的访问被拒绝"))
                {
                    if (Err.Message.Length > 12)
                    {
                        string sFile = Err.Message.Substring(4, Err.Message.Length - 12);
                        Succ = DeleteFile(aimPath + "\\" + sFile);
                    }
                    else
                        Succ = false;
                }
                else
                    Succ = false;
                if (!Succ)
                {
                    DialogResult dr = MessageBox.Show("清空目录过程中出现了如下错误：\n\n" + Err.Message + "\n\n您要重试吗？", "出错了", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    switch (dr)
                    {
                        case DialogResult.Yes:
                            return ClearDirectory(aimPath, DeleteSelf);
                        case DialogResult.No:
                            return false;
                    }
                    return false;
                }
                else
                    return true;
            }
        }
        #endregion

        #region OpenFile(string strFile)  开启文档。
        /// <summary>
        /// 开启文档。
        /// </summary>
        /// <param name="strFile"></param>
        /// <returns></returns>
        public void OpenFile(string strFile)
        {
            System.Diagnostics.Process.Start(strFile);
        }
        #endregion

        #region 窗体加载。
        private void SmartExplorer_Load(object sender, EventArgs e)
        {
            //默认按文件大小倒序排序
            iSortColumn = 3;
            bAscOrder = false;
            //解析继承的排序规则
            if (args.Length >= 2)
            {
                try
                {
                    string sSort = args[1];
                    iSortColumn = int.Parse(sSort.Split('|')[0]);
                    bAscOrder = sSort.Split('|')[1] == "1";
                }
                catch
                { }
            }
            SortListView();
            //解析指定的目录
            if (args.Length >= 1)
            {
                string sPath = args[0];
                if (Directory.Exists(sPath))
                {
                    PrepareAnalyse(sPath);
                }
            }
            showState("正在加载磁盘列表...");
            ShowTreeViewNodes(tvDir.Nodes[0]);
            tvDir.Nodes[0].Expand();
            showState("等待选择目录，并执行分析...");
        }
        #endregion

        #region 选择TreeView某节点时展开其下属节点。
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowTreeViewNodes(e.Node);
        }
        #endregion

        #region 单击ListView某列时进行排序。
        private void lvResult_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            iSortColumn = e.Column;
            bAscOrder = !lvResult.Columns[e.Column].Text.Contains(ASC); //每次点击列标题切换排序顺序
            SortListView();
        }
        #endregion

        #region SortListView  对列表进行排序。
        /// <summary>
        /// 对列表进行排序。
        /// </summary>
        private void SortListView()
        {
            //如果是希望排序的那个列，则给列的标题增加排序标志，其他列的标题去掉排序标志
            for (int i = 0; i < lvResult.Columns.Count; i++)
            {
                string oldCaption = lvResult.Columns[i].Text;
                if (oldCaption.Contains(ASC))
                    lvResult.Columns[i].Text = oldCaption.Replace(ASC, "");
                else if (oldCaption.Contains(DESC))
                    lvResult.Columns[i].Text = oldCaption.Replace(DESC, "");
                if (iSortColumn == i)
                {
                    lvResult.Columns[i].Text += (bAscOrder ? ASC : DESC);
                }
            }
            //文件大小列安装文件大小排序，其他列按字符串排序
            if (iSortColumn == 3)
                lvResult.ListViewItemSorter = new ListViewItemComparerBySize(iSortColumn, bAscOrder);
            else
                lvResult.ListViewItemSorter = new ListViewItemComparerByString(iSortColumn, bAscOrder);
            lvResult.Sort();
        }
        #endregion

        #region 双击ListView某项时执行的动作。
        private void lvResult_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (lvResult.SelectedItems.Count > 0)
                {
                    ListViewItem lvi = lvResult.SelectedItems[0];
                    string selePath = lvi.SubItems[2].Text;
                    if (lvi.Text == "文件夹")
                        PrepareAnalyse(selePath);
                    else
                        OpenFile(selePath);
                }
            }
        }
        #endregion

        #region PrepareAnalyse
        private void PrepareAnalyse(string sPath)
        {
            if (Directory.Exists(sPath))
            {
                showState("准备启动分析...");
                sDirPath = sPath;
                lvResult.Items.Clear();
                ListItems.Clear();
                iDirCount = 0;
                iFileCount = 0;
                thread = new Thread(new ParameterizedThreadStart(AnalyseDirectory));
                thread.Start(sPath);
                timer2.Enabled = true;
            }
            else
                showState("目录不正确!");
        }
        #endregion

        #region “分析磁盘开销>>>”按钮被单击时分析TreeView选中的节点。
        private void btnAnalyse_Click(object sender, EventArgs e)
        {
            if (!timer2.Enabled)
            {
                timer2.Enabled = true;
                sDirPath = GetNodeFullPath(tvDir.SelectedNode);
                PrepareAnalyse(sDirPath);
            }
            else
            {
                timer2.Enabled = false;
                thread.Abort();
                ListItems.Clear();
                btnAnalyse.Text = "分析磁盘开销>>>";
            }
        }
        #endregion

        #region “独立分析”按钮
        private void btnAnalyseFloder_Click(object sender, EventArgs e)
        {
            Process.Start(Application.ExecutablePath, string.Format("\"{0}\" {1}", sSelectedDirPath, iSortColumn.ToString() + '|' + (bAscOrder ? 1 : 0)));
        }
        #endregion

        #region “分析上一级”按钮。
        private void btnPreExplorer_Click(object sender, EventArgs e)
        {
            string souFloder = sDirPath.Substring(0, sDirPath.LastIndexOf("\\"));
            if (souFloder.EndsWith(":"))
                souFloder += "\\";
            PrepareAnalyse(souFloder);
        }
        #endregion

        #region “复制...”按钮。
        private void btnCopyTo_Click(object sender, EventArgs e)
        {
            if (lvResult.SelectedItems.Count < 1)
            {
                MessageBox.Show("请选择您要复制的项目。", "尚未选择项目");
            }
            else
            {
                TextBox tmpTB = new TextBox();
                ExplorerFloder(tmpTB);
                string AimFloder = tmpTB.Text;
                if (AimFloder != "")
                {
                    showState("正在复制指定目录或文件...");
                    bool Succ = false;
                    foreach (ListViewItem lvi in lvResult.SelectedItems)
                    {
                        string sourcePath = lvi.SubItems[2].Text;
                        string sourceFile = lvi.SubItems[1].Text;
                        if (lvi.Text == "文件夹")
                            Succ = CopyDirectory(sourcePath, AimFloder + "\\" + sourceFile);
                        else
                            Succ = CopyFile(sourcePath, AimFloder + "\\" + sourceFile);
                        if (!Succ)
                            break;
                    }
                    if(Succ)
                        showState("复制任务完成！");
                    else
                        showState("复制任务被中断！");
                }
            }
        }
        #endregion

        #region “剪切...”按钮。
        private void btnCutTo_Click(object sender, EventArgs e)
        {
            if (lvResult.SelectedItems.Count < 1)
            {
                MessageBox.Show("请选择您要剪切的项目。", "尚未选择项目");
            }
            else
            {
                TextBox tmpTB = new TextBox();
                ExplorerFloder(tmpTB);
                string AimFloder = tmpTB.Text;
                if (AimFloder != "")
                {
                    showState("正在剪切指定目录或文件...");
                    bool Succ = false;
                    foreach (ListViewItem lvi in lvResult.SelectedItems)
                    {
                        string sourceFile = lvi.SubItems[1].Text;
                        string sourcePath = lvi.SubItems[2].Text;
                        if (lvi.Text == "文件夹")
                            Succ = CutDirectory(sourcePath, AimFloder + "\\" + sourceFile);
                        else
                            Succ = CutFile(sourcePath, AimFloder + "\\" + sourceFile);
                        if (!Succ)
                            break;
                    }
                    if (Succ)
                        showState("剪切任务完成！");
                    else
                        showState("剪切任务被中断！");
                }
            }
        }
        #endregion

        #region “打开”按钮
        private void btnOpen_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvResult.SelectedItems)
            {
                OpenFile(lvi.SubItems[2].Text);
            }
        }
        #endregion

        #region “清空目录”按钮。
        private void btnClear_Click(object sender, EventArgs e)
        {
            DeletePath.Clear();
            foreach (ListViewItem lvi in lvResult.SelectedItems)
            {
                if (lvi.Text == "文件夹")
                {
                    DeletePath.Add(lvi.SubItems[2].Text);
                }
            }
            if (DeletePath.Count > 0 && DeletePath[0].Length > 3)
            {
                string msg = "继续操作将清空指定目录内的所有内容，包括文件夹和文件。\n\n您确认要清空以下选定的目录吗？\n\n";
                int i = 0;
                while (i < 10 && i < DeletePath.Count)
                {
                    msg += DeletePath[i] + "\n";
                    i++;
                }
                if (DeletePath.Count > 10)
                    msg += "...\n";
                if (DialogResult.OK == MessageBox.Show(msg, "清空目录警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                {
                    bDeleteSelf = false;
                    thread = new Thread(new ThreadStart(DoClearDirectory));
                    thread.Start();
                    timer2.Start();
                }
                else
                    showState("用户取消清空任务。");
            }
            else
                MessageBox.Show("请选择要清空的目录。", "没有选择目录");
        }
        #endregion

        #region “删  除”按钮。
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeletePath.Clear();
            foreach (ListViewItem lvi in lvResult.SelectedItems)
                DeletePath.Add(lvi.SubItems[2].Text);
            if (DeletePath.Count > 0 && DeletePath[0].Length > 3)
            {
                showState("等待用户确认删除...");
                string msg = "继续操作将删除指定的文件或者目录及其内的所有内容，包括文件夹和文件。\n\n您确认要删除以下选定的文件和目录吗？\n\n";
                int i = 0;
                while (i < 10 && i < DeletePath.Count)
                {
                    msg += DeletePath[i] + "\n";
                    i++;
                }
                if (DeletePath.Count > 10)
                    msg += "...\n";
                if (DialogResult.OK == MessageBox.Show(msg, "删除警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                {
                    bDeleteSelf = true;
                    thread = new Thread(new ThreadStart(DoClearDirectory));
                    thread.Start();
                    timer2.Start();
                }
                else
                    showState("用户取消删除任务。");
            }
            else
                MessageBox.Show("请选择要删除的目录。", "没有选择目录");
        }
        #endregion

        #region timer1_Tick
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!(bAnalysing || bDeleting))
            {
                btnAnalyse.Text = "分析磁盘开销>>>";
                btnPreFloder.Enabled = true;
                btnOpen.Enabled = true;
                btnCopyTo.Enabled = true;
                btnCutTo.Enabled = true;
                btnClear.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                btnAnalyse.Text = "取消本次任务";
                btnPreFloder.Enabled = false;
                btnOpen.Enabled = false;
                btnCopyTo.Enabled = false;
                btnCutTo.Enabled = false;
                btnClear.Enabled = false;
                btnDelete.Enabled = false;
            }
            tssLabPath.Text = sDirPath;
            tssLabCount.Text = iDirCount + "/" + iFileCount;
            if (bAnalysing)
            {
                if (ListItems.Count > 0)
                {
                    for (var i = 0; i < ListItems.Count; i++)
                    {
                        ListItem item = ListItems[0];
                        ListViewItem lvi = new ListViewItem(item.Type);
                        lvi.SubItems.Add(item.Name);
                        lvi.SubItems.Add(item.Path);
                        ListViewItem.ListViewSubItem newSubItem = new ListViewItem.ListViewSubItem();
                        newSubItem.Text = item.Size;
                        newSubItem.Tag = item.Length;
                        lvi.SubItems.Add(newSubItem);
                        lvi.SubItems.Add(item.LastWriteTime);
                        lvi.SubItems.Add(item.CreationTime);
                        lvResult.Items.Add(lvi);
                        ListItems.RemoveAt(0);
                    }
                }
                else if (!thread.IsAlive)
                {
                    bAnalysing = false;
                    if (sStatus.Contains("目录分析完毕"))
                        StopTimer2("目录分析完成。");
                    else
                        StopTimer2(sStatus);
                }
            }
            else if (bDeleting && !thread.IsAlive)
            {
                if (bDeleteOK)
                {
                    foreach (ListViewItem lvi in lvResult.SelectedItems)
                    {
                        //如果删除的是当前分析的目录，则把分析结果列表清空
                        if (lvi.SubItems[2].Text == sDirPath)
                        {
                            lvResult.Items.Clear();
                            break;
                        }
                        else
                            lvi.Remove();
                    }
                    StopTimer2("删除目录任务已经完成。");
                }
                bDeleting = false;
            }
            Application.DoEvents();
        }
        #endregion

        #region timer2_Tick
        private void timer2_Tick(object sender, EventArgs e)
        {
            iTimes++;
            string sDot = "";
            for (int i = 0; i < iTimes; i++)
                sDot += ".";
            if (iTimes == 5)
                iTimes = 0;
            tssLabStatus.Text = sStatus + " " + sDot;
            Application.DoEvents();
        }
        #endregion

        #region StopTimer2
        private void StopTimer2(string sMsg)
        {
            showState(sMsg);
            tssLabStatus.Text = sMsg;
            timer2.Enabled = false;
        }
        #endregion

        #region FormMain_Resize
        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                tvDir.Height = this.Height - 82;
                lvResult.Height = this.Height - 111;
                lvResult.Width = this.Width - 260;
                float changePercent = (float)(lvResult.Width - iBoxWidth) / iBoxWidth;
                iBoxWidth = lvResult.Width;
                for (int i = 0; i < 6; i++)
                {
                    lvResult.Columns[i].Width += (int)(lvResult.Columns[i].Width * changePercent);
                }
                tssLabStatus.Width += (int)(tssLabStatus.Width * changePercent);
                tssLabPath.Width += (int)(tssLabPath.Width * changePercent);
            }
        }
        #endregion

        #region lvResult_ColumnWidthChanged
        private void lvResult_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                if (lvResult.Columns[i].Width > 0)
                {
                    iColWidth[i] = lvResult.Columns[i].Width;
                }
            }
        }
        #endregion

        #region lvResult_ItemSelectionChanged
        private void lvResult_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                btnAnalyseFloder.Enabled = e.Item.Text == "文件夹";
                sSelectedDirPath = e.Item.SubItems[2].Text;
            }
        }
        #endregion
    }
}

