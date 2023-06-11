using DsiCodeTech.Common.DataAccess.Infraestructure;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Repository
{
    public  class VentaDevolucionRepository : BaseRepository<venta_devolucion>
    {
        public VentaDevolucionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
