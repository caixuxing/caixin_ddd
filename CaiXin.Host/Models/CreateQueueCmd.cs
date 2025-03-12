using FluentValidation;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CaiXin.Host.Models
{
    /// <summary>
    /// 创建队列命令
    /// </summary>

    public record CreateQueueCmd
    {
        /// <summary>
        /// 数据
        /// </summary>
        [Required]
        public string JosnData { get; set; } = default!;
        /// <summary>
        /// 回调地址
        /// </summary>
        [Required]
        public string CallbackUrl { get; set; } = default!;

    }

    /// <summary>
    /// 创建队列命令验证器
    /// </summary>
    public class CreateQueueCmdValidator : AbstractValidator<CreateQueueCmd>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CreateQueueCmdValidator()
        {
            RuleFor(x => x.JosnData)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Json数据不能为空!")
                .Must(IsValidJson).WithMessage("Json数据格式不正确!");
            RuleFor(x => x.CallbackUrl)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("回调地址不能为空!")
                .Must(IsValidUrl).WithMessage("回调地址格式不正确!");
        }


        /// <summary>
        /// 验证回调地址是否有效
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static bool IsValidUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? result))
                return (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
            return false;
        }
        /// <summary>
        /// 验证Json数据是否有效
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        static bool IsValidJson(string jsonString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonString)) return false;
                JToken.Parse(jsonString);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }
    }
}
