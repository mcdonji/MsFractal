namespace Fractal.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameAuditDcDcId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AuditDomainConcepts", name: "Id", newName: "Audit_Id");
            RenameIndex(table: "dbo.AuditDomainConcepts", name: "IX_Id", newName: "IX_Audit_Id");
            AddColumn("dbo.AuditDomainConcepts", "DcId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuditDomainConcepts", "DcId");
            RenameIndex(table: "dbo.AuditDomainConcepts", name: "IX_Audit_Id", newName: "IX_Id");
            RenameColumn(table: "dbo.AuditDomainConcepts", name: "Audit_Id", newName: "Id");
        }
    }
}
