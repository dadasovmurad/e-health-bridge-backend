using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHealthBridgeAPI.Application.Extensions
{
    public static class NamingConventionExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var stringBuilder = new StringBuilder();
            var previousUpper = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (char.IsUpper(c))
                {
                    // Insert underscore before capital letter (not at the beginning)
                    if (i > 0 && !previousUpper)
                    {
                        stringBuilder.Append('_');
                    }

                    stringBuilder.Append(char.ToLower(c));
                    previousUpper = true;
                }
                else
                {
                    stringBuilder.Append(c);
                    previousUpper = false;
                }
            }

            return stringBuilder.ToString();
        }
    }
}
