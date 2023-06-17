using Wallet.Tools.generic_module;

namespace Wallet.Modules.user_module
{
    public interface IUserService : IGenericService<User>
    {
        Task<User> Creat(UserDTO userDto);
        Task<List<User>> Read(string? id, string? ticker);
        Task<User> Update(string id, User user);
        Task<User> Delete(string id);
        Task<string> Login(UserDTO userDTO);
        string CreateRandomCpf();
    }
}