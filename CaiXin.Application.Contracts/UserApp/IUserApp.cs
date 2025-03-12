using CaiXin.Application.Contracts.UserApp.Commands;
using CaiXin.Application.Contracts.UserApp.Dto;
using CaiXin.Application.Contracts.UserApp.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Application.Contracts.UserApp
{
    /// <summary>
    /// 用户App
    /// </summary>
    public interface IUserApp
    {

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task<long> CreateUserAsync([NotNull]CreateUserCmd cmd);

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<bool> CheckUserNameExistsAsync(string userName);


        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task<bool> UpdateUserAsync(UpdateUserCmd cmd);


        /// <summary>
        /// 按ID查询用户信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<UserDto> FindById(long Id);

        /// <summary>
        /// 按分页条件筛选
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        Task<IEnumerable<UserDto>> QueryPageList(UserPageQry qry);

    }
}
