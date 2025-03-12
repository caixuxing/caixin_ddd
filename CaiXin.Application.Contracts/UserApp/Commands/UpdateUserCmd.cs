using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Application.Contracts.UserApp.Commands
{
    public record UpdateUserCmd
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; } = default!;

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string PassWord { get; set; } = default!;

        /// <summary>
        /// 用户主键
        /// </summary>
        [Required]
        public string Id { get; set; } = default!;
    }

    /// <summary>
    /// 
    /// </summary>
    public class UpdateUserCmdValidator : AbstractValidator<UpdateUserCmd>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UpdateUserCmdValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("用户名不能为空!")
                .MaximumLength(50).WithMessage("用户名长度不能超过50个字符串!");
            RuleFor(x => x.PassWord)
                .NotEmpty().WithMessage("密码不能为空!")
                .MaximumLength(50).WithMessage("密码长度不能超过50个字符串!");
        }
    }
}
