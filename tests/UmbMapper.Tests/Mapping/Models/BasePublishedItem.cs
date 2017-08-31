using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbMapper.Tests.Mapping.Models
{
    public class BasePublishedItem
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
    }
}
