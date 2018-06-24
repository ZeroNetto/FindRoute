using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GeneticRoute.Util
{
	public class ValueType<T> : IEquatable<T> where T : class
	{
		private static readonly Func<T, int> GetHashCodeFast;

		static ValueType()
		{
			var props = GetPublicInstanceProperties();

			var valueTypeParam = Expression.Parameter(typeof(T));
			var ghc = typeof(object).GetMethod("GetHashCode");

			if (ghc == null)
				throw new NullReferenceException("Unexpected null from typeof(object).GetMethod(\"GetHashCode\")");

			var p = props
				.Select(prop =>
					Expression.Call(Expression.MakeMemberAccess(valueTypeParam, prop), ghc))
				.Aggregate<Expression>((a, propHashCodeExpression) =>
					Expression.Add(a, propHashCodeExpression));

			GetHashCodeFast = Expression.Lambda<Func<T, int>>(p, valueTypeParam).Compile();
		}

		private static PropertyInfo[] GetPublicInstanceProperties()
		{
			return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
		}

		private static IEnumerable<PropertyInfo> GetVisibleInstancePropertiesOrdered()
		{
			return GetPublicInstanceProperties().Where(p => p.CanRead)
				.OrderBy(p => p.Name, StringComparer.Ordinal);
		}

		private IEnumerable<string> ToStringAllVisibleInstanceProperties()
		{
			return GetVisibleInstancePropertiesOrdered()
				.Select(property => $"{property.Name}: {property.GetValue(this)}");
		}

		public override string ToString()
		{
			var propString = string.Join("; ", ToStringAllVisibleInstanceProperties());

			return $"{typeof(T).Name}({propString})";
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (!obj.GetType().IsEqualOrSubclassOf(typeof(T))) return false;
			return Equals((T)obj);
		}

		public bool Equals(T other)
		{
			if (ReferenceEquals(null, other)) return false;

			return !(from property in GetVisibleInstancePropertiesOrdered()
					 let value = property.GetValue(this)
					 where !Equals(value, property.GetValue(other))
					 select value).Any();
		}

		public override int GetHashCode()
		{
			return GetHashCodeFast != null && this is T ? GetHashCodeFast(this as T) : GetHashCodeSlow();
		}

		private int GetHashCodeSlow()
		{
			unchecked
			{
				return (from property in GetVisibleInstancePropertiesOrdered()
						select property.GetValue(this)
						into value
						select value?.GetHashCode() ?? 0)
					.Aggregate(0, (current, hash) => (current * 397) ^ hash);
			}
		}
	}
}
