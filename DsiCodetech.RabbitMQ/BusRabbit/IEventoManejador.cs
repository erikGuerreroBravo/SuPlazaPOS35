using DsiCodetech.RabbitMQ.Eventos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodetech.RabbitMQ.BusRabbit
{
    /// <summary>
    /// Interfaz la cual implementaremos para realizar las operaciones con la entidad que reciva.
    /// Venta
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IEventoManejador<in TEvent> : IEventoManejador where TEvent : Evento   
    {
        Task Handle(TEvent entidad);
    }

    public interface IEventoManejador
    {

    }

}
