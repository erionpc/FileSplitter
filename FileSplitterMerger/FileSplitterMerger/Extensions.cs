using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitterMerger
{
    public static class Extensions
    {
        public static T GetAttribute<T>(this Enum enumValue) where T : Attribute =>
            enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<T>();
    }
}
