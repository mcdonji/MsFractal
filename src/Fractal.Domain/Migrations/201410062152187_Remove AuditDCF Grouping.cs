namespace Fractal.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAuditDCFGrouping : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AuditDomainConceptFieldGroups", "Audit_Id", "dbo.Audits");
            DropForeignKey("dbo.AuditDomainConceptFields", "AuditDomainConceptFieldGroup_Id", "dbo.AuditDomainConceptFieldGroups");
            DropIndex("dbo.AuditDomainConceptFieldGroups", new[] { "Audit_Id" });
            DropIndex("dbo.AuditDomainConceptFields", new[] { "AuditDomainConceptFieldGroup_Id" });
            DropColumn("dbo.AuditDomainConceptFields", "AuditDomainConceptFieldGroup_Id");
            DropTable("dbo.AuditDomainConceptFieldGroups");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AuditDomainConceptFieldGroups",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Audit_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AuditDomainConceptFields", "AuditDomainConceptFieldGroup_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AuditDomainConceptFields", "AuditDomainConceptFieldGroup_Id");
            CreateIndex("dbo.AuditDomainConceptFieldGroups", "Audit_Id");
            AddForeignKey("dbo.AuditDomainConceptFields", "AuditDomainConceptFieldGroup_Id", "dbo.AuditDomainConceptFieldGroups", "Id");
            AddForeignKey("dbo.AuditDomainConceptFieldGroups", "Audit_Id", "dbo.Audits", "Id");
        }
    }
}
