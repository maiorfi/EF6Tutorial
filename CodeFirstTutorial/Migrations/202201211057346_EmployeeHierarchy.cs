namespace CodeFirstTutorial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeHierarchy : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Level = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Custodians",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false),
                        Shift = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false),
                        Subject = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Teachers", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Custodians", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Teachers", new[] { "EmployeeId" });
            DropIndex("dbo.Custodians", new[] { "EmployeeId" });
            DropTable("dbo.Teachers");
            DropTable("dbo.Custodians");
            DropTable("dbo.Employees");
        }
    }
}
