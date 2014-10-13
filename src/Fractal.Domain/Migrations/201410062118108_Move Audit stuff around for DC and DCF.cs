namespace Fractal.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveAuditstuffaroundforDCandDCF : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AuditDomainConceptFieldGroups", "AuditDomainConcept_AuditDomainConceptId", "dbo.AuditDomainConcepts");
            DropForeignKey("dbo.AuditDomainConceptFields", "Child_AuditDomainConceptFieldId", "dbo.AuditDomainConceptFields");
            DropForeignKey("dbo.AuditDomainConceptFields", "Id", "dbo.Audits");
            DropForeignKey("dbo.AuditDomainConceptFields", "Id", "dbo.AuditDomainConceptFieldGroups");
            DropIndex("dbo.AuditDomainConceptFieldGroups", new[] { "AuditDomainConcept_AuditDomainConceptId" });
            DropIndex("dbo.AuditDomainConceptFields", new[] { "Id" });
            DropIndex("dbo.AuditDomainConceptFields", new[] { "Child_AuditDomainConceptFieldId" });
            RenameColumn(table: "dbo.AuditDomainConceptFields", name: "Id", newName: "AuditDomainConceptFieldGroup_Id");
            AddColumn("dbo.Audits", "Aorder", c => c.Int(nullable: false));
            AddColumn("dbo.AuditDomainConceptFields", "DcfId", c => c.String());
            AddColumn("dbo.AuditDomainConcepts", "Aorder", c => c.Int(nullable: false));
            AlterColumn("dbo.AuditDomainConceptFields", "AuditDomainConceptFieldGroup_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AuditDomainConceptFields", "AuditDomainConceptFieldGroup_Id");
            AddForeignKey("dbo.AuditDomainConceptFields", "AuditDomainConceptFieldGroup_Id", "dbo.AuditDomainConceptFieldGroups", "Id");
            DropColumn("dbo.AuditDomainConceptFieldGroups", "AuditDomainConcept_AuditDomainConceptId");
            DropColumn("dbo.AuditDomainConceptFields", "Child_AuditDomainConceptFieldId");
            DropColumn("dbo.AuditDomainConcepts", "Order");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AuditDomainConcepts", "Order", c => c.Int(nullable: false));
            AddColumn("dbo.AuditDomainConceptFields", "Child_AuditDomainConceptFieldId", c => c.String(maxLength: 128));
            AddColumn("dbo.AuditDomainConceptFieldGroups", "AuditDomainConcept_AuditDomainConceptId", c => c.String(maxLength: 128));
            DropForeignKey("dbo.AuditDomainConceptFields", "AuditDomainConceptFieldGroup_Id", "dbo.AuditDomainConceptFieldGroups");
            DropIndex("dbo.AuditDomainConceptFields", new[] { "AuditDomainConceptFieldGroup_Id" });
            AlterColumn("dbo.AuditDomainConceptFields", "AuditDomainConceptFieldGroup_Id", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.AuditDomainConcepts", "Aorder");
            DropColumn("dbo.AuditDomainConceptFields", "DcfId");
            DropColumn("dbo.Audits", "Aorder");
            RenameColumn(table: "dbo.AuditDomainConceptFields", name: "AuditDomainConceptFieldGroup_Id", newName: "Id");
            CreateIndex("dbo.AuditDomainConceptFields", "Child_AuditDomainConceptFieldId");
            CreateIndex("dbo.AuditDomainConceptFields", "Id");
            CreateIndex("dbo.AuditDomainConceptFieldGroups", "AuditDomainConcept_AuditDomainConceptId");
            AddForeignKey("dbo.AuditDomainConceptFields", "Id", "dbo.AuditDomainConceptFieldGroups", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AuditDomainConceptFields", "Id", "dbo.Audits", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AuditDomainConceptFields", "Child_AuditDomainConceptFieldId", "dbo.AuditDomainConceptFields", "AuditDomainConceptFieldId");
            AddForeignKey("dbo.AuditDomainConceptFieldGroups", "AuditDomainConcept_AuditDomainConceptId", "dbo.AuditDomainConcepts", "AuditDomainConceptId");
        }
    }
}
