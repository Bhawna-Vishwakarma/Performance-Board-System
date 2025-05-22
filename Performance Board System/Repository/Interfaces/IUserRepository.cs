using Performance_Board_System.Models;
namespace Performance_Board_System.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<int> RegisterUser(User user);

        Task<int> LoginUser(string email, string passwordHash);

        Task<User> GetUserByEmail(string email);
    }
}
