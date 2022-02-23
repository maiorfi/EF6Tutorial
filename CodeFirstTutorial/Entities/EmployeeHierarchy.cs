using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstTutorial.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Level { get; set; }
    }

    [Table("Teachers")]
    public class Teacher : Employee
    {
        public string Subject { get; set; }
    }

    [Table("Custodians")]
    public class Custodian : Employee
    {
        public int Shift { get; set; }
    }
}
