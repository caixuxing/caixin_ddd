using FluentValidation;
using FluentValidation.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;

namespace CaiXin.Application.Contracts
{
  
    /// <summary>
    /// 
    /// </summary>
    public class CaiXinApplicationContractsModule : AbpModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {

            context.Services.AddFluentValidationClientsideAdapters();
            context.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return base.ConfigureServicesAsync(context);
        }
    }
}
