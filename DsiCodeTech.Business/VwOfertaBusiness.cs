using DsiCodeTech.Business.Interface;
using DsiCodeTech.Common.Constant;
using DsiCodeTech.Common.DataAccess.Domain;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Common.Exception;
using DsiCodeTech.Repository;
using DsiCodeTech.Repository.Infraestructure;
using DsiCodeTech.Repository.PosCaja;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DsiCodeTech.Business
{
    public class VwOfertaBusiness:IVwOfertaBusiness
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly Vw_OfertaRepository repository;

        public VwOfertaBusiness()
        {
            unitOfWork = new UnitOfWork();
            repository = new Vw_OfertaRepository(unitOfWork);
        }

        /// <summary>
        /// Este metodo se encarga de consultar la vista de SQL VW_OFERTA
        /// </summary>
        /// <param name="codigoBarras">el parametro de busqueda</param>
        /// <returns>una entidad del tipo Vw_OfertaDM</returns>
        /// <exception cref="BusinessException">excepcion no controlada por el usuario</exception>
        public Vw_OfertaDM GetFirstOferta(string codigoBarras)
        {
            try
            {
                vw_oferta oferta= repository.SingleOrDefault(p => p.cod_barras.Equals(codigoBarras));
                Vw_OfertaDM vw_OfertaDM = new Vw_OfertaDM
                {
                    Cod_Barras = oferta.cod_barras,
                    Precio_Oferta = oferta.precio_oferta
                };
                return vw_OfertaDM;
            }
            catch (Exception ex) when (ex is DataException || ex is SqlException)
            {
                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);
            }
        }


    }
}
