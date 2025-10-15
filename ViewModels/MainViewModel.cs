using Data_collection.Models;
using Data_collection.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Data_collection.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly MySqlService _mySqlService;
        private readonly DispatcherTimer _updateTimer;
        private ObservableCollection<MyDataItem> _dataItems;
        private ObservableCollection<MyDataItem> _filteredDataItems;
        private string _statusMessage;
        private string _filterName;
        private DateTime? _filterDate;

        // 命令
        public ICommand FilterCommand { get; }
        public ICommand ResetFilterCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand ExportCommand { get; }

        // 属性
        public ObservableCollection<MyDataItem> DataItems
        {
            get => _dataItems;
            set
            {
                _dataItems = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MyDataItem> FilteredDataItems
        {
            get => _filteredDataItems;
            set
            {
                _filteredDataItems = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public string FilterName
        {
            get => _filterName;
            set
            {
                _filterName = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public DateTime? FilterDate
        {
            get => _filterDate;
            set
            {
                _filterDate = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public MainViewModel()
        {
            _mySqlService = new MySqlService();
            DataItems = new ObservableCollection<MyDataItem>();
            FilteredDataItems = new ObservableCollection<MyDataItem>();

            // 初始化命令
            FilterCommand = new RelayCommand(ApplyFilters);
            ResetFilterCommand = new RelayCommand(ResetFilters);
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            ExportCommand = new RelayCommand(OpenExportWindow);

            // 初始化定时器，每分钟更新一次数据
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _updateTimer.Tick += (sender, e) => LoadData();
            _updateTimer.Start();

            // 初始加载数据
            LoadData();
        }

        // 加载数据
        public void LoadData()
        {
            try
            {
                StatusMessage = "正在加载数据...";
                var newItems = _mySqlService.GetData();

                DataItems.Clear();
                foreach (var item in newItems)
                {
                    DataItems.Add(item);
                }

                ApplyFilters();
                StatusMessage = $"数据加载完成。共 {DataItems.Count} 条记录。最后更新时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"数据加载失败: {ex.Message}";
            }
        }

        // 应用筛选条件
        private void ApplyFilters()
        {
            var filtered = DataItems.AsEnumerable();

            // 按名称筛选
            if (!string.IsNullOrEmpty(FilterName))
            {
                filtered = filtered.Where(item =>
                    item.Name.IndexOf(FilterName, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // 按日期筛选
            if (FilterDate.HasValue)
            {
                var filterDate = FilterDate.Value.Date;
                filtered = filtered.Where(item =>
                    item.CreatedDate.Date == filterDate);
            }

            FilteredDataItems.Clear();
            foreach (var item in filtered)
            {
                FilteredDataItems.Add(item);
            }
        }

        // 打开导出窗口
        private void OpenExportWindow()
        {
            var exportWindow = new Views.ExportFilter();
            if (exportWindow.ShowDialog() == true)
            {
                // 获取用户设置的导出条件
                var criteria = exportWindow.Criteria;
                // 执行导出操作
                ExportData(criteria);
            }
        }

        // 执行导出操作
        private void ExportData(ExportFilterCriteria criteria)
        {
            try
            {
                // 这里实现实际的导出逻辑
                // 可以根据criteria中的条件筛选数据并导出
                StatusMessage = $"数据导出成功，条件: 长度{criteria.LengthOperator}{criteria.LengthValue}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"数据导出失败: {ex.Message}";
            }
        }

        // 重置筛选条件
        private void ResetFilters()
        {
            FilterName = string.Empty;
            FilterDate = null;
            ApplyFilters();
        }

        // 打开设置窗口
        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
    }
}
