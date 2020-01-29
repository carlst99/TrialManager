using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Model.Csv
{
    internal class EntityStatusConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text))
                return EntityStatus.None;
            else
                return Enum.Parse(typeof(EntityStatus), text);
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((EntityStatus)value).ToString();
        }
    }
}
