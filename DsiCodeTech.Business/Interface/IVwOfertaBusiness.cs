using DsiCodeTech.Common.DataAccess.Domain;

namespace DsiCodeTech.Business.Interface
{
    public interface IVwOfertaBusiness
    {
        Vw_OfertaDM GetFirstOferta(string codigoBarras);
    }
}
