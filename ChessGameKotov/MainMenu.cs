using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGameKotov
{
    public partial class MainMenu: Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HumanAndHuman gameHumans = new HumanAndHuman();
            gameHumans.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HumanAndPC gameHumanAndPC = new HumanAndPC();
            gameHumanAndPC.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PCandPC gamePCandPC = new PCandPC();
            gamePCandPC.Show();
        }
    }
}
