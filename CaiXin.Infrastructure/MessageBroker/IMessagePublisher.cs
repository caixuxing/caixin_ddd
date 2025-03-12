
using NLog.Config;
using RabbitMQ.Client;
using System.Text;


namespace CaiXin.Infrastructure.MessageBroker
{

    public interface IMessagePublisher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <returns></returns>
        Task<bool> PublishAsync<T>(T message, string exchange, string routingKey);
    }
}
