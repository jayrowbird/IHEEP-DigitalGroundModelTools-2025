namespace OpenTKDTMContours
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            existingGroundToolStripMenuItem2 = new ToolStripMenuItem();
            ProposedGroundToolStripMenuItem3 = new ToolStripMenuItem();
            modelsFileToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator = new ToolStripSeparator();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            printToolStripMenuItem = new ToolStripMenuItem();
            printPreviewToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            Tools = new ToolStripMenuItem();
            showToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemShowExisting = new ToolStripMenuItem();
            trianglesToolStripMenuItem1 = new ToolStripMenuItem();
            pointsToolStripMenuItem = new ToolStripMenuItem();
            contoursToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemShowProposed = new ToolStripMenuItem();
            trianglesToolStripMenuItem = new ToolStripMenuItem();
            pointsToolStripMenuItem1 = new ToolStripMenuItem();
            contoursToolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItemShowModel = new ToolStripMenuItem();
            settingToolStripMenuItem = new ToolStripMenuItem();
            existingContoursToolStripMenuItem = new ToolStripMenuItem();
            majorToolStripMenuItem = new ToolStripMenuItem();
            minorToolStripMenuItem = new ToolStripMenuItem();
            proposedContoursToolStripMenuItem = new ToolStripMenuItem();
            majorToolStripMenuItem1 = new ToolStripMenuItem();
            minorToolStripMenuItem1 = new ToolStripMenuItem();
            computeContoursToolStripMenuItem = new ToolStripMenuItem();
            existingGroundToolStripMenuItem = new ToolStripMenuItem();
            proposedGroundToolStripMenuItem = new ToolStripMenuItem();
            computeVolumesToolStripMenuItem = new ToolStripMenuItem();
            orthometricCameraToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            contentsToolStripMenuItem = new ToolStripMenuItem();
            indexToolStripMenuItem = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            LogTextBox = new TextBox();
            glControl = new OpenTK.GLControl.GLControl();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, Tools, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripSeparator, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator1, printToolStripMenuItem, printPreviewToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Image = (Image)resources.GetObject("newToolStripMenuItem.Image");
            newToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem.Size = new Size(146, 22);
            newToolStripMenuItem.Text = "&New";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { existingGroundToolStripMenuItem2, ProposedGroundToolStripMenuItem3, modelsFileToolStripMenuItem });
            openToolStripMenuItem.Image = (Image)resources.GetObject("openToolStripMenuItem.Image");
            openToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new Size(146, 22);
            openToolStripMenuItem.Text = "&Open";
            // 
            // existingGroundToolStripMenuItem2
            // 
            existingGroundToolStripMenuItem2.Name = "existingGroundToolStripMenuItem2";
            existingGroundToolStripMenuItem2.Size = new Size(180, 22);
            existingGroundToolStripMenuItem2.Text = "Existing Ground";
            existingGroundToolStripMenuItem2.Click += existingGroundToolStripMenuItem2_Click;
            // 
            // ProposedGroundToolStripMenuItem3
            // 
            ProposedGroundToolStripMenuItem3.Name = "ProposedGroundToolStripMenuItem3";
            ProposedGroundToolStripMenuItem3.Size = new Size(180, 22);
            ProposedGroundToolStripMenuItem3.Text = "Proposed Ground";
            ProposedGroundToolStripMenuItem3.Click += ProposedGroundToolStripMenuItem3_Click;
            // 
            // modelsFileToolStripMenuItem
            // 
            modelsFileToolStripMenuItem.Name = "modelsFileToolStripMenuItem";
            modelsFileToolStripMenuItem.Size = new Size(180, 22);
            modelsFileToolStripMenuItem.Text = "Models file";
            modelsFileToolStripMenuItem.Visible = false;
            modelsFileToolStripMenuItem.Click += modelsFileToolStripMenuItem_Click;
            // 
            // toolStripSeparator
            // 
            toolStripSeparator.Name = "toolStripSeparator";
            toolStripSeparator.Size = new Size(143, 6);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Image = (Image)resources.GetObject("saveToolStripMenuItem.Image");
            saveToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(146, 22);
            saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(146, 22);
            saveAsToolStripMenuItem.Text = "Save &As";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(143, 6);
            // 
            // printToolStripMenuItem
            // 
            printToolStripMenuItem.Image = (Image)resources.GetObject("printToolStripMenuItem.Image");
            printToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            printToolStripMenuItem.Name = "printToolStripMenuItem";
            printToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.P;
            printToolStripMenuItem.Size = new Size(146, 22);
            printToolStripMenuItem.Text = "&Print";
            // 
            // printPreviewToolStripMenuItem
            // 
            printPreviewToolStripMenuItem.Image = (Image)resources.GetObject("printPreviewToolStripMenuItem.Image");
            printPreviewToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            printPreviewToolStripMenuItem.Size = new Size(146, 22);
            printPreviewToolStripMenuItem.Text = "Print Pre&view";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(143, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(146, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // Tools
            // 
            Tools.DropDownItems.AddRange(new ToolStripItem[] { showToolStripMenuItem, settingToolStripMenuItem, computeContoursToolStripMenuItem, computeVolumesToolStripMenuItem, orthometricCameraToolStripMenuItem });
            Tools.Name = "Tools";
            Tools.Size = new Size(46, 20);
            Tools.Text = "Tools";
            // 
            // showToolStripMenuItem
            // 
            showToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItemShowExisting, toolStripMenuItemShowProposed, toolStripMenuItemShowModel });
            showToolStripMenuItem.Name = "showToolStripMenuItem";
            showToolStripMenuItem.Size = new Size(183, 22);
            showToolStripMenuItem.Text = "Show";
            // 
            // toolStripMenuItemShowExisting
            // 
            toolStripMenuItemShowExisting.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripMenuItemShowExisting.DropDownItems.AddRange(new ToolStripItem[] { trianglesToolStripMenuItem1, pointsToolStripMenuItem, contoursToolStripMenuItem });
            toolStripMenuItemShowExisting.Enabled = false;
            toolStripMenuItemShowExisting.Name = "toolStripMenuItemShowExisting";
            toolStripMenuItemShowExisting.Size = new Size(180, 22);
            toolStripMenuItemShowExisting.Text = "Existing Ground";
            // 
            // trianglesToolStripMenuItem1
            // 
            trianglesToolStripMenuItem1.CheckOnClick = true;
            trianglesToolStripMenuItem1.Name = "trianglesToolStripMenuItem1";
            trianglesToolStripMenuItem1.Size = new Size(123, 22);
            trianglesToolStripMenuItem1.Text = "Triangles";
            trianglesToolStripMenuItem1.Click += ShowExistingTriangles_Click;
            // 
            // pointsToolStripMenuItem
            // 
            pointsToolStripMenuItem.CheckOnClick = true;
            pointsToolStripMenuItem.Name = "pointsToolStripMenuItem";
            pointsToolStripMenuItem.Size = new Size(123, 22);
            pointsToolStripMenuItem.Text = "Points";
            pointsToolStripMenuItem.Click += ShowExistingPoints_Click;
            // 
            // contoursToolStripMenuItem
            // 
            contoursToolStripMenuItem.CheckOnClick = true;
            contoursToolStripMenuItem.Name = "contoursToolStripMenuItem";
            contoursToolStripMenuItem.Size = new Size(123, 22);
            contoursToolStripMenuItem.Text = "Contours";
            contoursToolStripMenuItem.Click += ShowExistingContours_Click;
            // 
            // toolStripMenuItemShowProposed
            // 
            toolStripMenuItemShowProposed.DropDownItems.AddRange(new ToolStripItem[] { trianglesToolStripMenuItem, pointsToolStripMenuItem1, contoursToolStripMenuItem1 });
            toolStripMenuItemShowProposed.Enabled = false;
            toolStripMenuItemShowProposed.Name = "toolStripMenuItemShowProposed";
            toolStripMenuItemShowProposed.Size = new Size(180, 22);
            toolStripMenuItemShowProposed.Text = "Proposed Ground";
            // 
            // trianglesToolStripMenuItem
            // 
            trianglesToolStripMenuItem.CheckOnClick = true;
            trianglesToolStripMenuItem.Name = "trianglesToolStripMenuItem";
            trianglesToolStripMenuItem.Size = new Size(123, 22);
            trianglesToolStripMenuItem.Text = "Triangles";
            trianglesToolStripMenuItem.Click += ShowProposedTriangles_Click;
            // 
            // pointsToolStripMenuItem1
            // 
            pointsToolStripMenuItem1.CheckOnClick = true;
            pointsToolStripMenuItem1.Name = "pointsToolStripMenuItem1";
            pointsToolStripMenuItem1.Size = new Size(123, 22);
            pointsToolStripMenuItem1.Text = "Points";
            pointsToolStripMenuItem1.Click += ShowProposedPoints_Click;
            // 
            // contoursToolStripMenuItem1
            // 
            contoursToolStripMenuItem1.CheckOnClick = true;
            contoursToolStripMenuItem1.Name = "contoursToolStripMenuItem1";
            contoursToolStripMenuItem1.Size = new Size(123, 22);
            contoursToolStripMenuItem1.Text = "Contours";
            contoursToolStripMenuItem1.Click += ShowProposedContours_Click;
            // 
            // toolStripMenuItemShowModel
            // 
            toolStripMenuItemShowModel.CheckOnClick = true;
            toolStripMenuItemShowModel.Enabled = false;
            toolStripMenuItemShowModel.Name = "toolStripMenuItemShowModel";
            toolStripMenuItemShowModel.Size = new Size(180, 22);
            toolStripMenuItemShowModel.Text = "Models";
            toolStripMenuItemShowModel.Visible = false;
            // 
            // settingToolStripMenuItem
            // 
            settingToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { existingContoursToolStripMenuItem, proposedContoursToolStripMenuItem });
            settingToolStripMenuItem.Name = "settingToolStripMenuItem";
            settingToolStripMenuItem.Size = new Size(183, 22);
            settingToolStripMenuItem.Text = "Setting";
            // 
            // existingContoursToolStripMenuItem
            // 
            existingContoursToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { majorToolStripMenuItem, minorToolStripMenuItem });
            existingContoursToolStripMenuItem.Name = "existingContoursToolStripMenuItem";
            existingContoursToolStripMenuItem.Size = new Size(176, 22);
            existingContoursToolStripMenuItem.Text = "Existing Contours";
            // 
            // majorToolStripMenuItem
            // 
            majorToolStripMenuItem.Name = "majorToolStripMenuItem";
            majorToolStripMenuItem.Size = new Size(106, 22);
            majorToolStripMenuItem.Text = "Major";
            // 
            // minorToolStripMenuItem
            // 
            minorToolStripMenuItem.Name = "minorToolStripMenuItem";
            minorToolStripMenuItem.Size = new Size(106, 22);
            minorToolStripMenuItem.Text = "Minor";
            // 
            // proposedContoursToolStripMenuItem
            // 
            proposedContoursToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { majorToolStripMenuItem1, minorToolStripMenuItem1 });
            proposedContoursToolStripMenuItem.Name = "proposedContoursToolStripMenuItem";
            proposedContoursToolStripMenuItem.Size = new Size(176, 22);
            proposedContoursToolStripMenuItem.Text = "Proposed Contours";
            // 
            // majorToolStripMenuItem1
            // 
            majorToolStripMenuItem1.Name = "majorToolStripMenuItem1";
            majorToolStripMenuItem1.Size = new Size(106, 22);
            majorToolStripMenuItem1.Text = "Major";
            // 
            // minorToolStripMenuItem1
            // 
            minorToolStripMenuItem1.Name = "minorToolStripMenuItem1";
            minorToolStripMenuItem1.Size = new Size(106, 22);
            minorToolStripMenuItem1.Text = "Minor";
            // 
            // computeContoursToolStripMenuItem
            // 
            computeContoursToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { existingGroundToolStripMenuItem, proposedGroundToolStripMenuItem });
            computeContoursToolStripMenuItem.Name = "computeContoursToolStripMenuItem";
            computeContoursToolStripMenuItem.Size = new Size(183, 22);
            computeContoursToolStripMenuItem.Text = "Compute Contours";
            computeContoursToolStripMenuItem.Click += computeContoursToolStripMenuItem_Click;
            // 
            // existingGroundToolStripMenuItem
            // 
            existingGroundToolStripMenuItem.Name = "existingGroundToolStripMenuItem";
            existingGroundToolStripMenuItem.Size = new Size(167, 22);
            existingGroundToolStripMenuItem.Text = "Existing Ground";
            existingGroundToolStripMenuItem.Click += existingGroundToolStripMenuItem_Click;
            // 
            // proposedGroundToolStripMenuItem
            // 
            proposedGroundToolStripMenuItem.Name = "proposedGroundToolStripMenuItem";
            proposedGroundToolStripMenuItem.Size = new Size(167, 22);
            proposedGroundToolStripMenuItem.Text = "Proposed Ground";
            proposedGroundToolStripMenuItem.Click += proposedGroundToolStripMenuItem_Click;
            // 
            // computeVolumesToolStripMenuItem
            // 
            computeVolumesToolStripMenuItem.Enabled = false;
            computeVolumesToolStripMenuItem.Name = "computeVolumesToolStripMenuItem";
            computeVolumesToolStripMenuItem.Size = new Size(183, 22);
            computeVolumesToolStripMenuItem.Text = "Compute Volumes";
            computeVolumesToolStripMenuItem.Click += computeVolumesToolStripMenuItem_Click;
            // 
            // orthometricCameraToolStripMenuItem
            // 
            orthometricCameraToolStripMenuItem.Name = "orthometricCameraToolStripMenuItem";
            orthometricCameraToolStripMenuItem.Size = new Size(183, 22);
            orthometricCameraToolStripMenuItem.Text = "Orthometric Camera";
            orthometricCameraToolStripMenuItem.Click += orthometricCameraToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { contentsToolStripMenuItem, indexToolStripMenuItem, searchToolStripMenuItem, toolStripSeparator5, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // contentsToolStripMenuItem
            // 
            contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            contentsToolStripMenuItem.Size = new Size(180, 22);
            contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            indexToolStripMenuItem.Size = new Size(180, 22);
            indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(180, 22);
            searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(177, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(180, 22);
            aboutToolStripMenuItem.Text = "&About...";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 24);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = Color.Moccasin;
            splitContainer1.Panel1.Controls.Add(LogTextBox);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(glControl);
            splitContainer1.Size = new Size(800, 426);
            splitContainer1.SplitterDistance = 140;
            splitContainer1.TabIndex = 1;
            // 
            // LogTextBox
            // 
            LogTextBox.Dock = DockStyle.Bottom;
            LogTextBox.Location = new Point(0, 376);
            LogTextBox.Multiline = true;
            LogTextBox.Name = "LogTextBox";
            LogTextBox.Size = new Size(140, 50);
            LogTextBox.TabIndex = 0;
            // 
            // glControl
            // 
            glControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl.APIVersion = new Version(4, 3, 0, 0);
            glControl.Dock = DockStyle.Fill;
            glControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glControl.IsEventDriven = true;
            glControl.Location = new Point(0, 0);
            glControl.Name = "glControl";
            glControl.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
            glControl.SharedContext = null;
            glControl.Size = new Size(656, 426);
            glControl.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            Shown += FormViewer_Shown;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem printToolStripMenuItem;
        private ToolStripMenuItem printPreviewToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem contentsToolStripMenuItem;
        private ToolStripMenuItem indexToolStripMenuItem;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private SplitContainer splitContainer1;
        internal OpenTK.GLControl.GLControl glControl;
        internal TextBox LogTextBox;
        private ToolStripMenuItem existingGroundToolStripMenuItem2;
        private ToolStripMenuItem ProposedGroundToolStripMenuItem3;
        private ToolStripMenuItem Tools;
        private ToolStripMenuItem showToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemShowExisting;
        private ToolStripMenuItem toolStripMenuItemShowProposed;
        private ToolStripMenuItem toolStripMenuItemShowModel;
        private ToolStripMenuItem settingToolStripMenuItem;
        private ToolStripMenuItem existingContoursToolStripMenuItem;
        private ToolStripMenuItem majorToolStripMenuItem;
        private ToolStripMenuItem minorToolStripMenuItem;
        private ToolStripMenuItem proposedContoursToolStripMenuItem;
        private ToolStripMenuItem majorToolStripMenuItem1;
        private ToolStripMenuItem minorToolStripMenuItem1;
        private ToolStripMenuItem modelsFileToolStripMenuItem;
        private ToolStripMenuItem computeContoursToolStripMenuItem;
        private ToolStripMenuItem existingGroundToolStripMenuItem;
        private ToolStripMenuItem proposedGroundToolStripMenuItem;
        internal ToolStripMenuItem computeVolumesToolStripMenuItem;
        private ToolStripMenuItem orthometricCameraToolStripMenuItem;
        private ToolStripMenuItem trianglesToolStripMenuItem1;
        private ToolStripMenuItem trianglesToolStripMenuItem;
        private ToolStripMenuItem pointsToolStripMenuItem;
        private ToolStripMenuItem contoursToolStripMenuItem;
        private ToolStripMenuItem pointsToolStripMenuItem1;
        private ToolStripMenuItem contoursToolStripMenuItem1;
    }
}
