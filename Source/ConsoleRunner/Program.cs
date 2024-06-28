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
                Console.WriteLine(Expression.Parse(icfpReply).Eval([]).ToString() ?? "");
                Console.WriteLine("-----End reply-----");
            }
        }
        else if (line.StartsWith("send-raw "))
        {
            string msg = line[9..^0].Trim();

            if (msg != "")
            {
                string icfpReply = Communicator.Send(msg);
                Console.WriteLine("-----Begin raw reply-----");
                Console.WriteLine(icfpReply);
                Console.WriteLine("-----End raw reply-----");
                Console.WriteLine("-----Begin reply-----");
                Console.WriteLine(Expression.Parse(icfpReply).Eval([]).ToString() ?? "");
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
                Console.WriteLine(Expression.Parse(msg).Eval([]).ToString());
            }
        }
        else if (line.StartsWith("download "))
        {
            string taskName = line[8..^0].Trim();

            if (taskName != "")
            {
                TaskDownloader.Download(taskName);
            }
        }
        else if (line.StartsWith("strings "))
        {
            string msg = line[6..^0].Trim();

            if (msg != "")
            {
                foreach (string token in msg.Split(' '))
                {
                    if (token.StartsWith("S"))
                    {
                        Console.WriteLine(new Str(token).ToString());
                    }
                }
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
                send <expr>          Send a message to the server
                send-raw <icfp>      Send a raw ICFP message to the server
                encode <expr>        Encode a string to ICFP
                decode <icfp>        Decode an ICFP string
                download <taskname>  Downloads a task and its problems
                strings <icfp>       Outputs all strings in the ICFP
                exit                 Exit
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