using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DsiCodetech.RabbitMQ.Eventos;
using DsiCodeTech.Common.DataAccess.Domain;


namespace DsiCodetech.RabbitMQ.EventQueue
{
    public class VentaCanceladaQueue : Evento
    {
        public VentaCanceladaDM Body { get; set; }
    }
}
