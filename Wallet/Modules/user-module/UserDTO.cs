using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Wallet.Modules.user_module
{
    public class UserDTO
    {
        public string? Name { get; set; }

        public string UserName { get; set; }

        public string? Email { get; set; }

        public string Password { get; set; }

        public string? CPF { get; set; }
    }
}
