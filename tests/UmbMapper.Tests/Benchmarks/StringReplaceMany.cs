using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace UmbMapper.Tests.Benchmarks
{
    public class StringReplaceMany
    {
        private static string text = "1,2.3:4&5#6";

        private static char[] chars = new[] { ',', '.', ':', '&', '#' };

        private static char replacement = '*';

        private static Dictionary<string, string> replacements = new Dictionary<string, string>(5)
        {
            {",", "*" },
            {".", "*" },
            {":", "*" },
            {"&", "*" },
            {"#", "*" },
        };

        [Benchmark(Description = "Umbraco String.ReplaceMany", Baseline = true)]
        public string UmbracoReplaceMany()
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (chars == null)
            {
                throw new ArgumentNullException("chars");
            }

            string result = text;
            return chars.Aggregate(result, (current, c) => current.Replace(c, replacement));
        }

        [Benchmark(Description = "UmbMapper String.ReplaceMany")]
        public string UmbMapperReplaceMany()
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (chars == null)
            {
                throw new ArgumentNullException("chars");
            }

            string result = text;
            for (int i = 0; i < chars.Length; i++)
            {
                result = result.Replace(chars[i], replacement);
            }

            return result;
        }

        [Benchmark(Description = "Umbraco String.ReplaceMany Dictionary")]
        public string UmbracoReplaceManyDictionary()
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (replacements == null)
            {
                throw new ArgumentNullException("replacements");
            }

            return replacements.Aggregate(text, (current, kvp) => current.Replace(kvp.Key, kvp.Value));
        }

        [Benchmark(Description = "UmbMapper String.ReplaceMany Dictionary")]
        public string UmbMapperReplaceManyDictionary()
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (replacements == null)
            {
                throw new ArgumentNullException("replacements");
            }

            string result = text;
            foreach (KeyValuePair<string, string> item in replacements)
            {
                result = result.Replace(item.Key, item.Value);
            }

            return result;
        }
    }
}