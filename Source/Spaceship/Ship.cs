using System;
using System.Collections.Generic;
using System.Linq;

namespace Spaceship
{
    public class Ship
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int VX { get; private set; }
        public int VY { get; private set; }

        public Ship()
        {
            X = 0;
            Y = 0;
            VX = 0;
            VY = 0;
        }

        public void Move(int command)
        {
            // Update velocity based on command
            if (new[] { 7, 8, 9 }.Contains(command))
                VY++;
            else if (new[] { 1, 2, 3 }.Contains(command))
                VY--;

            if (new[] { 1, 4, 7 }.Contains(command))
                VX--;
            else if (new[] { 3, 6, 9 }.Contains(command))
                VX++;

            // Update position
            X += VX;
            Y += VY;
        }

        public (int, int) GetPosition() => (X, Y);
    }
}