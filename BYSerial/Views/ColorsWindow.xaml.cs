using BYSerial.Base;
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
using System.Collections.ObjectModel;
using System.Reflection;

namespace BYSerial.Views
{
    /// <summary>
    /// ColorsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ColorsWindow : Window
    {
        public ColorsWindow()
        {
            InitializeComponent();
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() => {
                GetAllColors();
                lstColor.ItemsSource = MyColors;
            }));
            
        }

        private void GetAllColors()
        {
            try
            {
                MyColors = new ObservableCollection<ColorsUnit>();
                Type type = typeof(Brushes);
                PropertyInfo[] props = type.GetProperties(BindingFlags.Static |BindingFlags.Public );
                foreach (PropertyInfo prop in props)
                {
                    SolidColorBrush brush = (SolidColorBrush)prop.GetValue(null);
                    ColorsUnit cunit = new ColorsUnit();
                    cunit.ColorName = prop.Name;
                    cunit.BackColor = brush;
                    byte br = brush.Color.R;
                    byte br2 = brush.Color.G;
                    byte br3 = brush.Color.B;
                    byte br4 = brush.Color.A;
                    cunit.ColorRGB = $"{br4},{br},{br2},{br3}";
                    cunit.ColorHEX = brush.Color.ToString();
                    
                    MyColors.Add(cunit);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private ObservableCollection<ColorsUnit> _MyColors;
        public ObservableCollection<ColorsUnit> MyColors
        {
            get => _MyColors;
            set
            {
                _MyColors = value;
            }
        }

        public class ColorsUnit : NotificationObject
        {
            private string _ColorName;

            public string ColorName
            {
                get { return _ColorName; }
                set
                {
                    _ColorName = value;
                    RaisePropertyChanged("ColorName");
                }
            }

            private string _ColorRGB;

            public string ColorRGB
            {
                get { return _ColorRGB; }
                set
                {
                    _ColorRGB = value;
                    RaisePropertyChanged("ColorRGB");
                }
            }
            private string _ColorHEX;

            public string ColorHEX
            {
                get { return _ColorHEX; }
                set
                {
                    _ColorHEX = value;
                    RaisePropertyChanged("ColorHEX");
                }
            }
                        
            private SolidColorBrush _BackColor;

            public SolidColorBrush BackColor
            {
                get { return _BackColor; }
                set
                {
                    _BackColor = value;
                    RaisePropertyChanged("BackColor");
                }
            }


        }
    }
}
