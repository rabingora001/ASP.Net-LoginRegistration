using System.ComponentModel.DataAnnotations;


//no other fields
namespace LogReg.Models
{
    public class LoginUser
    {
        [Required]
        public string Email {get;set;}
        [Required]
        public string Password {get;set;}
        
    }
}