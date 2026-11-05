using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.Sqlite;

namespace CloudyWing.DatabaseFacade.Tests {
    [TestFixture]
    internal class ParameterCollectionTests {
        private ParameterCollection collection;

        [SetUp]
        [MemberNotNull(nameof(collection))]
        public void SetUp() {
            collection = new ParameterCollection(new CommandExecutor());
        }

        [Test]
        public void Add_WithMetadata_ShouldAddToCollection() {
            ParameterMetadata metadata = new() {
                ParameterName = "param1",
                Value = "value1"
            };

            collection.Add(metadata);

            Assert.That(collection, Does.Contain(metadata));
        }

        [Test]
        public void Add_WithNameAndValue_ShouldAddToCollection() {
            string parameterName = "param1";
            object value = "value1";

            collection.Add(parameterName, value);

            Assert.That(collection, Has.Some.Matches<ParameterMetadata>(x => x.ParameterName == parameterName && x.Value == value));
        }

        [Test]
        public void Add_WithNameValueAndDbType_ShouldAddToCollection() {
            string parameterName = "param1";
            object value = "value1";
            DbType dbType = DbType.String;

            collection.Add(parameterName, value, dbType);

            Assert.That(collection, Has.Some.Matches<ParameterMetadata>(x => x.ParameterName == parameterName && x.Value == value && x.DbType == dbType));
        }

        [Test]
        public void Add_WithNameValueDbTypeAndSize_ShouldAddToCollection() {
            string parameterName = "param1";
            object value = "value1";
            DbType dbType = DbType.String;
            int size = 50;

            collection.Add(parameterName, value, dbType, size);

            Assert.That(collection, Has.Some.Matches<ParameterMetadata>(x => x.ParameterName == parameterName && x.Value == value && x.DbType == dbType && x.Size == size));
        }

        [Test]
        public void Add_WithNameValueDbTypePrecisionAndScale_ShouldAddToCollection() {
            string parameterName = "param1";
            object value = "value1";
            DbType dbType = DbType.Decimal;
            byte precision = 5;
            byte scale = 2;

            collection.Add(parameterName, value, dbType, precision, scale);

            Assert.That(collection, Has.Some.Matches<ParameterMetadata>(x => x.ParameterName == parameterName && x.Value == value && x.DbType == dbType && x.Precision == precision && x.Scale == scale));
        }

        [Test]
        public void Add_WithNameValueDbTypeAndDirection_ShouldAddToCollection() {
            string parameterName = "param1";
            object value = "value1";
            DbType dbType = DbType.String;
            ParameterDirection direction = ParameterDirection.Output;

            collection.Add(parameterName, value, dbType, direction);

            Assert.That(collection, Has.Some.Matches<ParameterMetadata>(x => x.ParameterName == parameterName && x.Value == value && x.DbType == dbType && x.Direction == direction));
        }

        [Test]
        public void Add_WithParameter_ShouldAddToCollection() {
            SqliteParameter parameter = new() {
                ParameterName = "param1",
                Value = "value1"
            };

            collection.Add(parameter);

            Assert.That(collection, Has.Some.Matches<ParameterMetadata>(x => x.ParameterName == parameter.ParameterName && x.Value == parameter.Value));
        }

        [Test]
        public void Add_WithNullMetadata_ShouldThrowArgumentNullException() {
            ParameterMetadata? metadata = null;

            Assert.That(() => collection.Add(metadata), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Add_WithNullParameter_ShouldThrowArgumentNullException() {
            SqliteParameter? parameter = null;

            Assert.That(() => collection.Add(parameter), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCase(42L, 42)]
        [TestCase("42", 42)]
        [TestCase(42.0, 42)]
        public void GetValue_WhenValueTypeDiffers_ShouldConvertValue(object value, int expected) {
            collection.Add("Total", value);

            int result = collection.GetValue<int>("Total");

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetValue_WhenTargetIsEnum_ShouldConvertValue() {
            collection.Add("Direction", 2L);

            ParameterDirection result = collection.GetValue<ParameterDirection>("Direction");

            Assert.That(result, Is.EqualTo(ParameterDirection.Output));
        }

        [Test]
        public void GetValue_WhenTargetIsGuidAndValueIsString_ShouldParseValue() {
            Guid expected = Guid.NewGuid();
            collection.Add("Id", expected.ToString());

            Guid? result = collection.GetValue<Guid?>("Id");

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetValue_WhenValueCannotBeConverted_ShouldThrowInvalidCastException() {
            collection.Add("Total", "Wing");

            Assert.That(() => collection.GetValue<int>("Total"), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void GetValue_WhenParameterNotFound_ShouldThrowKeyNotFoundException() {
            Assert.That(() => collection.GetValue<int>("Total"), Throws.TypeOf<KeyNotFoundException>());
        }
    }
}
