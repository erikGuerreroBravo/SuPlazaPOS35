using DsiCodeTech.Business.Interface;
using DsiCodeTech.Common.Constant;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Common.Exception;
using DsiCodeTech.Repository;
using DsiCodeTech.Repository.PosCaja;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Business
{
    public class PosSettingsBusiness : IPosSettingsBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PosSettingsRepository _posSettingsRepository;

        public PosSettingsBusiness(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._posSettingsRepository = new PosSettingsRepository(unitOfWork);
        }

        public pos_settings GetPosSettings()
        {
            try
            {
                pos_settings settings = this._posSettingsRepository.GetAll().FirstOrDefault();

                if (settings is null)
                {
                    throw new BusinessException("PV-VENTAS-002", "Contacte al administrador, no existe una configuración previa en pos settings.");
                }

                return settings;
            }
            catch (Exception ex) when (ex is DataException || ex is SqlException)
            {

                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);
            }
        }
    }
}