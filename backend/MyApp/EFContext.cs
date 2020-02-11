namespace MyApp.Database
{
    using Microsoft.EntityFrameworkCore;
    using MyApp.Logic.Entities;
    using Reusable.CRUD.Contract;
    using Reusable.CRUD.Entities;
    using Reusable.CRUD.JsonEntities;
    using ServiceStack.Text;
    using System.IO;

    public class EFContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connString = JsonObject.Parse(File.ReadAllText(@"appsettings.json"))["dbConnectionString"];
            if (connString == "%%CONN_STR%%")
                connString = JsonObject.Parse(File.ReadAllText(@"appsettings.json"))["dbConnectionStringDev"];

            optionsBuilder.UseSqlServer(connString);
        }

        #region App
        ///start:generated:dbsets<<<
        public virtual DbSet<Activity> Activitys { get; set; }
        public virtual DbSet<Approval> Approvals { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<AdvancedSort> AdvancedSorts { get; set; }
        public virtual DbSet<ApplicationTask> ApplicationTasks { get; set; }
        public virtual DbSet<FilterData> FilterDatas { get; set; }
        public virtual DbSet<SortData> SortDatas { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        ///end:generated:dbsets<<<
        #endregion

        #region Reusable
        public virtual DbSet<Catalog> Catalogs { get; set; }
        public virtual DbSet<CatalogDefinition> CatalogDefinitionnitions { get; set; }
        public virtual DbSet<Field> CatalogDefinitionFields { get; set; }
        public virtual DbSet<CatalogFieldValue> CatalogFieldValues { get; set; }
        public virtual DbSet<Revision> Revisions { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entity.ClrType).ToTable(entity.ClrType.Name);
            }

            modelBuilder.Ignore<IEntity>();
            modelBuilder.Ignore<BaseEntity>();
            modelBuilder.Ignore<BaseCatalog>();
            modelBuilder.Ignore<Trackable>();
            modelBuilder.Ignore<BaseDocument>();

            modelBuilder.Ignore<Contact>();
        }
    }
}
