using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MainTutorial
{
    using CoppiaCorsoStudente = Tuple<Course, Student>;

    internal class Program
    {
        public async static Task Main(string[] args)
        {
            await runSimpleAsyncQuery();
            await runGetByPrimaryKey();
            await runSimpleListAsyncQuery();
            await runSimpleSortedListAsyncQuery();
            await runSimpleGroupByAsyncQuery();
            await runSelectManyAsyncQuery();

            Console.ReadKey(intercept: true);
        }

        private async static Task runSimpleAsyncQuery()
        {
            using (var ctx = new SchoolDBEntities())
            {
                var student = await ctx.Students
                    .Where(s => s.StudentName == "Bill")
                    .FirstOrDefaultAsync();

                //Console.WriteLine(ObjectDumper.Dump(student));

                Debug.Assert(student != null);
            }
        }

        private async static Task runGetByPrimaryKey()
        {
            using (var ctx = new SchoolDBEntities())
            {
                var student = await ctx.Students.FindAsync(1);

                //Console.WriteLine(ObjectDumper.Dump(student));

                Debug.Assert(student != null);
                Debug.Assert(student.StudentID == 1);
            }
        }

        private async static Task runSimpleListAsyncQuery()
        {
            using (var ctx = new SchoolDBEntities())
            {
                var students = await ctx.Students.Where(s => s.StudentName.StartsWith("M")).ToListAsync();

                //Console.WriteLine(ObjectDumper.Dump(students));

                Debug.Assert(students != null);
                Debug.Assert(students.Any());
                Debug.Assert(students.All(s => s.StudentName.StartsWith("M")));
            }
        }

        private async static Task runSimpleGroupByAsyncQuery()
        {
            using (var ctx = new SchoolDBEntities())
            {
                await ctx.Students.GroupBy(s => s.StandardId)/*.Where(g => g.Count() > 1)*/.ForEachAsync(async g =>
                    {
                        using (var ctx2 = new SchoolDBEntities())
                        {
                            var standard = await ctx2.Standards.FindAsync(g.Key);

                            Console.WriteLine($"Standard : Id={g.Key}, Nome={standard.StandardName}, Numero Studenti={g.Count()}");
                        }
                    });
            }
        }

        private async static Task runSimpleSortedListAsyncQuery()
        {
            using (var ctx = new SchoolDBEntities())
            {
                var students = await ctx.Students.OrderBy(s => s.StudentName).ToListAsync();

                students.ForEach(student => Console.WriteLine(student.StudentName));
            }
        }

        private async static Task runSelectManyAsyncQuery()
        {

            using (var ctx = new SchoolDBEntities())
            {
                var pairs = await ctx.Students.Where(s => s.StudentName.StartsWith("M")).SelectMany(s => s.Courses.Select(c => new { Corso = c, Studente = s })).OrderBy(cs => cs.Studente.StudentName).ToListAsync();

                foreach (var cs in pairs)
                {
                    Console.WriteLine($"{cs.Corso.CourseName} - {cs.Studente.StudentName}");
                }

            }
        }
    }
}