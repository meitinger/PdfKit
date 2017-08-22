using System.Windows.Forms;

namespace Aufbauwerk.Tools.PdfKit
{
    public partial class ViewForm : Form
    {
        public ViewForm(string path)
        {
            InitializeComponent();
            Text = path;
            viewer.Path = path;
        }
    }
}
