using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLog;
using Newtonsoft.Json;
using RabbitMQ.Client;

using DsiCodetech.RabbitMQ.BusRabbit;
using DsiCodetech.RabbitMQ.Comandos;
using DsiCodetech.RabbitMQ.Eventos;


namespace DsiCodetech.RabbitMQ.Implement
{
    public class RabbitEventBus : IRabbitEventBus
    {
        private readonly Dictionary<string, List<Type>> _manejadores;
        private readonly List<Type> _eventoTipos;
        private readonly ILogger _logger = null;
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitEventBus()
        {
            _manejadores = new Dictionary<string, List<Type>>();
            _eventoTipos = new List<Type>();
            _logger = LogManager.GetCurrentClassLogger();
            _connectionFactory = new ConnectionFactory()
            {
                HostName = Settings.Default.HostName,
                UserName = Settings.Default.UserName,
                Password = Settings.Default.Password,
                AutomaticRecoveryEnabled = true,
            };

            _connection = this._connectionFactory.CreateConnection();
            _channel = this._connection.CreateModel();
        }

        /// <summary>
        /// Metodo el cual se encargara el encolamiento de la venta con un Exchange: Directo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entidad"></param>
        public void Producer<T>(T entidad) where T : class
        {
            var eventName = entidad.GetType().Name;
            //se manda el evento en la cola
            _channel.QueueDeclare(eventName, true, false, false, null);
            //agregamos el mensaje dentro de la queue
            var message = JsonConvert.SerializeObject(entidad);
            var body = Encoding.UTF8.GetBytes(message);
            //metodo para publicar un evento dentro del bus de rabbit
            _channel.BasicPublish("", eventName, null, body);
        }

        /// <summary>
        /// Metodo el cual se encargara de realizar el consumo de la cola directa de la Venta en el punto de venta
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <exception cref="ArgumentException"></exception>
        public void Consumer<T, TH>()
            where T : Evento
            where TH : IEventoManejador<T>
        {
            var eventoNombre = typeof(T).Name;
            var manejadorEventoTipo = typeof(TH);

            _logger.Debug("Nombre del evento: {0}", eventoNombre);
            _logger.Debug("Nombre del manejador: {0}", manejadorEventoTipo.Name);

            //mandamos llamar la lista de eventos
            //preguntamos primero si no contiene un elemento o mensaje lo agrega a la lista
            if (!_eventoTipos.Contains(typeof(T)))
            {
                _eventoTipos.Add(typeof(T));
            }
            if (!_manejadores.ContainsKey(eventoNombre))
            {
                _manejadores.Add(eventoNombre, new List<Type>());
            }
            //validamos que no se haya agregado otro manejador en otro evento
            if (_manejadores[eventoNombre].Any(x => x.GetType() == manejadorEventoTipo))
            {
                throw new ArgumentException($"El manejador {manejadorEventoTipo.Name} fue registrado anteriormente por {eventoNombre}");
            }
            _manejadores[eventoNombre].Add(manejadorEventoTipo);

            _channel.QueueDeclare(eventoNombre, true, false, false, null);

            _logger.Debug("Estableciendo conexión con la bandeja MQ {0}", _channel.IsOpen);

            _channel.BasicQos(0, 1, false);
            ///pasamos el nombre del queue a consumir
            _channel.BasicConsume(eventoNombre, false, new MessageReceiver(_eventoTipos, _manejadores, _channel, eventoNombre));
        }

        /// <summary>
        /// Metodo el cual implementara un exchange en este caso un Topic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entidad"></param>
        public void Topic<T>(T entidad) where T : class
        {
            var eventName = entidad.GetType().Name;

            //Queue Principal 

            // Exchange del Topic principal con nombre "topic_exchange"
            _channel.ExchangeDeclare(exchange: "topic_exchange", ExchangeType.Topic, true, false,
                arguments: new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "miDleExchange" }
                });

            // Exchange para cuando ocurrra un mensaje en la cola principal DLE
            _channel.ExchangeDeclare(exchange: "miDleExchange", type: ExchangeType.Fanout, durable: true, false);

            // Declaración de la cola principal y vinculación con la routing key
            _channel.QueueDeclare(eventName, durable: true, exclusive: false, autoDelete: false,
                arguments: null);

            // Declaración de la cola DLE y vinculación con la routing key DLE
            _channel.QueueDeclare(eventName, durable: true, exclusive: false, autoDelete: false,
                arguments: null);

            // Vinculamos la Queue principal y como parametros estableceremos el DLE routing key.
            _channel.QueueBind(queue: eventName, exchange: "topic_exchange", routingKey: eventName,
                arguments: new Dictionary<string, object>
                {
                    { "x-dead-letter-routing-key", eventName }
                });

            //Vinculamos la queue con el exchange el cual implementa DLE
            _channel.QueueBind(queue: "dle_topic_queue", exchange: "miDleExchange", routingKey: "");

            //Se crea el Mensaje y lo envía al Exchange Topic con la routing key "eventName" (La cual sera el tipo de entidad)
            var message = JsonConvert.SerializeObject(entidad);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Expiration = "7200000";

            properties.Persistent = true;

            _channel.BasicPublish(exchange: "topic_exchange", routingKey: eventName, basicProperties: properties, body);
        }

        /// <summary>
        /// Metodo el cual sera un Consumer, el cual se suscribira a una Queue atravez de routing key.
        /// El Tipo de Exchange es Topic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        public void TopicConsumer<T, TH>()
            where T : Evento
            where TH : IEventoManejador<T>
        {
            var eventoNombre = typeof(T).Name;
            var manejadorEventoTipo = typeof(TH);

            if (!_eventoTipos.Contains(typeof(T)))
            {
                _eventoTipos.Add(typeof(T));
            }
            if (!_manejadores.ContainsKey(eventoNombre))
            {
                _manejadores.Add(eventoNombre, new List<Type>());
            }

            if (_manejadores[eventoNombre].Any(x => x.GetType() == manejadorEventoTipo))
            {
                throw new ArgumentException($"El manejador {manejadorEventoTipo.Name} fue registrado anteriormente por {eventoNombre}");
            }
            _manejadores[eventoNombre].Add(manejadorEventoTipo);

            // Exchange Topic con nombre "topic_exchange"
            _channel.ExchangeDeclare("topic_exchange", ExchangeType.Topic, durable: true, autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "miDleExchange" }
                });

            // Crea una cola y la une al Exchange Topic con la routing key "eventoNombre" (La cual sera el tipo de entidad)
            _channel.QueueBind(queue: eventoNombre, exchange: "topic_exchange", routingKey: eventoNombre,
                arguments: null);

            //Exchange Fanout el cual sera DLE con nombre "miDleExchange"
            _channel.ExchangeDeclare("miDleExchange", ExchangeType.Fanout, durable: true, autoDelete: false,
                arguments: null);

            // Crea una cola y la une al Exchange Fanout de la implementacion de DLE
            _channel.QueueDeclare("dle_topic_queue", durable: true, exclusive: false, autoDelete: false,
                arguments: null);

            _channel.QueueBind(queue: "dle_topic_queue", exchange: "miDleExchange", routingKey: "");

            _channel.BasicQos(0, 1, false);

            ///pasamos el nombre del queue a consumir
            _channel.BasicConsume(eventoNombre, false, new MessageReceiver(_eventoTipos, _manejadores, _channel, eventoNombre));
        }

        public Task EnviarComando<T>(T comando) where T : Comando
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cierra las conexiones con Rabbit MQ y libera memoria.
        /// </summary>
        public void Close()
        {

        }
    }
}
