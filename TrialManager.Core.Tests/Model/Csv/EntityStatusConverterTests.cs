using TrialManager.Core.Model.Csv;
using TrialManager.Core.Model.TrialistDb;
using Xunit;

namespace TrialManager.Core.Tests.Model.Csv
{
    public class EntityStatusConverterTests
    {
        [Fact]
        public void TestConvertFromString()
        {
            EntityStatusConverter converter = new EntityStatusConverter();
            Assert.Equal(EntityStatus.None, converter.ConvertFromString(null, null, null));

            const string value = nameof(EntityStatus.Open);
            Assert.Equal(EntityStatus.Open, converter.ConvertFromString(value, null, null));
        }

        [Fact]
        public void TestConvertToString()
        {
            EntityStatusConverter converter = new EntityStatusConverter();
            const string value = nameof(EntityStatus.Intermediate);
            Assert.Equal(value, converter.ConvertToString(EntityStatus.Intermediate, null, null));
        }
    }
}
