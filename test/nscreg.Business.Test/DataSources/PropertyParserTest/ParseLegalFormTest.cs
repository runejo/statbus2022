using nscreg.Business.DataSources;
using nscreg.Data.Entities;
using Newtonsoft.Json;
using Xunit;

namespace nscreg.Business.Test.DataSources.PropertyParserTest
{
    public class ParseLegalFormTest
    {
        [Fact]
        private void ShouldParseSimilarJsonShape()
        {
            const string expected = "some_name";
            var raw = JsonConvert.SerializeObject(new LegalForm {Name = expected});

            var actual = PropertyParser.ParseLegalForm(raw);

            Assert.Equal(actual.Name, expected);
        }
    }
}