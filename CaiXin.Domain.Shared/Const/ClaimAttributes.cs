﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Domain.Shared.Const
{
    /// <summary>
    /// Claim属性
    /// </summary>
    public static class ClaimAttributes
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public const string UserId = "id";
        /// <summary>
        /// 用户名
        /// </summary>
        public const string UserName = "na";

        /// <summary>
        /// 姓名
        /// </summary>
        public const string UserNickName = "nn";

        /// <summary>
        /// 刷新有效期
        /// </summary>
        public const string RefreshExpires = "re";
        /// <summary>
        /// 刷新token
        /// </summary>
        public const string RefreshToken = "token";

        /// <summary>
        /// 组织
        /// </summary>
        public const string OrgCode = "org";
    }
}
