using HandyControl.Controls;
using IoTClient.Common.Enums;
using IoTClient.Enums;
using IoTClientDeskTop.Controls;
using IoTServer.Common;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace IoTClientDeskTop
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //加载模拟服务的历史数据
            DataPersist.LoadData();
            #region 初始化设置上次选择的tab
            string tabIndex = GetTabName();
            if (!string.IsNullOrWhiteSpace(tabIndex))
            {
                int index = 0;
                int.TryParse(tabIndex, out index );
                tabctrl.SelectedIndex = index;                
            }
            #endregion
            Task.Run(async () =>
            {
                await Task.Delay(1000 * 60 * 1);//1分钟自动保存一次
                DataPersist.SaveData();
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title += "_V:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()+"_Preview";
        }
        private string GetTabName()
        {
            var dataString = string.Empty;
            
            var path = @"C:\IoTClient";
            var filePath = path + @"\TabName.Data";
            if (File.Exists(filePath))
                dataString = File.ReadAllText(filePath);
            else
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                File.SetAttributes(path, FileAttributes.Hidden);
            }
            return dataString;
        }
        private void SaveTabName(string tagName)
        {
            var path = @"C:\IoTClient";
            var filePath = path + @"\TabName.Data";
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    sw.Write(tagName);
                }
            }
        }

        private void tabctrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveTabName(tabctrl.SelectedIndex.ToString());
            switch(tabctrl.SelectedIndex)
            {                
                case 3:
                    CreateNewCtrl(typeof(SiemensControl), SiemensVersion.S7_200);
                    cctrl_s7200.Content = siemens_s7200;
                    break;
                case 4:
                    CreateNewCtrl(typeof(SiemensControl), SiemensVersion.S7_200Smart);
                    cctrl_s7200smart.Content = siemens_s7200smart;
                    break;
                case 5:
                    CreateNewCtrl(typeof(SiemensControl), SiemensVersion.S7_300);
                    cctrl_s7300.Content = siemens_s7300;
                    break; 
                case 6:
                    CreateNewCtrl(typeof(SiemensControl), SiemensVersion.S7_400);
                    cctrl_s7400.Content = siemens_s7400;
                    break; 
                case 7:
                    CreateNewCtrl(typeof(SiemensControl), SiemensVersion.S7_1200);
                    cctrl_s71200.Content = siemens_s71200;
                    break;
                case 8:
                    CreateNewCtrl(typeof(SiemensControl), SiemensVersion.S7_1500);
                    cctrl_s71500.Content = siemens_s71500;
                    break;
                case 9:
                    CreateNewCtrl(typeof(MitsubishiMCControl), MitsubishiVersion.Qna_3E);
                    cctrl_mcqna3e.Content = mits_Qna_3E;
                    break;
                case 10:
                    CreateNewCtrl(typeof(MitsubishiMCControl), MitsubishiVersion.A_1E);
                    cctrl_mca1e.Content = mits_A_1E;
                    break;
            }

        }

        MitsubishiMCControl mits_Qna_3E=null;
        MitsubishiMCControl mits_A_1E=null;
        SiemensControl siemens_s7200 = null;
        SiemensControl siemens_s7200smart = null;
        SiemensControl siemens_s7300 = null;
        SiemensControl siemens_s7400 = null;
        SiemensControl siemens_s71200 = null;
        SiemensControl siemens_s71500 = null;
        private void CreateNewCtrl(Type type,object enum1)
        {            
            if(type==typeof(MitsubishiMCControl))
            {                
                MitsubishiVersion version= (MitsubishiVersion)enum1;
                if(version==MitsubishiVersion.Qna_3E)
                {
                    if(mits_Qna_3E==null)
                    {
                        mits_Qna_3E = new MitsubishiMCControl(version);
                    }                    
                }
                else if(version==MitsubishiVersion.A_1E)
                {
                    if(mits_A_1E==null)
                    {
                        mits_A_1E = new MitsubishiMCControl(version);
                    }                    
                }                
            }
            else if(type== typeof(SiemensControl))
            {
                SiemensVersion version= (SiemensVersion)enum1;
                switch(version)
                {
                    case SiemensVersion.S7_200:
                        if(siemens_s7200==null)
                        {
                            siemens_s7200 = new SiemensControl(version);
                        }
                        break;
                    case SiemensVersion.S7_200Smart:
                        if (siemens_s7200smart == null)
                        {
                            siemens_s7200smart = new SiemensControl(version);
                        }
                        break;
                    case SiemensVersion.S7_300:
                        if (siemens_s7300 == null)
                        {
                            siemens_s7300 = new SiemensControl(version);
                        }
                        break;
                    case SiemensVersion.S7_400:
                        if (siemens_s7400 == null)
                        {
                            siemens_s7400 = new SiemensControl(version);
                        }
                        break;
                    case SiemensVersion.S7_1200:
                        if (siemens_s71200 == null)
                        {
                            siemens_s71200 = new SiemensControl(version);
                        }
                        break;
                    case SiemensVersion.S7_1500:
                        if (siemens_s71500 == null)
                        {
                            siemens_s71500 = new SiemensControl(version);
                        }
                        break;
                }
            }
            
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow window = new AboutWindow();
            window.Show();
        }

        private void btnDonate_Click(object sender, RoutedEventArgs e)
        {
            DonateWindow window = new DonateWindow();
            window.Show();
        }
    }
}
