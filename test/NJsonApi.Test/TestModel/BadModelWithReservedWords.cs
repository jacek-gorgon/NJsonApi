using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Test.TestModel
{
    public class BadModelWithReservedWords
    {
        public string Relationships { get; set; }

        public string Links { get; set; }
    }
}
