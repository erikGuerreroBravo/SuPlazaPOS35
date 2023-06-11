using DsiCodeTech.Common.DataAccess.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Business.Interface
{
    public interface IVentaDevolucionArticuloBusiness
    {
        List<VentaDevolucionArticuloDM> GetSalePurseByIdSale(Guid idDevolucion);
    }
}
