using Bogus;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MainTutorial
{
    internal class Program
    {
        private const string ActivitySourceName = "EntityFramework6.Tutorial.App";

        private static readonly ActivitySource ThisAppActivitySource = new ActivitySource(ActivitySourceName);

        private static Faker<Student> _studentsWithAddress;
        private static Faker<Student> _studentsWithStandardAndAddressFaker;

        public async static Task Main(string[] args)
        {
            var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetSampler(new AlwaysOnSampler())
            .AddSource(ActivitySourceName)
            .AddSqlClientInstrumentation(options =>
            {
                options.SetDbStatement = true;
                options.EnableConnectionLevelAttributes = true;
            })
            .AddJaegerExporter()
            .AddZipkinExporter()
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MainProgram"))
            .Build();

            _studentsWithAddress = new Faker<Student>()
                .RuleFor(s => s.StudentName, f => f.Person.FullName)
                .RuleFor(s => s.StandardId, _ => 1)
                .RuleFor(s => s.StudentAddress, f => new StudentAddress()
                {
                    Address1 = f.Address.StreetAddress(),
                    City = f.Address.City(),
                    State = f.Address.State()
                });

            _studentsWithStandardAndAddressFaker = new Faker<Student>()
                .RuleFor(s => s.StudentName, f => f.Person.FullName)
                .RuleFor(s => s.StandardId, _ => 1)
                .RuleFor(s => s.StudentAddress, f => new StudentAddress()
                {
                    Address1 = f.Address.StreetAddress(),
                    City = f.Address.City(),
                    State = f.Address.State()
                })
                .RuleFor(s=>s.Standard, f=>new Standard()
                {
                    StandardName=f.Random.Words(1),
                    Description = f.Random.Words(2)
                });

            using (tracerProvider)
            {
                using (var activity = ThisAppActivitySource.StartActivity(thisMethodName()))
                {
                    await runSimpleAsyncQuery();
                    await runGetByPrimaryKey();
                    await runSimpleListAsyncQuery();
                    await runSimpleSortedListAsyncQuery();
                    await runSimpleGroupByAsyncQuery();
                    await runSelectManyAsyncQuery();
                    await runIncludeAsyncQuery();
                    await runAddNewStudentWithAddress();
                    await runAddNewStudentWithAddressAndStandard();
                }
            }

            Console.ReadKey(intercept: true);
        }

        public static string thisMethodName([CallerMemberName] string caller = null)
        {
            return caller;
        }

        private async static Task runSimpleAsyncQuery()
        {
            using (var activity = ThisAppActivitySource.StartActivity(thisMethodName()))
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
        }

        private async static Task runGetByPrimaryKey()
        {
            using (var activity = ThisAppActivitySource.StartActivity(thisMethodName()))
            {
                using (var ctx = new SchoolDBEntities())
                {
                    var student = await ctx.Students.FindAsync(1);

                    //Console.WriteLine(ObjectDumper.Dump(student));

                    Debug.Assert(student != null);
                    Debug.Assert(student.StudentID == 1);
                }
            }
        }

        private async static Task runSimpleListAsyncQuery()
        {
            using (var activity = ThisAppActivitySource.StartActivity(thisMethodName()))
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
        }

        private async static Task runSimpleGroupByAsyncQuery()
        {
            using (var activity = ThisAppActivitySource.StartActivity(thisMethodName()))
            {
                using (var ctx = new SchoolDBEntities())
                {
                    await ctx.Students.GroupBy(s => s.StandardId)/*.Where(g => g.Count() > 1)*/.ForEachAsync(async g =>
                        {
                            using (var ctx2 = new SchoolDBEntities())
                            {
                                activity.AddEvent(new ActivityEvent($"Inizio processing gruppo {g.Key}"));

                                var standard = await ctx2.Standards.FindAsync(g.Key);

                                activity.AddEvent(new ActivityEvent($"Fine processing gruppo {g.Key} ({g.Count()} elementi)"));

                                Console.WriteLine($"Standard : Id={g.Key}, Nome={standard.StandardName}, Numero Studenti={g.Count()}");
                            }
                        });
                }
            }
        }

        private async static Task runSimpleSortedListAsyncQuery()
        {
            using (var activity = ThisAppActivitySource.StartActivity(thisMethodName()))
            {
                using (var ctx = new SchoolDBEntities())
                {
                    var students = await ctx.Students.OrderBy(s => s.StudentName).ToListAsync();

                    students.ForEach(student => Console.WriteLine(student.StudentName));
                }
            }
        }

        private async static Task runSelectManyAsyncQuery()
        {
            using (var activity = ThisAppActivitySource.StartActivity(thisMethodName()))
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

        private async static Task runIncludeAsyncQuery()
        {
            using (var activity = ThisAppActivitySource.StartActivity(thisMethodName()))
            {
                IEnumerable<Student> students;

                using (var ctx = new SchoolDBEntities())
                {
                    students = await ctx.Students.Include(s => s.Standard.Teachers).OrderBy(s => s.StudentName).ToListAsync();
                }

                foreach (var s in students)
                {
                    Debug.Assert(s.Standard != null);

                    Console.WriteLine($"{s.StudentName} - {s.Standard.Description} - {string.Join("; ", s.Standard.Teachers.Select(t => $"{t.TeacherName} ({t.TeacherType})")).Trim()}");
                }
            }
        }

        private async static Task runAddNewStudentWithAddress()
        {
            using (var activity = ThisAppActivitySource.StartActivity(thisMethodName()))
            {

                using (var ctx = new SchoolDBEntities())
                {
                    var student = ctx.Students.Add(_studentsWithAddress.Generate());
                    await ctx.SaveChangesAsync();

                    Console.WriteLine($"Creato studente di nome {student.StudentName} e indirizzo {student.StudentAddress.Address1} - {student.StudentAddress.City} ({student.StudentAddress.State}) con Id {student.StudentID}");
                }
            }
        }

        private async static Task runAddNewStudentWithAddressAndStandard()
        {
            using (var activity = ThisAppActivitySource.StartActivity(thisMethodName()))
            {

                using (var ctx = new SchoolDBEntities())
                {
                    var student = ctx.Students.Add(_studentsWithStandardAndAddressFaker.Generate());
                    await ctx.SaveChangesAsync();

                    Console.WriteLine($"Creato studente di nome {student.StudentName} e indirizzo {student.StudentAddress.Address1} - {student.StudentAddress.City} ({student.StudentAddress.State}) e Standard {student.Standard.StandardName} ({student.Standard.Description}), con Id {student.StudentID}");
                }
            }
        }
    }
}