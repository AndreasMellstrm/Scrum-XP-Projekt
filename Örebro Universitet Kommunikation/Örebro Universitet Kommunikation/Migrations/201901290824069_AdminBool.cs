namespace Örebro_Universitet_Kommunikation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminBool : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Admin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Admin");
        }
    }
}
