using System;
using System.Linq;
using System.Reflection;
using Umbraco.Web;

namespace UmbMapper.Factories
{
    /// <summary>
    /// Factory to generate a mapping processor passing the the type to map to 
    /// in the IUmbMapperConfig instance
    /// </summary>
    public class MappingProcessorFactory : IMappingProcessorFactory
    {
        private readonly IUmbracoContextFactory umbracoContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProcessorFactory"/> class.
        /// Standard Constructor
        /// </summary>
        /// <param name="umbracoContextFactory"></param>
        public MappingProcessorFactory(IUmbracoContextFactory umbracoContextFactory)
        {
            this.umbracoContextFactory = umbracoContextFactory;
        }

        /// <summary>
        /// Main factory method to create an instance of <see cref="IMappingProcessor"/>
        /// </summary>
        /// <param name="config">IUmbMapperConfig instance that defines what's being mapped to</param>
        /// <param name="umbMapperService">IUmbMapperService needed by the processor to do the mapping</param>
        /// <returns>Instance of <see cref="IMappingProcessor"/></returns>
        public IMappingProcessor Create(IUmbMapperConfig config, IUmbMapperService umbMapperService)
        {
            Type genericType = typeof(MappingProcessor<>);
            Type[] typeArgs = { config.MappedType };

            Type constructedGenericType = genericType.MakeGenericType(typeArgs);
            object[] constructorArgs = new object[] { config, umbMapperService, this.umbracoContextFactory };

            ConstructorInfo ctor = constructedGenericType.GetConstructors().First();

            // The performance of this coudl be improved 
            // ref: https://rogerjohansson.blog/2008/02/28/linq-expressions-creating-objects/
            return Activator.CreateInstance(constructedGenericType, constructorArgs) as IMappingProcessor;
        }
    }
}
