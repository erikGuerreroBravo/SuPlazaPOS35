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
    }
}