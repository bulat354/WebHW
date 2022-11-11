using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyORM.Attributes;
using MyORM;
using MyServer.DAO;
using System.Data.SqlClient;
using MyServer.Repository;

namespace MyServer.Controllers
{
    [Table("Accounts")]
    public class Account
    {
        private static MiniORM orm = new MiniORM();

        [Key, Identity]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Password { get; set; }
        [Required, MaxLength(50)]
        public string Email { get; set; }

        #region With ORM only
        public static List<Account> GetAll()
        {
            return orm
                .Select<Account>()
                .Go<Account>()
                .ToList();
        }
        public static Account? GetAccountById(int id)
        {
            return orm
                .Select<Account>()
                .Where("Id = @id", ("@id", id))
                .Go<Account>()
                .SingleOrDefault();
        }
        public static Account? CheckAccount(Account account)
        {
            return orm
                .Select<Account>()
                .Where("Email = @email AND Password = @pass",
                    ("@email", account.Email),
                    ("@pass", account.Password))
                .Go<Account>()
                .SingleOrDefault();
        }
        public static bool Delete(Account account)
        {
            return orm
                .Delete<Account>()
                .Where("Id = @id", ("@id", account.Id))
                .Go() > 0;
        }
        public static bool Insert(Account account)
        {
            return orm
                .Insert(account)
                .Go() > 0;
        }
        public static bool Update(Account account)
        {
            return orm
                .Update(account)
                .Where("Id = @id", ("@id", account.Id))
                .Go() > 0;
        }
        #endregion
    }
}
