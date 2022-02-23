using CodeFirstTutorial.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstTutorial
{
    public class SchoolContext : DbContext
    {
        const string CONNECTION_STRING_NAME_FOR_MIGRATIONS = "SchoolDBEntities";

        public SchoolContext() : base(CONNECTION_STRING_NAME_FOR_MIGRATIONS)
        {

        }

        public SchoolContext(string connectionName) : base(connectionName)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SchoolContext, Migrations.Configuration>());
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}
