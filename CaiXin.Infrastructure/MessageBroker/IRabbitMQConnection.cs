using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaiXin.Infrastructure.MessageBroker
{
    /// <summary>
    /// rabbitMq连接对象
    /// </summary>
    public interface IRabbitMQConnection
    {
        /// <summary>
        /// 创建连接对象
        /// </summary>
        /// <returns></returns>
        Task<IConnection> CreateConnectionAsync();
    }

  

}
