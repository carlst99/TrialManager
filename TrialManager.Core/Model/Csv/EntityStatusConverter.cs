using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Model.Csv
{
    internal class EntityStatusConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text))
                return EntityStatus.Maiden;
            else
                return Enum.Parse(typeof(EntityStatus), text);
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((EntityStatus)value).ToString();
        }
    }
}
