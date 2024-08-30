using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Logging;
using Producer.Schemas;

namespace Models.Avro
{
    public class SimpleDeserializer() : IDeserializer<Simple>
    {
        const byte lineFeed = 0x0A;
        public Simple Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var values = new List<byte[]>();

            int i;
            while (true)
            {
                i = data.IndexOf(lineFeed);
                Console.WriteLine($"data is still: {BitConverter.ToString(data.ToArray())}");
                Console.WriteLine($"i is {i}");
                if (i <= 0)
                {
                    if (data.Length > 0)
                    {
                        values.Add(data.ToArray());
                    }
                    break;
                }

                values.Add(data.Slice(0, i).ToArray());
                if (data.Length > i)
                {
                    data = data.Slice(i + 1);
                }
            }
            if (values.Count != 3)
            {
                return new Simple();
            }
            return new Simple
            {
                ID = Encoding.ASCII.GetString(values[1]),
                Value = Encoding.ASCII.GetString(values[2]),
            };
        }
    }
}