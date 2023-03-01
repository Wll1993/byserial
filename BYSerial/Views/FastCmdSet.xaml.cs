using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BYSerial.Models;

namespace BYSerial.Views
{
    /// <summary>
    /// FastCmdSet.xaml 的交互逻辑
    /// </summary>
    public partial class FastCmdSet : Window
    {
        public CmdButtonPara CmdPara { get; set; }=new CmdButtonPara();
        public FastCmdSet(CmdButtonPara para)
        {
            InitializeComponent();
            txtCaption.Text=para.Content;
            txtCmd.Text = para.CmdString;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string txt=txtCaption.Text.Trim();
            string cmd=txtCmd.Text.Trim();
            if(txt=="" ||cmd=="")
            {
                MessageBox.Show("两参数均不可为空", "提示");
                return;
            }
            CmdPara.Content=txt;
            CmdPara.CmdString = cmd;
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
