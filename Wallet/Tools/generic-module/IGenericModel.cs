using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Wallet.Tools.generic_module
{
    public interface IGenericModel : IDisposable
    {
        string Id { get; set; }
    }
}