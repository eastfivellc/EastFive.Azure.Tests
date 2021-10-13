using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EastFive.Extensions;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;

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
    public class TestsDoubleLookupsBatch
    {
        [TestInitialize]
        public void Initialize()
        {
            EastFive.Azure.Tests.Utilities.Configuration.Construct();
        }

        [TestMethod]

        public async Task TestStoryBoardItemBatchFailure()
        {
            var exampleParentItemRef = Ref<ExampleParentItem>.NewRef();
            var intermediaryItems = await IntermediaryItem
                .MakeIntermediaryItemsAsync(exampleParentItemRef)
                .ToArrayAsync();

            var parentItem = new ExampleParentItem()
            {
                exampleParentItemRef = exampleParentItemRef,
                lastAutomated = default,
                coverImage = Ref<ExampleRefedStorage>.NewRef().Optional(),

                items = intermediaryItems
                    .Select(intermediaryItem => intermediaryItem.intermediaryItemRef)
                    .Refs(),
                Name = "Example Name",
                ownerRef = Ref<RelatedModel>.NewRef(),
                template = Ref<ComplexStorageModel>.NewRef().Optional(),
            };
            Assert.IsTrue(await parentItem.StorageCreateAsync(
                (discard) => true));


            var itemsRetreivedBefore = await RetreiveItemsAsync(exampleParentItemRef);

            var lastAutomated = DateTime.UtcNow;
            var parentItemRef = parentItem.exampleParentItemRef;
            var itemsSaved = await parentItemRef.StorageUpdateAsync(
                async (parentItem, saveAsync) =>
                {
                    parentItem.lastAutomated = lastAutomated;
                    await saveAsync(parentItem);

                    var intermediaryItemsInner = await parentItem.exampleParentItemRef
                        .StorageGetByIdProperty(
                            (IntermediaryItem sbi) => sbi.board)
                        .ToArrayAsync();

                    Assert.AreEqual(intermediaryItems.Length, intermediaryItemsInner.Length);

                    ExampleLinkedStorage[] saved = //new ExampleLinkedStorage[] { };
                    await intermediaryItemsInner
                    .Select(
                        intermediaryItem =>
                        {
                            return intermediaryItem.intermediaryItemRef
                                .StorageGetByIdProperty(
                                    (ExampleLinkedStorage sbmi) => sbmi.item)
                                .Where(sbmi => !sbmi.userFixed)
                                .Select(
                                    sbmi =>
                                    {
                                        if (new Random().NextDouble() < 0.8)
                                            return (false, sbmi);
                                        var intermediaryItemUpdated = intermediaryItemsInner.Random();
                                        if (sbmi.item.id != intermediaryItemUpdated.id)
                                        {
                                            sbmi.item = intermediaryItemUpdated;
                                            return (true, sbmi);
                                        }
                                        return (false, sbmi);
                                    })
                                .SelectWhere()
                                .StorageCreateOrReplaceBatch(
                                    (saved, tr) => saved);
                        })
                    .SelectMany()
                    .ToArrayAsync();
                    return saved;
                });

            var itemsRetreivedAfter = await RetreiveItemsAsync(parentItemRef);

            Assert.AreEqual(itemsRetreivedBefore.Length, itemsRetreivedAfter.Length);

            Task<ExampleLinkedStorage[]> RetreiveItemsAsync(IRef<ExampleParentItem> parentItemToRetrieve) => parentItemToRetrieve
                .StorageGetByIdProperty((IntermediaryItem sbi) => sbi.board)
                .SelectAsyncMany(
                    (intermediaryItem) =>
                    {
                        var intermediaryItemRef = intermediaryItem.intermediaryItemRef;
                        var storyBoardMediaObjects = intermediaryItemRef
                            .StorageGetByIdProperty(
                                (ExampleLinkedStorage storyBoardItemMedia) =>
                                    storyBoardItemMedia.item)
                            .Select(
                                itemMedia => itemMedia
                                    .SetContentLink(new Random().Next()));

                        return storyBoardMediaObjects;
                    })
                .ToArrayAsync();
        }
    }
}