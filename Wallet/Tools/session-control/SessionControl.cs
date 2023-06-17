using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.Modules.user_module;
using Wallet.Tools.generic_module;

namespace Wallet.Tools.session_control
{
    [Table("SESSION_CONTROL")]
    public class SessionControl : GenericModel
    {
        [Column()]
        [Display()]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Column()]
        [Display()]
        public string SessionId { get; set; }

        [Column()]
        [Display()]
        public DateTime? DateLogin { get; set; } = DateTime.Now;

        [Column()]
        [Display()]
        public DateTime? DateLogout { get; set; }

        [Column()]
        [Display()]
        public eStatusSessionControl Status { get; set; }
    }
}
