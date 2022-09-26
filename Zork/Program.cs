using System;
using System.Collections.Generic;

namespace Zork
{
    class Program
    {
        public static Room CurrentRoom
        {
            get
            {
                return _rooms[_location.Row, _location.Column];
            }
        }

        static void Main(string[] args)
        {
            InitializeRoomDescriptions();
            Console.WriteLine("Welcome to Zork!");

            Commands command = Commands.UNKNOWN;

            while (command != Commands.QUIT)
            {
                Console.Write(CurrentRoom);

                Console.Write("\n> ");
                string outputString = null;

                command = ToCommand(Console.ReadLine().Trim());

                switch (command)
                {
                    case Commands.QUIT:
                        outputString = "Thank you for playing!";
                        break;

                    case Commands.LOOK:
                        Console.WriteLine(CurrentRoom.Description);
                        break;

                    case Commands.NORTH:
                    case Commands.SOUTH:
                    case Commands.EAST:
                    case Commands.WEST:
                        if (Move(command))
                        {
                            outputString = $"You moved {command}.";
                        }
                        else
                        {
                            outputString = "The way is shut!";
                        }
                        break;

                    default:
                        outputString = "Unknown command.";
                        break;
                }
                if (outputString != null)
                {
                    Console.WriteLine(outputString);
                }
            }
        }
        private static Commands ToCommand(string commandString) => (Enum.TryParse<Commands>(commandString, true, out Commands result) ? result : Commands.UNKNOWN);
        private static bool Move(Commands command)
        {
            bool didMove = false;

            switch (command)
            {
                case Commands.NORTH when _location.Row < _rooms.GetLength(0) - 1:
                    _location.Row++;
                    didMove = true;
                    break;

                case Commands.SOUTH when _location.Row > 0:
                    _location.Row--;
                    didMove = true;
                    break;

                case Commands.EAST when _location.Column < _rooms.GetLength(1) - 1:
                    _location.Column++;
                    didMove = true;
                    break;

                case Commands.WEST when _location.Column > 0:
                    _location.Column--;
                    didMove = true;
                    break;

            }

            return didMove;
        }

        private static void InitializeRoomDescriptions()
        {
            _rooms[0, 0].Description = "You are on a rock-strewn trail.";
            _rooms[0, 1].Description = "You are facing the south side of a white house. There is no door here, and all the windows are barred.";
            _rooms[0, 1].Description = "You are at the top of the Great Canyon on its south wall.";

            _rooms[1, 0].Description = "This is a forest, with trees in all directions around you.";
            _rooms[1, 1].Description = "This is an open field west of a white house, with a boarded front door.";
            _rooms[1, 2].Description = "You are behind the white house. In one corner of the house there is a small window which is slightly ajar.";

            _rooms[2, 0].Description = "This is a dimly lit forest, with large trees all around. To the east, there appears to be sunlight.";
            _rooms[2, 1].Description = "You are facing the north side of a white house. There is no door here, and all the windows are barred.";
            _rooms[2, 2].Description = "You are in a clearing, with a forest surrounding you on the west and south.";
        }

        private static readonly Room[,] _rooms =
        {
            { new Room("Rocky Trail"), new Room("South of House"), new Room("Canyon View") },
            { new Room("Forest"), new Room("West of House"), new Room("Behind House") },
            { new Room("Dense Woods"), new Room("North of House"), new Room("Clearing")}
        };

        private static (int Row, int Column) _location = (1, 1);
    }
}