using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Model.Csv
{
    public class TrialistMapping : ClassMap<MappedTrialist>
    {
        public TrialistMapping()
        {
            Map(m => m.FullName).Index(1).Name("Full Name");
            Map(m => m.Status).Index(2).Name("Status");
            Map(m => m.Address).Index(3).Name("Address");
        }
    }
}
