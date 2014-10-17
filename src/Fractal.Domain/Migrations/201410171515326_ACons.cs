namespace Fractal.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ACons : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuditConnections", "ParId", c => c.String());
            AddColumn("dbo.AuditConnections", "ChiId", c => c.String());
            AddColumn("dbo.AuditConnections", "Aorder", c => c.Int(nullable: false));
            AddColumn("dbo.AuditConnections", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.AuditConnections", "ConId", c => c.String());
            AddColumn("dbo.AuditConnections", "CdId", c => c.String());
            AddColumn("dbo.AuditConnections", "LeftDciId", c => c.String());
            AddColumn("dbo.AuditConnections", "RightDciId", c => c.String());
            AddColumn("dbo.AuditConnections", "LeftDci_AuditDomainConceptInstanceId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AuditConnections", "Chi_AuditConnectionId", c => c.String(maxLength: 128));
            AddColumn("dbo.AuditConnections", "RightDci_AuditDomainConceptInstanceId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.AuditConnections", "LeftDci_AuditDomainConceptInstanceId");
            CreateIndex("dbo.AuditConnections", "Chi_AuditConnectionId");
            CreateIndex("dbo.AuditConnections", "RightDci_AuditDomainConceptInstanceId");
            AddForeignKey("dbo.AuditConnections", "LeftDci_AuditDomainConceptInstanceId", "dbo.AuditDomainConceptInstances", "AuditDomainConceptInstanceId");
            AddForeignKey("dbo.AuditConnections", "Chi_AuditConnectionId", "dbo.AuditConnections", "AuditConnectionId");
            AddForeignKey("dbo.AuditConnections", "RightDci_AuditDomainConceptInstanceId", "dbo.AuditDomainConceptInstances", "AuditDomainConceptInstanceId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuditConnections", "RightDci_AuditDomainConceptInstanceId", "dbo.AuditDomainConceptInstances");
            DropForeignKey("dbo.AuditConnections", "Chi_AuditConnectionId", "dbo.AuditConnections");
            DropForeignKey("dbo.AuditConnections", "LeftDci_AuditDomainConceptInstanceId", "dbo.AuditDomainConceptInstances");
            DropIndex("dbo.AuditConnections", new[] { "RightDci_AuditDomainConceptInstanceId" });
            DropIndex("dbo.AuditConnections", new[] { "Chi_AuditConnectionId" });
            DropIndex("dbo.AuditConnections", new[] { "LeftDci_AuditDomainConceptInstanceId" });
            DropColumn("dbo.AuditConnections", "RightDci_AuditDomainConceptInstanceId");
            DropColumn("dbo.AuditConnections", "Chi_AuditConnectionId");
            DropColumn("dbo.AuditConnections", "LeftDci_AuditDomainConceptInstanceId");
            DropColumn("dbo.AuditConnections", "RightDciId");
            DropColumn("dbo.AuditConnections", "LeftDciId");
            DropColumn("dbo.AuditConnections", "CdId");
            DropColumn("dbo.AuditConnections", "ConId");
            DropColumn("dbo.AuditConnections", "Deleted");
            DropColumn("dbo.AuditConnections", "Aorder");
            DropColumn("dbo.AuditConnections", "ChiId");
            DropColumn("dbo.AuditConnections", "ParId");
        }
    }
}
