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

namespace BYSerial.Views
{
    /// <summary>
    /// ColorPickerWin.xaml 的交互逻辑
    /// </summary>
    public partial class ColorPickerWin : Window
    {

        private SolidColorBrush _SelectedBrush =Brushes.Black;
        public SolidColorBrush SelectedBrush
        {
            get => _SelectedBrush;
            private set
            {
                _SelectedBrush = value;
            }
        }
        public ColorPickerWin()
        {
            InitializeComponent();
            
        }

        private void ColorSel_Canceled(object sender, EventArgs e)
        {
            this.DialogResult = false;
        }

        private void ColorSel_SelectedColorChanged(object sender, HandyControl.Data.FunctionEventArgs<Color> e)
        {
            //if (!IsLoad) return;
            //this.SelectedBrush = ColorSel.SelectedBrush;
            //this.DialogResult = true;
        }

        private bool IsLoad = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoad = true;
        }

        private void ColorSel_Confirmed(object sender, HandyControl.Data.FunctionEventArgs<Color> e)
        {
            this.SelectedBrush = ColorSel.SelectedBrush;
            this.DialogResult = true;
        }
    }
}
