using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CaiXin.Domain.Comm
{
    public abstract class BaseModel<TRequest>
    {
       

        //public async Task Validate(TRequest request)
        //{
        //    using var scope = _scopeFactory.CreateScope();
        //    IValidator<TRequest> validator = scope.ServiceProvider.GetRequiredService<IValidator<TRequest>>();

        //    var validateResult = await validator.ValidateAsync(request);
        //    if (!validateResult.IsValid)
        //    {
        //        var eorrList = validateResult.Errors.Select(_ => new { key = _.PropertyName, msg = _.ErrorMessage }).ToList();
        //        throw new Exception("验证失败");
        //    }
        //}


        public TRequest SetValue<KT>(Action<KT> setter, KT value)
        {
            setter(value);
            return default!;
        }
    }

}
