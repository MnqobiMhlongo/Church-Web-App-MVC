namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PriceonItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemDonations", "Price", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemDonations", "Price");
        }
    }
}
