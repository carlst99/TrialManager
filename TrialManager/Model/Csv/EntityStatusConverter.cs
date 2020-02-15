using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Serilog;
using System;
using TrialManager.Model.TrialistDb;

namespace TrialManager.Model.Csv
{
    internal class EntityStatusConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return EntityStatus.None;
                else
                    return Enum.Parse(typeof(EntityStatus), text);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not parse enum value");
                return EntityStatus.None;
            }
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((EntityStatus)value).ToString();
        }
    }
}
