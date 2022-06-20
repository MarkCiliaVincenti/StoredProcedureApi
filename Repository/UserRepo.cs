using System.Data;
using System.Dynamic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StoredProcedureApi.Models;
using StoredProcedureApi.SPEndpoints;
using System.Data.SqlClient;
using System.Data.Common;

namespace StoredProcedureApi.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepo> _logger;

        public UserRepo(AppDbContext context, ILogger<UserRepo> logger)
        {
            _context = context;
            _logger = logger;
            
        }

        public async Task<int> CreateUserAsync(UserProfile model)
        {
           //Declaring Parameters binding
           var emailAddParm = new SqlParameter("@emailAddress",model.EmailAddress);
           var passwordHashParam = new SqlParameter("@passwordHash", model.PasswordHash);
           var oldParam = new SqlParameter("@old", model.Old);
           var oldProvParam = new SqlParameter("@oldProvider", string.IsNullOrEmpty(model.OldProvider)? "": model.OldProvider);
           var userIdParam = new SqlParameter("@useridout", SqlDbType.Int);
           userIdParam.Direction = ParameterDirection.Output; 
           int result = new int();
           try
           {
               if(model != null)
                {                            
                   await _context.Database.ExecuteSqlRawAsync(Endpoints.SqlCreateUsers,emailAddParm,passwordHashParam,oldParam,oldProvParam,userIdParam);                  
                   var result2 = Convert.ToInt32(userIdParam.Value); 
                   result = result2;

                }
                else
                {
                    return -1;
                }
           
           }
           catch (SqlException ex)
           {
               _logger.LogInformation($"Error creating User:{ex.Message}");
                
           }
            return result;
          
        }

        public Task<bool> DeletUsersAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserProfile>> GetUsersAsync(int id)
        {
            
            var idParam = new SqlParameter("@id", id);
            //var passwordHashParam = new SqlParameter("@passwordHash", passwordHash);
             
            var users =  _context.UserProfiles.FromSqlRaw(Endpoints.SqlGetUsers, idParam).ToList();
            
             if(users.Count < 0)
              {
                return null;
              }  
              else
              {
                 return Task.FromResult<List<UserProfile>>(users).Result;
              }     
        }

        public Task<UserProfile> UpdateUser(UserProfile model)
        {
            throw new NotImplementedException();
        }
    }











    public interface IUserRepo
    {
        Task<int> CreateUserAsync(UserProfile model);
        Task <List<UserProfile>> GetUsersAsync(int id);

        Task <bool> DeletUsersAsync(int id);

        Task<UserProfile> UpdateUser(UserProfile model);
    }

}





    