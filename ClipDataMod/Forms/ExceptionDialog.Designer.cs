namespace ClipDataMod.Forms
{
    partial class ExceptionDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionDialog));
            LblDesc = new Label();
            LblType = new Label();
            LblMessage = new Label();
            TbStack = new TextBox();
            BtnPrev = new Button();
            BtnNext = new Button();
            BtnExport = new Button();
            BtnClose = new Button();
            CbFilter = new CheckBox();
            ToolTipHelp = new ToolTip(components);
            LblPlugin = new Label();
            SuspendLayout();
            // 
            // LblDesc
            // 
            LblDesc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LblDesc.AutoEllipsis = true;
            LblDesc.Location = new Point(12, 9);
            LblDesc.Name = "LblDesc";
            LblDesc.Size = new Size(460, 27);
            LblDesc.TabIndex = 0;
            LblDesc.Text = "Description";
            // 
            // LblType
            // 
            LblType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LblType.AutoEllipsis = true;
            LblType.Location = new Point(12, 36);
            LblType.Name = "LblType";
            LblType.Size = new Size(460, 27);
            LblType.TabIndex = 0;
            LblType.Text = "Type";
            // 
            // LblMessage
            // 
            LblMessage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LblMessage.AutoEllipsis = true;
            LblMessage.Location = new Point(12, 63);
            LblMessage.Name = "LblMessage";
            LblMessage.Size = new Size(460, 27);
            LblMessage.TabIndex = 0;
            LblMessage.Text = "Message";
            // 
            // TbStack
            // 
            TbStack.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TbStack.Location = new Point(12, 120);
            TbStack.Multiline = true;
            TbStack.Name = "TbStack";
            TbStack.ReadOnly = true;
            TbStack.ScrollBars = ScrollBars.Both;
            TbStack.Size = new Size(460, 200);
            TbStack.TabIndex = 1;
            TbStack.WordWrap = false;
            // 
            // BtnPrev
            // 
            BtnPrev.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnPrev.Location = new Point(12, 326);
            BtnPrev.Name = "BtnPrev";
            BtnPrev.Size = new Size(75, 23);
            BtnPrev.TabIndex = 2;
            BtnPrev.Text = "<<";
            BtnPrev.UseVisualStyleBackColor = true;
            BtnPrev.Click += BtnPrev_Click;
            // 
            // BtnNext
            // 
            BtnNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnNext.Location = new Point(93, 326);
            BtnNext.Name = "BtnNext";
            BtnNext.Size = new Size(75, 23);
            BtnNext.TabIndex = 3;
            BtnNext.Text = ">>";
            BtnNext.UseVisualStyleBackColor = true;
            BtnNext.Click += BtnNext_Click;
            // 
            // BtnExport
            // 
            BtnExport.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnExport.Location = new Point(290, 326);
            BtnExport.Name = "BtnExport";
            BtnExport.Size = new Size(101, 23);
            BtnExport.TabIndex = 4;
            BtnExport.Text = "Report a bug";
            ToolTipHelp.SetToolTip(BtnExport, "Export exception details to JSON");
            BtnExport.UseVisualStyleBackColor = true;
            BtnExport.Click += BtnExport_Click;
            // 
            // BtnClose
            // 
            BtnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnClose.Location = new Point(397, 326);
            BtnClose.Name = "BtnClose";
            BtnClose.Size = new Size(75, 23);
            BtnClose.TabIndex = 5;
            BtnClose.Text = "Close";
            BtnClose.UseVisualStyleBackColor = true;
            BtnClose.Click += BtnClose_Click;
            // 
            // CbFilter
            // 
            CbFilter.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            CbFilter.AutoSize = true;
            CbFilter.Checked = true;
            CbFilter.CheckState = CheckState.Checked;
            CbFilter.Location = new Point(174, 330);
            CbFilter.Name = "CbFilter";
            CbFilter.Size = new Size(110, 17);
            CbFilter.TabIndex = 6;
            CbFilter.Text = "Filter Stack Trace";
            ToolTipHelp.SetToolTip(CbFilter, "If checked, shows only stack trace lines with associated source files");
            CbFilter.UseVisualStyleBackColor = true;
            CbFilter.CheckedChanged += CbFilter_CheckedChanged;
            // 
            // LblPlugin
            // 
            LblPlugin.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LblPlugin.AutoEllipsis = true;
            LblPlugin.Location = new Point(12, 90);
            LblPlugin.Name = "LblPlugin";
            LblPlugin.Size = new Size(460, 27);
            LblPlugin.TabIndex = 0;
            LblPlugin.Text = "Plugin";
            // 
            // ExceptionDialog
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 361);
            Controls.Add(CbFilter);
            Controls.Add(BtnClose);
            Controls.Add(BtnExport);
            Controls.Add(BtnNext);
            Controls.Add(BtnPrev);
            Controls.Add(TbStack);
            Controls.Add(LblPlugin);
            Controls.Add(LblMessage);
            Controls.Add(LblType);
            Controls.Add(LblDesc);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(500, 400);
            Name = "ExceptionDialog";
            Text = "An unexpected error occured";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LblDesc;
        private Label LblType;
        private Label LblMessage;
        private TextBox TbStack;
        private Button BtnPrev;
        private Button BtnNext;
        private Button BtnExport;
        private Button BtnClose;
        private CheckBox CbFilter;
        private ToolTip ToolTipHelp;
        private Label LblPlugin;
    }
}