namespace Wallet.Tools.generic_module
{
    public interface IGenericResult<T>
    {
        T data { get; set; }
        string message { get; set; }
        bool success { get; set; }
    }
}
