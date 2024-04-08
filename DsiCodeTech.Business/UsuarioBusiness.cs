using DsiCodeTech.Business.Interface;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Common.Exception;
using DsiCodeTech.Repository;
using DsiCodeTech.Repository.Infraestructure;
using System;
using static DsiCodeTech.Common.Constant.DsiCodeConst;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsiCodeTech.Common.DataAccess.Domain;
using DsiCodeTech.Repository.PosCaja;

namespace DsiCodeTech.Business
{
    public class UsuarioBusiness: IUsuarioBusiness
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UsarioRepository repository;
        
        public UsuarioBusiness(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
            repository = new UsarioRepository(unitOfWork);    
        }
        public UsuarioBusiness()
        {
            unitOfWork = new UnitOfWork();
            repository = new UsarioRepository(unitOfWork);
        }

        /// <summary>
        /// Este metodo se encarga de consultar si existe algun usuario dentro del sistema
        /// </summary>
       /// <returns>una respuetsa del tipo true/false</returns>
       /// <exception cref="BusinessException">lanza una excepcion en caso de ocurrir un problema</exception>
        public bool ValidateAnyUser()
        {
            repository.startTransaction();
            try
            {

                bool result = repository.GetAll().Any();
                if (result)
                {
                    repository.commitTransaction();
                }
                return result;
            }
            catch (Exception ex)
            {
                repository.rollbackTransaction();
                throw new BusinessException(RESULT_WITHEXCPETION_ID, RESULT_WITHEXCPETION, ex);
            }    
        }

        /// <summary>
        /// Este metodo se encarga de consultar y validar a un usuario con las credenciales asignadas en el sistema
        /// </summary>
        /// <param name="userName">el nombre de usuario</param>
        /// <param name="password">el password</param>
        /// <returns>un objeto usuario del sistema</returns>
        /// <exception cref="BusinessException">se lanza una excepcion en caso de error</exception>
        public UsuarioDM ValidateLogin(string userName, string password)
        {
            repository.startTransaction();
            try
            {
                DsiCodeTech.Repository.PosCaja.usuario user = repository.SingleOrDefault(u => u.user_name.Equals(userName) && u.password.Equals(password) && u.usuario_permiso.FirstOrDefault(up => up.id_permiso.Equals("pos_caja")) != null);
                UsuarioDM usuarioDM = new UsuarioDM() {
                    UserName = user.user_name.Trim(), 
                    Password = password.Trim()
                };
                return usuarioDM;
            }
            catch (Exception ex)
            {
                throw new BusinessException(RESULT_WITHEXCPETION_ID, RESULT_WITHEXCPETION, ex);
            }
        }



    }
}
