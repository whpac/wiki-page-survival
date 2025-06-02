using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Parsers
{
    internal static class PhpDeserializer
    {

        internal static object? Deserialize(string input)
        {
            int position = 0;
            var bytes = Encoding.UTF8.GetBytes(input);
            return DeserializeInner(bytes, ref position);
        }

        internal static bool TryDeserialize(string input, out object? result)
        {
            try
            {
                result = Deserialize(input);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        private static object? DeserializeInner(ReadOnlySpan<byte> input, ref int position)
        {
            byte type = input[position];

            // For nulls, it's the place where the next value starts,
            // For others it's the place where the value or metadata start.
            position += 2;

            if (type == 78) // 'N'
                return null;

            int valueStart = position;
            byte c;
            do
            {
                c = input[position];
                position++;
            } while (c != ';' && c != ':');

            var value = Encoding.UTF8.GetString(input[valueStart..(position - 1)]);

            switch (type)
            {
                case 78: // 'N'
                    return null;
                case 98: // 'b'
                    return value[0] == '1';
                case 105: // 'i
                    return int.Parse(value);
                case 100: // 'd'
                    return value switch
                    {
                        "NAN" => double.NaN,
                        "INF" => double.PositiveInfinity,
                        "-INF" => double.NegativeInfinity,
                        _ => (object)double.Parse(value),
                    };
                case 115: // 's'
                    // value is the length of the string
                    position++; // skip the opening quote
                    var bytes = input[position..(position+int.Parse(value))];
                    position += int.Parse(value) + 2; // skip the closing quote and the semicolon
                    return Encoding.UTF8.GetString(bytes);
                case 97: // 'a'
                    // value is the length of the array
                    Dictionary<object, object?> dict = [];
                    position++; // skip the opening brace
                    for (int i = 0; i < int.Parse(value); i++)
                    {
                        object? key = DeserializeInner(input, ref position);
                        object? val = DeserializeInner(input, ref position);

                        if (key is not int and not string)
                            throw new NotSupportedException("Only strings and integers can be used as array keys.");

                        dict[key] = val;
                    }
                    position++; // skip the closing brace
                    return dict;
                case 79: // 'O'
                    throw new NotSupportedException("Deserializing of PHP objects is not supported.");
                default:
                    throw new NotSupportedException($"Unknown type: {type}");
            }
        }
    }
}
