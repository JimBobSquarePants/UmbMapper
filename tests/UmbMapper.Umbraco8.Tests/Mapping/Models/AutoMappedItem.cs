using System;

namespace UmbMapper.Umbraco8.Tests.Mapping.Models
{
    public class AutoMappedItem
    {
        // We're deliberately mixing virtual/non virtual properties here for testing.
        public int Id { get; set; }

        public virtual string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual DateTime UpdateDate { get; set; }
    }
}
