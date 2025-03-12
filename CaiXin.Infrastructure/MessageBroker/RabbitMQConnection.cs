using NLog.Config;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Infrastructure.MessageBroker
{
    /// <summary>
    /// RabbitMq连接
    /// </summary>
    public class RabbitMQConnection : IRabbitMQConnection
    {
        /// <summary>
        /// 接连工厂
        /// </summary>
        private readonly IConnectionFactory _factory;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="factory"></param>
        public RabbitMQConnection(IConnectionFactory factory)=> _factory = factory;
     

        /// <summary>
        /// 创建连接对象
        /// </summary>
        /// <returns></returns>
        public  Task<IConnection> CreateConnectionAsync()=>  _factory.CreateConnectionAsync();
            
        
        
      
    }
}
