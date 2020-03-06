using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Service.Core.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreZeroNumericProperties<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);

            List<PropertyInfo> numericPropertis = sourceType.GetProperties().ToList();
            foreach (PropertyInfo propertyInfo in numericPropertis)
            {
                string sourcePropertyName = propertyInfo.Name;
                Type sourcePropertyType = propertyInfo.PropertyType;

                if (!sourcePropertyType.IsNumericType()) continue;

                bool isTheSamePropertyExistInDestinationType = destinationType.GetProperties().Any(q => q.Name == sourcePropertyName);
                if (!isTheSamePropertyExistInDestinationType) continue;

                ParameterExpression parameterExpression = Expression.Parameter(sourceType, "c");
                MemberExpression memberExpression = Expression.Property(parameterExpression, sourcePropertyName);
                object value = Convert.ChangeType(0, sourcePropertyType);
                ConstantExpression constantExpression = Expression.Constant(value);
                Expression<Func<TSource, bool>> lambdaExpression = Expression.Lambda<Func<TSource, bool>>(Expression.GreaterThan(memberExpression, constantExpression), parameterExpression);
                Func<TSource, bool> func = lambdaExpression.Compile();

                expression.ForMember(sourcePropertyName, opt => opt.Condition(func));
            }
            return expression;
        }
    }
}
