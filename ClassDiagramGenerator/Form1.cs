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

namespace ClassDiagramGenerator
{
    public partial class Form1 : Form
    {
        Bitmap generatedImage;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = FileHandler.SelectOpenFile(openFileDialog1);
            label1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = FileHandler.SelectSaveFile(saveFileDialog1);
            label1.Visible = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            generatedImage = ImageGenerator.GenerateImage(textBox1.Text, textBox2.Text);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Visible = true;
            var diagramForm = new Diagram(generatedImage);
            diagramForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
    }
}
