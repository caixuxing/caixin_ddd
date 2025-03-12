using CaiXin.Application.Contracts.UserApp.Dto;
using CaiXin.Application.Contracts.UserApp.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Application.Contracts.UserApp
{
    /// <summary>
    /// 授权认证App
    /// </summary>
    public interface IAuthApp
    {

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        Task<UserDto> Login(LoginQry qry);

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        Task<string> GetToken();
    }
}
