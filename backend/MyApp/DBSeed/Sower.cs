using MyApp.Logic.Entities;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace MyApp
{
    public class Sower
    {
        static public bool Seed(IDbConnectionFactory DbConnectionFactory)
        {
            using var db = DbConnectionFactory.Open();
            using var transaction = db.OpenTransaction();
            try
            {
                #region Catalog Definition
                //Orer is important, less dependent to more dependent.
                var defaultCatalogs = new string[] { };
                var catalogDefinitionSeed = new List<CatalogDefinition>();
                for (int i = 0; i < defaultCatalogs.Length; i++)
                {
                    catalogDefinitionSeed.Add(new CatalogDefinition
                    {
                        Id = i + 1,
                        Name = defaultCatalogs[i]
                    });
                }
                var existentCatalogDefinitions = db.Select<CatalogDefinition>(e => Sql.In(e.Name, defaultCatalogs));
                var missingCatalogDefinitions = catalogDefinitionSeed.Where(e => !existentCatalogDefinitions.Any(c => c.Name == e.Name));
                //Catalog Definition
                foreach (var item in missingCatalogDefinitions)
                {
                    db.Save(item);
                }
                //Catalog Definition Fields
                foreach (var item in missingCatalogDefinitions)
                {
                    // if (item.Name == "Job")
                    // {
                    //     item.Fields = new List<Field>
                    //     {
                    //         new Field
                    //         {
                    //             Id=1,
                    //             FieldName = "Quantity Produced",
                    //             FieldType = "number"
                    //         },
                    //         new Field
                    //         {
                    //             Id = 2,
                    //             FieldName = "Station",
                    //             FieldType = "Relationship: Has One",
                    //             ForeignId = missingCatalogDefinitions.FirstOrDefault(e => e.Name == "Station").Id
                    //         }
                    //     };
                    // }
                }
                foreach (var catalog in missingCatalogDefinitions)
                {
                    db.SaveReferences(catalog, catalog.Fields);
                }
                #endregion

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
