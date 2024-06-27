// See https://aka.ms/new-console-template for more information
using ConsoleRunner;
using Runner;

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