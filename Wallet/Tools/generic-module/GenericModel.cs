using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Wallet.Tools.generic_module
{
    public class GenericModel : IGenericModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Id")]
        [Display(Name = "Id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public void Dispose() { }

        public object Minify(Func<object> minify = null)
        {
            if (minify != null) { return minify; }
            return this;
        }
    }
}
