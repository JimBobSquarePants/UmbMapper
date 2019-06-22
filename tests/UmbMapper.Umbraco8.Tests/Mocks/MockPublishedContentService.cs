using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core;
using UmbMapper.Umbraco8.Tests.Mapping.Models;

namespace UmbMapper.Umbraco8.Tests.Mocks
{
    /// <summary>
    /// This is used to get content from and ID
    /// This is because internally Umbaco will resolve a lot of stoff 
    /// </summary>
    public static class MockPublishedPropertService
    {
        //TODO - look at how Umbraco does this internally and implement something similar that's suitable for testing
        public static IPublishedProperty GetProperty(IPublishedContent content, string alias, bool recurse)
        {
            IPublishedProperty prop = content.Properties.SingleOrDefault(p => p.Alias.InvariantEquals(alias));

            if (prop == null && recurse && content.Parent != null)
            {
                return null;
            }

            if (prop == null)
            {
                return null;
            }

            int id;
            if (int.TryParse(prop.GetValue()?.ToString(), out id))
            {
                // If - how does umbraco test that this is a valid id?
                // How do we replicate the same functionality?
                if (id > 999)
                {
                    return new
                        MockPublishedProperty(
                        prop.PropertyType.Alias,
                        new MockPublishedContent() { Id = id }
                        );
                }

            }



                return prop;
        }
    }
}
