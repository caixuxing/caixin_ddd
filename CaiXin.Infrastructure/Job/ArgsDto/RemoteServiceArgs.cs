using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Infrastructure.Job.ArgsDto
{
    public class RemoteServiceArgs
    {
        /// <summary>
        /// 数据
        /// </summary>
        public string JosnData { get; set; } = default!;

        /// <summary>
        /// 回调地址
        /// </summary>
        public string CallbackUrl { get; set; } = default!;
    }
}
