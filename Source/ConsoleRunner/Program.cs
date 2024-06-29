// See https://aka.ms/new-console-template for more information
using ConsoleRunner;
using Core;
using Lib;
using Runner;
using System.Text;
using System.Text.RegularExpressions;

if (args.Length == 0)
{
    Console.WriteLine("Welcome to CATCOM");
    Console.WriteLine("");
    Console.Write("> ");

    Finder tasksFinder = new(Finder.GIT.GetRelativeDir("Tasks").FullName);
    Finder micfpFinder = new(Finder.GIT.GetRelativeDir("MICFP").FullName);

    string? line;
    while ((line = Console.ReadLine()) != null)
    {
        line = line.Trim();
        string cmd = line;

        if (cmd.StartsWith("exit"))
        {
            return;
        }

        int spaceIdx = line.IndexOf(' ');
        string cmdargs = "";

        if (spaceIdx > 0)
        {
            cmd = line[0..spaceIdx];
            cmdargs = line[(spaceIdx + 1)..].Trim();

            string[] splitArgs = cmdargs.Split(' ');

            // If the args are just "< file.txt", read args from that file
            if (splitArgs.Length == 2 && splitArgs[0] == "<")
            {
                // Search for the file recursively in Tasks
                var file = tasksFinder.FindFile(splitArgs[1]);
                file ??= micfpFinder.FindFile(splitArgs[1]);

                if (file == null)
                {
                    Console.WriteLine("No such file: " + splitArgs[1]);
                    continue;
                }

                cmdargs = File.ReadAllText(file.FullName);
            }
        }
        else if (cmd != "help" && cmd != "download")
        {
            Console.WriteLine("Enter command input, end with \"EOF\" on its own line");
            Console.WriteLine("-----");

            cmdargs = "";

            while ((line = Console.ReadLine())?.Trim() != "EOF")
            {
                cmdargs += line + "\n";
            }

            // Remove last newline
            cmdargs = cmdargs[..^1];
        }

        if (cmd == "send")
        {
            string icfp = Unparse(cmdargs);
            Console.WriteLine("Sending: " + icfp);
            string? icfpReply = Communicator.Send(icfp);

            if (icfpReply == null)
            {
                continue;
            }

            Console.WriteLine("-----Begin raw reply-----");
            Console.WriteLine(icfpReply);
            Console.WriteLine("-----End raw reply-----");
            Console.WriteLine("-----Begin reply-----");
            Console.WriteLine(Expression.Parse(icfpReply).Eval().ToString() ?? "");
            Console.WriteLine("-----End reply-----");
        }
        else if (cmd == "send-raw")
        {
            string? icfpReply = Communicator.Send(cmdargs);

            if (icfpReply == null)
            {
                continue;
            }

            Console.WriteLine("-----Begin raw reply-----");
            Console.WriteLine(icfpReply);
            Console.WriteLine("-----End raw reply-----");
            Console.WriteLine("-----Begin reply-----");
            Console.WriteLine(Expression.Parse(icfpReply).Eval().ToString() ?? "");
            Console.WriteLine("-----End reply-----");
        }
        else if (cmd == "encode")
        {
            Console.WriteLine(Unparse(cmdargs));
        }
        else if (cmd == "eval")
        {
            Console.WriteLine(Expression.Parse(cmdargs).Eval());
        }
        else if (cmd == "download" && cmdargs != "")
        {
            TaskDownloader.Download(cmdargs);
        }
        else if (cmd == "strings")
        {
            foreach (string token in cmdargs.Split(' '))
            {
                if (token.StartsWith('S') && token.Length > 1)
                {
                    Console.WriteLine(new Str(token[1..]));
                }
            }
        }
        else if (cmd == "vis")
        {
            Vis(cmdargs, false);
        }
        else if (cmd == "parse")
        {
            Vis(cmdargs, true);
        }
        else if (cmd == "unparse")
        {
            Console.WriteLine(new Regex(@"\s+").Replace(cmdargs, " "));
        }
        else if (cmd == "compile")
        {
            Console.WriteLine(MICFP.Compile(cmdargs));
        }
        else if (cmd.StartsWith("echo"))
        {
            // For normal echo, prefix command with U$ to convert number to string
            // because B. only works for string args
            string intToStr = (cmd == "echo") ? "U$ " : "";
            string? icfpReply = Communicator.Send("B. S%#(/} " + intToStr + cmdargs);

            if (icfpReply == null)
            {
                continue;
            }

            string reply = Expression.Parse(icfpReply).Eval().ToString() ?? "";
            string value = reply.Replace("You scored some points for using the echo service!", "").Trim();

            if (cmd == "echo")
            {
                // Undo the U$ we used by applying a U#
                Console.WriteLine(new Unary('#', Str.Make(value)).Eval());
            }
            else
            {
                Console.WriteLine(value);
            }
        }
        else
        {
            if (cmd != "help")
            {
                Console.WriteLine("Unknown command. Try:");
            }

            Console.WriteLine("""
                send <expr>          Send a message to the server
                send-raw <icfp>      Send a raw ICFP message to the server
                encode <expr>        Encode a string to ICFP
                eval <icfp>          Evals an ICFP string
                download <taskname>  Downloads a task and its problems
                strings <icfp>       Outputs all strings in the ICFP
                vis <icfp>           Outputs the expression's parse tree in a more readable view
                parse <icfp>         Outputs the expression's parse tree
                unparse              Returns parsed output to single-line
                compile <micfp>      Compiles MICFP to ICFP
                echo <icfp>          Evals a number-valued ICFP using the server's echo program
                echo-str <icfp>      Evals a string-valued ICFP using the server's echo program
                exit                 Exit

                Any command run without an argument will enter input mode, which is
                useful for entering multiline content.

                Any command accepting an argument can also accept a file by using <
                eval < spaceship1-raw.txt

                All such files are searched for recursively in the Tasks and MICFP directories.
            """);
        }

        Console.Write("> ");
    }

    return;
}

Start(args);

/// <summary>
/// The Console Runner is a thin client for running the program in the console.
/// It is little more than an EXE shim over the code in the Runner library.
/// It is used primarily for batch runs or prior to the Squigglizer being functional.
/// 
/// To run within Visual Studio, set ConsoleRunner as the startup project.
/// To provide arguments when running within Visual Studio
/// - Debug -> ConsoleRunner Debug Properties
/// - In Command line arguments textbox, provide the arguments you want to pass
/// - Now those arguments are provided whenever you F5 to run the code
/// </summary>
static void Start(string[] args)
{
    var logger = new RunLogger();
    var (solverName, rest) = Args.ParseArgs(args, logger);

    var run = new Run(solverName, rest, logger);
    run.RunUntilDone();
}

static string Unparse(string expr)
{
    List<string> strs = [];
    bool inStr = false;
    string str = "";
    bool inLambda = false;

    foreach (string token in expr.Split(' '))
    {
        if (inStr)
        {
            if (token.EndsWith('"'))
            {
                str += " " + token[0..^1];
                inStr = false;
                strs.Add(Str.Make(str).ToICFP());
                str = "";
            }
            else
            {
                str += " " + token;
            }

            continue;
        }

        if (long.TryParse(token, out long n))
        {
            strs.Add(new Integer(n).ToICFP());
        }
        else if (token.ToLower() == "false")
        {
            strs.Add(Bool.False.ToICFP());
        }
        else if (token.ToLower() == "true")
        {
            strs.Add(Bool.True.ToICFP());
        }
        else if (token.ToLower() == "if")
        {
            strs.Add("?");
        }
        else if (token.Length == 2 && (token[0] == 'U' || token[0] == 'B'))
        {
            strs.Add(token);
        }
        else if (token.StartsWith('"'))
        {
            if (token.EndsWith('"'))
            {
                strs.Add(Str.Make(token[1..^1]).ToICFP());
            }
            else
            {
                str += token[1..^0];
                inStr = true;
            }
        }
        else if (token.StartsWith('(') && token.EndsWith(')'))
        {

        }
        else
        {
            strs.Add(Str.Make(token).ToICFP());
        }
    }

    return string.Join(' ', strs);
}

static void Vis(string icfp, bool exact)
{
    try
    {
        VisExpr(Expression.Parse(icfp), 0, exact);
    }
    catch (Exception e)
    {
        Console.WriteLine("Problem parsing expression: " + e);
    }
}

static void VisExpr(Expression expr, int indent, bool exact)
{
    Console.Write(Indent(indent));
    string? simple = SimpleExpr(expr, exact);

    if (simple != null)
    {
        Console.WriteLine(simple);
    }
    else if (expr is Unary u)
    {
        string? simpleOp = SimpleExpr(u.Operand, exact);

        if (simpleOp != null)
        {
            Console.WriteLine($"U{u.Operator} {simpleOp}");
        }
        else
        {
            Console.WriteLine("U" + u.Operator);
            VisExpr(u.Operand, indent + 1, exact);
        }
    }
    else if (expr is Binary bin)
    {
        string? simpleLeft = SimpleExpr(bin.Left, exact);
        string? simpleRight = SimpleExpr(bin.Right, exact);

        if (simpleLeft != null && simpleRight != null)
        {
            Console.WriteLine($"B{bin.Operator} {simpleLeft} {simpleRight}");
        }
        else
        {
            Console.WriteLine("B" + bin.Operator);
            VisExpr(bin.Left, indent + 1, exact);
            VisExpr(bin.Right, indent + 1, exact);
        }
    }
    else if (expr is If iif)
    {
        Console.WriteLine(exact ? "?" : "if");
        VisExpr(iif.Condition, indent + 1, exact);
        VisExpr(iif.Then, indent + 1, exact);
        VisExpr(iif.Else, indent + 1, exact);
    }
    else if (expr is Lambda lam)
    {
        if (exact)
        {
            Console.WriteLine($"L{new Integer(lam.VariableKey).MachineValue}");
            VisExpr(lam.Content, indent + 1, exact);
        }
        else
        {
            Console.WriteLine($"lambda(v{lam.VariableKey}) {{");
            VisExpr(lam.Content, indent + 1, exact);
            Console.WriteLine(Indent(indent) + "}");
        }
    }
    else
    {
        Console.WriteLine("Unknown expr: " + expr.ToString());
    }
}

static string? SimpleExpr(Expression expr, bool exact)
{
    if (exact)
    {
        if (exact && SimpleExpr(expr, false) != null)
        {
            return expr.ToICFP();
        }
        else
        {
            return null;
        }
    }

    if (expr is Bool b)
    {
        return b.AsBool().ToString();
    }
    else if (expr is Integer n)
    {
        return n.AsInt().ToString();
    }
    else if (expr is Str s)
    {
        return "\"" + s.ToString() + "\"";
    }
    else if (expr is Variable v)
    {
        return $"v{v.Key}";
    }

    return null;
}

static string Indent(int indent)
{
    StringBuilder sb = new();

    for (int i = 0; i < indent; i++)
    {
        sb.Append("  ");
    }

    return sb.ToString();
}