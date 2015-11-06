using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Scripts.Model.WindowLogic
{
    public static class MiscMethods
    {
        //Uses Reflection to return all properties in a class
        private static IEnumerable<PropertyInfo> GetProperties(Object theObject)
        {
            return theObject.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance);
        }

        //Returns a name for each property in a class
        public static IEnumerable<string> GetPropertyNames(Object yourClass)
        {
            foreach (PropertyInfo property in GetProperties(yourClass))
            {
                yield return property.Name;
            }
        }
    }
}
