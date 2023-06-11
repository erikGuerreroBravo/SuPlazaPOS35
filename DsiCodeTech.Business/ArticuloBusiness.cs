using DsiCodeTech.Common.DataAccess.Domain;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository;
using DsiCodeTech.Repository.Infraestructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsiCodeTech.Common.Exception;
using DsiCodeTech.Common.Constant;
using DsiCodeTech.Repository.PosCaja;
using DsiCodeTech.Business.Interface;

namespace DsiCodeTech.Business
{
    public class ArticuloBusiness: IArticuloBusiness
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ArticuloRepository repository;
        public ArticuloBusiness(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
            repository = new ArticuloRepository(unitOfWork);
        }
        public ArticuloBusiness()
        {
            unitOfWork = new UnitOfWork();
            repository = new ArticuloRepository(unitOfWork);
        }

        public articulo GetArticleByBarCode(string barcode) 
        {
            try
            {
                articulo articulo = repository.SingleOrDefault(p => p.cod_barras.Equals(barcode));
                return articulo;
            }
            catch (Exception ex) when (ex is DataException || ex is SqlException)
            {

                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);
            }
        }

    }
}
