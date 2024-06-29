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
                tokens.Add("""
                    L" B$ L# B$ v" B$ v# v# L# B$ v" B$ v# v#
                    """);
            }
            else if (line == "Z")
            {
                // Z combinator macro
                // This is equivalent to the Y combinator but uses the call-by-value B!
                tokens.Add("""
                    L" B! L# B! v" L$ B! B! v# v# v$ L# B! v" L$ B! B! v# v# v$
                    """);
            }
            else
            {
                tokens.Add(SPACES.Replace(line, " "));
            }
        }

        return string.Join(' ', tokens);
    }
}
