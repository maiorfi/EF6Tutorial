namespace CodeFirstTutorial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Courses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        CourseId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Teacher_EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.CourseId)
                .ForeignKey("dbo.Teachers", t => t.Teacher_EmployeeId)
                .Index(t => t.Teacher_EmployeeId);
            
            CreateTable(
                "dbo.CourseStudents",
                c => new
                    {
                        Course_CourseId = c.Int(nullable: false),
                        Student_StudentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Course_CourseId, t.Student_StudentID })
                .ForeignKey("dbo.Courses", t => t.Course_CourseId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.Student_StudentID, cascadeDelete: true)
                .Index(t => t.Course_CourseId)
                .Index(t => t.Student_StudentID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "Teacher_EmployeeId", "dbo.Teachers");
            DropForeignKey("dbo.CourseStudents", "Student_StudentID", "dbo.Students");
            DropForeignKey("dbo.CourseStudents", "Course_CourseId", "dbo.Courses");
            DropIndex("dbo.CourseStudents", new[] { "Student_StudentID" });
            DropIndex("dbo.CourseStudents", new[] { "Course_CourseId" });
            DropIndex("dbo.Courses", new[] { "Teacher_EmployeeId" });
            DropTable("dbo.CourseStudents");
            DropTable("dbo.Courses");
        }
    }
}
