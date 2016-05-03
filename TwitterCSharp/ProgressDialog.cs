using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TwitterCSharp
{
    public partial class ProgressDialog : Form
    {
        public ProgressDialog()
        {
            InitializeComponent();
        }
        public void SetIndeterminate(bool isIndeterminate)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.BeginInvoke(new Action(() =>
                {
                    if (isIndeterminate)
                        progressBar1.Style = ProgressBarStyle.Marquee;
                    else
                        progressBar1.Style = ProgressBarStyle.Blocks;
                }
                ));
            }
            else
            {
                if (isIndeterminate)
                    progressBar1.Style = ProgressBarStyle.Marquee;
                else
                    progressBar1.Style = ProgressBarStyle.Blocks;
            }
        }
        private void myWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void ProgressDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
