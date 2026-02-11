using System;
using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace task4.Benchmarks
{
    [MemoryDiagnoser]
    public class EmailHtmlStripBenchmark
    {
        private string _largeHtml;
        private Regex _regex;
        [GlobalSetup]
        public void Setup()
        {
            // prepare a reasonably large HTML payload to simulate real email content
            var sb = new StringBuilder();
            var fragment = "<div><p>Hello <strong>user</strong>,</p><p>This is a <a href=\"#\">sample</a> email with <em>HTML</em> content.</p><ul><li>Line 1</li><li>Line 2</li></ul></div>";
            for (int i = 0; i < 5000; i++)
            {
                sb.Append(fragment);
            }

            _largeHtml = sb.ToString();
            _regex = new Regex("<[^>]+>", RegexOptions.Compiled);
        }

        [Benchmark(Baseline = true)]
        public string RegexStrip()
        {
            // current approach used in EmailSender: Regex.Replace(html, "<[^>]+>", string.Empty)
            return _regex.Replace(_largeHtml, string.Empty);
        }

        [Benchmark]
        public string CharScannerStrip()
        {
            // simple manual scanner that removes anything between '<' and '>'
            var sb = new StringBuilder(_largeHtml.Length);
            bool inTag = false;
            foreach (var ch in _largeHtml)
            {
                if (ch == '<')
                {
                    inTag = true;
                    continue;
                }

                if (ch == '>')
                {
                    inTag = false;
                    continue;
                }

                if (!inTag)
                    sb.Append(ch);
            }

            return sb.ToString();
        }
    }
}