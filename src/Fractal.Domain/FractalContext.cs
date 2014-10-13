using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Fractal.Domain
{
    public class FractalContext : DbContext
    {
        public DbSet<DomainConcept> Dcs { get; set; }
        public DbSet<DomainConceptField> Dcfs { get; set; }
        public DbSet<DomainConceptInstance> Dcis { get; set; }
        public DbSet<DomainConceptInstanceFieldValue> Dcifs { get; set; }
        public DbSet<ConnectionDescription> Cds { get; set; }
        public DbSet<ConnectionDescriptionParameter> Cdps { get; set; }

        public DbSet<Audit> Audits { get; set; }
        public DbSet<AuditDomainConcept> ADcs { get; set; }
        public DbSet<AuditDomainConceptField> ADcfs { get; set; }
        public DbSet<AuditDomainConceptInstance> ADcis { get; set; }
        public DbSet<AuditDomainConceptInstanceFieldValue> ADcifs { get; set; }
        public DbSet<AuditConnectionDescription> ACds { get; set; }
        public DbSet<AuditConnectionDescriptionParameter> ACdps { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DomainConcept>().HasMany(dc => dc.Fields);
            modelBuilder.Entity<DomainConcept>().HasMany(dc => dc.AConnectionDescriptions);
            modelBuilder.Entity<DomainConcept>().HasMany(dc => dc.BConnectionDescriptions);

            modelBuilder.Entity<DomainConceptInstance>().HasMany(dci => dci.Fields);

            modelBuilder.Entity<Audit>().HasOptional(a => a.Parent).WithOptionalPrincipal(p => p.Child);
            modelBuilder.Entity<Audit>().HasOptional(a => a.Child).WithOptionalDependent(c=>c.Parent);
            modelBuilder.Entity<Audit>().HasMany(a => a.AuditDomainConcepts);

            modelBuilder.Entity<AuditDomainConcept>().HasRequired(a => a.Audit);
            modelBuilder.Entity<AuditDomainConcept>().HasOptional(a => a.Parent).WithOptionalPrincipal(p => p.Child);
            modelBuilder.Entity<AuditDomainConcept>().HasOptional(a => a.Child).WithOptionalDependent(c=>c.Parent);
            modelBuilder.Entity<AuditDomainConcept>().HasMany(a => a.AuditDomainConceptFields);
            modelBuilder.Entity<AuditDomainConcept>().HasMany(a => a.AuditAConnectionDescriptions);
            modelBuilder.Entity<AuditDomainConcept>().HasMany(a => a.AuditBConnectionDescriptions);


            modelBuilder.Entity<AuditDomainConceptField>().HasRequired(a => a.ADc);

            modelBuilder.Entity<AuditDomainConceptInstance>().HasRequired(a => a.Audit);
            modelBuilder.Entity<AuditDomainConceptInstance>().HasOptional(a => a.Parent).WithOptionalPrincipal(p => p.Child);
            modelBuilder.Entity<AuditDomainConceptInstance>().HasOptional(a => a.Child).WithOptionalDependent(c => c.Parent);
            modelBuilder.Entity<AuditDomainConceptInstance>().HasMany(a => a.AuditDomainConceptInstanceFieldValues);

            modelBuilder.Entity<AuditDomainConceptInstanceFieldValue>().HasRequired(a => a.ADci);


            modelBuilder.Entity<AuditConnectionDescription>().HasRequired(a => a.Audit);
            modelBuilder.Entity<AuditConnectionDescription>().HasOptional(a => a.Parent).WithOptionalPrincipal(p => p.Child);
            modelBuilder.Entity<AuditConnectionDescription>().HasOptional(a => a.Child).WithOptionalDependent(c => c.Parent);
            modelBuilder.Entity<AuditConnectionDescription>().HasMany(a => a.AuditConnectionDescriptionParameters);

            modelBuilder.Entity<AuditConnectionDescription>().HasRequired(a => a.DcA);
            modelBuilder.Entity<AuditConnectionDescription>().HasRequired(a => a.DcB);

        }
    }
}