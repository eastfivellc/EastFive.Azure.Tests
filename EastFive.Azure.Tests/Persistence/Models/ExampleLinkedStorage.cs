using System;
using System.Linq;
using System.Threading.Tasks;
using EastFive.Api;
using EastFive.Azure.Persistence.AzureStorageTables;
using EastFive.Linq.Async;
using EastFive.Persistence;
using EastFive.Persistence.Azure.StorageTables;
using Newtonsoft.Json;

namespace EastFive.Azure.Tests.Persistence.Models
{
    // StoryBoardItemMedia
    [DisplayEntryPoint]
    [StorageTable]
    public struct ExampleLinkedStorage : IReferenceable
    {
        #region Properties

        #region ID / Persistence

        [JsonIgnore]
        public Guid id => exampleLinkedStorageRef.id;

        public const string IdPropertyName = "id";
        [ApiProperty(PropertyName = IdPropertyName)]
        [JsonProperty(PropertyName = IdPropertyName)]
        [RowKey]
        [StandardParititionKey]
        [ResourceIdentifier]
        public IRef<ExampleLinkedStorage> exampleLinkedStorageRef;

        [ETag]
        [JsonIgnore]
        public string eTag;

        #endregion

        public const string MediaPropertyName = "media";
        [ApiProperty(PropertyName = MediaPropertyName)]
        [JsonProperty(PropertyName = MediaPropertyName)]
        [Storage]
        [IdPrefixLookup(Characters = 2)]
        public IRef<ExampleRefedStorage> media;

        public const string ItemPropertyName = "item";
        [ApiProperty(PropertyName = ItemPropertyName)]
        [JsonProperty(PropertyName = ItemPropertyName)]
        [Storage]
        [IdPrefixLookup(Characters = 2)]
        public IRef<IntermediaryItem> item;

        public const string ContentPropertyName = "content";
        [ApiProperty(PropertyName = ContentPropertyName)]
        [JsonProperty(PropertyName = ContentPropertyName)]
        [ResourceTitle]
        public Uri contentLocation;

        public const string NotesPropertyName = "notes";
        [ApiProperty(PropertyName = NotesPropertyName)]
        [JsonProperty(PropertyName = NotesPropertyName)]
        [Storage]
        public string notes;

        public const string RankPropertyName = "rank";
        [ApiProperty(PropertyName = RankPropertyName)]
        [JsonProperty(PropertyName = RankPropertyName)]
        [Storage]
        public double rank;

        public const string CreatedPropertyName = "created";
        [ApiProperty(PropertyName = CreatedPropertyName)]
        [JsonProperty(PropertyName = CreatedPropertyName)]
        [Storage]
        [JsonIgnore]
        public DateTime? created;

        public const string FixedPropertyName = "fixed";
        [ApiProperty(PropertyName = FixedPropertyName)]
        [JsonProperty(PropertyName = FixedPropertyName)]
        [Storage]
        public bool userFixed;

        [Storage]
        [JsonIgnore]
        public IRef<EastFive.Api.Azure.Resources.Content> content;

        [Storage]
        [JsonIgnore]
        public int contentOriginalWidth;

        [Storage]
        [JsonIgnore]
        public int contentOriginalHeight;

        #endregion

        internal ExampleLinkedStorage SetContentLink(int contentOriginalWidth)
        {
            this.contentOriginalWidth = contentOriginalWidth;
            return this;
        }

        public static async Task<ExampleLinkedStorage> MakeAsync(
            IRef<IntermediaryItem> exampleParentItemRef)
        {
            var exampleLinkedStorage = Make(exampleParentItemRef);
            var exampleLinkedStorageStored = await exampleLinkedStorage.StorageCreateAsync(
                tr => tr.Entity);
            return exampleLinkedStorageStored;
        }

        public static ExampleLinkedStorage Make(IRef<IntermediaryItem> exampleParentItemRef)
        {
            return new ExampleLinkedStorage()
            {
                exampleLinkedStorageRef = Ref<ExampleLinkedStorage>.NewRef(),
                content = Ref<Api.Azure.Resources.Content>.NewRef(),
                contentOriginalHeight = 300,
                contentOriginalWidth = 400,
                created = DateTime.UtcNow,
                item = exampleParentItemRef,
                media = Ref<ExampleRefedStorage>.NewRef(),
                notes = "blah",
                rank = 1,
                userFixed = false,
            };
        }
    }


    // StoryBoardItem
    [StorageTable]
    [DisplayEntryPoint]
    public struct IntermediaryItem : IReferenceable, IRef<IntermediaryItem>
    {
        #region Properties

        #region Base

        [JsonIgnore]
        public Guid id => intermediaryItemRef.id;

        public const string IdPropertyName = "id";
        [ApiProperty(PropertyName = IdPropertyName)]
        [JsonProperty(PropertyName = IdPropertyName)]
        [RowKey]
        [StandardParititionKey]
        [ResourceIdentifier]
        public IRef<IntermediaryItem> intermediaryItemRef;

        [ETag]
        [JsonIgnore]
        public string eTag;

        [LastModified]
        [JsonIgnore]
        public DateTime lastModified;

        #endregion

        public const string BoardPropertyName = "board";
        [ApiProperty(PropertyName = BoardPropertyName)]
        [JsonProperty(PropertyName = BoardPropertyName)]
        [Storage]
        [IdPrefixLookup]
        public IRef<ExampleParentItem> board;

        public const string NamePropertyName = "name";
        [ApiProperty(PropertyName = NamePropertyName)]
        [JsonProperty(PropertyName = NamePropertyName)]
        [Storage]
        [ResourceTitle]
        public string name;

        public const string TitlePropertyName = "title";
        [ApiProperty(PropertyName = TitlePropertyName)]
        [JsonProperty(PropertyName = TitlePropertyName)]
        [Storage]
        public string title;

        public const string RankPropertyName = "rank";
        [ApiProperty(PropertyName = RankPropertyName)]
        [JsonProperty(PropertyName = RankPropertyName)]
        [Storage]
        public double rank;

        public const string NotesPropertyName = "notes";
        [ApiProperty(PropertyName = NotesPropertyName)]
        [JsonProperty(PropertyName = NotesPropertyName)]
        [Storage]
        public string notes;

        public const string SourcePropertyName = "source";
        [ApiProperty(PropertyName = SourcePropertyName)]
        [JsonProperty(PropertyName = SourcePropertyName)]
        [Storage]
        [IdPrefixLookup(Characters = 2)]
        public IRef<IntermediaryItem> source;

        #endregion

        public static IEnumerableAsync<IntermediaryItem> MakeIntermediaryItemsAsync(
            IRef<ExampleParentItem> exampleParentItemRef)
        {
            return Enumerable
                .Range(0, new Random().Next() % 20)
                .Select(
                    i =>
                    {
                        var children = new Random().Next() % 3;
                        return MakeIntermediaryItemAsync(exampleParentItemRef, children);
                    })
                .AsyncEnumerable();
        }


        public static async Task<IntermediaryItem> MakeIntermediaryItemAsync(
            IRef<ExampleParentItem> exampleParentItemRef,
            int children)
        {
            var ii = MakeIntermediaryItem(exampleParentItemRef);
            var iiStored = await ii.StorageCreateAsync(
                tr => tr.Entity);

            ExampleLinkedStorage [] childrenMade = await Enumerable
                .Range(0, children)
                .Select(
                    i =>
                    {
                        return ExampleLinkedStorage.MakeAsync(ii.intermediaryItemRef);
                    })
                .AsyncEnumerable()
                .ToArrayAsync();

            return iiStored;
        }


        public static IntermediaryItem MakeIntermediaryItem(IRef<ExampleParentItem> exampleParentItemRef)
        {
            return new IntermediaryItem()
            {
                board = exampleParentItemRef,
                intermediaryItemRef = Ref<IntermediaryItem>.NewRef(),
                name = "Example Name",
                notes = "Example notes",
                rank = 20,
                title = "Title -- Example",
            };
        }

    }

    // StoryBoard
    [StorageTable]
    [DisplayEntryPoint]
    public struct ExampleParentItem : IReferenceable, IRef<ExampleParentItem>
    {
        #region Properties

        #region Base

        [JsonIgnore]
        public Guid id => exampleParentItemRef.id;

        public const string IdPropertyName = "id";
        [ApiProperty(PropertyName = IdPropertyName)]
        [JsonProperty(PropertyName = IdPropertyName)]
        [RowKey]
        [StandardParititionKey]
        [ResourceIdentifier]
        public IRef<ExampleParentItem> exampleParentItemRef;

        [ETag]
        [JsonIgnore]
        public string eTag;

        #endregion

        public const string TemplatePropertyName = "template";
        [ApiProperty(PropertyName = TemplatePropertyName)]
        [JsonProperty(PropertyName = TemplatePropertyName)]
        [Storage]
        public IRefOptional<ComplexStorageModel> template;

        public const string NamePropertyName = "name";
        [ApiProperty(PropertyName = NamePropertyName)]
        [JsonProperty(PropertyName = NamePropertyName)]
        [Storage]
        [ResourceTitle]
        public string Name;

        public const string OwnerPropertyName = "owner";
        [ApiProperty(PropertyName = OwnerPropertyName)]
        [JsonProperty(PropertyName = OwnerPropertyName)]
        [Storage]
        [IdStandardPartitionLookup]
        public IRef<RelatedModel> ownerRef;

        public const string CoverImagePropertyName = "cover_image";
        [ApiProperty(PropertyName = CoverImagePropertyName)]
        [JsonProperty(PropertyName = CoverImagePropertyName)]
        [Storage]
        public IRefOptional<ExampleRefedStorage> coverImage;

        public const string ItemsPropertyName = "items";
        [JsonIgnore]
        [Storage]
        public IRefs<IntermediaryItem> items;

        public const string LastAutomatedPropertyName = "last_automated";
        [ApiProperty(PropertyName = LastAutomatedPropertyName)]
        [JsonProperty(PropertyName = LastAutomatedPropertyName)]
        [Storage]
        public DateTime? lastAutomated;

        #endregion
    }

    [StorageTable]
    public struct ExampleRefedStorage : IReferenceable
    {
        public const string SearchTypeName = "x-application/x-storyboard-media";

        #region Properties

        [JsonIgnore]
        public Guid id => exampleRefedStorageRef.id;

        public const string IdPropertyName = "id";
        [ApiProperty(PropertyName = IdPropertyName)]
        [JsonProperty(PropertyName = IdPropertyName)]
        [RowKey]
        [StandardParititionKey]
        [ResourceIdentifier]
        public IRef<ExampleRefedStorage> exampleRefedStorageRef;

        [ETag]
        [JsonIgnore]
        public string eTag;

        public const string ContentIdPropertyName = "content_id";
        [JsonIgnore]
        [Storage]
        public IRef<EastFive.Api.Azure.Resources.Content> content;

        public const string ContentPropertyName = "content";
        [ApiProperty(PropertyName = ContentPropertyName)]
        [JsonProperty(PropertyName = ContentPropertyName)]
        [ResourceTitle]
        public Uri contentLocation;

        public const string OwnerPropertyName = "owner";
        [ApiProperty(PropertyName = OwnerPropertyName)]
        [JsonProperty(PropertyName = OwnerPropertyName)]
        [Storage]
        [IdPrefixLookup(Characters = 2)]
        public IRef<RelatedModel> owner;

        #endregion

    }
}