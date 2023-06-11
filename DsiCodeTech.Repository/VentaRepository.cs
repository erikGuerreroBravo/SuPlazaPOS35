using DsiCodeTech.Common.DataAccess.Infraestructure;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Repository
{
    public class VentaRepository: BaseRepository<venta>
    {
        public VentaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
