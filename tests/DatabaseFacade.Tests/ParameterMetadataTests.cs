using System.Data;
using Microsoft.Data.Sqlite;

namespace CloudyWing.DatabaseFacade.Tests {
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

            metadata.ParameterName.Should().Be("Name");
        }

        [Test]
        public void Constructor_ByParameterMetadata_應將建構參數的值套用至Properties() {
            ParameterMetadata destination = new(sourceMetadata);

            destination.DbType.Should().Be(sourceMetadata.DbType);
            destination.Direction.Should().Be(sourceMetadata.Direction);
            destination.ParameterName.Should().Be(sourceMetadata.ParameterName);
            destination.Precision.Should().Be(sourceMetadata.Precision);
            destination.Scale.Should().Be(sourceMetadata.Scale);
            destination.Size.Should().Be(sourceMetadata.Size);
            destination.SourceColumn.Should().Be(sourceMetadata.SourceColumn);
            destination.SourceVersion.Should().Be(sourceMetadata.SourceVersion);
            destination.Value.Should().Be(sourceMetadata.Value);
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

            destination.DbType.Should().Be(source.DbType);
            destination.Direction.Should().Be(source.Direction);
            destination.ParameterName.Should().Be(source.ParameterName);
            destination.Precision.Should().Be(source.Precision);
            destination.Scale.Should().Be(source.Scale);
            destination.Size.Should().Be(source.Size);
            destination.SourceColumn.Should().Be(source.SourceColumn);
            destination.SourceVersion.Should().Be(source.SourceVersion);
            destination.Value.Should().Be(source.Value);
        }

        [Test]
        public void Clone_應複製出新的ParameterMetadata() {
            ParameterMetadata? destination = sourceMetadata.Clone() as ParameterMetadata;

            destination?.DbType.Should().Be(sourceMetadata.DbType);
            destination?.Direction.Should().Be(sourceMetadata.Direction);
            destination?.ParameterName.Should().Be(sourceMetadata.ParameterName);
            destination?.Precision.Should().Be(sourceMetadata.Precision);
            destination?.Scale.Should().Be(sourceMetadata.Scale);
            destination?.Size.Should().Be(sourceMetadata.Size);
            destination?.SourceColumn.Should().Be(sourceMetadata.SourceColumn);
            destination?.SourceVersion.Should().Be(sourceMetadata.SourceVersion);
            destination?.Value.Should().Be(sourceMetadata.Value);
        }

        [Test]
        public void ApplyParameter_應將ParameterMetadata的屬性套用到SqliteParameter() {
            SqliteParameter destination = new();
            sourceMetadata.ApplyParameter(destination);

            // SqliteParameter 的 Precision 和 Scale 只會為 0
            // SqliteParameter 的 SourceVersion 只會為 Default
            destination.DbType.Should().Be(sourceMetadata.DbType);
            destination.Direction.Should().Be(sourceMetadata.Direction);
            destination.ParameterName.Should().Be(sourceMetadata.ParameterName);
            destination.Size.Should().Be(sourceMetadata.Size);
            destination.SourceColumn.Should().Be(sourceMetadata.SourceColumn);
            destination.Value.Should().Be(sourceMetadata.Value);
        }
    }
}
