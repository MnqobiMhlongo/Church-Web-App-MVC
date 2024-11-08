namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemOIT : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "item", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderItems", "item");
        }
    }
}
