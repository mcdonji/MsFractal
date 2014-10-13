namespace Fractal.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdcorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuditDomainConcepts", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuditDomainConcepts", "Order");
        }
    }
}
