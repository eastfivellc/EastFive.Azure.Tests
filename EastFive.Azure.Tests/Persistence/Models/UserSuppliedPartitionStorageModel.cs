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
    public struct UserSuppliedPartitionStorageModel : IReferenceable
    {
        public Guid id => resourceRef.id;

        [RowKey]
        public IRef<UserSuppliedPartitionStorageModel> resourceRef;

        [ParititionKey]
        public string partitionKey;

        [Storage]
        public bool toggle;

        public static UserSuppliedPartitionStorageModel GetModel()
        {
            var resourceRef = Guid.NewGuid().AsRef<UserSuppliedPartitionStorageModel>();
            var partitionKey = resourceRef.id.ToString("N").Substring(0, 3);
            return new UserSuppliedPartitionStorageModel
            {
                resourceRef = resourceRef,
                partitionKey = partitionKey,
            };
        }
    }
}
