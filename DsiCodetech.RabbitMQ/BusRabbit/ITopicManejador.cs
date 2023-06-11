using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodetech.RabbitMQ.BusRabbit
{
    /// <summary>
    /// Interfaz la cual implementara el evento para manejar los eventos que se realizen con el Topic
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface ITopicManejador<in TEvent> : ITopicManejador where TEvent : class
    {
        Task Handle(TEvent entidad);
    }

    public interface ITopicManejador
    {

    }
}
