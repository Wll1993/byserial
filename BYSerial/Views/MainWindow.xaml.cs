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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BYSerial.Util;
using BYSerial.ViewModels;
using BYSerial.Models;

namespace BYSerial.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _IsLoaded = false;
        private MainWindowViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewModel.SendPara.IsLoop=false;
            GlobalPara.SaveCurCfg();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void txtRich_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtRich.ScrollToEnd();
        }

        //private void devPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (!_IsLoaded) return;
        //    if(devPort.SelectedIndex==(devPort.Items.Count-1))
        //    {
        //        viewModel.IsSerialTest = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        viewModel.IsSerialTest = Visibility.Visible;
        //    }
        //}

        private void tcpMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_IsLoaded) return;
            try
            {
                if (tcpMode.SelectedIndex == 0)
                {
                    viewModel.TcpPara.IsTcpClient = Visibility.Visible;
                }
                else
                {
                    viewModel.TcpPara.IsTcpClient = Visibility.Collapsed;
                }
                if (viewModel.IsStopCan)
                {
                    viewModel.OnStopCommand.ExecuteAction(null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _IsLoaded=true;
            try
            {                
                if(GlobalPara.IsCheckUpdate)
                {
                    Task.Run(() => {
                        int i = 0;
                        if (Util.Update.InternetGetConnectedState(out i, 0))
                        {
                            BYSerial.Util.Update.CheckUpdate();
                        }
                    });
                }
                for(int i=0;i<15;i++)
                {
                    _CmdStringLst.Add("");
                }
                this.Title+= "_Version:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }

        private void CbbHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_IsLoaded) return;
            viewModel. AddHisToSendText();
        }
        /// <summary>
        /// 命令字符串列表
        /// </summary>
        private List<string> _CmdStringLst = new List<string>();
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;
                int id = Convert.ToInt32(btn.Tag);
                string cmd = _CmdStringLst[id];
                if(cmd=="")
                {
                    MessageBox.Show("命令字符串为空，请先设置命令字符串", "错误提示");
                    return;
                }
                viewModel.SendCommandByFast(cmd);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "错误提示");
            }
        }

        private void btn1_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;
                int id =Convert.ToInt32( btn.Tag);
                string caption = "";
                if(btn.Content!=null)
                {
                    caption=btn.Content.ToString();
                }
                CmdButtonPara cbp=new CmdButtonPara() { CmdString = _CmdStringLst[id], Content=caption};
                FastCmdSet fcs = new FastCmdSet(cbp);
                bool bret=(bool) fcs.ShowDialog();
                if(bret)
                {
                    _CmdStringLst[id] = fcs.CmdPara.CmdString;
                    btn.Content = fcs.CmdPara.Content;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"错误提示");
            }
        }
    }
}
