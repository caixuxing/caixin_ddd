using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Domain.Shared.Const
{
    /// <summary>
    /// RabbitMq常量信息
    /// 
    /// 常量命名格式【服务.模块.事件/通知.模式】
    /// </summary>
    public static class RabbitMqConst
    {

        /// <summary>
        /// 交换机.用户事件
        /// </summary>
        public const string CaiXin_Exchange_User_Event = "caixin.user.event";

        /// <summary>
        /// 路由.用户事件.测试
        /// </summary>
        public const string CaiXin_Routing_User_Test = "caixin.user.event.test";

        /// <summary>
        /// 队列.用户事件.测试
        /// </summary>
        public const string CaiXin_Queue_User_Event_Test = "caixin.user.event.test";
        /// <summary>
        /// 队列.用户事件.消费
        /// </summary>
        public const string CaiXin_Queue_User_Event_Consume = "caixin.user.event.consume";
    }
}
