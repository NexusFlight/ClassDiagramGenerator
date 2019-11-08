using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassDiagramGenerator
{
    public partial class Diagram : Form
    {
        public Diagram(Bitmap generatedImage)
        {
            InitializeComponent();
            pictureBox1.Image = generatedImage;
        }
    }
}
