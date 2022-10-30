using BYSerial.ViewModels;
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

using LiveCharts;
using LiveCharts.Configurations;
using BYSerial.Models;

namespace BYSerial.Views
{
    /// <summary>
    /// ChartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChartWindow : Window
    {
        public ChartViewModel viewModel { get; set; }
        private ChartWindow() { }
        public ChartWindow(ChartParas para)
        {
            InitializeComponent();
            viewModel = new ChartViewModel(para);
            this.DataContext = viewModel;
            //To handle live data easily, in this case we built a specialized type
            //the MeasureModel class, it only contains 2 properties
            //DateTime and Value
            //We need to configure LiveCharts to handle MeasureModel class
            //The next code configures MeasureModel  globally, this means
            //that LiveCharts learns to plot MeasureModel and will use this config every time
            //a IChartValues instance uses this type.
            //this code ideally should only run once
            //you can configure series in many ways, learn more at 
            //http://lvcharts.net/App/examples/v1/wpf/Types%20and%20Configuration
            var mapper = Mappers.Xy<MeasureData>()
                .X(model => model.DateTime.Ticks)
                .Y(model => model.Value);
            //lets save the mapper globally.
            Charting.For<MeasureData>(mapper);
            //Binding bindstroke1 = new Binding("LineStroke") { Source = this };
            //LineS1.SetBinding(LiveCharts.Wpf.LineSeries.StrokeProperty, bindstroke1);
            //Binding bindFill = new Binding("LineFill") { Source = this };
            //LineS1.SetBinding(LiveCharts.Wpf.LineSeries.FillProperty, bindFill);
           
        }       

        // Using a DependencyProperty as the backing store for MainBackGround.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainBackGroundProperty =
            DependencyProperty.Register("MainBackGround", typeof(Brush), typeof(ChartWindow), new PropertyMetadata(Brushes.Transparent));
        /// <summary>
        /// 线条阴影
        /// </summary>
        public Brush LineFill
        {
            get { return (Brush)GetValue(LineFillProperty); }
            set
            {
                SetValue(LineFillProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for MainBackGround.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineFillProperty =
            DependencyProperty.Register("LineFill", typeof(Brush), typeof(ChartWindow), new PropertyMetadata(Brushes.Transparent));

        public Brush LineStroke
        {
            get { return (Brush)GetValue(LineStrokeProperty); }
            set
            {
                SetValue(LineStrokeProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for MainBackGround.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineStrokeProperty =
            DependencyProperty.Register("LineStroke", typeof(Brush), typeof(ChartWindow), new PropertyMetadata(Brushes.Transparent));

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalPara.IsShowChart = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GlobalPara.IsShowChart = false;
        }
    }
}
