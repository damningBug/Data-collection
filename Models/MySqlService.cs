using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Models
{
    // 数据库服务类，负责与MySQL交互
    public class MySqlService
    {
        // 从配置文件获取连接字符串
        private string _connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        // 获取数据列表
        public List<MyDataItem> GetData()
        {
            List<MyDataItem> dataItems = new List<MyDataItem>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // 这里的SQL查询需要根据实际表结构修改
                    string query = "SELECT Id, Name, Description, CreatedDate FROM data_table";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MyDataItem item = new MyDataItem
                                {
                                    Id = reader.GetInt32("Id"),
                                    Name = reader.GetString("Name"),
                                    Value = reader.GetInt32("Value"),
                                    Description = reader.GetString("Description"),
                                    CreatedDate = reader.GetDateTime("CreatedDate")
                                };
                                dataItems.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 实际应用中应使用日志记录
                Console.WriteLine($"数据库错误: {ex.Message}");
            }

            return dataItems;
        }
    }
}
