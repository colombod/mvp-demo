using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Naminator
{
    /// <summary>
    ///     Generates values for test cases.
    /// </summary>
    /// <remarks>
    ///     This class was inspired and refactored from the Any class presented in the Microsoft TDD class.
    ///     <see href="http://mylearning/coursedetails.aspx?COURSENO=COUR2006051616181090700057" />
    ///     Additional materials for the class can be found here:
    ///     <see href="http://www.netobjectives.com/downloads/TDD_Course_Materials.html" />
    /// </remarks>

    public static partial class Any
    {
        public delegate void Trace(string method, object value);

        public static Trace TraceGeneratedValues = (method, value) => { };

        private const string CharsFrom0ToA = @":;<=>?@";
        private const string CharsFromZToa = @"[\]^`_";
        private const string CharsFrom0Toz = CharsFrom0ToA + CharsFromZToa;
        private const int LargestChar = 250;
        private const int SmallestChar = 32;

        /// <remarks>
        /// Since this is a static class and Random is not thread-safe, we use a ThreadLocal.
        /// </remarks>
        private static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random((int)DateTime.Now.Ticks));

        /// <summary>
        ///     Generates an alphanumeric char.
        /// </summary>
        /// <returns>A char.</returns>
        public static char AlphanumericChar()
        {
            char value;
            do
            {
                value = Char('0', 'z');
            } while (CharsFrom0Toz.IndexOf(value) != -1);

            TraceGeneratedValues("AlphanumericChar", value);

            return value;
        }

        /// <summary>
        ///     Generates a string.
        /// </summary>
        /// <param name="minLength">The minimum desired length.</param>
        /// <param name="maxLength">The maximum desired length.</param>
        public static string AlphanumericString(int minLength = 0, int? maxLength = null)
        {
            return String(minLength, maxLength, Characters.LatinLettersAndNumbers());
        }

        /// <summary>
        ///     Generate an ASCII char.
        /// </summary>
        public static char AsciiChar()
        {
            var value = (char)Int(SmallestChar, LargestChar);
            TraceGeneratedValues("AsciiChar", value);
            return value;
        }

        /// <summary>
        ///     Generate Bool Value
        /// </summary>
        public static bool Bool()
        {
            var value = Int(0, 1) == 1;
            TraceGeneratedValues("Bool", value);
            return value;
        }

        /// <summary>
        ///     Generate Byte Value
        /// </summary>
        public static byte Byte()
        {
            var value = (byte)Int(byte.MinValue, byte.MaxValue);
            TraceGeneratedValues("Byte", value);
            return value;
        }

        /// <summary>
        ///     Generate Char Value limited from below and from above
        /// </summary>
        /// <param name="first">Lower limit</param>
        /// <param name="last">Upper Limit</param>
        public static char Char(char first, char last)
        {
            var value = (char)Int(first, last);
            TraceGeneratedValues("Char", value);
            return value;
        }

        /// <summary>
        /// Generate a random <see cref="DateTimeOffset" />.
        /// </summary>
        public static DateTimeOffset DateTimeOffset(
            string min = "1970-01-01 00:00:00 +00:00",
            string max = "2050-01-01 00:00:00 +00:00",
            TimeSpan? offset = null)
        {
            var ticks = Long(System.DateTimeOffset.Parse(min).Ticks, System.DateTimeOffset.Parse(max).Ticks);

            var value = new DateTimeOffset(ticks,
                offset ?? TimeSpan(0, Int(-12, 12), 0, 0, 0));

            TraceGeneratedValues("DateTimeOffset", value.ToString());
            return value;
        }

        /// <summary>
        /// Generate a random <see cref="TimeSpan" />.
        /// </summary>
        public static TimeSpan TimeSpan(
            int? days = null,
            int? hours = null,
            int? minutes = null,
            int? seconds = null,
            int? milliseconds = null)
        {
            var value = new TimeSpan(days ?? Int(0, 1000),
                hours ?? Int(0, 23),
                minutes ?? Int(0, 59),
                seconds ?? Int(0, 59),
                milliseconds ?? Int(0, 999));
            TraceGeneratedValues("TimeSpan", value.ToString());
            return value;
        }

        /// <summary>
        ///     Generate a decimal.
        /// </summary>
        /// <param name="min">The minimum value to be generated.</param>
        /// <param name="max">The maximum value to be generated.</param>
        /// <returns>A decimal between the specified min and max values.</returns>
        public static decimal Decimal(decimal min = decimal.MinValue, decimal max = decimal.MaxValue)
        {
            var double0To1 = random.Value.NextDouble();
            var randomDouble = (double0To1 * (double)max) + ((1 - double0To1) * (double)min);
            var value = (decimal)randomDouble;
            TraceGeneratedValues("Decimal", value);
            return value;
        }

        /// <summary>
        ///     Generate a double.
        /// </summary>
        /// <param name="min">The minimum value to be generated.</param>
        /// <param name="max">The maximum value to be generated.</param>
        /// <returns>A double</returns>
        public static double Double(double min = double.MinValue, double max = double.MaxValue)
        {
            var double0To1 = random.Value.NextDouble();
            var randomDouble = (double0To1 * max) + ((1 - double0To1) * min);
            TraceGeneratedValues("Double", randomDouble);
            return randomDouble;
        }

        /// <summary>
        ///     Generates an email.
        /// </summary>
        /// <param name="topLevelDomain">The top level domain for the email.</param>
        /// <returns>An email</returns>
        public static string Email(string topLevelDomain = null)
        {
            var email = $"{Regex.Replace(FullName(), @"(""[^""""]+"")|([ .,]+)", @"")}@{DomainName(topLevelDomain)}";
            TraceGeneratedValues("Email", email);
            return email;
        }

        /// <summary>
        /// Generates a domain name
        /// </summary>
        /// <param name="topLevelDomain">The top level domain.</param>
        public static string DomainName(string topLevelDomain = null)
        {
            if (string.IsNullOrWhiteSpace(topLevelDomain))
            {
                topLevelDomain = TopLevelDomain();
            }

            return CamelCaseName(Int(1, 3)) + topLevelDomain;
        }

        /// <summary>
        /// Generates a company name
        /// </summary>
        public static string CompanyName()
        {
            string companyName = null;

            switch (Int(1, 5))
            {
                case 1:
                case 2:
                    companyName = string.Format(Names.CompanyNameWordFormats.RandomElement(),
                        Words(1, true).Single());
                    break;
                case 3:
                case 4:
                    companyName = string.Format(Names.CompanyNameFormats.RandomElement(),
                        string.Join(" ",
                            Enumerable.Range(1, Int(1, 2))
                                .SelectMany(_ => Words(1, true))));
                    break;
                case 5:
                    companyName =
                        $"{string.Join(", ", Enumerable.Range(1, Int(1, 3)).Select(_ => LastName()))} & {LastName()}";
                    break;
                default:
                    companyName = DomainName();
                    break;
            }

            if (Int(1, 3) == 1)
            {
                companyName = string.Format(Names.CompanyNameFormats.RandomElement(), companyName);
            }

            return companyName;
        }

        /// <summary>
        /// Returns a random top level domain.
        /// </summary>
        public static string TopLevelDomain()
        {
            return Names.TopLevelDomains.RandomElement();
        }

        /// <summary>
        ///     Generates a float
        /// </summary>
        /// <param name="min">The minimum value to be generated.</param>
        /// <param name="max">The maximum value to be generated.</param>
        public static float Float(float min = float.MinValue, float max = float.MaxValue)
        {
            var value = (float)Double(min, max);
            TraceGeneratedValues("Float", value);
            return value;
        }

        /// <summary>
        ///     Generates a guid
        /// </summary>
        public static Guid Guid()
        {
            var guid = System.Guid.NewGuid();
            TraceGeneratedValues("Guid", guid);
            return guid;
        }

        /// <summary>
        ///     Generate Integer Value limited from below and from above
        /// </summary>
        /// <param name="min">The minimum value to be generated.</param>
        /// <param name="max">The maximum value to be generated.</param>
        public static int Int(int min = int.MinValue, int max = int.MaxValue)
        {
            var val = random.Value.Next(min, max >= int.MaxValue ? int.MaxValue : max + 1);
            TraceGeneratedValues("Int", val);
            return val;
        }

        /// <summary>
        ///     Generates an IntPtr
        /// </summary>
        public static IntPtr IntPtr()
        {
            var value = (IntPtr)Int();
            TraceGeneratedValues("IntPtr", value);
            return value;
        }

        /// <summary>
        ///     Generate Long Value
        /// </summary>
        public static long Long(long min = long.MinValue, long max = long.MaxValue)
        {
            var nextDouble = random.Value.NextDouble();
            double range = max - min;

            var value = (long)(nextDouble * range) + min;
            TraceGeneratedValues("Long", value);
            return value;
        }

        /// <summary>
        ///     Generate Positive Integer Value limited from above
        /// </summary>
        /// <param name="max">Upper Limit</param>
        public static int PositiveInt(int max = int.MaxValue)
        {
            return Int(1, max);
        }

        /// <summary>
        ///     Returns a random value from the values of the given enumeration type.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration to select a value from.</typeparam>
        /// <returns>A random value from the values of the given enumeration type.</returns>
        /// <remarks>
        /// These is a way in IL to represent an enumeration constraint, but it's too complicated for just this usage
        /// see: http://msmvps.com/blogs/jon_skeet/archive/2009/09/10/generic-constraints-for-enums-and-delegates.aspx
        /// </remarks>
        public static TEnum EnumValue<TEnum>()
            where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException(string.Format("TEnum must be an enum type ({0} is not).", typeof(TEnum).FullName));
            }

            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray().RandomElement();
        }

        /// <summary>
        ///     Selects a random element from the specified collection.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A random element from the specified collection.</returns>
        public static T RandomElement<T>(this IEnumerable<T> source)
        {
            return source.RandomSequence(1).Single();
        }

        /// <summary>
        ///     Selects the specified number of elements randomly from the given sequence.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="numberOfElements">The number of elements to return.</param>
        /// <returns>The specified number of elements, selected randomly from the specified sequence.</returns>
        /// <remarks>
        /// Note that the returned elements are not distinct (the same element may be returned multiple times).
        /// </remarks>
        public static IEnumerable<T> RandomSequence<T>(this IEnumerable<T> source, int numberOfElements)
        {
            var sourceAsList = source.AsList();
            var lastItemIndex = sourceAsList.Count - 1;
            return Enumerable.Range(1, numberOfElements)
                .Select(_ => sourceAsList[Int(min: 0, max: lastItemIndex)]);
        }

        /// <summary>
        ///     Sorts the elements of a sequence in a random order.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>A sequence whose elements are sorted in a random order.</returns>
        public static IEnumerable<T> OrderByRandom<T>(this IEnumerable<T> source)
        {
            var sourceAsList = source.AsList();
            var indices = Enumerable.Range(0, sourceAsList.Count).ToArray();
            indices.Shuffle();
            return indices.Select(index => sourceAsList[index]);
        }


        /// <summary>
        ///     Selects the specified number of distinct elements randomly from the given sequence.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="numberOfElements">The number of elements to return.</param>
        /// <returns>The specified number of elements, selected randomly from the specified sequence.</returns>
        public static IEnumerable<T> RandomSubset<T>(this IEnumerable<T> source, int numberOfElements)
        {
            return source.OrderByRandom().Take(numberOfElements);
        }

        /// <summary>
        ///     Returns a sequence using the provided function to generate each item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="next">A function called to create each item in the sequence.</param>
        /// <param name="count">The number of items in the sequence.</param>
        public static IEnumerable<T> Sequence<T>(Func<int, T> next, int count = 5)
        {
            return Enumerable.Range(1, count).Select(next);
        }

        /// <summary>
        /// Generate a random common English words
        /// </summary>
        public static string Word()
        {
            return Words(1).Single();
        }

        /// <summary>
        /// Generate a sequence of common English words
        /// </summary>
        /// <param name="wordCount">Number of words to return</param>
        /// <param name="capitalize">True to capitalize the words</param>
        public static IEnumerable<string> Words(int wordCount = 4, bool capitalize = false)
        {
            var words = Naminator.Words.Common.RandomSequence(wordCount);
            if (capitalize)
            {
                words = words.Select(w => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(w));
            }
            return words;
        }

        /// <summary>
        /// Generate a CamelCaseName using <see cref="Words"/>
        /// </summary>
        /// <param name="wordCount">Number of words to compose together as the name</param>
        public static string CamelCaseName(int wordCount = 4)
        {
            return string.Join("", Words(wordCount, true));
        }

        /// <summary>
        /// Returns a random first name.
        /// </summary>
        public static string FirstName()
        {
            var firstName = Names.FirstNames.RandomElement();
            TraceGeneratedValues("FirstName", firstName);
            return firstName;
        }

        /// <summary>
        /// Returns a random full name, including possible titles and suffixes, because they're funny.
        /// </summary>
        public static string FullName()
        {
            var firstName = FirstName();
            string middle = "";

            switch (Int(1, 10))
            {
                case 1:
                case 2:
                case 3:
                    // middle initial
                    middle = Characters.LatinUppercase.RandomElement() + ". ";
                    break;
                case 4:
                case 5:
                    // middle name option 1
                    middle = FirstName();
                    break;
                case 6:
                case 7:
                case 8:
                    // middle name option 2
                    middle = Words(1, true).Single();
                    break;
                case 9:
                case 10:
                    // "nickname"
                    middle = "\"" + ((Int(1, 2) == 1 ? "The " : "")) + Words(1, true).Single() + "\"";

                    // sometimes nicknames come before the first name
                    if (Int(1, 4) == 1)
                    {
                        var temp = middle;
                        middle = firstName;
                        firstName = temp;
                    }
                    break;
            }

            var title = (Int(1, 10) == 1 ? Names.Titles.RandomElement() + " " : "");
            var suffix = (Int(1, 10) == 1 ? ", " + Names.Suffixes.RandomElement() : "");

            var fullName = string.Format("{0}{1} {2} {3}{4}", title, firstName, middle, LastName(), suffix).Replace("  ", " ");
            TraceGeneratedValues("FullName", fullName);
            return fullName;
        }

        /// <summary>
        /// Returns a random last name.
        /// </summary>
        public static string LastName()
        {
            var lastName = "";
            var prefixOrSuffixFreqPer10 = 1;

            switch (Int(1, 8))
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    // choose a name from Names.LastName
                    lastName = Names.LastNames.RandomElement();
                    prefixOrSuffixFreqPer10 = 2;
                    break;
                case 5:
                case 6:
                    // pick a word
                    lastName = Words(1, true).Single();
                    prefixOrSuffixFreqPer10 = 6;
                    break;
                case 7:
                case 8:
                    // make one up out of words!
                    lastName = Words(1, true).Single() + (Bool() ? Words(1).Single() : "");
                    prefixOrSuffixFreqPer10 = 0;
                    break;
            }

            if (Int(1, 10) <= prefixOrSuffixFreqPer10)
            {
                // some people have a prefix
                var prefix = Names.LastNamePrefixes.RandomElement();
                lastName = prefix + lastName;
            }

            if (Int(1, 10) <= prefixOrSuffixFreqPer10)
            {
                // some people have a suffix
                var suffix = Names.LastNameSuffixes.RandomElement();
                lastName = lastName + suffix;
            }

            TraceGeneratedValues("LastName", lastName);

            return lastName;
        }

        /// <summary>
        /// Generate a bunch of words space-delimited using <see cref="Words"/>
        /// </summary>
        /// <param name="wordCount">Number of words to compose together as the name</param>
        public static string Paragraph(int wordCount = 50)
        {
            return string.Join(" ", Words(wordCount));
        }

        /// <summary>
        ///     Generate Short Value
        /// </summary>
        public static short Short()
        {
            var value = (short)Int(short.MinValue, short.MaxValue);
            TraceGeneratedValues("Short", value);
            return value;
        }

        /// <summary>
        ///     Generates a random string.
        /// </summary>
        /// <param name="minLength">The minimum desired length.</param>
        /// <param name="maxLength">The maximum desired length. If null, a string of length 1 - 100 is returned.</param>
        /// <param name="characterSet">The character set from which the string is drawn. If null, <see cref="Characters.Unicode" /> is used.</param>
        /// <returns>
        ///     The generated string.
        /// </returns>
        public static string String(int minLength = 0, int? maxLength = null, IEnumerable<char> characterSet = null)
        {
            maxLength ??= (Int(1, 100) + minLength);
            characterSet ??= Characters.Unicode();

            var s = new string(Enumerable
                .Range(0, Int(minLength, maxLength.Value))
                .Select(i => characterSet.RandomElement()).ToArray());

            TraceGeneratedValues("String", s);

            return s;
        }

        /// <summary>
        /// Generates a random <see cref="Uri" />.
        /// </summary>
        /// <param name="scheme">The URI scheme.</param>
        /// <param name="tld">The URI's top-level domain.</param>
        /// <param name="allowQuerystring">If true, allow a random querystring to be part of the URI.</param>
        public static Uri Uri(
            string scheme = "http",
            string tld = null,
            bool allowQuerystring = true)
        {
            var builder = new UriBuilder(scheme)
            {
                Host = DomainName(tld),
                Path = string.Join("/", Words(Int(0, 5)))
            };

            if (allowQuerystring && Bool())
            {
                builder.Query = string.Join("&", Enumerable.Range(1, 10).Select(_ => string.Format("{0}={1}", Paragraph(1), Paragraph(1))));
            }

            return builder.Uri;
        }


    }
}