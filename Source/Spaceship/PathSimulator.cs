namespace Spaceship
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PathSimulator
    {
        public static void SimulateAndPrintPath(List<int> path)
        {
            var ship = new Ship();
            Console.WriteLine("Initial State: X=0, Y=0, VX=0, VY=0");

            for (int i = 0; i < path.Count; i++)
            {
                int command = path[i];
                ship.Move(command);
                Console.WriteLine($"After move {i + 1} (Command {command}): " +
                                  $"X={ship.X}, Y={ship.Y}, VX={ship.VX}, VY={ship.VY}");
            }

            Console.WriteLine($"Final Position: {ship.GetPosition()}");
        }
    }
}