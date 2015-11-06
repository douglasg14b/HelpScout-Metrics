using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpScoutMetrics.Model
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

        //Extension method to read all Bytes from a stream
        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }

        }

        /// <summary>
        /// A better string comparison method
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
