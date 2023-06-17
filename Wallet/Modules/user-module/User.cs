using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using Wallet.Modules.asset_module.enums;
using Wallet.Tools.generic_module;

namespace Wallet.Modules.user_module
{
    [Table("USER")]
    public class User : GenericModel
    {
        [Column("Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Column("UserName")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Column("Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Column("PasswordHash")]
        [Display(Name = "PasswordHash")]
        public byte[] PasswordHash { get; set; }

        [Column("PasswordSalt")]
        [Display(Name = "PasswordSalt")]
        public byte[] PasswordSalt { get; set; }

        [Column("CPF")]
        [Display(Name = "CPF")]
        public string CPF { get; set; }

        [Column("Role")]
        [Display(Name = "Role")]
        public eRole Role { get; set; } = eRole.User;

        [Column("IsEmailConfirmed")]
        [Display(Name = "IsEmailConfirmed")]
        public bool IsEmailConfirmed { get; set; } = false;
    }
}
