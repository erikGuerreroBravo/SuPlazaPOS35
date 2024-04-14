using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Builder.Generic
{
    public class Factories
    {
        public static T Default<T>() => Default<T>(true);
        public static T Default<T>(bool nonPublic) => (T)Activator.CreateInstance(typeof(T), nonPublic);
        public static T Uninitialized<T>() => (T)FormatterServices.GetUninitializedObject(typeof(T));
    }
}
