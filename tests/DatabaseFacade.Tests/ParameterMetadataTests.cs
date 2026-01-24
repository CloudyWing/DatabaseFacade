using System.Data;
using Microsoft.Data.Sqlite;

namespace CloudyWing.DatabaseFacade.Tests;
internal class ParameterMetadataTests {
    private readonly ParameterMetadata sourceMetadata = new() {
        DbType = DbType.String,
        Direction = ParameterDirection.Input,
        ParameterName = "ParameterName",
        Precision = 1,
        Scale = 2,
        Size = 3,
        SourceColumn = "SourceColumn",
        SourceVersion = DataRowVersion.Current,
        Value = "Value"

    };

    [Test]
    [TestCase("@Name")]
    [TestCase(":Name")]
    [TestCase("?Name")]
    [TestCase("Name")]
    public void ParameterName_測試去除符號_開頭應該沒有符號(string parameterName) {
        ParameterMetadata metadata = new() {
            ParameterName = parameterName,
        };

        Assert.That(metadata.ParameterName, Is.EqualTo("Name"));
    }

    [Test]
    public void Constructor_ByParameterMetadata_應將建構參數的值套用至Properties() {
        ParameterMetadata destination = new(sourceMetadata);

        Assert.That(destination.DbType, Is.EqualTo(sourceMetadata.DbType));
        Assert.That(destination.Direction, Is.EqualTo(sourceMetadata.Direction));
        Assert.That(destination.ParameterName, Is.EqualTo(sourceMetadata.ParameterName));
        Assert.That(destination.Precision, Is.EqualTo(sourceMetadata.Precision));
        Assert.That(destination.Scale, Is.EqualTo(sourceMetadata.Scale));
        Assert.That(destination.Size, Is.EqualTo(sourceMetadata.Size));
        Assert.That(destination.SourceColumn, Is.EqualTo(sourceMetadata.SourceColumn));
        Assert.That(destination.SourceVersion, Is.EqualTo(sourceMetadata.SourceVersion));
        Assert.That(destination.Value, Is.EqualTo(sourceMetadata.Value));
    }

    [Test]
    public void Constructor_BySqliteParameter_應將建構參數的值套用至Properties() {
        SqliteParameter source = new() {
            DbType = DbType.String,
            Direction = ParameterDirection.Input,
            ParameterName = "ParameterName",
            Precision = 1,
            Scale = 2,
            Size = 3,
            SourceColumn = "SourceColumn",
            SourceVersion = DataRowVersion.Current,
            Value = "Value"

        };

        ParameterMetadata destination = new(source);

        Assert.That(destination.DbType, Is.EqualTo(source.DbType));
        Assert.That(destination.Direction, Is.EqualTo(source.Direction));
        Assert.That(destination.ParameterName, Is.EqualTo(source.ParameterName));
        Assert.That(destination.Precision, Is.EqualTo(source.Precision));
        Assert.That(destination.Scale, Is.EqualTo(source.Scale));
        Assert.That(destination.Size, Is.EqualTo(source.Size));
        Assert.That(destination.SourceColumn, Is.EqualTo(source.SourceColumn));
        Assert.That(destination.SourceVersion, Is.EqualTo(source.SourceVersion));
        Assert.That(destination.Value, Is.EqualTo(source.Value));
    }

    [Test]
    public void Clone_應複製出新的ParameterMetadata() {
        ParameterMetadata destination = sourceMetadata.Clone() as ParameterMetadata;

        Assert.That(destination?.DbType, Is.EqualTo(sourceMetadata.DbType));
        Assert.That(destination?.Direction, Is.EqualTo(sourceMetadata.Direction));
        Assert.That(destination?.ParameterName, Is.EqualTo(sourceMetadata.ParameterName));
        Assert.That(destination?.Precision, Is.EqualTo(sourceMetadata.Precision));
        Assert.That(destination?.Scale, Is.EqualTo(sourceMetadata.Scale));
        Assert.That(destination?.Size, Is.EqualTo(sourceMetadata.Size));
        Assert.That(destination?.SourceColumn, Is.EqualTo(sourceMetadata.SourceColumn));
        Assert.That(destination?.SourceVersion, Is.EqualTo(sourceMetadata.SourceVersion));
        Assert.That(destination?.Value, Is.EqualTo(sourceMetadata.Value));
    }

    [Test]
    public void ApplyParameter_應將ParameterMetadata的屬性套用到SqliteParameter() {
        SqliteParameter destination = new();
        sourceMetadata.ApplyParameter(destination);

        // SqliteParameter 的 Precision 和 Scale 只會為 0
        // SqliteParameter 的 SourceVersion 只會為 Default
        Assert.That(destination.DbType, Is.EqualTo(sourceMetadata.DbType));
        Assert.That(destination.Direction, Is.EqualTo(sourceMetadata.Direction));
        Assert.That(destination.ParameterName, Is.EqualTo(sourceMetadata.ParameterName));
        Assert.That(destination.Size, Is.EqualTo(sourceMetadata.Size));
        Assert.That(destination.SourceColumn, Is.EqualTo(sourceMetadata.SourceColumn));
        Assert.That(destination.Value, Is.EqualTo(sourceMetadata.Value));
    }
}
