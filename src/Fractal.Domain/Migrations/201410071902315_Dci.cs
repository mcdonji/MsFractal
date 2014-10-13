namespace Fractal.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dci : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditDomainConceptInstances",
                c => new
                    {
                        AuditDomainConceptInstanceId = c.String(nullable: false, maxLength: 128),
                        Aorder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        DciId = c.String(),
                        DomainConceptId = c.String(),
                        DomainConceptName = c.String(),
                        Audit_Id = c.String(nullable: false, maxLength: 128),
                        Child_AuditDomainConceptInstanceId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditDomainConceptInstanceId)
                .ForeignKey("dbo.Audits", t => t.Audit_Id, cascadeDelete: true)
                .ForeignKey("dbo.AuditDomainConceptInstances", t => t.Child_AuditDomainConceptInstanceId)
                .Index(t => t.Audit_Id)
                .Index(t => t.Child_AuditDomainConceptInstanceId);
            
            CreateTable(
                "dbo.AuditDomainConceptInstanceFieldValues",
                c => new
                    {
                        AuditDomainConceptInstanceFieldValueId = c.String(nullable: false, maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                        DomainConceptInstanceFieldValueId = c.String(),
                        DomainConceptId = c.String(),
                        DomainConceptName = c.String(),
                        DomainConceptFieldId = c.String(),
                        DomainConceptFieldName = c.String(),
                        Forder = c.Int(nullable: false),
                        FieldValue = c.String(),
                        ADci_AuditDomainConceptInstanceId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditDomainConceptInstanceFieldValueId)
                .ForeignKey("dbo.AuditDomainConceptInstances", t => t.ADci_AuditDomainConceptInstanceId, cascadeDelete: true)
                .Index(t => t.ADci_AuditDomainConceptInstanceId);
            
            CreateTable(
                "dbo.DomainConceptInstanceFieldValues",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DomainConceptId = c.String(),
                        DomainConceptName = c.String(),
                        DomainConceptFieldId = c.String(),
                        DomainConceptFieldName = c.String(),
                        Forder = c.Int(nullable: false),
                        FieldValue = c.String(),
                        Dci_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DomainConceptInstances", t => t.Dci_Id, cascadeDelete: true)
                .Index(t => t.Dci_Id);
            
            CreateTable(
                "dbo.DomainConceptInstances",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DomainConceptId = c.String(),
                        DomainConceptName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DomainConceptInstanceFieldValues", "Dci_Id", "dbo.DomainConceptInstances");
            DropForeignKey("dbo.AuditDomainConceptInstances", "Child_AuditDomainConceptInstanceId", "dbo.AuditDomainConceptInstances");
            DropForeignKey("dbo.AuditDomainConceptInstanceFieldValues", "ADci_AuditDomainConceptInstanceId", "dbo.AuditDomainConceptInstances");
            DropForeignKey("dbo.AuditDomainConceptInstances", "Audit_Id", "dbo.Audits");
            DropIndex("dbo.DomainConceptInstanceFieldValues", new[] { "Dci_Id" });
            DropIndex("dbo.AuditDomainConceptInstanceFieldValues", new[] { "ADci_AuditDomainConceptInstanceId" });
            DropIndex("dbo.AuditDomainConceptInstances", new[] { "Child_AuditDomainConceptInstanceId" });
            DropIndex("dbo.AuditDomainConceptInstances", new[] { "Audit_Id" });
            DropTable("dbo.DomainConceptInstances");
            DropTable("dbo.DomainConceptInstanceFieldValues");
            DropTable("dbo.AuditDomainConceptInstanceFieldValues");
            DropTable("dbo.AuditDomainConceptInstances");
        }
    }
}
