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
        private static AccountDAO dao = new AccountDAO();
        private static AccountRepository repository = new AccountRepository();

        [Key, Identity]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Password { get; set; }
        [Required, MaxLength(50)]
        public string Email { get; set; }

        #region With ORM only
        public static List<Account> GetAll()
        {
            return orm.Select<Account>(orm.Select()
                .Select<Account>()).ToList();
        }
        public static Account? GetAccountById(int id)
        {
            return orm.Select<Account>(orm.Select()
                .Select<Account>()
                .Where("Id = @id", new SqlParameter("@id", id)))
                .SingleOrDefault();
        }
        public static Account? CheckAccount(Account account)
        {
            return orm.Select<Account>(orm.Select()
                .Select<Account>()
                .Where("Email = @email AND Password = @pass",
                    new SqlParameter("@email", account.Email),
                    new SqlParameter("@pass", account.Password)))
                .SingleOrDefault();
        }
        public static bool Delete(Account account)
        {
            return orm.Delete<Account>(orm.Delete()
                .Delete<Account>()
                .Where("Id = @id", new SqlParameter("@id", account.Id)));
        }
        public static bool Insert(Account account)
        {
            return orm.Insert<Account>(orm.Insert()
                .Insert(account));
        }
        public static bool Update(Account account)
        {
            return orm.Update<Account>(orm.Update()
                .Update(account)
                .Where("Id = @id", new SqlParameter("@id", account.Id)));
        }
        #endregion

        #region With DAO pattern
        public static List<Account> GetAllDao()
        {
            return dao.GetAll();
        }
        public static Account GetAccountByIdDao(int id)
        {
            return dao.GetEntityById(id);
        }
        public static bool DeleteDao(Account account)
        {
            return dao.Delete(account);
        }
        public static bool InsertDao(Account account)
        {
            return dao.Insert(account);
        }
        public static bool UpdateDao(Account account)
        {
            return dao.Update(account);
        }
        #endregion

        #region With Repository Pattern
        public static List<Account> GetAllRepository()
        {
            return repository.GetAll();
        }
        public static Account GetAccountByIdRepository(int id)
        {
            return repository.GetValues(new AccountSpecificationById(id)).SingleOrDefault();
        }
        public static bool DeleteRepository(Account account)
        {
            return repository.Delete(account);
        }
        public static bool InsertRepository(Account account)
        {
            return repository.Insert(account);
        }
        public static bool UpdateRepository(Account account)
        {
            return repository.Update(account);
        }
        #endregion
    }
}
