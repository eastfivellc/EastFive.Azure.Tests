using EastFive.Persistence;
using EastFive.Persistence.Azure.StorageTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Azure.Tests.Persistence.Models
{
    [DataContract]
    [StorageTable]
    public struct LookupByIdModel : IReferenceable
    {
        public Guid id => resourceRef.id;

        [RowKey]
        [RowKeyPrefix(Characters = 1)]
        public IRef<LookupByIdModel> resourceRef;

        [ETag]
        public string ETag;

        [Storage]
        [IdHashXX32Lookup]
        public IRef<LookupModel> idLookup;

        public static LookupByIdModel GetModel(Guid lookupValue)
        {
            var resourceRef = Guid.NewGuid().AsRef<LookupByIdModel>();
            return new LookupByIdModel
            {
                resourceRef = resourceRef,
                idLookup = lookupValue.AsRef<LookupModel>(),
            };
        }
    }

    [DataContract]
    [StorageTable]
    public struct LookupModel : IReferenceable
    {
        public Guid id => resourceRef.id;

        [RowKey]
        [RowKeyPrefix(Characters = 1)]
        public IRef<LookupModel> resourceRef;

        [Storage]
        [StringLookupHashXX32]
        public string stringLookup;

        [Storage]
        [BinaryLookup]
        public bool toggle;

        public static LookupModel GetModel()
        {
            var resourceRef = Guid.NewGuid().AsRef<LookupModel>();
            var partitionKey = Guid.NewGuid().ToString("N").Substring(0, 3);
            return new LookupModel
            {
                resourceRef = resourceRef,
                stringLookup = partitionKey,
                toggle = (Guid.NewGuid().ToByteArray().First() % 2) == 0,
            };
        }
    }
}
