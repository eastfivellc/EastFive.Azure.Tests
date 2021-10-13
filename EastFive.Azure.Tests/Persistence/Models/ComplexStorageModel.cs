using EastFive.Persistence;
using EastFive.Persistence.Azure.StorageTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Azure.Tests.Persistence.Models
{
    [StorageTable]
    public struct ComplexStorageModel : IReferenceable
    {
        public Guid id => resourceRef.id;

        public const string ResourceIdPropertyName = "id";
        [RowKey]
        [StandardParititionKey]
        public IRef<ComplexStorageModel> resourceRef;

        public class EmbeddedModel
        {
            #region Singles

            [Storage]
            public Guid guid;

            [Storage]
            public string stringProperty;

            [Storage]
            public object objectInt;

            [Storage]
            public object objectString;

            [Storage]
            public IRef<RelatedModel> relatedRef;

            [Storage]
            public IRefOptional<RelatedModel> relatedOptionalRef;

            #endregion

            #region Arrays

            [Storage]
            public Guid[] arrayGuid;

            [Storage]
            public string[] arrayString;

            [Storage]
            public object[] arrayObjectInt;

            [Storage]
            public object[] arrayObjectString;

            [Storage]
            public IRef<RelatedModel>[] arrayRef;

            [Storage]
            public IRef<IReferenceable>[] arrayRefObjObj;

            [Storage]
            public IRefOptional<RelatedModel>[] arrayRelatedOptionalRef;

            [Storage]
            public IRefOptional<IReferenceable>[] arrayRelatedOptionalObjObjRef;

            #endregion
        }

        public enum ExampleEnum
        {
            ascending,
            desending,
            neutral,
        }

        #region Singles

        public const string GuidPropertyName = "guid";
        [Storage(Name = GuidPropertyName)]
        public Guid guid;

        public const string StringPropertyName = "string";
        [Storage(Name = StringPropertyName)]
        public string stringProperty;

        [Storage]
        public object objectInt;

        [Storage]
        public object objectString;

        public const string RealtedPropertyName = "related";
        [Storage(Name = RealtedPropertyName)]
        public IRef<RelatedModel> relatedRef;

        [Storage]
        public IRef<IReferenceable> refObjObj;

        [Storage]
        public IRefOptional<RelatedModel> relatedOptionalRef;

        [Storage]
        public IRefOptional<IReferenceable> relatedOptionalObjObjRef;

        [Storage]
        public EmbeddedModel embeddedModel;

        #endregion

        #region Arrays

        [Storage]
        public Guid[] arrayGuid;

        [Storage]
        public string[] arrayString;

        [Storage]
        public object[] arrayObjectInt;

        [Storage]
        public object[] arrayObjectString;

        [Storage]
        public ExampleEnum[] arrayEnum;

        [Storage]
        public IRef<RelatedModel>[] arrayRef;

        [Storage]
        public IRef<IReferenceable>[] arrayRefObjObj;

        [Storage]
        public IRef<RelatedModelObj>[] arrayRefObj;

        [Storage]
        public IRefOptional<RelatedModel>[] arrayRelatedOptionalRef;

        [Storage]
        public IRefOptional<IReferenceable>[] arrayRelatedOptionalObjObjRef;

        [Storage]
        public EmbeddedModel[] arrayEmbeddedModel;

        #endregion

        #region Dictionary

        [Storage]
        public IDictionary<Guid, Guid[]> dictGuid;

        [Storage]
        public IDictionary<object, object[]> dictObject;

        [Storage]
        public IDictionary<string, string[]> dictObjectIntArrayEmbedArray;

        [Storage]
        public IDictionary<IRef<RelatedModel>, IRef<RelatedModel>[]> dictRef;

        [Storage]
        public IDictionary<IRef<IReferenceable>, IRef<IReferenceable>[]> dictRefObjectObj;

        [Storage]
        public IDictionary<IRefOptional<RelatedModel>, IRefOptional<RelatedModel>[]> dictRefOptional;

        [Storage]
        public IDictionary<IRefOptional<IReferenceable>, IRefOptional<IReferenceable>[]> dictRefOptionalObjObj;

        [Storage]
        public IDictionary<EmbeddedModel, EmbeddedModel[]> dictEmbeddedModel;

        #endregion


        public static ComplexStorageModel Create()
        {

            var resourceRef = Guid.NewGuid().AsRef<ComplexStorageModel>();

            var embedded1 = new ComplexStorageModel.EmbeddedModel
            {
                guid = Guid.NewGuid(),
                objectInt = 3,
                objectString = "Barf",
                relatedOptionalRef = Guid.NewGuid().AsRef<RelatedModel>().Optional(),
                relatedRef = Guid.NewGuid().AsRef<RelatedModel>(),
                stringProperty = "Food",
                arrayGuid = new[] { Guid.NewGuid(), Guid.NewGuid() },
                arrayObjectInt = new object[] { 1, 2 },
                arrayObjectString = new object[] { null, "barF", string.Empty, "fooD" },
                arrayRef = new[] { Guid.NewGuid().AsRef<RelatedModel>(), Guid.NewGuid().AsRef<RelatedModel>() },
                arrayRefObjObj = new[] { Guid.NewGuid().AsRef<IReferenceable>(), Guid.NewGuid().AsRef<IReferenceable>() },
                arrayRelatedOptionalObjObjRef = new[]
                {
                    Guid.NewGuid().AsRefOptional<IReferenceable>(),
                    default(Guid?).AsRefOptional<IReferenceable>(),
                    Guid.NewGuid().AsRefOptional<IReferenceable>(),
                },
                arrayRelatedOptionalRef = new[]
                {
                    Guid.NewGuid().AsRefOptional<RelatedModel>(),
                    default(Guid?).AsRefOptional<RelatedModel>(),
                    Guid.NewGuid().AsRefOptional<RelatedModel>(),
                },
                arrayString = new[] { "BARRF", null, string.Empty, "food", },
            };
            var embedded2 = new ComplexStorageModel.EmbeddedModel
            {
                guid = Guid.NewGuid(),
                objectInt = 4,
                objectString = "barf",
                relatedOptionalRef = Guid.NewGuid().AsRefOptional<RelatedModel>(),
                relatedRef = Guid.NewGuid().AsRef<RelatedModel>(),
                stringProperty = "food",
                arrayGuid = new[] { Guid.NewGuid(), Guid.NewGuid() },
                arrayObjectInt = new object[] { 1, 2 },
                arrayObjectString = new object[] { "food", string.Empty, null, "bar" },
                arrayRef = new[] { Guid.NewGuid().AsRef<RelatedModel>(), Guid.NewGuid().AsRef<RelatedModel>() },
                arrayRefObjObj = new[] { Guid.NewGuid().AsRef<IReferenceable>(), Guid.NewGuid().AsRef<IReferenceable>() },
                arrayRelatedOptionalObjObjRef = new[]
                {
                    Guid.NewGuid().AsRefOptional<IReferenceable>(),
                    default(Guid?).AsRefOptional<IReferenceable>(),
                    Guid.NewGuid().AsRefOptional<IReferenceable>(),
                },
                arrayRelatedOptionalRef = new[]
                {
                    Guid.NewGuid().AsRefOptional<RelatedModel>(),
                    default(Guid?).AsRefOptional<RelatedModel>(),
                    Guid.NewGuid().AsRefOptional<RelatedModel>(),
                },
                arrayString = new[] { "Barf", null, string.Empty, "bar", },
            };

            var resource = new ComplexStorageModel
            {
                resourceRef = resourceRef,
                guid = Guid.NewGuid(),
                objectInt = 3,
                objectString = "food",
                embeddedModel = embedded1,
                refObjObj = Guid.NewGuid().AsRef<IReferenceable>(),
                relatedOptionalObjObjRef = Guid.NewGuid().AsRefOptional<IReferenceable>(),
                relatedOptionalRef = Guid.NewGuid().AsRefOptional<RelatedModel>(),
                relatedRef = Guid.NewGuid().AsRef<RelatedModel>(),
                stringProperty = "barf",
                arrayGuid = new[] { Guid.NewGuid(), Guid.NewGuid() },
                arrayEnum = new[]
                {
                    ComplexStorageModel.ExampleEnum.neutral,
                    ComplexStorageModel.ExampleEnum.ascending,
                    ComplexStorageModel.ExampleEnum.desending,
                },
                arrayObjectInt = new object[] { 1, 2 },
                arrayObjectString = new object[] { string.Empty, "foo", "bar", null },
                arrayRef = new[] { Guid.NewGuid().AsRef<RelatedModel>(), Guid.NewGuid().AsRef<RelatedModel>() },
                arrayRefObj = new[] { Guid.NewGuid().AsRef<RelatedModelObj>(), Guid.NewGuid().AsRef<RelatedModelObj>() },
                arrayRefObjObj = new[] { Guid.NewGuid().AsRef<IReferenceable>(), Guid.NewGuid().AsRef<IReferenceable>() },
                arrayRelatedOptionalObjObjRef = new[]
                {
                    Guid.NewGuid().AsRefOptional<IReferenceable>(),
                    default(Guid?).AsRefOptional<IReferenceable>(),
                    Guid.NewGuid().AsRefOptional<IReferenceable>(),
                },
                arrayRelatedOptionalRef = new[]
                {
                    Guid.NewGuid().AsRefOptional<RelatedModel>(),
                    default(Guid?).AsRefOptional<RelatedModel>(),
                    Guid.NewGuid().AsRefOptional<RelatedModel>(),
                },
                arrayString = new[] { "Bar", null, string.Empty, "Food", },
                arrayEmbeddedModel = new[] { embedded1, embedded2 },

                dictGuid = new Dictionary<Guid, Guid[]>
                {
                    { Guid.NewGuid(), new [] { Guid.NewGuid(), Guid.NewGuid() } },
                    { Guid.NewGuid(), new [] { Guid.NewGuid(), Guid.NewGuid() } },
                },
                dictObject = new Dictionary<object, object[]>
                {
                    { 1, new object [] { 33, "asdf", 44 } },
                    { "3", new object [] { Guid.NewGuid(), Guid.NewGuid() } },
                },
                //dictEmbeddedModel = new Dictionary<ComplexStorageModel.EmbeddedModel, ComplexStorageModel.EmbeddedModel[]>
                //{
                //    { embedded1, new [] { embedded2, embedded2, } },
                //    { embedded2, new [] { embedded2, embedded1, } },
                //},
            };
            return resource;
        }
    }
}
