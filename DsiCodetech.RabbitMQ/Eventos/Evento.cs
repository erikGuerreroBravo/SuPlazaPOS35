using System;

namespace DsiCodetech.RabbitMQ.Eventos
{
    public class Evento
    {
        public DateTime TimeStamp { get; protected set; }

        public string CorrelationId { get; set; }

        protected Evento()
        {
            TimeStamp = DateTime.Now;
        }

    }
}
