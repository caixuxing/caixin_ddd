using CaiXin.Domain.Shared.Const;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CaiXin.Infrastructure.MessageBroker
{
    public class RabbitMQConsumerService : IHostedService
    {
        private readonly IRabbitMQConnection _connection;

        public RabbitMQConsumerService(IRabbitMQConnection connection)
        {
            _connection = connection;
        }

        public async Task StartAsync(CancellationToken token)
        {
            var connection = await _connection.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            // 声明主题交换机
            await channel.ExchangeDeclareAsync(RabbitMqConst.CaiXin_Exchange_User_Event, ExchangeType.Topic, durable: true);

            // 声明队列
            await channel.QueueDeclareAsync(RabbitMqConst.CaiXin_Queue_User_Event_Test,
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            // 绑定队列到交换机，使用特定的路由键
            await channel.QueueBindAsync(RabbitMqConst.CaiXin_Queue_User_Event_Test, RabbitMqConst.CaiXin_Exchange_User_Event, "caixin.user.event.*");



            // 设置QoS（防止消息堆积）
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false); // 每次只推送1条[6]()

            // 创建异步消费者 
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, data) =>
            {
                try
                {
                    var body = data.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);


                    // 处理消息（模拟业务逻辑）
                    Console.WriteLine($"队列{RabbitMqConst.CaiXin_Queue_User_Event_Test}消费:{message}");

                    // 手动确认消息 
                    await channel.BasicAckAsync(data.DeliveryTag, multiple: false);
                }
                catch 
                {
                    // 处理失败：拒绝消息并重新入队 
                   await channel.BasicNackAsync(data.DeliveryTag, multiple: false, requeue: true); 
                }
            };
            // 启动消费者，开始接收消息
           await channel.BasicConsumeAsync(queue: RabbitMqConst.CaiXin_Queue_User_Event_Test, autoAck: false,consumer: consumer);
        }
  

        public Task StopAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
