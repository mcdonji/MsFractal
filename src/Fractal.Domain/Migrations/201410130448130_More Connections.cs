namespace Fractal.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreConnections : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AuditConnectionDescriptionParameters", "AuditConnectionDescription_AuditConnectionDescriptionId1", "dbo.AuditConnectionDescriptions");
            DropIndex("dbo.AuditConnectionDescriptionParameters", new[] { "AuditConnectionDescription_AuditConnectionDescriptionId1" });
            DropColumn("dbo.AuditConnectionDescriptionParameters", "AuditConnectionDescription_AuditConnectionDescriptionId1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AuditConnectionDescriptionParameters", "AuditConnectionDescription_AuditConnectionDescriptionId1", c => c.String(maxLength: 128));
            CreateIndex("dbo.AuditConnectionDescriptionParameters", "AuditConnectionDescription_AuditConnectionDescriptionId1");
            AddForeignKey("dbo.AuditConnectionDescriptionParameters", "AuditConnectionDescription_AuditConnectionDescriptionId1", "dbo.AuditConnectionDescriptions", "AuditConnectionDescriptionId");
        }
    }
}
