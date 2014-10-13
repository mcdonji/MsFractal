namespace Fractal.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConnectionDescription : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditConnectionDescriptionParameters",
                c => new
                    {
                        AuditConnectionDescriptionParameterId = c.String(nullable: false, maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                        ConnectionDescriptionParameterId = c.String(),
                        Code = c.String(),
                        Description = c.String(),
                        FunctionCode = c.String(),
                        AuditConnectionDescription_AuditConnectionDescriptionId = c.String(maxLength: 128),
                        AuditConnectionDescription_AuditConnectionDescriptionId1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditConnectionDescriptionParameterId)
                .ForeignKey("dbo.AuditConnectionDescriptions", t => t.AuditConnectionDescription_AuditConnectionDescriptionId)
                .Index(t => t.AuditConnectionDescription_AuditConnectionDescriptionId)
                .Index(t => t.AuditConnectionDescription_AuditConnectionDescriptionId1);
            
            CreateTable(
                "dbo.AuditConnectionDescriptions",
                c => new
                    {
                        AuditConnectionDescriptionId = c.String(nullable: false, maxLength: 128),
                        Aorder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CdId = c.String(),
                        ConnectionName = c.String(),
                        Reciprical = c.Boolean(nullable: false),
                        Cardinality = c.Int(nullable: false),
                        Directed = c.Boolean(nullable: false),
                        Required = c.Boolean(nullable: false),
                        AuditDomainConcept_AuditDomainConceptId = c.String(maxLength: 128),
                        AuditDomainConcept_AuditDomainConceptId1 = c.String(maxLength: 128),
                        Audit_Id = c.String(nullable: false, maxLength: 128),
                        DcA_AuditDomainConceptId = c.String(nullable: false, maxLength: 128),
                        DcB_AuditDomainConceptId = c.String(nullable: false, maxLength: 128),
                        Child_AuditConnectionDescriptionId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditConnectionDescriptionId)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.AuditDomainConcept_AuditDomainConceptId)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.AuditDomainConcept_AuditDomainConceptId1)
                .ForeignKey("dbo.Audits", t => t.Audit_Id, cascadeDelete: false)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.DcA_AuditDomainConceptId, cascadeDelete: false)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.DcB_AuditDomainConceptId, cascadeDelete: false)
                .ForeignKey("dbo.AuditConnectionDescriptions", t => t.Child_AuditConnectionDescriptionId)
                .Index(t => t.AuditDomainConcept_AuditDomainConceptId)
                .Index(t => t.AuditDomainConcept_AuditDomainConceptId1)
                .Index(t => t.Audit_Id)
                .Index(t => t.DcA_AuditDomainConceptId)
                .Index(t => t.DcB_AuditDomainConceptId)
                .Index(t => t.Child_AuditConnectionDescriptionId);
            
            CreateTable(
                "dbo.ConnectionDescriptionParameters",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Code = c.String(),
                        Description = c.String(),
                        FunctionCode = c.String(),
                        ConnectionDescription_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ConnectionDescriptions", t => t.ConnectionDescription_Id)
                .Index(t => t.ConnectionDescription_Id);
            
            CreateTable(
                "dbo.ConnectionDescriptions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ConnectionName = c.String(),
                        Reciprical = c.Boolean(nullable: false),
                        Cardinality = c.Int(nullable: false),
                        Directed = c.Boolean(nullable: false),
                        Required = c.Boolean(nullable: false),
                        DomainConcept_Id = c.String(maxLength: 128),
                        DomainConcept_Id1 = c.String(maxLength: 128),
                        DcA_Id = c.String(maxLength: 128),
                        DcB_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DomainConcepts", t => t.DomainConcept_Id)
                .ForeignKey("dbo.DomainConcepts", t => t.DomainConcept_Id1)
                .ForeignKey("dbo.DomainConcepts", t => t.DcA_Id)
                .ForeignKey("dbo.DomainConcepts", t => t.DcB_Id)
                .Index(t => t.DomainConcept_Id)
                .Index(t => t.DomainConcept_Id1)
                .Index(t => t.DcA_Id)
                .Index(t => t.DcB_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConnectionDescriptions", "DcB_Id", "dbo.DomainConcepts");
            DropForeignKey("dbo.ConnectionDescriptions", "DcA_Id", "dbo.DomainConcepts");
            DropForeignKey("dbo.ConnectionDescriptions", "DomainConcept_Id1", "dbo.DomainConcepts");
            DropForeignKey("dbo.ConnectionDescriptions", "DomainConcept_Id", "dbo.DomainConcepts");
            DropForeignKey("dbo.ConnectionDescriptionParameters", "ConnectionDescription_Id", "dbo.ConnectionDescriptions");
            DropForeignKey("dbo.AuditConnectionDescriptions", "Child_AuditConnectionDescriptionId", "dbo.AuditConnectionDescriptions");
            DropForeignKey("dbo.AuditConnectionDescriptions", "DcB_AuditDomainConceptId", "dbo.AuditDomainConcepts");
            DropForeignKey("dbo.AuditConnectionDescriptions", "DcA_AuditDomainConceptId", "dbo.AuditDomainConcepts");
            DropForeignKey("dbo.AuditConnectionDescriptionParameters", "AuditConnectionDescription_AuditConnectionDescriptionId", "dbo.AuditConnectionDescriptions");
            DropForeignKey("dbo.AuditConnectionDescriptions", "Audit_Id", "dbo.Audits");
            DropForeignKey("dbo.AuditConnectionDescriptions", "AuditDomainConcept_AuditDomainConceptId1", "dbo.AuditDomainConcepts");
            DropForeignKey("dbo.AuditConnectionDescriptions", "AuditDomainConcept_AuditDomainConceptId", "dbo.AuditDomainConcepts");
            DropIndex("dbo.ConnectionDescriptions", new[] { "DcB_Id" });
            DropIndex("dbo.ConnectionDescriptions", new[] { "DcA_Id" });
            DropIndex("dbo.ConnectionDescriptions", new[] { "DomainConcept_Id1" });
            DropIndex("dbo.ConnectionDescriptions", new[] { "DomainConcept_Id" });
            DropIndex("dbo.ConnectionDescriptionParameters", new[] { "ConnectionDescription_Id" });
            DropIndex("dbo.AuditConnectionDescriptions", new[] { "Child_AuditConnectionDescriptionId" });
            DropIndex("dbo.AuditConnectionDescriptions", new[] { "DcB_AuditDomainConceptId" });
            DropIndex("dbo.AuditConnectionDescriptions", new[] { "DcA_AuditDomainConceptId" });
            DropIndex("dbo.AuditConnectionDescriptions", new[] { "Audit_Id" });
            DropIndex("dbo.AuditConnectionDescriptions", new[] { "AuditDomainConcept_AuditDomainConceptId1" });
            DropIndex("dbo.AuditConnectionDescriptions", new[] { "AuditDomainConcept_AuditDomainConceptId" });
            DropIndex("dbo.AuditConnectionDescriptionParameters", new[] { "AuditConnectionDescription_AuditConnectionDescriptionId1" });
            DropIndex("dbo.AuditConnectionDescriptionParameters", new[] { "AuditConnectionDescription_AuditConnectionDescriptionId" });
            DropTable("dbo.ConnectionDescriptions");
            DropTable("dbo.ConnectionDescriptionParameters");
            DropTable("dbo.AuditConnectionDescriptions");
            DropTable("dbo.AuditConnectionDescriptionParameters");
        }
    }
}
