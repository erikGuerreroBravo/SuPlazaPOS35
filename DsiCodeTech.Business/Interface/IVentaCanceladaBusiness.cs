using DsiCodeTech.Common.DataAccess.Domain;
using DsiCodeTech.Repository.PosCaja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Business.Interface
{
    public interface IVentaCanceladaBusiness
    {
        /// <summary>
        /// Inserta una cancelación junto con sus artículos
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        venta_cancelada Insert (venta_cancelada entity);

        /// <summary>
        /// Actualiza una cancelación junto con sus artículos 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        venta_cancelada Update (venta_cancelada entity);


        VentaCanceladaDM GetCancelSaleByIdCancelSale(Guid idVentaCancelada);
        /// <summary>
        /// Este metodo se encarga de actualizar el campo upload de la entidad venta_cancelada 
        /// despues de enviarla a rabbitMQ
        /// </summary>
        /// <param name="IdVenta">el identificador de la venta cancelada</param>
        /// <exception cref="BusinessException">excepcion en caso de no estar disponible el contexto</exception>
        void UpdateUploadField(Guid IdVentaCancelada);
    }
}
