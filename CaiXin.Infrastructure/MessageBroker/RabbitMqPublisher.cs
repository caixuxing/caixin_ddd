using CaiXin.Domain.Job;
using CaiXin.Infrastructure.Job.ArgsDto;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using Volo.Abp.DependencyInjection;

namespace CaiXin.Infrastructure.MessageBroker
{


   
    public class RabbitMqPublisher : IMessagePublisher
    {
     


        private readonly IRabbitMQConnection _connection;

        public RabbitMqPublisher(IRabbitMQConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> PublishAsync<T>(T message, string exchange, string routingKey)
        {

            using var connection = await _connection.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            //声明主题交换机
            await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, durable: true);
      
            //消息内容
            var body = JsonSerializer.SerializeToUtf8Bytes(message);
            var properties = new RabbitMQ.Client.BasicProperties
            {
                Persistent = true // 设置消息持久化
            };
            //发布消息到交换机
             await channel.BasicPublishAsync(exchange, routingKey, false,properties,body: body);

            return true;
        }
    }
}
