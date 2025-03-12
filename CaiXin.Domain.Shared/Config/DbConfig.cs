using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Domain.Shared.Config
{
    /// <summary>
    /// 数据库连接对象
    /// </summary>
    public class DbConfig
    {
        public string ConnectionString { get; set; }

        public bool IsAutoCloseConnection { get; set; }
    }
}
