using DsiCodeTech.Common.DataAccess.Domain;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository;
using DsiCodeTech.Repository.Infraestructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsiCodeTech.Common.Exception;
using DsiCodeTech.Common.Constant;
using DsiCodeTech.Repository.PosCaja;
using DsiCodeTech.Business.Interface;

namespace DsiCodeTech.Business
{
    public class VentaDevolucionArticuloBusiness: IVentaDevolucionArticuloBusiness
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly VentaDevolucionArticuloRepository repository;
        public VentaDevolucionArticuloBusiness()
        {
            unitOfWork = new UnitOfWork();
            repository = new VentaDevolucionArticuloRepository(unitOfWork);
        }

        public List<VentaDevolucionArticuloDM> GetSalePurseByIdSale(Guid idDevolucion)
        {
            try
            {
                List<VentaDevolucionArticuloDM> ventaDevoluciones = new List<VentaDevolucionArticuloDM>();
                List<venta_devolucion_articulo> devoluciones = repository.GetAll(p => p.id_devolucion.Equals(idDevolucion)).ToList();
                foreach (var vtda in devoluciones)
                {
                    VentaDevolucionArticuloDM ventaDevolucion = new VentaDevolucionArticuloDM();
                    ventaDevolucion.IdDevolucion = vtda.id_devolucion !=null? vtda.id_devolucion : Guid.Empty;
                    ventaDevolucion.NoArticulo = vtda.no_articulo;
                    ventaDevolucion.CodBarras = vtda.cod_barras;
                    ventaDevolucion.Cantidad = vtda.cantidad;
                    ventaDevoluciones.Add(ventaDevolucion);
                }
                return ventaDevoluciones;
            }
            catch (Exception ex) when (ex is DataException || ex is SqlException)
            {

                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);
            }
        }
    }
}
