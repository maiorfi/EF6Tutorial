namespace CodeFirstTutorial.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Due : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Grades", "Level", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Grades", "Level");
        }
    }
}
