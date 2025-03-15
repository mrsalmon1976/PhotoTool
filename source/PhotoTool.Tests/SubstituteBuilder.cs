using AutoBogus;
using NSubstitute;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace PhotoTool.Test
{
    public class SubstituteBuilder<T> where T : class
    {
        private T _instance;

        public SubstituteBuilder()
        {
            _instance = Substitute.For<T>();
        }

        public T Build()
        {
            return _instance;
        }

        public SubstituteBuilder<T> WithProperty<TValue>(Expression<Func<T, TValue>> memberLamda, TValue value)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    if (!property.CanWrite)
                    {
                        throw new InvalidOperationException("Unable to set property as it is readonly");
                    }

                    if (value == null)
                    {
                        property.SetValue(_instance, value, null);
                    }
                    else if (property.PropertyType.IsEnum)
                    {
                        property.SetValue(_instance, Enum.Parse(property.PropertyType, value.ToString()!));
                    }
                    else
                    {
                        property.SetValue(_instance, value, null);
                    }
                }
            }

            return this;
        }

        public SubstituteBuilder<T> WithRandomProperties()
        {
            var faker = new AutoFaker<T>();
            faker.Populate(_instance);
            return this;
        }


    }
}