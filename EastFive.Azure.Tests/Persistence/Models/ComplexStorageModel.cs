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

    }
}
