using DsiCodetech.RabbitMQ.Eventos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodetech.RabbitMQ.Comandos
{
    public class Comando : Message
    {
        public DateTime TimeStamp { get; protected set; }

        protected Comando()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
