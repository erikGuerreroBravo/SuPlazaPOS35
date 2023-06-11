using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodetech.RabbitMQ.Enum
{
    public enum OperacionType
    {
        PV_NUEVO_ARTICULO,
        PV_ACTUALIZAR_ARTICULO,
        PV_ACTUALIZAR_ARTICULO_MASIVO,
        ADMIN_VENTA,
        ADMIN_VENTA_DEVOLUCION,
        ADMIN_VENTA_CANCELADA
    }
}
