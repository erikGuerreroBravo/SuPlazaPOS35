using DsiCodeTech.Common.DataAccess.Infraestructure;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Repository
{
    public class Vw_OfertaRepository:BaseRepository<vw_oferta>
    {
        public Vw_OfertaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
                
        }
    }
}
