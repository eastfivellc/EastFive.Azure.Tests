using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EastFive.Extensions;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using EastFive;
using EastFive.Collections.Generic;
using EastFive.Api;
using EastFive.Azure.Persistence.AzureStorageTables;
using EastFive.Linq;
using EastFive.Linq.Async;
using EastFive.Persistence;
using EastFive.Persistence.Azure.StorageTables;
using EastFive.Azure.Tests.Persistence.Models;

namespace EastFive.Azure.Tests.Persistence
{
    
    [TestClass]
    public class TestsPersistenceLookups
    {
        [TestInitialize]
        public void Initialize()
        {
            EastFive.Azure.Tests.Utilities.Configuration.Construct();
        }

        [TestMethod]
        public async Task LookupsWork()
        {
            var lookupModel = LookupModel.GetModel();
            Assert.IsTrue(await lookupModel.StorageCreateAsync(
                (resourceIdCreated) => true,
                () =>
                {
                    return false;
                }));

            #region Test String

            var itemString = await lookupModel.stringLookup
                .StorageGetBy(
                    (LookupModel lm) => lm.stringLookup)
                .FirstAsync();
            Assert.AreEqual(itemString.id, lookupModel.id);

            #endregion

            #region Test Boolean

            Assert.IsTrue(await lookupModel.toggle
                .StorageGetBy(
                    (LookupModel lm) => lm.toggle)
                .ContainsAsync(
                    lm => lm.id == lookupModel.id));

            #endregion
        }

        [TestMethod]
        public async Task DoesDeleteDetectModifications()
        {
            var idValue = Guid.NewGuid();
            var model = await CreateLookupById(idValue);

            var updatedModel = await model.resourceRef.StorageUpdateAsync(
                async (otherModel, saveAsync) =>
                {
                    otherModel.idLookup = Guid.NewGuid().AsRef<LookupModel>();
                    await saveAsync(otherModel);
                    return otherModel;
                });

            bool hitModified = false;
            Assert.IsTrue(await model.StorageDeleteAsync(
                onDeleted: () => true,
                onModified: async (otherUpdatedModel, deleteAnyway) =>
                {
                    Assert.AreEqual(updatedModel.idLookup.id, otherUpdatedModel.idLookup.id);
                    await deleteAnyway();
                    hitModified = true;
                    return true;
                },
                onAlreadyDeleted: () => false));
            Assert.IsTrue(hitModified);

            Assert.IsTrue(await model.resourceRef.StorageGetAsync(
                m => false,
                () => true));
        }

        [TestMethod]
        public async Task AsyncLookupsByIdWorks()
        {
            var idValue = Guid.NewGuid();
            var count = 100;
            var totalCreated = await CreateAsync(count);
            var total = await idValue
                .AsRef<LookupModel>()
                .StorageGetByIdProperty(
                    (LookupByIdModel lookup) => lookup.idLookup)
                .CountAsync();
            Assert.AreEqual(totalCreated, total);

            async Task<int> CreateAsync(int iterations)
            {
                var rand = new Random();
                var array = await Enumerable.Range(0, iterations)
                    .Select(
                        async index =>
                        {
                            var createdModel = await CreateLookupById(idValue);
                            return (index, createdModel);
                        })
                    .AsyncEnumerable()
                    .Select(
                        async indexAndModel =>
                        {
                            var (index, model) = indexAndModel;
                            if (rand.Next() % 3 == 1)
                            {
                                await DeleteLookupById(model);
                                return (false, model);
                            }
                            return (true, model);
                        })
                    .Await(readAhead: count)
                    .SelectWhere()
                    .ToArrayAsync();
                return array.Length;
            }
        }

        private static async Task<LookupByIdModel> CreateLookupById(Guid idValue)
        {
            var idLookup = LookupByIdModel.GetModel(idValue);
            var entityFromDb = await idLookup.StorageCreateAsync(
                (discard) => discard.Entity);
            return entityFromDb;
        }

        private static async Task DeleteLookupById(LookupByIdModel model)
        {
            Assert.IsTrue(await model.StorageDeleteAsync(
                () => true));
        }

        private static async Task<LookupByIdModel> CreateAndDeleteLookupById(Guid idValue)
        {
            var idLookup = await CreateLookupById(idValue);
            await DeleteLookupById(idLookup);
            return idLookup;
        }
    }
}