using System.Windows.Forms;

namespace OpenTKDTMContours
{
    public partial class Form1 : Form
    {
        private WinOpenTKNGIViewer? winOpenTKViewer;
        private bool SHOWEXISTINGTRIANGLES = false;
        private bool SHOWEXISTINGPOINTS = false;
        private bool SHOWEXISTINGCONTOURS = false;

        private bool SHOWPROPOSEDTRIANGLES = false;
        private bool SHOWPROPOSEDPOINTS = false;
        private bool SHOWPROPOSEDCONTOURS = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void FormViewer_Shown(object? sender, EventArgs e)
        {
            winOpenTKViewer = new WinOpenTKNGIViewer(this);
        }

        private string OpenFile()
        {
            string selectedFilename = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Wavefront_obj files (*.obj)|*.obj|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;
                    if (filePath != null && filePath.ToLower().EndsWith(".obj"))
                    {
                        //Load Wavefront obj file for viewing
                        LogTextBox.AppendText("\n ToDo add code to load obj files");

                        selectedFilename = filePath;
                    }
                    else
                    {
                        throw new Exception("unknown file type selected");
                    }
                }
            }
            return selectedFilename;
        }

        private void existingGroundToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string filename = OpenFile();
            if (string.IsNullOrEmpty(filename) == false)
            {
                winOpenTKViewer?.LoadExistingGroundFromFile(filename);
                toolStripMenuItemShowExisting.Enabled = true;
                ShowExistingPoints_Click(pointsToolStripMenuItem, new EventArgs());
            }
        }

        private void ProposedGroundToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            string filename = OpenFile();
            if (string.IsNullOrEmpty(filename) == false)
            {
                winOpenTKViewer?.LoadProposedGroundFromFile(filename);
                toolStripMenuItemShowProposed.Enabled = true;
                ShowProposedPoints_Click(pointsToolStripMenuItem1, new EventArgs());
            }
        }

        private void modelsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = OpenFile();
            if (string.IsNullOrEmpty(filename) == false)
            {
                winOpenTKViewer?.LoadModelsFromFile(filename);
                //Fix this later
                modelsFileToolStripMenuItem.Enabled = true;
                toolStripMenuItemShowModel.Enabled = true;
                toolStripMenuItemShowModel.Checked = true;
            }
        }


        //remove below when convenient
        private void computeContoursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //winOpenTKViewer.
        }

        private void existingGroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            winOpenTKViewer?.ComputeExistingGroundContours();
            ShowExistingContours_Click(contoursToolStripMenuItem, new EventArgs());

        }

        private void proposedGroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            winOpenTKViewer?.ComputeProposedGroundContours();
            ShowProposedContours_Click(contoursToolStripMenuItem1, new EventArgs());
        }

        private void computeVolumesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            winOpenTKViewer?.ComputeVolumes();
        }

        private void orthometricCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            winOpenTKViewer?.UseOrthometricCamera();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowExistingTriangles_Click(object sender, EventArgs e)
        {
            SHOWEXISTINGTRIANGLES = !SHOWEXISTINGTRIANGLES;
            winOpenTKViewer.ShowExistingTriangles(SHOWEXISTINGTRIANGLES);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowExistingPoints_Click(object sender, EventArgs e)
        {
            SHOWEXISTINGPOINTS = !SHOWEXISTINGPOINTS;
            ToolStripMenuItem? mitem = sender as ToolStripMenuItem;
            mitem.Checked = SHOWEXISTINGPOINTS;
            winOpenTKViewer.ShowExistingPoints(SHOWEXISTINGPOINTS);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowExistingContours_Click(object sender, EventArgs e)
        {
            SHOWEXISTINGCONTOURS = !SHOWEXISTINGCONTOURS;
            ToolStripMenuItem? mitem = sender as ToolStripMenuItem;
            mitem.Checked = SHOWEXISTINGCONTOURS;
            winOpenTKViewer.ShowExistingContours(SHOWEXISTINGCONTOURS);
        }

        private void ShowProposedTriangles_Click(object sender, EventArgs e)
        {
            SHOWPROPOSEDTRIANGLES = !SHOWPROPOSEDTRIANGLES;
            winOpenTKViewer.ShowProposedTriangles(SHOWPROPOSEDTRIANGLES);
        }

        private void ShowProposedPoints_Click(object sender, EventArgs e)
        {
            SHOWPROPOSEDPOINTS = !SHOWPROPOSEDPOINTS;
            ToolStripMenuItem? mitem = sender as ToolStripMenuItem;
            mitem.Checked = SHOWPROPOSEDPOINTS;
            winOpenTKViewer.ShowProposedPoints(SHOWPROPOSEDPOINTS);
        }

        private void ShowProposedContours_Click(object sender, EventArgs e)
        {
            SHOWPROPOSEDCONTOURS = !SHOWPROPOSEDCONTOURS;
            ToolStripMenuItem? mitem = sender as ToolStripMenuItem;
            mitem.Checked = SHOWPROPOSEDCONTOURS;
            winOpenTKViewer.ShowProposedContours(SHOWPROPOSEDCONTOURS);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}