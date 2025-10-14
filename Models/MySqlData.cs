using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_collection.Models
{
    // 数据模型类，对应数据库中的表结构
    public class MyDataItem
    {
        public int Id { get; set; }
        public string Name { get; set; }  //名字
        public int Value { get; set; }  //数据
        public string Description { get; set; }  //描述
        public DateTime CreatedDate { get; set; }  //创建时间
        // 根据实际数据库表结构添加更多属性
    }
}
