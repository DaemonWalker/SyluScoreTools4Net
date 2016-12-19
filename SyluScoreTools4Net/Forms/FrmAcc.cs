using SyluScoreTools4Net.Class;
using SyluScroeTools4Net.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SyluScoreTools4Net.Forms
{
    public partial class FrmAcc : Form
    {
        private WebPost WebPost = new WebPost();

        public string UserName
        {
            get
            {
                return tbAcc.Text;
            }
        }

        public string Password
        {
            get
            {
                return tbPwd.Text;
            }
        }
        public FrmAcc()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            WebPost.UserName = tbAcc.Text;
            WebPost.PassWord = tbPwd.Text;
            try
            {
                WebPost.CheckAccPsw();
                MessageBox.Show("添加成功!");
                this.DialogResult = DialogResult.OK;
                this.Hide();
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
