using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SuPlazaPOS35.domain;

namespace SuPlazaPOS35.Remote
{
    /// <summary>
    /// Flujo que se deberá analizar para desacoplar la sincronización entre la BD Local (PVs) y BD Remota (Administrador).
    /// Investigar cúal es la mejor opción de mensajería (Rabbit MQ, Kafka, Active MQ) con C#
    /// Crear un Queue para enviar las ventas a travez de un mensaje.
    /// Crear el producer para enviar el mensaje en formato JSON
    /// Crear el consumer para obtener el JSON, trasformalo en un objeto y almacenarlo en la base de datos.
    /// </summary>
    public interface IVentaRemote
    {
        /// <summary>
        /// Método temporal para enviar la venta a la base de datos pos_admin.
        /// </summary>
        /// <param name="ticket"></param>
        public void SyncSaleRemote(venta venta);
    }
}
