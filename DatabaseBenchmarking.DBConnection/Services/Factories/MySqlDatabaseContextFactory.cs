using DatabaseBenchmarking.DBConnection.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseBenchmarking.DBConnection.Services.Factories
{
    public class MySqlDatabaseContextFactory : DatabaseContextFactory
    {
        public override DatabaseContext GetContext()
        {
            return new MySqlDatabaseContext();
        }
    }
}
