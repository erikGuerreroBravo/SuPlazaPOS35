using DsiCodeTech.Common.DataAccess.Infraestructure;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Repository
{
    public class UsarioRepository : BaseRepository<usuario>
    {
        public UsarioRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
                
        }
    }
}
