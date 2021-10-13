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
    public class TestsPersistenceBasic
    {
        [TestInitialize]
        public void Initialize()
        {
            EastFive.Azure.Tests.Utilities.Configuration.Construct();
        }

        [TestMethod]
        public async Task DataStoresCorrectly()
        {
            var resource = ComplexStorageModel.Create();
            var resourceRef = resource.resourceRef;

            Assert.IsTrue(await resource.StorageCreateAsync(
                (resourceIdCreated) => true,
                () =>
                {
                    return false;
                }));

            var resourceLoaded = await resourceRef.StorageGetAsync(
                rl => rl,
                () =>
                {
                    Assert.Fail("Failed to load resource.");
                    throw new Exception();
                });

            Assert.AreEqual(resource.id, resourceLoaded.id);
            Assert.AreEqual(resource.guid, resourceLoaded.guid);
            Assert.AreEqual(resource.objectInt, resourceLoaded.objectInt);
            Assert.AreEqual(resource.objectString, resourceLoaded.objectString);
            Assert.AreEqual(resource.stringProperty, resourceLoaded.stringProperty);

            #region Embedded object test

            Assert.AreEqual(resource.embeddedModel.guid, resourceLoaded.embeddedModel.guid);
            Assert.AreEqual(resource.embeddedModel.objectInt, resourceLoaded.embeddedModel.objectInt);
            Assert.AreEqual(resource.embeddedModel.objectString, resourceLoaded.embeddedModel.objectString);
            Assert.AreEqual(resource.embeddedModel.stringProperty, resourceLoaded.embeddedModel.stringProperty);

            Assert.AreEqual(resource.embeddedModel.relatedOptionalRef.id, resourceLoaded.embeddedModel.relatedOptionalRef.id);
            Assert.AreEqual(resource.embeddedModel.relatedRef.id, resourceLoaded.embeddedModel.relatedRef.id);

            Assert.AreEqual(resource.embeddedModel.arrayGuid[0], resourceLoaded.embeddedModel.arrayGuid[0]);
            Assert.AreEqual(resource.embeddedModel.arrayGuid[1], resourceLoaded.embeddedModel.arrayGuid[1]);
            Assert.AreEqual(resource.embeddedModel.arrayObjectInt[0], resourceLoaded.embeddedModel.arrayObjectInt[0]);
            Assert.AreEqual(resource.embeddedModel.arrayObjectInt[1], resourceLoaded.embeddedModel.arrayObjectInt[1]);
            Assert.AreEqual(resource.embeddedModel.arrayObjectString[0], resourceLoaded.embeddedModel.arrayObjectString[0]);
            Assert.AreEqual(resource.embeddedModel.arrayObjectString[1], resourceLoaded.embeddedModel.arrayObjectString[1]);
            Assert.AreEqual(resource.embeddedModel.arrayString[0], resourceLoaded.embeddedModel.arrayString[0]);
            Assert.AreEqual(resource.embeddedModel.arrayString[1], resourceLoaded.embeddedModel.arrayString[1]);
            Assert.AreEqual(resource.embeddedModel.arrayString[2], resourceLoaded.embeddedModel.arrayString[2]);
            Assert.AreEqual(resource.embeddedModel.arrayString[3], resourceLoaded.embeddedModel.arrayString[3]);
            Assert.AreEqual(resource.embeddedModel.arrayRef[0].id, resourceLoaded.embeddedModel.arrayRef[0].id);
            Assert.AreEqual(resource.embeddedModel.arrayRef[1].id, resourceLoaded.embeddedModel.arrayRef[1].id);
            Assert.AreEqual(resource.embeddedModel.arrayRefObjObj[0].id, resourceLoaded.embeddedModel.arrayRefObjObj[0].id);
            Assert.AreEqual(resource.embeddedModel.arrayRefObjObj[1].id, resourceLoaded.embeddedModel.arrayRefObjObj[1].id);
            Assert.AreEqual(resource.embeddedModel.arrayRelatedOptionalObjObjRef[0].id, resourceLoaded.embeddedModel.arrayRelatedOptionalObjObjRef[0].id);
            Assert.AreEqual(resource.embeddedModel.arrayRelatedOptionalObjObjRef[1].id, resourceLoaded.embeddedModel.arrayRelatedOptionalObjObjRef[1].id);
            Assert.AreEqual(resource.embeddedModel.arrayRelatedOptionalRef[0].id, resourceLoaded.embeddedModel.arrayRelatedOptionalRef[0].id);
            Assert.AreEqual(resource.embeddedModel.arrayRelatedOptionalRef[1].id, resourceLoaded.embeddedModel.arrayRelatedOptionalRef[1].id);

            #endregion

            Assert.AreEqual(resource.refObjObj.id, resourceLoaded.refObjObj.id);
            Assert.AreEqual(resource.relatedOptionalObjObjRef.id, resourceLoaded.relatedOptionalObjObjRef.id);
            Assert.AreEqual(resource.relatedOptionalRef.id, resourceLoaded.relatedOptionalRef.id);
            Assert.AreEqual(resource.relatedRef.id, resourceLoaded.relatedRef.id);
            Assert.AreEqual(resource.resourceRef.id, resourceLoaded.resourceRef.id);

            Assert.AreEqual(resource.arrayGuid.Length, resourceLoaded.arrayGuid.Length);
            Assert.AreEqual(resource.arrayGuid[0], resourceLoaded.arrayGuid[0]);
            Assert.AreEqual(resource.arrayGuid[1], resourceLoaded.arrayGuid[1]);

            Assert.AreEqual(resource.arrayEnum[0], resourceLoaded.arrayEnum[0]);
            Assert.AreEqual(resource.arrayEnum[1], resourceLoaded.arrayEnum[1]);
            Assert.AreEqual(resource.arrayEnum[2], resourceLoaded.arrayEnum[2]);

            Assert.AreEqual(resource.arrayObjectInt.Length, resourceLoaded.arrayObjectInt.Length);
            Assert.AreEqual(resource.arrayObjectInt[0], resourceLoaded.arrayObjectInt[0]);
            Assert.AreEqual(resource.arrayObjectInt[1], resourceLoaded.arrayObjectInt[1]);

            Assert.AreEqual(resource.arrayObjectString.Length, resourceLoaded.arrayObjectString.Length);
            Assert.AreEqual(resource.arrayObjectString[0], resourceLoaded.arrayObjectString[0]);
            Assert.AreEqual(resource.arrayObjectString[1], resourceLoaded.arrayObjectString[1]);
            Assert.AreEqual(resource.arrayObjectString[2], resourceLoaded.arrayObjectString[2]);
            Assert.AreEqual(resource.arrayObjectString[3], resourceLoaded.arrayObjectString[3]);

            Assert.AreEqual(resource.arrayRef.Length, resourceLoaded.arrayRef.Length);
            Assert.AreEqual(resource.arrayRef[0].id, resourceLoaded.arrayRef[0].id);
            Assert.AreEqual(resource.arrayRef[1].id, resourceLoaded.arrayRef[1].id);

            Assert.AreEqual(resource.arrayRefObj.Length, resourceLoaded.arrayRefObj.Length);
            Assert.AreEqual(resource.arrayRefObj[0].id, resourceLoaded.arrayRefObj[0].id);
            Assert.AreEqual(resource.arrayRefObj[1].id, resourceLoaded.arrayRefObj[1].id);

            Assert.AreEqual(resource.arrayRefObjObj.Length, resourceLoaded.arrayRefObjObj.Length);
            Assert.AreEqual(resource.arrayRefObjObj[0].id, resourceLoaded.arrayRefObjObj[0].id);
            Assert.AreEqual(resource.arrayRefObjObj[1].id, resourceLoaded.arrayRefObjObj[1].id);

            Assert.AreEqual(resource.arrayRelatedOptionalObjObjRef.Length, resourceLoaded.arrayRelatedOptionalObjObjRef.Length);
            Assert.AreEqual(resource.arrayRelatedOptionalObjObjRef[0].id, resourceLoaded.arrayRelatedOptionalObjObjRef[0].id);
            Assert.AreEqual(resource.arrayRelatedOptionalObjObjRef[1].id, resourceLoaded.arrayRelatedOptionalObjObjRef[1].id);
            Assert.AreEqual(resource.arrayRelatedOptionalObjObjRef[2].id, resourceLoaded.arrayRelatedOptionalObjObjRef[2].id);

            Assert.AreEqual(resource.arrayRelatedOptionalRef.Length, resourceLoaded.arrayRelatedOptionalRef.Length);
            Assert.AreEqual(resource.arrayRelatedOptionalRef[0].id, resourceLoaded.arrayRelatedOptionalRef[0].id);
            Assert.AreEqual(resource.arrayRelatedOptionalRef[1].id, resourceLoaded.arrayRelatedOptionalRef[1].id);
            Assert.AreEqual(resource.arrayRelatedOptionalRef[2].id, resourceLoaded.arrayRelatedOptionalRef[2].id);

            Assert.AreEqual(resource.arrayString.Length, resourceLoaded.arrayString.Length);
            Assert.AreEqual(resource.arrayString[0], resourceLoaded.arrayString[0]);
            Assert.AreEqual(resource.arrayString[1], resourceLoaded.arrayString[1]);
            Assert.AreEqual(resource.arrayString[2], resourceLoaded.arrayString[2]);
            Assert.AreEqual(resource.arrayString[3], resourceLoaded.arrayString[3]);

            // TODO: Dictionaries

            Assert.AreEqual(resource.dictGuid[resource.dictGuid.Keys.First()][0], resourceLoaded.dictGuid[resource.dictGuid.Keys.First()][0]);
            Assert.AreEqual(resource.dictGuid[resource.dictGuid.Keys.Skip(1).First()][1], resourceLoaded.dictGuid[resource.dictGuid.Keys.Skip(1).First()][1]);

            Assert.AreEqual(resource.dictObject[resource.dictObject.Keys.First()][0], resourceLoaded.dictObject[resource.dictObject.Keys.First()][0]);
            Assert.AreEqual(resource.dictObject[resource.dictObject.Keys.Skip(1).First()][1], resourceLoaded.dictObject[resource.dictObject.Keys.Skip(1).First()][1]);

            //Assert.AreEqual(resource.dictEmbeddedModel[resource.dictEmbeddedModel.Keys.First()][0].arrayRef[0].id, resourceLoaded.dictEmbeddedModel[resource.dictEmbeddedModel.Keys.First()][0].arrayRef[0].id);
            //Assert.AreEqual(resource.dictEmbeddedModel[resource.dictEmbeddedModel.Keys.Skip(1).First()][1].stringProperty, resourceLoaded.dictEmbeddedModel[resource.dictEmbeddedModel.Keys.Skip(1).First()][1].stringProperty);

        }

        [TestMethod]
        public async Task CanSupplyPartition()
        {
            var instance = UserSuppliedPartitionStorageModel.GetModel();
            var instanceWasCreated = await instance.StorageCreateAsync(
                (discard) => true,
                () => false);
            Assert.IsTrue(instanceWasCreated);

            instance.toggle = true;
            var instanceWasReplaced = await instance.StorageReplaceAsync(
                () => true,
                (info, why) => false);
            Assert.IsTrue(instanceWasReplaced);

            var foundInstanceMaybe = await instance.resourceRef.StorageGetAsync(
                additionalProperties:query => query
                    .Where(item => item.partitionKey == instance.partitionKey),
                (entity) => entity.AsOptional(),
                () => default);
            Assert.IsTrue(foundInstanceMaybe.HasValue);
            Assert.AreEqual(true, foundInstanceMaybe.Value.toggle);

            var instanceWasDeleted = await instance.resourceRef.StorageDeleteAsync(
                    additionalProperties: query => query
                        .Where(item => item.partitionKey == instance.partitionKey),
                (item) => true,
                () => false);
            Assert.IsTrue(instanceWasDeleted);

            foundInstanceMaybe = await instance.resourceRef.StorageGetAsync(
                    additionalProperties: query => query
                        .Where(item => item.partitionKey == instance.partitionKey),
                (entity) => entity.AsOptional(),
                () => default);
            Assert.IsFalse(foundInstanceMaybe.HasValue);

            instanceWasCreated = await instance.resourceRef.StorageCreateOrUpdateAsync(
                    additionalProperties: query => query
                            .Where(item => item.partitionKey == instance.partitionKey),
                async (created, entity, saveAsync) =>
                {
                    Assert.AreEqual(entity.resourceRef.id, instance.resourceRef.id);
                    Assert.AreEqual(entity.partitionKey, instance.partitionKey);
                    if (created)
                        await saveAsync(entity);
                    return created;
                });
            Assert.IsTrue(instanceWasCreated);

            var instanceWasUpdated = await instance.resourceRef.StorageCreateOrUpdateAsync(
                    additionalProperties: query => query
                            .Where(item => item.partitionKey == instance.partitionKey),
                async (created, entity, saveAsync) =>
                {
                    if (created)
                        return false;

                    await saveAsync(entity);
                    return true;
                });
            Assert.IsTrue(instanceWasUpdated);

            instanceWasUpdated = await instance.resourceRef.StorageUpdateAsync(
                    additionalProperties: query => query
                            .Where(item => item.partitionKey == instance.partitionKey),
                async (entity, saveAsync) =>
                {
                    entity.toggle = true;
                    await saveAsync(entity);
                    return true;
                },
                () => false);
            Assert.IsTrue(instanceWasUpdated);

            Expression<Func<UserSuppliedPartitionStorageModel, bool>> partitionKeyQuery =
                (item) => item.partitionKey == instance.partitionKey;
            var foundInstances = await partitionKeyQuery
                .StorageQuery()
                .ToArrayAsync();
            Assert.AreEqual(1, foundInstances.Length);
            Assert.AreEqual(true, foundInstances[0].toggle);

            instanceWasDeleted = await instance.resourceRef.StorageDeleteAsync(
                    additionalProperties: query => query
                            .Where(item => item.partitionKey == instance.partitionKey),
                (discard) => true,
                () => false);
            Assert.IsTrue(instanceWasDeleted);

            foundInstanceMaybe = await instance.resourceRef.StorageGetAsync(
                    additionalProperties: query => query
                            .Where(item => item.partitionKey == instance.partitionKey),
                (entity) => entity.AsOptional(),
                () => default);
            Assert.IsFalse(foundInstanceMaybe.HasValue);

            var trans = await instance.StorageCreateTransactionAsync(
                () => false);
            var transResult = trans.Success(() => true, (ok) => false);
            Assert.IsTrue(transResult);

            var deleteResults = await partitionKeyQuery
                .StorageQueryDelete()
                .ToArrayAsync();
            Assert.AreEqual(1, deleteResults.Length);
            Assert.IsTrue(deleteResults[0].HttpStatusCode < 300);
        }
    }
}