using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Data_collection.ViewModels
{
    public class Settings : BaseViewModel
    {
        private string _server;
        private string _database;
        private string _port;
        private string _username;
        private string _password;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand TestConnectionCommand { get; }

        // 数据库连接属性
        public string Server
        {
            get => _server;
            set
            {
                _server = value;
                OnPropertyChanged();
            }
        }

        public string Database
        {
            get => _database;
            set
            {
                _database = value;
                OnPropertyChanged();
            }
        }

        public string Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public Settings()
        {
            // 加载当前配置
            LoadCurrentSettings();

            // 初始化命令
            SaveCommand = new RelayCommand(SaveSettings);
            CancelCommand = new RelayCommand(Cancel);
            TestConnectionCommand = new RelayCommand(TestConnection);
        }

        // 加载当前的数据库设置
        private void LoadCurrentSettings()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"]?.ConnectionString;
            if (!string.IsNullOrEmpty(connectionString))
            {
                var parts = connectionString.Split(';');
                foreach (var part in parts)
                {
                    var keyValue = part.Split(new[] { '=' }, 2);
                    if (keyValue.Length == 2)
                    {
                        switch (keyValue[0].Trim().ToLower())
                        {
                            case "server":
                                Server = keyValue[1].Trim();
                                break;
                            case "database":
                                Database = keyValue[1].Trim();
                                break;
                            case "port":
                                Port = keyValue[1].Trim();
                                break;
                            case "uid":
                                Username = keyValue[1].Trim();
                                break;
                            case "pwd":
                                Password = keyValue[1].Trim();
                                break;
                        }
                    }
                }
            }

            // 设置默认端口
            if (string.IsNullOrEmpty(Port))
                Port = "3306";
        }

        // 保存数据库设置
        private void SaveSettings()
        {
            try
            {
                // 验证必要的字段
                if (string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Database) ||
                    string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Port))
                {
                    MessageBox.Show("服务器地址、数据库名、用户名和端口号不能为空！", "输入错误",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 构建新的连接字符串
                var connectionString = $"server={Server};database={Database};uid={Username};pwd={Password};port={Port};";

                // 保存到配置文件
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");

                if (connectionStringsSection.ConnectionStrings["MySqlConnection"] != null)
                {
                    connectionStringsSection.ConnectionStrings["MySqlConnection"].ConnectionString = connectionString;
                }
                else
                {
                    connectionStringsSection.ConnectionStrings.Add(
                        new ConnectionStringSettings("MySqlConnection", connectionString, "MySql.Data.MySqlClient"));
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");

                // 关闭窗口
                CloseWindow();

                // 提示用户并刷新主窗口数据
                MessageBox.Show("数据库设置已保存，将重新加载数据。", "保存成功",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                // 通知主窗口刷新数据
                if (Application.Current.MainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存设置失败: {ex.Message}", "错误",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 测试数据库连接
        private void TestConnection()
        {
            try
            {
                // 验证必要的字段
                if (string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Database) ||
                    string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Port))
                {
                    MessageBox.Show("服务器地址、数据库名、用户名和端口号不能为空！", "输入错误",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 构建测试连接字符串
                var connectionString = $"server={Server};database={Database};uid={Username};pwd={Password};port={Port};";

                // 尝试连接数据库
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        MessageBox.Show("数据库连接成功！", "测试成功",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败: {ex.Message}", "测试失败",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 取消设置
        private void Cancel()
        {
            CloseWindow();
        }

        // 关闭当前窗口
        private void CloseWindow()
        {
            var window = Application.Current.Windows.OfType<Window>()
                          .FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }
    }
}
