using System.Collections.Concurrent;
using System.Reflection;
using Autofac.Core.Activators.Reflection;

namespace Infrastructure.Processing;

internal class AllConstructorFinder : IConstructorFinder
{
    private static readonly ConcurrentDictionary<Type, ConstructorInfo[]> Cache =
        new ConcurrentDictionary<Type, ConstructorInfo[]>();


    public ConstructorInfo[] FindConstructors(Type targetType)
    {
        var result = Cache.GetOrAdd(targetType, 
            t => t.GetTypeInfo().DeclaredConstructors.ToArray());

        return result.Any() ? result : throw new NoConstructorsFoundException(targetType);

    }
}