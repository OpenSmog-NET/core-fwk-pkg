using OS.Core.QueryParsing;
using Shouldly;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace OS.Core.UnitTests
{
    public class PropertyInfoExtensionsTests
    {
        public class GivenIsExtension
        {
            [Fact]
            public void WhenApplyingIsIntOnAnIntProperty_ThenResultShouldBeTrue()
            {
                // Arrange
                var @object = new TestClass();

                var property = Property(() => @object.IntProperty);
                var nullableProperty = Property(() => @object.NullableIntProperty);

                // Act & Assert
                property.Is<int>().ShouldBe(true);
                nullableProperty.Is<int>().ShouldBe(true);
            }

            [Fact]
            public void WhenApplyingIsIntOnAnNonIntProperty_ThenResultShouldBeFalse()
            {
                // Arrange
                var @object = new TestClass();
                var property = Property(() => @object.LongProperty);

                // Act & Assert
                property.Is<int>().ShouldBe(false);
            }

            [Fact]
            public void WhenApplyingIsLongOnALongProperty_ThenResultShouldBeTrue()
            {
                // Arrange
                var @object = new TestClass();
                var property = Property(() => @object.LongProperty);
                var nullableProperty = Property(() => @object.NullableLongProperty);

                // Act & Assert
                property.Is<long>().ShouldBe(true);
                nullableProperty.Is<int>().ShouldBe(false);
            }

            [Fact]
            public void WhenApplyingIsLongOnANonLongProperty_ThenResultShouldBeFalse()
            {
                // Arrange
                var @object = new TestClass();
                var property = Property(() => @object.IntProperty);

                // Act & Assert
                property.Is<long>().ShouldBe(false);
            }

            [Fact]
            public void WhenApplyingIsDecimalOnADecimalProperty_ThenResultShouldBeTrue()
            {
                // Arrange
                var @object = new TestClass();
                var property = Property(() => @object.DecimalProperty);
                var nullableProperty = Property(() => @object.NullableDecimalProperty);

                // Act & Assert
                property.Is<decimal>().ShouldBe(true);
            }

            [Fact]
            public void WhenApplyingIsDecimalOnANonDecimalProperty_ThenResultShouldBeFalse()
            {
                var @object = new TestClass();

                var property = Property(() => @object.IntProperty);
                property.Is<decimal>().ShouldBe(false);
            }

            [Fact]
            public void WhenApplyingIsDateTimeOnADateTimeProperty_ThenResultShouldBeTrue()
            {
                // Arrange
                var @object = new TestClass();
                var property = Property(() => @object.DateTimeProperty);
                var nullableProperty = Property(() => @object.NullableDateTimeProperty);

                // Act & Assert
                property.Is<DateTime>().ShouldBe(true);
                nullableProperty.Is<DateTime>().ShouldBe(true);
            }

            [Fact]
            public void WhenApplyingIsDateTimeOnANonDateTimeProperty_ThenResultShouldBeFalse()
            {
                var @object = new TestClass();

                var property = Property(() => @object.IntProperty);
                property.Is<DateTime>().ShouldBe(false);
            }

            [Fact]
            public void WhenApplyingIsEnumOnAnEnumProperty_ThenResultShouldBeTrue()
            {
                // Arrange
                var @object = new TestClass();
                var property = Property(() => @object.EnumProperty);
                var nullableProperty = Property(() => @object.NullableEnumProperty);

                // Act & Assert
                property.IsEnum().ShouldBe(true);
                property.Is<TestEnum>().ShouldBe(true);
                nullableProperty.IsNullableEnum().ShouldBe(true);
                nullableProperty.Is<TestEnum>().ShouldBe(true);
            }

            [Fact]
            public void WhenApplyingIsEnumOnAnNonEnumProperty_ThenResultShouldBeFalse()
            {
                // Arrange
                var @object = new TestClass();
                var property = Property(() => @object.IntProperty);
                var nullableProperty = Property(() => @object.NullableIntProperty);

                // Act & Assert
                property.IsEnum().ShouldBe(false);
                nullableProperty.IsNullableEnum().ShouldBe(false);
            }
        }

        private class TestClass
        {
            public int IntProperty { get; set; }
            public int? NullableIntProperty { get; set; }

            public long LongProperty { get; set; }
            public long? NullableLongProperty { get; set; }

            public decimal DecimalProperty { get; set; }
            public decimal? NullableDecimalProperty { get; set; }

            public DateTime DateTimeProperty { get; set; }
            public DateTime? NullableDateTimeProperty { get; set; }

            public TestEnum EnumProperty { get; set; }
            public TestEnum? NullableEnumProperty { get; set; }
        }

        private enum TestEnum
        {
            TestValue1,
            TestValue2
        }

        public static string PropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var property = Property<T>(propertyExpression);

            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
            }

            var getMethod = property.GetGetMethod(true);

            if (getMethod == null)
            {
                // This shouldn't happen - the expression would reject the property before reaching this far.
                throw new ArgumentException("The referenced property does not have a get method.", "propertyExpression");
            }

            if (getMethod.IsStatic)
            {
                throw new ArgumentException("The referenced property is a static property.", "propertyExpression");
            }

            return property.Name;
        }

        public static PropertyInfo Property<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
            }

            return memberExpression.Member as PropertyInfo;
        }
    }
}