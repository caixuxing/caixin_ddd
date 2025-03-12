using CaiXin.Application.Contracts;
using CaiXin.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;

namespace CaiXin.Application
{
  

    [DependsOn(typeof(CaiXinInfrastructureModule),
        typeof(CaiXinApplicationContractsModule)
    )]
    public class CaiXinApplicationModule : AbpModule
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            return base.ConfigureServicesAsync(context);
        }
    }
}
