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

using BYSerial.ViewModels;

namespace BYSerial.Views
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        AboutViewModel aboutViewModel;
        public AboutWindow()
        {
            InitializeComponent();
            aboutViewModel=new AboutViewModel();
            this.DataContext = aboutViewModel;
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lblGit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string txt = lblGit.Content.ToString();
                int len = txt.Length;
                string html = txt.Substring(7, len - 7);

                Util.FileTool.OpenWebWithUrl(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lblBlog_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string txt = lblBlog.Content.ToString();
                int len =txt.Length;
                string html = txt.Substring(5, len - 5);

                Util.FileTool.OpenWebWithUrl(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
