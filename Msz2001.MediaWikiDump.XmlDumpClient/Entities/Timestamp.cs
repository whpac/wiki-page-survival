using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Entities
{
    public readonly struct Timestamp(DateTime value)
    {
        private readonly DateTime Value = value.ToUniversalTime();
        public static Timestamp Infinite => new(DateTime.MaxValue);

        public bool IsInfinite => Value == DateTime.MaxValue;

        public static Timestamp Parse(string value)
        {
            if (IsInfiniteDesignator(value))
                return Infinite;
            return new Timestamp(DateTime.Parse(value, CultureInfo.InvariantCulture));
        }

        public static bool TryParse(string value, out Timestamp result)
        {
            if (IsInfiniteDesignator(value))
            {
                result = Infinite;
                return true;
            }
            else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, out DateTime dt))
            {
                result = new Timestamp(dt);
                return true;
            }
            result = default;
            return false;
        }

        /// <summary>
        /// Converts the timestamp to a string in the format "yyyy-MM-ddTHH:mm:ssZ".
        /// </summary>
        /// <returns>The timestamp in a long format</returns>
        public override string ToString()
        {
            return Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the timestamp to a string in the format "yyyyMMddHHmmss".
        /// </summary>
        /// <returns>The timestamp is the "MediaWiki database" format</returns>
        public string ToShortString()
        {
            return Value.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        private static bool IsInfiniteDesignator(string value)
        {
            return value == "infinite" || value == "indefinite" || value == "infinity" || value == "never";
        }

        public override bool Equals([NotNullWhen(true)] object? obj) => Value.Equals(obj);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator >(Timestamp left, Timestamp right) => left.Value > right.Value;
        public static bool operator >=(Timestamp left, Timestamp right) => left.Value >= right.Value;
        public static bool operator <(Timestamp left, Timestamp right) => left.Value < right.Value;
        public static bool operator <=(Timestamp left, Timestamp right) => left.Value <= right.Value;
        public static bool operator ==(Timestamp left, Timestamp right) => left.Value == right.Value;
        public static bool operator !=(Timestamp left, Timestamp right) => left.Value != right.Value;

        public static Timestamp operator +(Timestamp left, TimeSpan right) =>
            left.IsInfinite ? Infinite : new(left.Value + right);
        public static Timestamp operator -(Timestamp left, TimeSpan right) =>
            left.IsInfinite ? Infinite : new(left.Value - right);

        public static TimeSpan operator -(Timestamp left, Timestamp right) =>
            left.IsInfinite && right.IsInfinite ? TimeSpan.Zero :
            left.IsInfinite ? TimeSpan.MaxValue :
            right.IsInfinite ? TimeSpan.MinValue :
            left.Value - right.Value;

        public static implicit operator DateTime(Timestamp timestamp) => timestamp.Value;
    }
}
