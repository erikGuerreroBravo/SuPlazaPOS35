using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Common.DataAccess.Infraestructure;
using DsiCodeTech.Repository.PosCaja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Repository
{
    public class ArticuloRepository : BaseRepository<articulo>
    {
        public ArticuloRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            
        }
    }
}
