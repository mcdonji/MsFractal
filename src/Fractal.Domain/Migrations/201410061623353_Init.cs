namespace Fractal.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditDomainConceptFieldGroups",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AuditDomainConcept_AuditDomainConceptId = c.String(maxLength: 128),
                        Audit_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.AuditDomainConcept_AuditDomainConceptId)
                .ForeignKey("dbo.Audits", t => t.Audit_Id)
                .Index(t => t.AuditDomainConcept_AuditDomainConceptId)
                .Index(t => t.Audit_Id);
            
            CreateTable(
                "dbo.Audits",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        Child_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Audits", t => t.Child_Id)
                .Index(t => t.Child_Id);
            
            CreateTable(
                "dbo.AuditDomainConceptFields",
                c => new
                    {
                        AuditDomainConceptFieldId = c.String(nullable: false, maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                        Id = c.String(nullable: false, maxLength: 128),
                        Forder = c.Int(nullable: false),
                        FieldName = c.String(),
                        DefaultValue = c.String(),
                        ADc_AuditDomainConceptId = c.String(nullable: false, maxLength: 128),
                        Child_AuditDomainConceptFieldId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditDomainConceptFieldId)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.ADc_AuditDomainConceptId, cascadeDelete: false)
                .ForeignKey("dbo.AuditDomainConceptFields", t => t.Child_AuditDomainConceptFieldId)
                .ForeignKey("dbo.Audits", t => t.Id, cascadeDelete: false)
                .ForeignKey("dbo.AuditDomainConceptFieldGroups", t => t.Id, cascadeDelete: false)
                .Index(t => t.Id)
                .Index(t => t.ADc_AuditDomainConceptId)
                .Index(t => t.Child_AuditDomainConceptFieldId);
            
            CreateTable(
                "dbo.AuditDomainConcepts",
                c => new
                    {
                        AuditDomainConceptId = c.String(nullable: false, maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Child_AuditDomainConceptId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditDomainConceptId)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.Child_AuditDomainConceptId)
                .ForeignKey("dbo.Audits", t => t.Id, cascadeDelete: false)
                .Index(t => t.Id)
                .Index(t => t.Child_AuditDomainConceptId);
            
            CreateTable(
                "dbo.DomainConceptFields",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Forder = c.Int(nullable: false),
                        FieldName = c.String(),
                        DefaultValue = c.String(),
                        Dc_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DomainConcepts", t => t.Dc_Id, cascadeDelete: true)
                .Index(t => t.Dc_Id);
            
            CreateTable(
                "dbo.DomainConcepts",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DomainConceptFields", "Dc_Id", "dbo.DomainConcepts");
            DropForeignKey("dbo.AuditDomainConceptFields", "Id", "dbo.AuditDomainConceptFieldGroups");
            DropForeignKey("dbo.AuditDomainConceptFieldGroups", "Audit_Id", "dbo.Audits");
            DropForeignKey("dbo.Audits", "Child_Id", "dbo.Audits");
            DropForeignKey("dbo.AuditDomainConcepts", "Id", "dbo.Audits");
            DropForeignKey("dbo.AuditDomainConceptFields", "Id", "dbo.Audits");
            DropForeignKey("dbo.AuditDomainConceptFields", "Child_AuditDomainConceptFieldId", "dbo.AuditDomainConceptFields");
            DropForeignKey("dbo.AuditDomainConceptFields", "ADc_AuditDomainConceptId", "dbo.AuditDomainConcepts");
            DropForeignKey("dbo.AuditDomainConcepts", "Child_AuditDomainConceptId", "dbo.AuditDomainConcepts");
            DropForeignKey("dbo.AuditDomainConceptFieldGroups", "AuditDomainConcept_AuditDomainConceptId", "dbo.AuditDomainConcepts");
            DropIndex("dbo.DomainConceptFields", new[] { "Dc_Id" });
            DropIndex("dbo.AuditDomainConcepts", new[] { "Child_AuditDomainConceptId" });
            DropIndex("dbo.AuditDomainConcepts", new[] { "Id" });
            DropIndex("dbo.AuditDomainConceptFields", new[] { "Child_AuditDomainConceptFieldId" });
            DropIndex("dbo.AuditDomainConceptFields", new[] { "ADc_AuditDomainConceptId" });
            DropIndex("dbo.AuditDomainConceptFields", new[] { "Id" });
            DropIndex("dbo.Audits", new[] { "Child_Id" });
            DropIndex("dbo.AuditDomainConceptFieldGroups", new[] { "Audit_Id" });
            DropIndex("dbo.AuditDomainConceptFieldGroups", new[] { "AuditDomainConcept_AuditDomainConceptId" });
            DropTable("dbo.DomainConcepts");
            DropTable("dbo.DomainConceptFields");
            DropTable("dbo.AuditDomainConcepts");
            DropTable("dbo.AuditDomainConceptFields");
            DropTable("dbo.Audits");
            DropTable("dbo.AuditDomainConceptFieldGroups");
        }
    }
}
