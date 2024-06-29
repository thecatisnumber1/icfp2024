using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core;

/// <summary>
/// Compiler for MICFP, which is just ICFP with macros, comments, and whitespace.
/// </summary>
public static class MICFP
{
    private static readonly Regex NEWLINES = new(@"[\r\n]+");
    private static readonly Regex SPACES = new(@"\s+");

    private const string Y_ICFP = """
        L" B$ L# B$ v" B$ v# v# L# B$ v" B$ v# v#
        """;

    private const string Z_ICFP = """
        L" B! L# B! v" L$ B! B! v# v# v$ L# B! v" L$ B! B! v# v# v$
        """;

    private const string REPEAT_ICFP = """
        L" L# ? B= v# I! S B. {STR} B$ v" B- v# I" {N}
        """;

    /// <summary>
    /// Compiles MICFP to ICFP
    /// </summary>
    public static string Compile(string micfp)
    {
        List<string> tokens = [];

        foreach (string rawline in NEWLINES.Split(micfp))
        {
            string line = rawline.Trim();

            // Ignore blank lines and comments
            if(line == "" || line.StartsWith("//"))
            {
                continue;
            }

            if (line == "Y")
            {
                // Y combinator macro
                tokens.Add(Y_ICFP);
            }
            else if (line == "Z")
            {
                // Z combinator macro
                // This is equivalent to the Y combinator but uses the call-by-value B!
                tokens.Add(Z_ICFP);
            }
            else if (line.StartsWith("REPEAT"))
            {
                // REPEAT {str} {n}
                // where str is the string to repeat
                // and n is the times to repeat it
                string[] parts = SPACES.Split(line);
                string str = parts[1];
                string strIcfp = Shorthand.S(str).ToICFP();
                long n = long.Parse(parts[2]);
                string nIcfp = Shorthand.I(n).ToICFP();

                string repeatIcfp = "B$ B$ "+ Y_ICFP + " " + REPEAT_ICFP.Replace("{STR}", strIcfp).Replace("{N}", nIcfp);

                if (1 + (str.Length * n) < repeatIcfp.Length)
                {
                    // If just repeating the string would be shorter than the ICFP
                    // to repeat it, just repeat the string
                    string repeated = "";
                    for (int i = 0; i < n; i++)
                    {
                        repeated += str;
                    }

                    tokens.Add(Shorthand.S(repeated).ToICFP());
                }
                else
                {
                    tokens.Add(repeatIcfp);
                }
            }
            else
            {
                tokens.Add(SPACES.Replace(line, " "));
            }
        }

        return string.Join(' ', tokens);
    }
}
