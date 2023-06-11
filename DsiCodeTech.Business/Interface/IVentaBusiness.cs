using DsiCodeTech.Common.DataAccess.Domain;
using DsiCodeTech.Repository.PosCaja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Business.Interface
{
    public interface IVentaBusiness
    {
        VentaDM GetVentaByFolio(long Folio);
        VentaDM GetVentaByIdVenta(Guid IdVenta);
        List<venta> GetVentasByDates(DateTime start, DateTime end);
    }
}
