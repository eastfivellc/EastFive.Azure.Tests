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
    public class TestRateLimits
    {
        [TestInitialize]
        public void Initialize()
        {
            EastFive.Azure.Tests.Utilities.Configuration.Construct();
        }

        [TestMethod]
        public async Task HitRateBuffer()
        {
            EastFive.Persistence.Azure.StorageTables.Driver.E5CloudTable.MinimumParallelConnections = 20;
            EastFive.Persistence.Azure.StorageTables.Driver.E5CloudTable.ConnectionCountGrowthStoppingPoint = 100;
            EastFive.Persistence.Azure.StorageTables.Driver.E5CloudTable.ConnectionCountReductionPoint = 55;

            var modelsTraditional = await Enumerable
                .Range(0, 1000)
                .Select(n => ComplexStorageModel.Create().AsTask())
                .AsyncEnumerable()
                .Select(model => model.StorageCreateAsync(
                    tr => tr.Entity))
                .Await(readAhead:50)
                .ToArrayAsync();

            ComplexStorageModel[] models = await modelsTraditional
                .Select(model => model.StorageDeleteAsync(
                    () => model))
                .AsyncEnumerable()
                .ToArrayAsync();
        }
    }
}