using System.Drawing;
using System.Windows.Forms;

namespace KLHash
{
    partial class MainForm
    {
        private Label lblTitle = null!;
        private Panel bottomContainer = null!;
        private TextBox txtDisplay = null!;
        private ProgressBar progressBar = null!;
        private Button btnBrowse = null!;
        private Button btnCancel = null!;
        private Button btnCopy = null!;
        private Button btnContextMenu = null!;
        private CheckBox chkUpperCase = null!;
        private Label lblStatus = null!;
        private FlowLayoutPanel buttonPanel = null!;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            lblTitle = new Label();
            bottomContainer = new Panel();
            lblStatus = new Label();
            progressBar = new ProgressBar();
            buttonPanel = new FlowLayoutPanel();
            btnCopy = new Button();
            btnCancel = new Button();
            btnBrowse = new Button();
            btnContextMenu = new Button();
            chkUpperCase = new CheckBox();
            txtDisplay = new TextBox();
            bottomContainer.SuspendLayout();
            buttonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = new Font("微软雅黑", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(41, 183, 203);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(808, 40);
            lblTitle.TabIndex = 2;
            lblTitle.Text = "KL-Hash 计算器";
            // 
            // bottomContainer
            // 
            bottomContainer.Controls.Add(lblStatus);
            bottomContainer.Controls.Add(progressBar);
            bottomContainer.Controls.Add(buttonPanel);
            bottomContainer.Dock = DockStyle.Bottom;
            bottomContainer.Location = new Point(20, 327);
            bottomContainer.Name = "bottomContainer";
            bottomContainer.Padding = new Padding(0, 10, 0, 0);
            bottomContainer.Size = new Size(808, 100);
            bottomContainer.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.ForeColor = Color.DimGray;
            lblStatus.Location = new Point(0, 22);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(808, 38);
            lblStatus.TabIndex = 0;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            progressBar.Dock = DockStyle.Top;
            progressBar.Location = new Point(0, 10);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(808, 12);
            progressBar.TabIndex = 1;
            progressBar.Visible = false;
            // 
            // buttonPanel
            // 
            buttonPanel.Controls.Add(btnCopy);
            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnBrowse);
            buttonPanel.Controls.Add(btnContextMenu);
            buttonPanel.Controls.Add(chkUpperCase);
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Location = new Point(0, 60);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Padding = new Padding(0, 5, 0, 0);
            buttonPanel.Size = new Size(808, 40);
            buttonPanel.TabIndex = 2;
            buttonPanel.WrapContents = false;
            // 
            // btnCopy
            // 
            btnCopy.Enabled = false;
            btnCopy.FlatStyle = FlatStyle.System;
            btnCopy.Location = new Point(718, 5);
            btnCopy.Margin = new Padding(10, 0, 0, 0);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(90, 32);
            btnCopy.TabIndex = 0;
            btnCopy.Text = "复制结果";
            btnCopy.Click += OnCopyClick;
            // 
            // btnCancel
            // 
            btnCancel.Enabled = false;
            btnCancel.FlatStyle = FlatStyle.System;
            btnCancel.Location = new Point(628, 5);
            btnCancel.Margin = new Padding(10, 0, 0, 0);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 32);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "取消";
            btnCancel.Visible = false;
            btnCancel.Click += OnCancelClick;
            // 
            // btnBrowse
            // 
            btnBrowse.FlatStyle = FlatStyle.System;
            btnBrowse.Location = new Point(518, 5);
            btnBrowse.Margin = new Padding(10, 0, 0, 0);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(100, 32);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "选择文件";
            btnBrowse.Click += OnBrowseClick;
            // 
            // btnContextMenu
            // 
            btnContextMenu.FlatStyle = FlatStyle.System;
            btnContextMenu.Location = new Point(398, 5);
            btnContextMenu.Margin = new Padding(10, 0, 0, 0);
            btnContextMenu.Name = "btnContextMenu";
            btnContextMenu.Size = new Size(110, 32);
            btnContextMenu.TabIndex = 4;
            btnContextMenu.Text = "添加到右键菜单";
            btnContextMenu.Click += OnContextMenuToggleClick;
            // 
            // chkUpperCase
            // 
            chkUpperCase.AutoSize = true;
            chkUpperCase.Location = new Point(260, 10);
            chkUpperCase.Margin = new Padding(10, 5, 0, 0);
            chkUpperCase.Name = "chkUpperCase";
            chkUpperCase.Size = new Size(112, 24);
            chkUpperCase.TabIndex = 3;
            chkUpperCase.Text = "字母使用大写";
            chkUpperCase.CheckedChanged += OnCaseToggle;
            // 
            // txtDisplay
            // 
            txtDisplay.AllowDrop = true;
            txtDisplay.BackColor = Color.White;
            txtDisplay.BorderStyle = BorderStyle.FixedSingle;
            txtDisplay.Dock = DockStyle.Fill;
            txtDisplay.Font = new Font("Consolas", 12F);
            txtDisplay.Location = new Point(20, 60);
            txtDisplay.MinimumSize = new Size(200, 100);
            txtDisplay.Multiline = true;
            txtDisplay.Name = "txtDisplay";
            txtDisplay.ReadOnly = true;
            txtDisplay.ScrollBars = ScrollBars.Vertical;
            txtDisplay.Size = new Size(808, 267);
            txtDisplay.TabIndex = 0;
            txtDisplay.DragDrop += OnDragDrop;
            txtDisplay.DragEnter += OnDragEnter;
            // 
            // MainForm
            // 
            AllowDrop = true;
            ClientSize = new Size(848, 447);
            Controls.Add(txtDisplay);
            Controls.Add(bottomContainer);
            Controls.Add(lblTitle);
            Font = new Font("微软雅黑", 10.5F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(640, 360);
            Name = "MainForm";
            Padding = new Padding(20);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "KL-Hash 计算器 1.2";
            DragDrop += OnDragDrop;
            DragEnter += OnDragEnter;
            bottomContainer.ResumeLayout(false);
            buttonPanel.ResumeLayout(false);
            buttonPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
