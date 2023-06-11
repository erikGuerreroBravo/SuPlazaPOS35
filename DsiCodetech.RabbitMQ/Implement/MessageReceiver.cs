using DsiCodetech.RabbitMQ.BusRabbit;
using Newtonsoft.Json;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
//using System.Threading.Channels;
using System.Threading.Tasks;

namespace DsiCodetech.RabbitMQ.Implement
{
    public class MessageReceiver : DefaultBasicConsumer
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, List<Type>> _manejadores;
        private readonly List<Type> _eventoTipos;
        private readonly IModel _channel;
        private readonly string _queueName;

        public MessageReceiver(List<Type> eventoTipos, Dictionary<string, List<Type>> manejadores, IModel channel, string queueName)
        {
            _eventoTipos = eventoTipos;
            _manejadores = manejadores;
            _channel = channel;
            _queueName = queueName;
        }

        public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            try
            {
                string message = Encoding.UTF8.GetString(body.ToArray());
                if (!string.IsNullOrEmpty(message) && _manejadores.ContainsKey(_queueName))
                {
                    var subscriptions = _manejadores[_queueName];
                    foreach (var sb in subscriptions)
                    {
                        //la clase que contenga el metodo handle podra inyectar nuevos objetos
                        var manejador = Activator.CreateInstance(sb);
                        if (manejador == null) continue;
                        var tipoEvento = _eventoTipos.SingleOrDefault(x => x.Name == _queueName);
                        //deserealizamos el mensaje de acuerdo al tipoEvento y mensaje
                        var eventoDS = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(body.ToArray()), tipoEvento);
                        var concretoTipo = typeof(IEventoManejador<>).MakeGenericType(tipoEvento);
                        await (Task)concretoTipo.GetMethod("Handle").Invoke(manejador, new object[] { eventoDS });
                    }
                    _channel.BasicAck(deliveryTag, false);
                }
            }
            catch(Exception ex)
            {
                Log.Error("Error al intentar procesar el mensaje {0}",body);
                Log.Error("Error: ",ex);
                _channel.BasicReject(deliveryTag, false);
                GetMessageDLE(routingKey);
            }
        }

        private void GetMessageDLE(string routingKey)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                AutomaticRecoveryEnabled = true,
            };

            IConnection connection = factory.CreateConnection();
            IModel channelDLE = connection.CreateModel();

            channelDLE.ExchangeDeclare(exchange: "miDleExchange", type: ExchangeType.Fanout, durable: true, false);
            channelDLE.QueueDeclare("dle_topic_queue", durable: true, exclusive: false, autoDelete: false,
                arguments: null);

            var consumerDLE = new EventingBasicConsumer(channelDLE);
            consumerDLE.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var properties = channelDLE.CreateBasicProperties();
                properties.Expiration = "7200000";

                channelDLE.BasicPublish(exchange: "topic_exchange", routingKey: routingKey, basicProperties: properties, body);
            };
            
            ///Quitamos el mensaje de la queue DLE
            channelDLE.BasicConsume("dle_topic_queue", autoAck: true, consumer: consumerDLE);
        }
    }
}
