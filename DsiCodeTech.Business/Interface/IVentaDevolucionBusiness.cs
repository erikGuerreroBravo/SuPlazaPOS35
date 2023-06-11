using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Business.Interface
{
    public interface IVentaDevolucionBusiness
    {
        List<venta_devolucion> GetDevolucionesByDates(DateTime start, DateTime end);

        /// <summary>
        /// Este metodo se encarga de actualizar el campo upload a verdadero cuando existe el envio correo de la
        /// devolucion a rabbitMQ
        /// </summary>
        /// <param name="ventaDevolucion">la entidad venta_devolucion</param>
        /// <exception cref="BusinessException">excepcion generada por no tener acceso al contexto o actualizacion del campo</exception>
        void UpdateUploadField(Guid id_devolucion);
    }
}