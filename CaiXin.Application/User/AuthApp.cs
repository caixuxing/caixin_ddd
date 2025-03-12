using CaiXin.Application.Contracts.UserApp;
using CaiXin.Application.Contracts.UserApp.Dto;
using CaiXin.Application.Contracts.UserApp.Query;
using CaiXin.Domain.SysUser.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace CaiXin.Application.User
{
    public class AuthApp : ApplicationService, IAuthApp
    {
        /// <summary>
        /// 用户仓储
        /// </summary>
        private ISimpleClient<UserDo> userRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<UserDo>>();
        public Task<string> GetToken()
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> Login(LoginQry qry)
        {
            //根据用户名查询用户信息
            var entity = await userRepo.AsQueryable().SingleAsync(x => x.Name == qry.UserName)
                ?? throw new InvalidOperationException("账号或密码错误");
            
            //判断密码是否相等
           _= !string.Equals(entity.Password, qry.PassWord, StringComparison.Ordinal) ? true : throw new InvalidOperationException("账号或密码错误");

            //UserDo To UserDto 并返回
            return entity.MapUserDto();
        }
    }

    internal static partial class MapExt
    {

    }
        
}
