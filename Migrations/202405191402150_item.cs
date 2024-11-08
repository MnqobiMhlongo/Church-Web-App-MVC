namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class item : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.baskets", "item", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.baskets", "item");
        }
    }
}
