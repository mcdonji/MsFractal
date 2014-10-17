namespace Fractal.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditConnectionDescriptionParameters",
                c => new
                    {
                        AuditConnectionDescriptionParameterId = c.String(nullable: false, maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                        ACdId = c.String(),
                        ConnectionDescriptionParameterId = c.String(),
                        Code = c.String(),
                        Description = c.String(),
                        FunctionCode = c.String(),
                        ACd_AuditConnectionDescriptionId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditConnectionDescriptionParameterId)
                .ForeignKey("dbo.AuditConnectionDescriptions", t => t.ACd_AuditConnectionDescriptionId)
                .Index(t => t.ACd_AuditConnectionDescriptionId);
            
            CreateTable(
                "dbo.AuditConnectionDescriptions",
                c => new
                    {
                        AuditConnectionDescriptionId = c.String(nullable: false, maxLength: 128),
                        AudId = c.String(nullable: false, maxLength: 128),
                        ParId = c.String(),
                        ChiId = c.String(),
                        Aorder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        CdId = c.String(),
                        LeftDcId = c.String(),
                        RightDcId = c.String(),
                        ConnectionName = c.String(),
                        Reciprical = c.Boolean(nullable: false),
                        Cardinality = c.Int(nullable: false),
                        Directed = c.Boolean(nullable: false),
                        Required = c.Boolean(nullable: false),
                        AuditDomainConcept_AuditDomainConceptId = c.String(maxLength: 128),
                        AuditDomainConcept_AuditDomainConceptId1 = c.String(maxLength: 128),
                        LeftDc_AuditDomainConceptId = c.String(nullable: false, maxLength: 128),
                        Chi_AuditConnectionDescriptionId = c.String(maxLength: 128),
                        RightDc_AuditDomainConceptId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditConnectionDescriptionId)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.AuditDomainConcept_AuditDomainConceptId)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.AuditDomainConcept_AuditDomainConceptId1)
                .ForeignKey("dbo.Audits", t => t.AudId)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.LeftDc_AuditDomainConceptId)
                .ForeignKey("dbo.AuditConnectionDescriptions", t => t.Chi_AuditConnectionDescriptionId)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.RightDc_AuditDomainConceptId)
                .Index(t => t.AudId)
                .Index(t => t.AuditDomainConcept_AuditDomainConceptId)
                .Index(t => t.AuditDomainConcept_AuditDomainConceptId1)
                .Index(t => t.LeftDc_AuditDomainConceptId)
                .Index(t => t.Chi_AuditConnectionDescriptionId)
                .Index(t => t.RightDc_AuditDomainConceptId);
            
            CreateTable(
                "dbo.Audits",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ParId = c.String(),
                        ChiId = c.String(),
                        Aorder = c.Int(nullable: false),
                        Chi_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Audits", t => t.Chi_Id)
                .Index(t => t.Chi_Id);
            
            CreateTable(
                "dbo.AuditDomainConceptInstances",
                c => new
                    {
                        AuditDomainConceptInstanceId = c.String(nullable: false, maxLength: 128),
                        AudId = c.String(nullable: false, maxLength: 128),
                        ParId = c.String(),
                        ChiId = c.String(),
                        Aorder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        DomainConceptInstanceId = c.String(),
                        DomainConceptId = c.String(),
                        DomainConceptName = c.String(),
                        Chi_AuditDomainConceptInstanceId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditDomainConceptInstanceId)
                .ForeignKey("dbo.AuditDomainConceptInstances", t => t.Chi_AuditDomainConceptInstanceId)
                .ForeignKey("dbo.Audits", t => t.AudId)
                .Index(t => t.AudId)
                .Index(t => t.Chi_AuditDomainConceptInstanceId);
            
            CreateTable(
                "dbo.AuditDomainConceptInstanceFieldValues",
                c => new
                    {
                        AuditDomainConceptInstanceFieldValueId = c.String(nullable: false, maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                        DomainConceptInstanceFieldValueId = c.String(),
                        ADciId = c.String(),
                        DomainConceptId = c.String(),
                        DomainConceptName = c.String(),
                        DomainConceptFieldId = c.String(),
                        DomainConceptFieldName = c.String(),
                        Forder = c.Int(nullable: false),
                        FieldValue = c.String(),
                        ADci_AuditDomainConceptInstanceId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditDomainConceptInstanceFieldValueId)
                .ForeignKey("dbo.AuditDomainConceptInstances", t => t.ADci_AuditDomainConceptInstanceId)
                .Index(t => t.ADci_AuditDomainConceptInstanceId);
            
            CreateTable(
                "dbo.AuditDomainConcepts",
                c => new
                    {
                        AuditDomainConceptId = c.String(nullable: false, maxLength: 128),
                        AudId = c.String(nullable: false, maxLength: 128),
                        ParId = c.String(),
                        ChiId = c.String(),
                        Aorder = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        DomainConceptId = c.String(),
                        Name = c.String(),
                        Chi_AuditDomainConceptId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditDomainConceptId)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.Chi_AuditDomainConceptId)
                .ForeignKey("dbo.Audits", t => t.AudId)
                .Index(t => t.AudId)
                .Index(t => t.Chi_AuditDomainConceptId);
            
            CreateTable(
                "dbo.AuditDomainConceptFields",
                c => new
                    {
                        AuditDomainConceptFieldId = c.String(nullable: false, maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                        DcfId = c.String(),
                        ADcId = c.String(),
                        Forder = c.Int(nullable: false),
                        FieldName = c.String(),
                        DefaultValue = c.String(),
                        Adc_AuditDomainConceptId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditDomainConceptFieldId)
                .ForeignKey("dbo.AuditDomainConcepts", t => t.Adc_AuditDomainConceptId)
                .Index(t => t.Adc_AuditDomainConceptId);
            
            CreateTable(
                "dbo.AuditConnections",
                c => new
                    {
                        AuditConnectionId = c.String(nullable: false, maxLength: 128),
                        AudId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.AuditConnectionId)
                .ForeignKey("dbo.Audits", t => t.AudId)
                .Index(t => t.AudId);
            
            CreateTable(
                "dbo.ConnectionDescriptionParameters",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Code = c.String(),
                        Description = c.String(),
                        FunctionCode = c.String(),
                        CdId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ConnectionDescriptions", t => t.CdId)
                .Index(t => t.CdId);
            
            CreateTable(
                "dbo.ConnectionDescriptions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LeftDcId = c.String(nullable: false, maxLength: 128),
                        RightDcId = c.String(nullable: false, maxLength: 128),
                        ConnectionName = c.String(),
                        Reciprical = c.Boolean(nullable: false),
                        Cardinality = c.Int(nullable: false),
                        Directed = c.Boolean(nullable: false),
                        Required = c.Boolean(nullable: false),
                        DomainConcept_Id = c.String(maxLength: 128),
                        DomainConcept_Id1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DomainConcepts", t => t.DomainConcept_Id)
                .ForeignKey("dbo.DomainConcepts", t => t.DomainConcept_Id1)
                .ForeignKey("dbo.DomainConcepts", t => t.LeftDcId)
                .ForeignKey("dbo.DomainConcepts", t => t.RightDcId)
                .Index(t => t.LeftDcId)
                .Index(t => t.RightDcId)
                .Index(t => t.DomainConcept_Id)
                .Index(t => t.DomainConcept_Id1);
            
            CreateTable(
                "dbo.DomainConcepts",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DomainConceptFields",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DcId = c.String(maxLength: 128),
                        Forder = c.Int(nullable: false),
                        FieldName = c.String(),
                        DefaultValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DomainConcepts", t => t.DcId)
                .Index(t => t.DcId);
            
            CreateTable(
                "dbo.Connections",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CdId = c.String(nullable: false, maxLength: 128),
                        LeftDciId = c.String(nullable: false, maxLength: 128),
                        RightDciId = c.String(nullable: false, maxLength: 128),
                        DomainConceptInstance_Id = c.String(maxLength: 128),
                        DomainConceptInstance_Id1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ConnectionDescriptions", t => t.CdId)
                .ForeignKey("dbo.DomainConceptInstances", t => t.DomainConceptInstance_Id)
                .ForeignKey("dbo.DomainConceptInstances", t => t.DomainConceptInstance_Id1)
                .ForeignKey("dbo.DomainConceptInstances", t => t.LeftDciId)
                .ForeignKey("dbo.DomainConceptInstances", t => t.RightDciId)
                .Index(t => t.CdId)
                .Index(t => t.LeftDciId)
                .Index(t => t.RightDciId)
                .Index(t => t.DomainConceptInstance_Id)
                .Index(t => t.DomainConceptInstance_Id1);
            
            CreateTable(
                "dbo.DomainConceptInstances",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DomainConceptId = c.String(),
                        DomainConceptName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DomainConceptInstanceFieldValues",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DciId = c.String(maxLength: 128),
                        DomainConceptId = c.String(),
                        DomainConceptName = c.String(),
                        DomainConceptFieldId = c.String(),
                        DomainConceptFieldName = c.String(),
                        Forder = c.Int(nullable: false),
                        FieldValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DomainConceptInstances", t => t.DciId)
                .Index(t => t.DciId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Connections", "RightDciId", "dbo.DomainConceptInstances");
            DropForeignKey("dbo.Connections", "LeftDciId", "dbo.DomainConceptInstances");
            DropForeignKey("dbo.Connections", "DomainConceptInstance_Id1", "dbo.DomainConceptInstances");
            DropForeignKey("dbo.Connections", "DomainConceptInstance_Id", "dbo.DomainConceptInstances");
            DropForeignKey("dbo.DomainConceptInstanceFieldValues", "DciId", "dbo.DomainConceptInstances");
            DropForeignKey("dbo.Connections", "CdId", "dbo.ConnectionDescriptions");
            DropForeignKey("dbo.ConnectionDescriptions", "RightDcId", "dbo.DomainConcepts");
            DropForeignKey("dbo.ConnectionDescriptions", "LeftDcId", "dbo.DomainConcepts");
            DropForeignKey("dbo.ConnectionDescriptions", "DomainConcept_Id1", "dbo.DomainConcepts");
            DropForeignKey("dbo.ConnectionDescriptions", "DomainConcept_Id", "dbo.DomainConcepts");
            DropForeignKey("dbo.DomainConceptFields", "DcId", "dbo.DomainConcepts");
            DropForeignKey("dbo.ConnectionDescriptionParameters", "CdId", "dbo.ConnectionDescriptions");
            DropForeignKey("dbo.AuditConnections", "AudId", "dbo.Audits");
            DropForeignKey("dbo.AuditConnectionDescriptions", "RightDc_AuditDomainConceptId", "dbo.AuditDomainConcepts");
            DropForeignKey("dbo.AuditConnectionDescriptions", "Chi_AuditConnectionDescriptionId", "dbo.AuditConnectionDescriptions");
            DropForeignKey("dbo.AuditConnectionDescriptions", "LeftDc_AuditDomainConceptId", "dbo.AuditDomainConcepts");
            DropForeignKey("dbo.AuditConnectionDescriptions", "AudId", "dbo.Audits");
            DropForeignKey("dbo.Audits", "Chi_Id", "dbo.Audits");
            DropForeignKey("dbo.AuditDomainConcepts", "AudId", "dbo.Audits");
            DropForeignKey("dbo.AuditDomainConcepts", "Chi_AuditDomainConceptId", "dbo.AuditDomainConcepts");
            DropForeignKey("dbo.AuditConnectionDescriptions", "AuditDomainConcept_AuditDomainConceptId1", "dbo.AuditDomainConcepts");
            DropForeignKey("dbo.AuditConnectionDescriptions", "AuditDomainConcept_AuditDomainConceptId", "dbo.AuditDomainConcepts");
            DropForeignKey("dbo.AuditDomainConceptFields", "Adc_AuditDomainConceptId", "dbo.AuditDomainConcepts");
            DropForeignKey("dbo.AuditDomainConceptInstances", "AudId", "dbo.Audits");
            DropForeignKey("dbo.AuditDomainConceptInstances", "Chi_AuditDomainConceptInstanceId", "dbo.AuditDomainConceptInstances");
            DropForeignKey("dbo.AuditDomainConceptInstanceFieldValues", "ADci_AuditDomainConceptInstanceId", "dbo.AuditDomainConceptInstances");
            DropForeignKey("dbo.AuditConnectionDescriptionParameters", "ACd_AuditConnectionDescriptionId", "dbo.AuditConnectionDescriptions");
            DropIndex("dbo.DomainConceptInstanceFieldValues", new[] { "DciId" });
            DropIndex("dbo.Connections", new[] { "DomainConceptInstance_Id1" });
            DropIndex("dbo.Connections", new[] { "DomainConceptInstance_Id" });
            DropIndex("dbo.Connections", new[] { "RightDciId" });
            DropIndex("dbo.Connections", new[] { "LeftDciId" });
            DropIndex("dbo.Connections", new[] { "CdId" });
            DropIndex("dbo.DomainConceptFields", new[] { "DcId" });
            DropIndex("dbo.ConnectionDescriptions", new[] { "DomainConcept_Id1" });
            DropIndex("dbo.ConnectionDescriptions", new[] { "DomainConcept_Id" });
            DropIndex("dbo.ConnectionDescriptions", new[] { "RightDcId" });
            DropIndex("dbo.ConnectionDescriptions", new[] { "LeftDcId" });
            DropIndex("dbo.ConnectionDescriptionParameters", new[] { "CdId" });
            DropIndex("dbo.AuditConnections", new[] { "AudId" });
            DropIndex("dbo.AuditDomainConceptFields", new[] { "Adc_AuditDomainConceptId" });
            DropIndex("dbo.AuditDomainConcepts", new[] { "Chi_AuditDomainConceptId" });
            DropIndex("dbo.AuditDomainConcepts", new[] { "AudId" });
            DropIndex("dbo.AuditDomainConceptInstanceFieldValues", new[] { "ADci_AuditDomainConceptInstanceId" });
            DropIndex("dbo.AuditDomainConceptInstances", new[] { "Chi_AuditDomainConceptInstanceId" });
            DropIndex("dbo.AuditDomainConceptInstances", new[] { "AudId" });
            DropIndex("dbo.Audits", new[] { "Chi_Id" });
            DropIndex("dbo.AuditConnectionDescriptions", new[] { "RightDc_AuditDomainConceptId" });
            DropIndex("dbo.AuditConnectionDescriptions", new[] { "Chi_AuditConnectionDescriptionId" });
            DropIndex("dbo.AuditConnectionDescriptions", new[] { "LeftDc_AuditDomainConceptId" });
            DropIndex("dbo.AuditConnectionDescriptions", new[] { "AuditDomainConcept_AuditDomainConceptId1" });
            DropIndex("dbo.AuditConnectionDescriptions", new[] { "AuditDomainConcept_AuditDomainConceptId" });
            DropIndex("dbo.AuditConnectionDescriptions", new[] { "AudId" });
            DropIndex("dbo.AuditConnectionDescriptionParameters", new[] { "ACd_AuditConnectionDescriptionId" });
            DropTable("dbo.DomainConceptInstanceFieldValues");
            DropTable("dbo.DomainConceptInstances");
            DropTable("dbo.Connections");
            DropTable("dbo.DomainConceptFields");
            DropTable("dbo.DomainConcepts");
            DropTable("dbo.ConnectionDescriptions");
            DropTable("dbo.ConnectionDescriptionParameters");
            DropTable("dbo.AuditConnections");
            DropTable("dbo.AuditDomainConceptFields");
            DropTable("dbo.AuditDomainConcepts");
            DropTable("dbo.AuditDomainConceptInstanceFieldValues");
            DropTable("dbo.AuditDomainConceptInstances");
            DropTable("dbo.Audits");
            DropTable("dbo.AuditConnectionDescriptions");
            DropTable("dbo.AuditConnectionDescriptionParameters");
        }
    }
}
