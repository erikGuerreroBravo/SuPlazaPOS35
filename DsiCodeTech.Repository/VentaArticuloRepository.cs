using DsiCodeTech.Common.DataAccess.Infraestructure;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository.PosCaja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Repository
{
    public class VentaArticuloRepository: BaseRepository<venta_articulo>
    {
        public VentaArticuloRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }
}
