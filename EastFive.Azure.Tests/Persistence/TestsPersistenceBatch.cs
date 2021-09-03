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
using System.Threading;

namespace EastFive.Azure.Tests.Persistence
{
    
    [TestClass]
    public class TestsPersistenceBatch
    {
        [TestInitialize]
        public void Initialize()
        {
            EastFive.Azure.Tests.Utilities.Configuration.Construct();
        }

        [TestMethod]
        public async Task BatchSavesLookups()
        {
            var totalResources = 100;
            var totalLookups = 30;
            var models = await CreateLookups(totalResources, totalLookups)
                .StorageCreateOrReplaceBatch(
                    (model, tr) => model)
                .ToArrayAsync();

            await ValidateAsync(models, totalResources);

            var rand = new Random();
            var modelsDeleted = await models
                .Where(model => rand.Next(3) == 0)
                .Select(model => model.AsTask())
                .AsyncEnumerable()
                .StorageDeleteBatch(
                    result =>
                    {
                        var tableEntity = (IAzureStorageTableEntity<LookupByIdModel>)result.Result;
                        Guid.TryParse(tableEntity.RowKey, out Guid resId);
                        return resId;
                    })
                .ToArrayAsync();

            var modelsRemaining = models
                .Where(model => !modelsDeleted.Contains(model.id))
                .ToArray();

            await ValidateAsync(modelsRemaining,
                totalResources - modelsDeleted.Length);


            async Task ValidateAsync(LookupByIdModel[] setToCount, int resourcesCount)
            {
                var lookups = setToCount
                    .Select(model => model.idLookup.id)
                    .Distinct()
                    .ToArray();

                // Note, not all lookups in the creation set are guarenteed to be used.

                var lookedUpResourceTotal = await lookups
                    .Select(
                        x => x
                            .AsRef<LookupModel>()
                            .StorageGetIdsBy((LookupByIdModel m) => m.idLookup)
                            .CountAsync())
                    .AsyncEnumerable()
                    .SumAsync();

                Assert.AreEqual(resourcesCount, lookedUpResourceTotal);
            }
        }

        [TestMethod]
        public async Task BatchPerformance()
        {
            var totalResources = 1000;
            var totalLookups = 34;

            var stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();

            var models = await CreateLookups(totalResources, totalLookups)
                .StorageCreateOrReplaceBatch(
                    (model, tr) => model)
                .ToArrayAsync();

            stopwatch.Stop();
            Console.WriteLine($"Batch Create: {stopwatch.Elapsed.TotalMilliseconds}ms");

            stopwatch.Restart();

            var modelsTraditional = await CreateLookups(totalResources, totalLookups)
                .Select(model => model.StorageCreateAsync(
                    tr => tr.Entity))
                .Await()
                .ToArrayAsync();

            stopwatch.Stop();
            Console.WriteLine($"Traditional Create: {stopwatch.Elapsed.TotalMilliseconds}ms");


            stopwatch.Restart();

            await models.StorageDeleteBatch(
                tr => tr)
                .ToArrayAsync();

            stopwatch.Stop();
            Console.WriteLine($"Batch Delete: {stopwatch.Elapsed.TotalMilliseconds}ms");

            stopwatch.Restart();

            await modelsTraditional
                .Select(model => model.StorageDeleteAsync(
                    () => model))
                .AsyncEnumerable()
                .ToArrayAsync();

            stopwatch.Stop();
            Console.WriteLine($"Traditional Delete: {stopwatch.Elapsed.TotalMilliseconds}ms");

        }

        private static IEnumerableAsync<LookupByIdModel> CreateLookups(int totalResources, int totalLookups)
        {
            var rand = new Random();
            var lookups = Enumerable
                .Range(0, totalLookups)
                .Select(index => Guid.NewGuid())
                .ToArray();

            return Enumerable
                .Range(0, totalResources)
                .Select(
                    index =>
                    {
                        return Task.Run<LookupByIdModel>(
                            () =>
                            {
                                var idValue = GetIdValue();
                                Thread.Sleep(0); //  (int)(rand.NextDouble() * 100));
                                return LookupByIdModel.GetModel(idValue);
                            });
                    })
                .AsyncEnumerable();

            Guid GetIdValue()
            {
                var index = rand.Next(0, totalLookups);
                return lookups[index];
            }
        }

    }
}