using SyluScoreTools4Net.Class;
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
    public partial class FrmVipSim : Form
    {
        private UserInfo _NowUser;
        public UserInfo NowUser
        {
            get
            {
                return _NowUser;
            }
            set
            {
                _NowUser = value;
                dgvData.DataSource = _NowUser.VIPClassList;
            }
        }
        public FrmVipSim()
        {
            InitializeComponent();
            dgvData.AutoGenerateColumns = false;
        }

        private string NewAvg
        {
            get
            {
                return labNewAvg.Text;
            }
            set
            {
                labNewAvg.Text = "新的绩点:" + value;
            }
        }

        private void dgvData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            double newScore = 0;
            if (!double.TryParse(NowUser.VIPClassList[e.RowIndex].NewScore, out newScore))
            {
                MessageBox.Show("你输入的绩点有误!");
                return;
            }
            CountNewAvg();
        }

        private void FrmVipSim_Load(object sender, EventArgs e)
        {
            labNewAvg.Alignment = ToolStripItemAlignment.Right;
            labOldAvg.Text = "当前绩点:" + NowUser.VIPAvg;
        }

        private void btnSetToOne_Click(object sender, EventArgs e)
        {
            dgvData.CellEndEdit -= dgvData_CellEndEdit;
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                if (double.Parse(row.Cells[3].Value.ToString()) < 1)
                {
                    row.Cells[3].Value = "1";
                }
            }
            dgvData.CellEndEdit += dgvData_CellEndEdit;
            CountNewAvg();
        }

        private void CountNewAvg()
        {
            double allScore = 0;
            double allPoints = 0;
            foreach (var info in NowUser.VIPClassList)
            {
                allPoints += info.ClassWeight;
                allScore += double.Parse(info.NewScore) * info.ClassWeight;
            }
            NewAvg = (allScore / allPoints).ToString();
        }
    }
}
