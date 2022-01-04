using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MainTutorial
{
    internal class Program
    {
        public async static Task Main(string[] args)
        {
            await runTest1();
        }

        private async static Task runTest1()
        {
            using (var ctx = new SchoolDBEntities())
            {
                var student = await ctx.Students
                    .Where(s => s.StudentName == "Bill")
                    .FirstOrDefaultAsync();

                Console.WriteLine(ObjectDumper.Dump(student));

                Debug.Assert(student != null);
            }
        }
    }
}