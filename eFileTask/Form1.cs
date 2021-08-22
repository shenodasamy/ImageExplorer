using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace eFileTask
{
    public partial class Form1 : Form
    {
        public string SelectedFolder { get; set; } = string.Empty;
        public string SelectedImage { get; set; } = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            this.SelectedFolder = fbd.SelectedPath;    

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string[] files = Directory.GetFiles(fbd.SelectedPath);
                string[] folders = Directory.GetDirectories(fbd.SelectedPath);
                foreach (var folder in folders)
                    filesTreeView.Nodes.Add(folder.GetUnitName());
                filesTreeView.Nodes.Add(new TreeNode());
                foreach (var file in files)
                {
                    var fileName = file.GetUnitName();
                    if(fileName.IsImage()) filesTreeView.Nodes.Add(fileName);
                }
            }
        }
        private void filesTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var currentImagePath = this.SelectedFolder + @"\" + e.Node.Text;
            if (currentImagePath.IsImage())
            {
                pictureBox1.Image = Image.FromFile(currentImagePath);
                SelectedImage= currentImagePath;
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            // connect to database
            String str = "server=.;database=eFile_ImageExplorer;Integrated Security=SSPI;MultipleActiveResultSets=true;";
            using SqlConnection con = new SqlConnection(str);
            con.Open();

            // check image exists
            if (String.IsNullOrEmpty(SelectedImage))
            {
                MessageBox.Show("Select an Image");
                return;
            }

            var image = new Bitmap(SelectedImage).ToByte();
            var funcResult = con.GetImageCount_Id(image);
            int count = funcResult.count;
            int id=funcResult.id;

            


            if (count > 0) {
                con.UpdateData(image, NameTb.Text, AddressTb.Text, EmailTb.Text);

            }
            else con.InsertData(image, NameTb.Text, AddressTb.Text, EmailTb.Text);
            con.Close();
        }
    }
}
