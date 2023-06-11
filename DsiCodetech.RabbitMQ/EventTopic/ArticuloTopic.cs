using DsiCodetech.RabbitMQ.Eventos;
using DsiCodeTech.Common.DataAccess.Domain;

namespace DsiCodetech.RabbitMQ.EventTopic
{
    public class ArticuloTopic : Evento
    {
        public ArticuloDM Body { get; set; }
    }
}
