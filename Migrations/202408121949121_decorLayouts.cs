namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class decorLayouts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventDecorLayOuts",
                c => new
                    {
                        Layoutid = c.Int(nullable: false, identity: true),
                        layoutName = c.String(),
                        layoutImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.Layoutid);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EventDecorLayOuts");
        }
    }
}
