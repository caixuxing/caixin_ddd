using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace CaiXin.Domain
{

    [DependsOn(
        typeof(AbpEventBusModule)
    )]
    public class CaiXinDomainModule:AbpModule
    {
    }
}
