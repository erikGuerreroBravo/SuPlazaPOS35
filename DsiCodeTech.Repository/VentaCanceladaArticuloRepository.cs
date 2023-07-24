using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DsiCodeTech.Common.DataAccess.Infraestructure;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Repository
{
    public class VentaCanceladaArticuloRepository : BaseRepository<venta_cancelada_articulo>
    {
        public VentaCanceladaArticuloRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
