using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Application.Contracts.UserApp.Query
{
    public record UserPageQry
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; }
    }
}
