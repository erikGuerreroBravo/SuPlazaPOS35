using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DsiCodeTech.Business.Interface;
using DsiCodeTech.Common.Constant;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Common.Exception;
using DsiCodeTech.Repository;
using DsiCodeTech.Repository.Infraestructure;
using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Business
{
    public class VentaCanceladaArticuloBusiness : IVentaCanceladaArticuloBusiness
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly VentaCanceladaArticuloRepository _ventaCanceladaArticuloRepository;

        public VentaCanceladaArticuloBusiness()
        {
            unitOfWork = new UnitOfWork();
            _ventaCanceladaArticuloRepository = new VentaCanceladaArticuloRepository(unitOfWork);
        }

        public bool Delete(Guid id)
        {
            try
            {

                if (this._ventaCanceladaArticuloRepository.Exists(query => query.id_venta_cancel.Equals(id)))
                {
                    this._ventaCanceladaArticuloRepository.startTransaction();

                    this._ventaCanceladaArticuloRepository.Delete(query => query.id_venta_cancel.Equals(id));

                    this._ventaCanceladaArticuloRepository.commitTransaction();
                }

                return true;
            }
            catch (Exception ex)
            {
                this._ventaCanceladaArticuloRepository.rollbackTransaction();
                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);
            }
            finally
            {
                this._ventaCanceladaArticuloRepository.disposeTransaction();
            }
        }
    }
}
