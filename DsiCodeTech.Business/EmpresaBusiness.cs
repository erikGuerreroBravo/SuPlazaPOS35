using DsiCodeTech.Common.Constant;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Common.Exception;
using DsiCodeTech.Repository;
using DsiCodeTech.Repository.Infraestructure;
using DsiCodeTech.Repository.PosCaja;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsiCodeTech.Common.DataAccess.Domain;
using DsiCodeTech.Business.Interface;

namespace DsiCodeTech.Business
{
    public class EmpresaBusiness: IEmpresaBusiness
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly EmpresaRepository _repository;
        public EmpresaBusiness()
        {
            unitOfWork = new UnitOfWork();
            _repository = new EmpresaRepository(unitOfWork);
        }

        /// <summary>
        /// Este metodo se encarga de consultar  a la empresa base o default
        /// </summary>
        /// <returns>la empresa que existe por default</returns>
        /// <exception cref="BusinessException">excepcion no controlada por el usuario</exception>
        public EmpresaDM GetEmpresa()
        {
            try
            {
                _repository.startTransaction();
                empresa empresa = _repository.GetAll().SingleOrDefault();
                EmpresaDM empresaDM = new EmpresaDM();
                empresaDM.Rfc = empresa.rfc;
                empresaDM.RazonSocial = empresa.razon_social;
                empresaDM.Representante = empresa.representante;
                empresaDM.Calle = empresa.calle;
                empresaDM.NoExterior = empresa.no_ext;
                empresaDM.NoInterior = empresa.no_int;
                empresaDM.Colonia = empresa.colonia;
                empresaDM.Municipio = empresa.municipio;
                empresaDM.Estado = empresa.estado;
                empresaDM.Pais = empresa.pais;
                empresaDM.CodPostal = empresa.codigo_postal;
                empresaDM.TelPrincipal = empresa.tel_principal;
                empresaDM.Email = empresa.e_mail;
                empresaDM.FechaRegistro = empresa.fecha_registro;
                
                return empresaDM;
            }
            catch (Exception ex) when (ex is DataException || ex is SqlException)
            {

                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);
            }
        }
    }
}
