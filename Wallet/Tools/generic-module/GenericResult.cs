namespace Wallet.Tools.generic_module
{
    public class GenericResult<T> : IGenericResult<T>
    {
        public bool success { get; set; }
        public T data { get; set; }
        public string message { get; set; }
    }

    public class OkResult<T> : GenericResult<T>
    {
        public OkResult(T data, string message)
        {
            success = true;
            data = data;
            message = message;
        }

    }
    public class InvalidResult<T> : GenericResult<T>
    {
        public InvalidResult() { success = false; }
        public InvalidResult(string ex) { success = false; message = ex; }
    }
}
