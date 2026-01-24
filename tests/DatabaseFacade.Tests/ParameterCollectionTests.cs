using System.Data;
using Microsoft.Data.Sqlite;

namespace CloudyWing.DatabaseFacade.Tests;
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
        SqliteParameter parameter = new SqliteParameter {
            ParameterName = "param1",
            Value = "value1"
        };

        collection.Add(parameter);

        Assert.That(collection, Has.Some.Matches<ParameterMetadata>(x => x.ParameterName == parameter.ParameterName && x.Value == parameter.Value));
    }

    [Test]
    public void Add_WithNullMetadata_ShouldThrowArgumentNullException() {
        ParameterMetadata metadata = null;

        Action action = () => collection.Add(metadata);

        Assert.Throws<ArgumentNullException>(() => action());
    }

    [Test]
    public void Add_WithNullParameter_ShouldThrowArgumentNullException() {
        SqliteParameter parameter = null;

        Action action = () => collection.Add(parameter);

        Assert.Throws<ArgumentNullException>(() => action());
    }
}
