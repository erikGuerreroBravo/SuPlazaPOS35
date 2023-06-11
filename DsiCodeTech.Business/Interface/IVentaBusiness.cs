using DsiCodeTech.Common.DataAccess.Domain;
using DsiCodeTech.Repository.PosCaja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Business.Interface
{
    public interface IVentaBusiness
    {
        VentaDM GetVentaByFolio(long Folio);
        VentaDM GetVentaByIdVenta(Guid IdVenta);
        List<venta> GetVentasByDates(DateTime start, DateTime end);
        
        /// <summary>
        /// Este metodo se encarga de actualizar el campo upload de la entidad venta 
        /// despues de enviarla a rabbitMQ
        /// </summary>
        /// <param name="IdVenta">el identificador de la venta</param>
        /// <exception cref="BusinessException">excepcion en caso de no estar disponible el contexto</exception>
        void UpdateUploadField(Guid IdVenta);
    }
}
