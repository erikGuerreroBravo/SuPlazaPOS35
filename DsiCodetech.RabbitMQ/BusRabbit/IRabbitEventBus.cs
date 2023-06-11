using DsiCodetech.RabbitMQ.Comandos;
using DsiCodetech.RabbitMQ.Eventos;
using System.Threading.Tasks;

namespace DsiCodetech.RabbitMQ.BusRabbit
{
    public interface IRabbitEventBus
    {
        Task EnviarComando<T>(T comando) where T : Comando;

        /// <summary>
        /// Metodos que utilizara para las ventas con un Exchange: Directo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entidad"></param>
        void Producer<T>(T entidad) where T : class;

        void Consumer<T, TH>() where T : Evento
                            where TH : IEventoManejador<T>;

        /// <summary>
        /// Metodos que utilizan un Exchange: Topic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entidad"></param>
        void Topic<T>(T entidad) where T : class;

        void TopicConsumer<T, TH>() where T : Evento
                            where TH : IEventoManejador<T>;
        /// <summary>
        /// Cierra las conexiones con Rabbit MQ y libera memoria.
        /// </summary>
        void Close();

    }
}