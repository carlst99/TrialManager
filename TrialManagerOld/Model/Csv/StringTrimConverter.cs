using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TrialManager.Model.Csv
{
    internal class StringTrimConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return text?.Trim();
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return value.ToString();
        }
    }
}
