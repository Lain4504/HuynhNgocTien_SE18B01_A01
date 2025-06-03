using System.ComponentModel.DataAnnotations;
namespace HuynhNgocTien_SE18B01_A01.ViewModel

{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
    }

}
