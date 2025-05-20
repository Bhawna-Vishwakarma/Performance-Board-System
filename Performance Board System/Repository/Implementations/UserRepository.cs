using Dapper;
using Microsoft.IdentityModel.Tokens;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Data;

namespace Performance_Board_System.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        public UserRepository(DapperContext context) {

            // Constructor logic here
            _context = context;
        }

        public async Task<int> RegisterUser(User user)
        {
            user.PasswordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.PasswordHash));

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@FullName", user.FullName);
                    parameters.Add("@Email", user.Email);
                    parameters.Add("@PasswordHash", user.PasswordHash);
                    parameters.Add("@Role", user.Role);
                    parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync("RegisterUser",parameters,commandType: CommandType.StoredProcedure);

                    int result = parameters.Get<int>("@Result");
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in RegisterUser: " + ex.Message);
                    return 0;
                }
            }
        }


    }
}
