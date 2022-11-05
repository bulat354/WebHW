using MyServer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Repository
{
    public class AccountSpecificationById : ISqlSpecification<Account>
    {
        public int Id;

        public AccountSpecificationById(int id)
        {
            this.Id = id;
        }

        public string ToSqlClauses()
        {
            return $"Id = {Id}";
        }
    }
}
