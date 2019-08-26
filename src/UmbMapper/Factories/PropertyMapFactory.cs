//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace UmbMapper.Factories
//{
//    internal class PropertyMapFactory : IPropertyMapFactory
//    {
//        bool GetOrCreateMap<T>(List<PropertyMap<T>> existingMaps, PropertyInfo property, out PropertyMap<T> map)
//            where T : class
//        {
//            bool exists = true;
//            map = existingMaps.Find(x => x.Info.Property.Name == property.Name);

//            if (map is null)
//            {
//                exists = false;
//                map = new PropertyMap<T>(property);
//            }

//            return exists;
//        }
//    }
//}
