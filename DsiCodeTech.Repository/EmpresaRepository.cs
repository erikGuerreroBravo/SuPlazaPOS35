using DsiCodeTech.Common.DataAccess.Infraestructure;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Repository
{
    public class EmpresaRepository: BaseRepository<empresa>
    {
        public EmpresaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
                
        }
    }
}
