using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Fractal.Domain
{
    public class FractalContext : DbContext
    {
        public FractalContext()
        {
            this.Configuration.LazyLoadingEnabled = false; 
        }

        public DbSet<DomainConcept> Dcs { get; set; }
        public DbSet<DomainConceptField> Dcfs { get; set; }
        public DbSet<DomainConceptInstance> Dcis { get; set; }
        public DbSet<DomainConceptInstanceFieldValue> Dcifs { get; set; }
        public DbSet<ConnectionDescription> Cds { get; set; }
        public DbSet<ConnectionDescriptionParameter> Cdps { get; set; }
        public DbSet<Connection> Cons { get; set; }

        public DbSet<Audit> Audits { get; set; }
        public DbSet<AuditDomainConcept> ADcs { get; set; }
        public DbSet<AuditDomainConceptField> ADcfs { get; set; }
        public DbSet<AuditDomainConceptInstance> ADcis { get; set; }
        public DbSet<AuditDomainConceptInstanceFieldValue> ADcifs { get; set; }
        public DbSet<AuditConnectionDescription> ACds { get; set; }
        public DbSet<AuditConnectionDescriptionParameter> ACdps { get; set; }
        public DbSet<AuditConnection> ACons { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //Domain
            modelBuilder.Entity<DomainConcept>().HasMany(dc => dc.Dcfs);
            modelBuilder.Entity<DomainConcept>().HasMany(dc => dc.LeftCds);
            modelBuilder.Entity<DomainConcept>().HasMany(dc => dc.RightCds);

            modelBuilder.Entity<ConnectionDescription>().HasRequired(cd => cd.LeftDc);
            modelBuilder.Entity<ConnectionDescription>().HasRequired(cd => cd.RightDc);
            modelBuilder.Entity<ConnectionDescription>().HasMany(cd => cd.Cdps);

            modelBuilder.Entity<DomainConceptInstance>().HasMany(dci => dci.Dcifs);
            modelBuilder.Entity<DomainConceptInstance>().HasMany(dc => dc.LeftCons);
            modelBuilder.Entity<DomainConceptInstance>().HasMany(dc => dc.RightCons);

            modelBuilder.Entity<Connection>().HasRequired(dc => dc.Cd);
            modelBuilder.Entity<Connection>().HasRequired(dc => dc.LeftDci);
            modelBuilder.Entity<Connection>().HasRequired(dc => dc.RightDci);

            //Audit
            modelBuilder.Entity<Audit>().HasOptional(a => a.Par).WithOptionalPrincipal(p => p.Chi);
            modelBuilder.Entity<Audit>().HasOptional(a => a.Chi).WithOptionalDependent(c=>c.Par);
            modelBuilder.Entity<Audit>().HasMany(a => a.ADcs);
            modelBuilder.Entity<Audit>().HasMany(a => a.ADcis);
            modelBuilder.Entity<Audit>().HasMany(a => a.ACds);

            modelBuilder.Entity<AuditDomainConcept>().HasRequired(a => a.Aud);
            modelBuilder.Entity<AuditDomainConcept>().HasOptional(a => a.Par).WithOptionalPrincipal(p => p.Chi);
            modelBuilder.Entity<AuditDomainConcept>().HasOptional(a => a.Chi).WithOptionalDependent(c=>c.Par);
            modelBuilder.Entity<AuditDomainConcept>().HasMany(a => a.ADcfs);
            modelBuilder.Entity<AuditDomainConcept>().HasMany(a => a.ALeftCds);
            modelBuilder.Entity<AuditDomainConcept>().HasMany(a => a.ARightCds);

            modelBuilder.Entity<AuditDomainConceptField>().HasRequired(a => a.Adc);

            modelBuilder.Entity<AuditDomainConceptInstance>().HasRequired(a => a.Aud);
            modelBuilder.Entity<AuditDomainConceptInstance>().HasOptional(a => a.Par).WithOptionalPrincipal(p => p.Chi);
            modelBuilder.Entity<AuditDomainConceptInstance>().HasOptional(a => a.Chi).WithOptionalDependent(c => c.Par);
            modelBuilder.Entity<AuditDomainConceptInstance>().HasMany(a => a.ADcfvs);

            modelBuilder.Entity<AuditDomainConceptInstanceFieldValue>().HasRequired(a => a.ADci);

            modelBuilder.Entity<AuditConnectionDescription>().HasRequired(a => a.Aud);
            modelBuilder.Entity<AuditConnectionDescription>().HasOptional(a => a.Par).WithOptionalPrincipal(p => p.Chi);
            modelBuilder.Entity<AuditConnectionDescription>().HasOptional(a => a.Chi).WithOptionalDependent(c => c.Par);
            modelBuilder.Entity<AuditConnectionDescription>().HasMany(a => a.ACdps);
            modelBuilder.Entity<AuditConnectionDescription>().HasRequired(a => a.LeftDc);
            modelBuilder.Entity<AuditConnectionDescription>().HasRequired(a => a.RightDc);

            modelBuilder.Entity<AuditConnection>().HasRequired(a => a.Aud);
            modelBuilder.Entity<AuditConnection>().HasOptional(a => a.Par).WithOptionalPrincipal(p => p.Chi);
            modelBuilder.Entity<AuditConnection>().HasOptional(a => a.Chi).WithOptionalDependent(c => c.Par);
            modelBuilder.Entity<AuditConnection>().HasRequired(a => a.LeftDci);
            modelBuilder.Entity<AuditConnection>().HasRequired(a => a.RightDci);

        }
    }
}