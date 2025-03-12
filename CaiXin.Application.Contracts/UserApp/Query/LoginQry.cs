using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Application.Contracts.UserApp.Query
{
   
    public record LoginQry
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = default!;

        /// <summary>
        /// 用户密码
        /// </summary>
        public string PassWord { get; set; } = default!;
    }
}
