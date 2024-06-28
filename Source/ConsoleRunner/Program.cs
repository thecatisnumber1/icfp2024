// See https://aka.ms/new-console-template for more information
using ConsoleRunner;
using Core;
using Runner;

if (args.Length == 0)
{
    Console.WriteLine("Welcome to CATCOM");
    Console.WriteLine("");
    Console.Write("> ");

    string? line;
    while ((line = Console.ReadLine()) != null)
    {
        line = line.Trim();

        if (line.StartsWith("send "))
        {
            string msg = line[5..^0].Trim();

            if (msg != "")
            {
                string icfp = Unparse(msg);
                Console.WriteLine("Sending: " + icfp);
                string icfpReply = Communicator.Send(icfp);
                Console.WriteLine("-----Begin raw reply-----");
                Console.WriteLine(icfpReply);
                Console.WriteLine("-----End raw reply-----");
                Console.WriteLine("-----Begin reply-----");
                Console.WriteLine(Expression.Parse(icfpReply).ToString() ?? "");
                Console.WriteLine("-----End reply-----");
            }
        }
        if (line.StartsWith("send-raw "))
        {
            string msg = line[9..^0].Trim();

            if (msg != "")
            {
                string icfpReply = Communicator.Send(msg);
                Console.WriteLine("-----Begin raw reply-----");
                Console.WriteLine(icfpReply);
                Console.WriteLine("-----End raw reply-----");
                Console.WriteLine("-----Begin reply-----");
                Console.WriteLine(Expression.Parse(icfpReply).ToString() ?? "");
                Console.WriteLine("-----End reply-----");
            }
        }
        else if (line.StartsWith("encode "))
        {
            string msg = line[7..^0].Trim();

            if (msg != "")
            {
                Console.WriteLine(Unparse(msg));
            }
        }
        else if (line.StartsWith("decode "))
        {
            string msg = line[7..^0].Trim();

            if (msg != "")
            {
                Console.WriteLine(Expression.Parse(msg).ToString());
            }
        }
        else if(line == "exit")
        {
            return;
        }
        else
        {
            Console.WriteLine("""
                Unknown command. Try:
                    send      Send a message to the server
                    send-raw  Send a raw ICFP message to the server
                    encode    Encode a string to ICFP
                    decode    Decode an ICFP string
                    exit      Exit
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

    foreach (string token in expr.Split(' '))
    {
        if (inStr)
        {
            if (token.EndsWith('"'))
            {
                str += " " + token[0..^1];
                inStr = false;
                strs.Add("S" + Str.Make(str).MachineValue);
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
            strs.Add("I" + new Integer(n).MachineValue);
        }
        else if (token.ToLower() == "false")
        {
            strs.Add("F");
        }
        else if (token.ToLower() == "true")
        {
            strs.Add("T");
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
                strs.Add("S" + Str.Make(token[1..^1]).MachineValue);
            }
            else
            {
                str += token[1..^0];
                inStr = true;
            }
        }
        else
        {
            strs.Add("S" + Str.Make(token).MachineValue);
        }
    }

    return string.Join(' ', strs);
}