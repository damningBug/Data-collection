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

namespace Data_collection.Views
{
    /// <summary>
    /// ExportFilter.xaml 的交互逻辑
    /// </summary>
    public partial class ExportFilter : Window
    {
        // 用于存储选择的条件
        public ExportFilterCriteria Criteria { get; private set; }
        public ExportFilter()
        {
            InitializeComponent();
            Criteria = new ExportFilterCriteria();

            // 默认选择等于操作符
            LengthOperator.SelectedIndex = 0;
            WidthOperator.SelectedIndex = 0;
            HeightOperator.SelectedIndex = 0;
        }
        // 颜色选择按钮事件
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var colorDialog = new System.Windows.Forms.ColorDialog();
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var button = sender as Button;
                    if (button == null)
                    {
                        System.Windows.MessageBox.Show("按钮引用无效。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var brush = new SolidColorBrush(Color.FromArgb(
                        colorDialog.Color.A,
                        colorDialog.Color.R,
                        colorDialog.Color.G,
                        colorDialog.Color.B));
                    button.Background = brush;

                    // 保存选择的颜色
                    switch (button.Name)
                    {
                        case "LengthColor":
                            Criteria.LengthColor = brush;
                            break;
                        case "WidthColor":
                            Criteria.WidthColor = brush;
                            break;
                        case "HeightColor":
                            Criteria.HeightColor = brush;
                            break;
                        default:
                            System.Windows.MessageBox.Show("未知的颜色按钮。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("颜色选择出错：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // 导出按钮事件
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            // 收集条件
            Criteria.LengthOperator = GetSelectedOperator(LengthOperator);
            Criteria.LengthValue = LengthValue.Text;

            Criteria.WidthOperator = GetSelectedOperator(WidthOperator);
            Criteria.WidthValue = WidthValue.Text;

            Criteria.HeightOperator = GetSelectedOperator(HeightOperator);
            Criteria.HeightValue = HeightValue.Text;

            DialogResult = true;
            Close();
        }

        // 取消按钮事件
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // 获取选择的操作符
        private string GetSelectedOperator(ComboBox comboBox)
        {
            return comboBox.SelectedItem is ComboBoxItem item ? item.Content.ToString() : "";
        }
    }
    // 导出条件实体类
    public class ExportFilterCriteria
    {
        public string LengthOperator { get; set; }
        public string LengthValue { get; set; }
        public Brush LengthColor { get; set; } = Brushes.White;

        public string WidthOperator { get; set; }
        public string WidthValue { get; set; }
        public Brush WidthColor { get; set; } = Brushes.White;

        public string HeightOperator { get; set; }
        public string HeightValue { get; set; }
        public Brush HeightColor { get; set; } = Brushes.White;
    }
}
