using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Azure.Tests.Persistence.Models
{
    public struct RelatedModel : IReferenceable
    {
        public Guid id { get; set; }
    }

    public class RelatedModelObj : IReferenceable
    {
        public Guid id { get; set; }
    }
}
