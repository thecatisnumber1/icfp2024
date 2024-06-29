// See https://aka.ms/new-console-template for more information
using ConsoleRunner;
using Core;
using Lib;
using Runner;

if (args.Length == 0)
{
    Console.WriteLine("Welcome to CATCOM");
    Console.WriteLine("");
    Console.Write("> ");

    Finder tasksFinder = new(Finder.GIT.GetRelativeDir("Tasks").FullName);

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
        string cmdargs;

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

                if (file == null)
                {
                    Console.WriteLine("No such file in Tasks: " + splitArgs[1]);
                    continue;
                }

                cmdargs = File.ReadAllText(file.FullName);
            }
        }
        else
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
        else if (cmd == "decode")
        {
            Console.WriteLine(Expression.Parse(cmdargs).Eval().ToString());
        }
        else if (cmd == "download")
        {
            TaskDownloader.Download(cmdargs);
        }
        else if (cmd == "strings")
        {
            foreach (string token in cmdargs.Split(' '))
            {
                if (token.StartsWith('S') && token.Length > 1)
                {
                    Console.WriteLine(new Str(token[1..]).ToString());
                }
            }
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

                Any command run without an argument will enter input mode, which is
                useful for entering multiline content.

                Any command accepting an argument can also accept a file by using <
                decode < spaceship1-raw.txt

                All such files are searched for recursively in the Tasks directory.
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