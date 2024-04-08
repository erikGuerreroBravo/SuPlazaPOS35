using DsiCodeTech.Common.DataAccess.Domain;

namespace DsiCodeTech.Business.Interface
{
    public interface IUsuarioBusiness
    {
        bool ValidateAnyUser();
        UsuarioDM ValidateLogin(string userName, string password);
    }
}
