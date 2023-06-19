using System.Data;
using Microsoft.Data.Sqlite;

namespace CloudyWing.DatabaseFacade.Tests {
    internal class ParameterCollectionTests {
        private ParameterCollection collection;

        [SetUp]
        public void SetUp() {
            collection = new ParameterCollection(new CommandExecutor());
        }

        [Test]
        public void Add_WithMetadata_ShouldAddToCollection() {
            ParameterMetadata metadata = new ParameterMetadata {
                ParameterName = "param1",
                Value = "value1"
            };

            collection.Add(metadata);

            collection.Should().Contain(metadata);
        }

        [Test]
        public void Add_WithNameAndValue_ShouldAddToCollection() {
            string parameterName = "param1";
            object value = "value1";

            collection.Add(parameterName, value);

            collection.Should().Contain(x => x.ParameterName == parameterName && x.Value == value);
        }

        [Test]
        public void Add_WithNameValueAndDbType_ShouldAddToCollection() {
            string parameterName = "param1";
            object value = "value1";
            DbType dbType = DbType.String;

            collection.Add(parameterName, value, dbType);

            collection.Should().Contain(x => x.ParameterName == parameterName && x.Value == value && x.DbType == dbType);
        }

        [Test]
        public void Add_WithNameValueDbTypeAndSize_ShouldAddToCollection() {
            string parameterName = "param1";
            object value = "value1";
            DbType dbType = DbType.String;
            int size = 50;

            collection.Add(parameterName, value, dbType, size);

            collection.Should().Contain(x => x.ParameterName == parameterName && x.Value == value && x.DbType == dbType && x.Size == size);
        }

        [Test]
        public void Add_WithNameValueDbTypePrecisionAndScale_ShouldAddToCollection() {
            string parameterName = "param1";
            object value = "value1";
            DbType dbType = DbType.Decimal;
            byte precision = 5;
            byte scale = 2;

            collection.Add(parameterName, value, dbType, precision, scale);

            collection.Should().Contain(x => x.ParameterName == parameterName && x.Value == value && x.DbType == dbType && x.Precision == precision && x.Scale == scale);
        }

        [Test]
        public void Add_WithNameValueDbTypeAndDirection_ShouldAddToCollection() {
            string parameterName = "param1";
            object value = "value1";
            DbType dbType = DbType.String;
            ParameterDirection direction = ParameterDirection.Output;

            collection.Add(parameterName, value, dbType, direction);

            collection.Should().Contain(x => x.ParameterName == parameterName && x.Value == value && x.DbType == dbType && x.Direction == direction);
        }

        [Test]
        public void Add_WithParameter_ShouldAddToCollection() {
            SqliteParameter parameter = new SqliteParameter {
                ParameterName = "param1",
                Value = "value1"
            };

            collection.Add(parameter);

            collection.Should().Contain(x => x.ParameterName == parameter.ParameterName && x.Value == parameter.Value);
        }

        [Test]
        public void Add_WithNullMetadata_ShouldThrowArgumentNullException() {
            ParameterMetadata? metadata = null;

            Action action = () => collection.Add(metadata);

            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Add_WithNullParameter_ShouldThrowArgumentNullException() {
            SqliteParameter? parameter = null;

            Action action = () => collection.Add(parameter);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
