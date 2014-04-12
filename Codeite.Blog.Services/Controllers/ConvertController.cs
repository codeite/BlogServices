using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace Codeite.Blog.Services.Controllers
{
    public class ConvertController : ApiController
    {
        private static readonly Regex _dissalowedChars;
        private readonly static TextInfo _textInfo;
        private readonly static Dictionary<char, string> _numbers;

        static ConvertController()
        {
            _dissalowedChars = new Regex("[^a-zA-Z]");
            _textInfo = new CultureInfo("en-US").TextInfo;
            _numbers = new Dictionary<char, string>
            {
                {'1', "one"},
                {'2', "two"},
                {'3', "three"},
                {'4', "four"},
                {'5', "five"},
                {'6', "six"},
                {'7', "seven"},
                {'8', "eight"},
                {'9', "nine"},
                {'0', "zero"},
            };
        }

        public ConvertController()
        {
            
        }

        public string Post()
        {
            return Post("camel");
        }

        // POST api/convert
        public string Post(string id)
        {
            var value = Request.Content.ReadAsStringAsync().Result;

            if (id.Equals("line-num", StringComparison.InvariantCultureIgnoreCase))
            {
                return AddLineNumbers(value);
            }

            return ConvertToCamelCase(value);
        }

        private string AddLineNumbers(string value)
        {
            var lines = value.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

            return string.Join("\n", lines.Select((x, i) => (i+1) + ". " + x));
        }

        private string ConvertToCamelCase(string value)
        {
            value = ReplaceNumbers(value.ToLower());

            var lines = value
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line =>
                {
                    var words = _dissalowedChars.Split(line).Where(x=>x!="").ToList();

                    var first = words.First().ToLower();
                    var rest = words.Skip(1).Select(x => _textInfo.ToTitleCase(x));

                    return first + string.Join("", rest);
                });


            return string.Join("\n", lines);
        }

        private static string ReplaceNumbers(string s)
        {
            var chars = s.ToArray().Select(c => _numbers.ContainsKey(c) ? " "+_numbers[c]+" " : c+"");

            return string.Join("", chars);
        }
    }
}
