using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Producer.Schemas
{
    public record Simple
    {
        public string ID { get; set; }
        public string Value { get; set; }
    }
}