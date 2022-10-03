using System.Reflection;
using Core.Common;

namespace Infrastructure.Processing;

internal class Assemblies
{
    public static readonly Assembly Application = typeof(IAggregateRoot).Assembly;
}