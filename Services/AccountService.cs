using HuynhNgocTien_SE18B01_A01.Models;
using HuynhNgocTien_SE18B01_A01.Repositories;
using HuynhNgocTien_SE18B01_A01.ViewModel;

namespace HuynhNgocTien_SE18B01_A01.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _repo;
        public AccountService(IAccountRepository repo) 
        {
            _repo = repo;            
        }
        public SystemAccount? CheckLogin(LoginViewModel model)
        {
            var user = _repo.SignIn(model);
            return user;
        }

        public List<SystemAccount> GetListUser() 
        {
            return _repo.GetListUser();
        }

        public SystemAccount? GetAccountById(short id) 
        {
            return _repo.GetAccountById(id);
        }

        public void Register(SystemAccount account)
        {
           _repo.CreateNewAccount(account);
        }


    }
}
