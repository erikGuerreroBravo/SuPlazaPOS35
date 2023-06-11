using DsiCodeTech.Common.DataAccess.Infraestructure;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Repository
{
    public class VentaDevolucionArticuloRepository: BaseRepository<venta_devolucion_articulo>
    {
        public VentaDevolucionArticuloRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
