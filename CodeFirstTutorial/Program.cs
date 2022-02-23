using CodeFirstTutorial.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstTutorial
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using (var ctx = new SchoolContext("name=SchoolDBEntities"))
            {
                // 1 - GRADI

                //var grade = new Grade() { GradeName = "Grade One", Section = "Section One", Level=1 };


                // 2 - STUDENTI

                //var student = new Student() { StudentFirstName = "Lawrence", StudentLastName = "Mayorfs", Grade = grade, DateOfBirth = DateTime.Now.AddYears(-60), Height = 155.7m, Photo = new byte[] { 0x01, 0x02, 0x03, 0x04 }, Weight = 62.8f };

                //ctx.Students.Add(student);

                // 3 - IMPIEGATI (INSEGNANTI E BIDELLI)

                //var teacher = new Teacher()
                //{
                //    FirstName = "Mario",
                //    LastName = "Bianchi",
                //    Level = 8,
                //    Subject = "Biology"
                //};

                //var custodian = new Custodian()
                //{
                //    FirstName = "Gigetto",
                //    LastName = "Dozzini",
                //    Level = 5,
                //    Shift = 1
                //};

                //ctx.Employees.Add(teacher);
                //ctx.Employees.Add(custodian);

                // 4 - CORSI

                //var teacher = await ctx.Employees.FindAsync(1);

                //var course = new Course()
                //{
                //    Name = "Biology 1",
                //    Teacher = teacher as Teacher                    
                //};

                //var student = await ctx.Students.FindAsync(1);

                //student.Courses.Add(course);
                //course.Students.Add(student);

                // 5 - QUERY POLIMORFICA
                var someEmployees = await ctx.Employees.Where(e=>e.Level>3).ToListAsync();
                var someTeachers = await ctx.Employees.OfType<Teacher>().Where(t=>t.Subject=="Biology").ToListAsync();
                var someCustodians = await ctx.Employees.OfType<Custodian>().Where(c => c.Shift != 2).ToListAsync();

                Console.WriteLine($"Alcuni Impiegati: {string.Join(",", someEmployees.Select(e => $"{e.FirstName} {e.LastName} {e.Level}"))}");
                Console.WriteLine($"Alcuni Insegnanti: {string.Join(",", someTeachers.Select(t => $"{t.FirstName} {t.LastName} {t.Subject}"))}");
                Console.WriteLine($"Alcuni Bidelli: {string.Join(",", someCustodians.Select(b => $"{b.FirstName} {b.LastName} {b.Shift}"))}");

                var allTeachers = await ctx.Employees.Where(e => e is Teacher).ToListAsync();
                var allTeachers2 = await ctx.Employees.OfType<Teacher>().ToListAsync();

                Console.WriteLine($"Tutti gli Insegnanti: {string.Join(",", allTeachers.Select(t => $"{t.FirstName} {t.LastName} {(t as Teacher).Subject}"))}");
                Console.WriteLine($"Tutti gli Insegnanti 2: {string.Join(",", allTeachers2.Select(t => $"{t.FirstName} {t.LastName} {t.Subject}"))}");

                // 99 - SALVATAGGIO
                //await ctx.SaveChangesAsync();
            }

            Console.ReadKey(true);
        }
    }
}
