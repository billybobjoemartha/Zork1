using System;

namespace Zork.Common
{
    public class Game
    {
        public World World { get; }

        public Player Player { get; }

        public Game(World world, string startingLocation)
        {
            World = world;
            Player = new Player(World, startingLocation);
        }

        public void Run()
        {
            Room previousRoom = null;
            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine(Player.CurrentRoom);
                if (previousRoom != Player.CurrentRoom)
                {
                    Console.WriteLine(Player.CurrentRoom.Description);
                    previousRoom = Player.CurrentRoom;
                    foreach (Item item in Player.CurrentRoom.Inventory)
                    {
                        Console.WriteLine(item.Description);
                    }
                }

                Console.Write("> ");

                string inputString = Console.ReadLine().Trim();
                // might look like:  "LOOK", "TAKE MAT", "QUIT"
                char separator = ' ';
                string[] commandTokens = inputString.Split(separator);

                string verb = null;
                string subject = null;
                if (commandTokens.Length == 0)
                {
                    continue;
                }
                else if (commandTokens.Length == 1)
                {
                    verb = commandTokens[0];
                }
                else
                {
                    verb = commandTokens[0];
                    subject = commandTokens[1];
                }

                Commands command = ToCommand(verb);
                string outputString;
                switch (command)
                {
                    case Commands.Quit:
                        isRunning = false;
                        outputString = "Thank you for playing!";
                        break;

                    case Commands.Look:
                        outputString = null;
                        Console.WriteLine(Player.CurrentRoom.Description);
                        foreach (Item item in Player.CurrentRoom.Inventory)
                        {
                            Console.WriteLine(item.Description);
                        }
                        break;

                    case Commands.North:
                    case Commands.South:
                    case Commands.East:
                    case Commands.West:
                        Directions direction = (Directions)command;
                        if (Player.Move(direction))
                        {
                            outputString = $"You moved {direction}.";
                        }
                        else
                        {
                            outputString = "The way is shut!";
                        }
                        break;

                    case Commands.Take:
                        if (subject == null)
                        {
                            Console.WriteLine("This command requires a subject");
                        }
                        else
                        {
                            bool itemIsInRoomInventory = false;
                            foreach (Item item in Player.CurrentRoom.Inventory)
                            {
                                if (item.Name == subject)
                                {
                                    itemIsInRoomInventory = true;
                                    Player.CurrentRoom.Inventory.Remove(item);
                                    Player.Inventory.Add(item);
                                    Console.WriteLine("Taken.");
                                    break;
                                }
                            }
                            if (itemIsInRoomInventory == false)
                            {
                                Console.WriteLine("You can't see any such thing.");
                            }
                        }
                        outputString = null;
                        break;

                    case Commands.Drop:
                        if (subject == null)
                        {
                            Console.WriteLine("This command requires a subject");
                        }
                        else
                        {
                            bool itemIsInInventory = false;
                            foreach (Item item in Player.Inventory)
                            {
                                if (item.Name == subject)
                                {
                                    itemIsInInventory = true;
                                    Player.Inventory.Remove(item);
                                    Player.CurrentRoom.Inventory.Add(item);
                                    Console.WriteLine("Dropped.");
                                    break;
                                }
                            }
                            if (itemIsInInventory == false)
                            {
                                Console.WriteLine("You don't have any such thing.");
                            }
                        }
                        outputString = null;
                        break;

                    case Commands.Inventory:
                        outputString = null;
                        if (Player.Inventory.Count > 0)
                        {
                            foreach (Item item in Player.Inventory)
                            {
                                Console.WriteLine(item.Description);
                            }
                        }
                        else
                        {
                            Console.WriteLine("You are empty handed.");
                        }
                        break;

                    default:
                        outputString = "Unknown command.";
                        break;
                }

                Console.WriteLine(outputString);
            }
        }

        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}
