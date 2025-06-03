
using HuynhNgocTien_SE18B01_A01.Models;
using HuynhNgocTien_SE18B01_A01.ViewModel;

namespace HuynhNgocTien_SE18B01_A01.Repositories
{
    public interface IAccountRepository
    {
        SystemAccount? SignIn(LoginViewModel model);

        List<SystemAccount> GetListUser ();

        SystemAccount? GetAccountById(short Id); 

        void AccountStatus(SystemAccount account);

       void CreateNewAccount(SystemAccount account);
    }
}
