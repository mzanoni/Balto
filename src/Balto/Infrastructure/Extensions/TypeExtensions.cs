using System;

namespace Balto.Infrastructure.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsInExactNamespace(this Type type, Type compareToType)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (compareToType == null) throw new ArgumentNullException(nameof(compareToType));

            return string.Equals(type.Namespace, compareToType.Namespace, StringComparison.Ordinal);
        }
    }
}