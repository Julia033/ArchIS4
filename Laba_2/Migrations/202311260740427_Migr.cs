namespace Laba_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migr : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cinemas", "DateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cinemas", "DateTime", c => c.String());
        }
    }
}
