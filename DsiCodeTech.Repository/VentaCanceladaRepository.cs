using DsiCodeTech.Common.DataAccess.Infraestructure;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Repository
{
    public class VentaCanceladaRepository:BaseRepository<venta_cancelada>
    {
        public VentaCanceladaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            
        }
    }
}
