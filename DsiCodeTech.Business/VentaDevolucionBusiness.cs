using DsiCodeTech.Business.Interface;
using DsiCodeTech.Common.Constant;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Common.Exception;
using DsiCodeTech.Repository;
using DsiCodeTech.Repository.Infraestructure;
using DsiCodeTech.Repository.PosCaja;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Business
{
    public class VentaDevolucionBusiness : IVentaDevolucionBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly VentaDevolucionRepository _ventaDevolucionRepository;


        public VentaDevolucionBusiness()
        {
            _unitOfWork = new UnitOfWork();
            _ventaDevolucionRepository = new VentaDevolucionRepository(_unitOfWork);
        }

        public VentaDevolucionBusiness(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ventaDevolucionRepository = new VentaDevolucionRepository(unitOfWork);
        }

        public List<venta_devolucion> GetDevolucionesByDates(DateTime start, DateTime end)
        {
            try
            {
                List<venta_devolucion> ventas = this._ventaDevolucionRepository.GetIncludeAll(v => v.fecha_dev >= start && v.fecha_dev <= end, "venta_devolucion_articulo").ToList();
                return ventas;
            }
            catch (Exception ex) when (ex is DataException || ex is SqlException)
            {

                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);
            }
        }


        /// <summary>
        /// Este metodo se encarga de actualizar el campo upload a verdadero cuando existe el envio correo de la
        /// devolucion a rabbitMQ
        /// </summary>
        /// <param name="ventaDevolucion">la entidad venta_devolucion</param>
        /// <exception cref="BusinessException">excepcion generada por no tener acceso al contexto o actualizacion del campo</exception>
        public void UpdateUploadField(Guid id_devolucion)
        {
            
            try
            {
                venta_devolucion ventaDv = this._ventaDevolucionRepository.SingleOrDefault(v => v.id_devolucion == id_devolucion);
                ventaDv.upload = true;
                this._ventaDevolucionRepository.Update(ventaDv);
            }
            catch (Exception ex)
            {
                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);

            }
        }
    }
}