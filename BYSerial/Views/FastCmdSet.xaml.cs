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
        public FastCmdModel CmdPara { get; set; }=new FastCmdModel();
        public FastCmdSet(FastCmdModel para)
        {
            InitializeComponent();
            txtCaption.Text=para.Remark;
            txtCmd.Text = para.CmdString;
            txtDelay.Text = para.DelayTime.ToString();
            CmdPara=para;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string txt=txtCaption.Text.Trim();
            string cmd=txtCmd.Text.Trim();
            string delay=txtDelay.Text.Trim();
            if(txt=="" ||cmd=="" || delay=="")
            {
                MessageBox.Show("两参数均不可为空", "提示");
                return;
            }
            CmdPara.Remark =txt;
            CmdPara.CmdString = cmd;
            int ide = 0;
            int.TryParse(delay, out ide);
            CmdPara.DelayTime=ide;
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
