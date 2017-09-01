namespace DiskSpaceAnalyzer
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("我的电脑");
            this.tvDir = new System.Windows.Forms.TreeView();
            this.lvResult = new System.Windows.Forms.ListView();
            this.类型 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.文件名 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.路径 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.大小 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.修改日期 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.创建日期 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAnalyse = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssLabStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssLabPath = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssLabCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnCopyTo = new System.Windows.Forms.Button();
            this.btnCutTo = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnPreFloder = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnAnalyseFloder = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvDir
            // 
            this.tvDir.HotTracking = true;
            this.tvDir.Location = new System.Drawing.Point(9, 12);
            this.tvDir.Name = "tvDir";
            treeNode1.Name = "MyComputer";
            treeNode1.Tag = "我的电脑";
            treeNode1.Text = "我的电脑";
            this.tvDir.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tvDir.ShowRootLines = false;
            this.tvDir.Size = new System.Drawing.Size(218, 393);
            this.tvDir.TabIndex = 0;
            this.tvDir.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // lvResult
            // 
            this.lvResult.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.lvResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.类型,
            this.文件名,
            this.路径,
            this.大小,
            this.修改日期,
            this.创建日期});
            this.lvResult.FullRowSelect = true;
            this.lvResult.GridLines = true;
            this.lvResult.Location = new System.Drawing.Point(235, 41);
            this.lvResult.Name = "lvResult";
            this.lvResult.Size = new System.Drawing.Size(697, 364);
            this.lvResult.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.lvResult.TabIndex = 7;
            this.lvResult.UseCompatibleStateImageBehavior = false;
            this.lvResult.View = System.Windows.Forms.View.Details;
            this.lvResult.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvResult_ColumnClick);
            this.lvResult.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.lvResult_ColumnWidthChanged);
            this.lvResult.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvResult_ItemSelectionChanged);
            this.lvResult.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvResult_MouseDoubleClick);
            // 
            // 类型
            // 
            this.类型.Text = "类型";
            this.类型.Width = 50;
            // 
            // 文件名
            // 
            this.文件名.Text = "文件名";
            this.文件名.Width = 110;
            // 
            // 路径
            // 
            this.路径.Text = "路径";
            this.路径.Width = 150;
            // 
            // 大小
            // 
            this.大小.Text = "大小";
            this.大小.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.大小.Width = 90;
            // 
            // 修改日期
            // 
            this.修改日期.Text = "修改日期";
            this.修改日期.Width = 120;
            // 
            // 创建日期
            // 
            this.创建日期.Text = "创建日期";
            this.创建日期.Width = 120;
            // 
            // btnAnalyse
            // 
            this.btnAnalyse.AutoSize = true;
            this.btnAnalyse.Location = new System.Drawing.Point(243, 12);
            this.btnAnalyse.Name = "btnAnalyse";
            this.btnAnalyse.Size = new System.Drawing.Size(105, 23);
            this.btnAnalyse.TabIndex = 8;
            this.btnAnalyse.Text = "分析磁盘开销>>>";
            this.btnAnalyse.UseVisualStyleBackColor = true;
            this.btnAnalyse.Click += new System.EventHandler(this.btnAnalyse_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssLabStatus,
            this.toolStripStatusLabel1,
            this.tssLabPath,
            this.toolStripStatusLabel2,
            this.tssLabCount});
            this.statusStrip1.Location = new System.Drawing.Point(0, 415);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(944, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssLabStatus
            // 
            this.tssLabStatus.AutoSize = false;
            this.tssLabStatus.Name = "tssLabStatus";
            this.tssLabStatus.Size = new System.Drawing.Size(380, 17);
            this.tssLabStatus.Text = "等待选择目标，并执行分析...";
            this.tssLabStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel1.Text = "当前目录：";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tssLabPath
            // 
            this.tssLabPath.AutoSize = false;
            this.tssLabPath.Name = "tssLabPath";
            this.tssLabPath.Size = new System.Drawing.Size(250, 17);
            this.tssLabPath.Text = "尚未指定";
            this.tssLabPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(121, 17);
            this.toolStripStatusLabel2.Text = "包含一级目录/文件：";
            // 
            // tssLabCount
            // 
            this.tssLabCount.Name = "tssLabCount";
            this.tssLabCount.Size = new System.Drawing.Size(27, 17);
            this.tssLabCount.Text = "0/0";
            // 
            // btnCopyTo
            // 
            this.btnCopyTo.AutoSize = true;
            this.btnCopyTo.Location = new System.Drawing.Point(635, 12);
            this.btnCopyTo.Name = "btnCopyTo";
            this.btnCopyTo.Size = new System.Drawing.Size(69, 23);
            this.btnCopyTo.TabIndex = 10;
            this.btnCopyTo.Text = "复制到...";
            this.btnCopyTo.UseVisualStyleBackColor = true;
            this.btnCopyTo.Click += new System.EventHandler(this.btnCopyTo_Click);
            // 
            // btnCutTo
            // 
            this.btnCutTo.AutoSize = true;
            this.btnCutTo.Location = new System.Drawing.Point(710, 12);
            this.btnCutTo.Name = "btnCutTo";
            this.btnCutTo.Size = new System.Drawing.Size(69, 23);
            this.btnCutTo.TabIndex = 11;
            this.btnCutTo.Text = "剪切到...";
            this.btnCutTo.UseVisualStyleBackColor = true;
            this.btnCutTo.Click += new System.EventHandler(this.btnCutTo_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.AutoSize = true;
            this.btnDelete.Location = new System.Drawing.Point(857, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(66, 23);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.Text = "删  除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClear
            // 
            this.btnClear.AutoSize = true;
            this.btnClear.Location = new System.Drawing.Point(785, 12);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(66, 23);
            this.btnClear.TabIndex = 13;
            this.btnClear.Text = "清空目录";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnPreFloder
            // 
            this.btnPreFloder.AutoSize = true;
            this.btnPreFloder.Location = new System.Drawing.Point(354, 12);
            this.btnPreFloder.Name = "btnPreFloder";
            this.btnPreFloder.Size = new System.Drawing.Size(101, 23);
            this.btnPreFloder.TabIndex = 14;
            this.btnPreFloder.Text = "分析上一级目录";
            this.btnPreFloder.UseVisualStyleBackColor = true;
            this.btnPreFloder.Click += new System.EventHandler(this.btnPreExplorer_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 300;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // btnOpen
            // 
            this.btnOpen.AutoSize = true;
            this.btnOpen.Location = new System.Drawing.Point(578, 12);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(51, 23);
            this.btnOpen.TabIndex = 15;
            this.btnOpen.Text = "打 开";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnAnalyseFloder
            // 
            this.btnAnalyseFloder.AutoSize = true;
            this.btnAnalyseFloder.Location = new System.Drawing.Point(499, 12);
            this.btnAnalyseFloder.Name = "btnAnalyseFloder";
            this.btnAnalyseFloder.Size = new System.Drawing.Size(73, 23);
            this.btnAnalyseFloder.TabIndex = 14;
            this.btnAnalyseFloder.Text = "独立分析";
            this.btnAnalyseFloder.UseVisualStyleBackColor = true;
            this.btnAnalyseFloder.Click += new System.EventHandler(this.btnAnalyseFloder_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 437);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.lvResult);
            this.Controls.Add(this.btnAnalyseFloder);
            this.Controls.Add(this.btnPreFloder);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCutTo);
            this.Controls.Add(this.btnCopyTo);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnAnalyse);
            this.Controls.Add(this.tvDir);
            this.MinimumSize = new System.Drawing.Size(960, 476);
            this.Name = "FormMain";
            this.Text = "磁盘空间分析工具  --  By：张新峰";
            this.Load += new System.EventHandler(this.SmartExplorer_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvDir;
        private System.Windows.Forms.ListView lvResult;
        private System.Windows.Forms.ColumnHeader 文件名;
        private System.Windows.Forms.ColumnHeader 类型;
        private System.Windows.Forms.ColumnHeader 路径;
        private System.Windows.Forms.ColumnHeader 大小;
        private System.Windows.Forms.ColumnHeader 修改日期;
        private System.Windows.Forms.ColumnHeader 创建日期;
        private System.Windows.Forms.Button btnAnalyse;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssLabStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel tssLabPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnCopyTo;
        private System.Windows.Forms.Button btnCutTo;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnPreFloder;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel tssLabCount;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnAnalyseFloder;
    }
}

