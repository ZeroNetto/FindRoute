using System;

namespace GeneticRoute
{
	public static class TypeExtensions
	{
		public static bool IsEqualOrSubclassOf(this Type type, Type otherType)
		{
			return type == otherType || type.IsSubclassOf(otherType);
		}
	}
}
