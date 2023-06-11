using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DsiCodeTech.Business.Interface;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Common.Exception;
using DsiCodeTech.Repository;
using DsiCodeTech.Repository.PosCaja;

using static DsiCodeTech.Common.Constant.DsiCodeConst;

namespace DsiCodeTech.Business
{
    public class OfertaBusiness : IOfertaBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OfertaRepository _ofertaRepository;
        private readonly OfertaArticuloRepository _ofertaArticuloRepository;

        public OfertaBusiness(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._ofertaRepository = new(_unitOfWork);
            this._ofertaArticuloRepository = new(_unitOfWork);
        }

        public oferta_articulo GetActiveOfferByCodBar(string cod_barras)
        {
            try
            {
                DateTime finish = new DateTime(year: DateTime.Now.Year, month: DateTime.Now.Month, day: DateTime.Now.Day, hour: 23, minute: 59, second: 59);
                oferta_articulo articulo = this._ofertaArticuloRepository
                    .SingleOrDefault(oa =>
                    oa.cod_barras.Equals(cod_barras) &&
                    oa.oferta.fecha_fin >= finish &&
                    new List<string>() { PRINCIPAL, ASOCIADO }.Contains(oa.articulo.tipo_articulo) &&
                    DISPONIBLE.Equals(oa.status_oferta));
                return articulo;
            }
            catch (Exception ex)
            {
                throw new BusinessException(RESULT_WITHEXCPETION_ID, RESULT_WITHEXCPETION, ex);
            }
        }
    }
}
