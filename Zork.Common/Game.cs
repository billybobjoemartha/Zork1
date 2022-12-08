using System;
using System.Linq;
using Newtonsoft.Json;

namespace Zork.Common
{
    public class Game
    {

        public World World { get; }

        [JsonIgnore]
        public Player Player { get; }

        [JsonIgnore]
        public IInputService Input { get; private set; }

        [JsonIgnore]
        public IOutputService Output { get; private set; }

        [JsonIgnore]
        public bool IsRunning { get; private set; }
        public int moves { get; private set; }
        public int rewardScore { get; private set; }
        public int playerHealth { get; private set; }
        public bool enemyAttackKey { get; private set; }

        public Game(World world, string startingLocation)
        {
            World = world;
            Player = new Player(World, startingLocation);
        }

        public void Run(IInputService input, IOutputService output)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Output = output ?? throw new ArgumentNullException(nameof(output));

            IsRunning = true;
            Input.InputReceived += OnInputReceived;
            Output.WriteLine("Welcome to Zork!");
            Look();
            Output.WriteLine($"{Player.CurrentRoom}");
            playerHealth = 25;
            enemyAttackKey = false;
        }

        public void OnInputReceived(object sender, string inputString)
        {
            char separator = ' ';
            string[] commandTokens = inputString.Split(separator);

            string verb;
            string subject = null;
            string withWord = null;
            string wSubject = null;
            if (commandTokens.Length == 0)
            {
                return;
            }
            else if (commandTokens.Length == 1)
            {
                verb = commandTokens[0];
            }
            else if (commandTokens.Length == 2)
            {
                verb = commandTokens[0];
                subject = commandTokens[1];
            }
            else if (commandTokens.Length == 3)
            {
                verb = commandTokens[0];
                subject = commandTokens[1];
                withWord = commandTokens[2];
            }
            else
            {
                verb = commandTokens[0];
                subject = commandTokens[1];
                withWord = commandTokens[2];
                wSubject = commandTokens[3];
            }

            Room previousRoom = Player.CurrentRoom;
            Commands command = ToCommand(verb);
            switch (command)
            {
                case Commands.Quit:
                    IsRunning = false;
                    Output.WriteLine("Thank you for playing!");
                    break;

                case Commands.Look:
                    Look();
                    moves++;
                    break;

                case Commands.Reward:
                    Reward();
                    moves++;
                    break;

                case Commands.Health:
                    Output.WriteLine($"Your current health is {playerHealth}.");
                    moves++;
                    break;

                case Commands.Score:
                    moves++;
                    Score();
                    break;

                case Commands.North:
                case Commands.South:
                case Commands.East:
                case Commands.West:
                    Directions direction = (Directions)command;
                    Output.WriteLine(Player.Move(direction) ? $"You moved {direction}." : "The way is shut!");
                    moves++;
                    enemyAttackKey = false;
                    break;

                case Commands.Take:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Take(subject);
                    }
                    moves++;
                    break;

                case Commands.Drop:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Drop(subject);
                    }
                    moves++;
                    break;

                case Commands.Attack:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires an enemy to target.");
                    }
                    else if (string.IsNullOrEmpty(withWord))
                    {
                        Output.WriteLine("This command requires the word with.");
                    }
                    else if (string.IsNullOrEmpty(wSubject))
                    {
                        Output.WriteLine("This command requires a weapon after the word with.");
                    }
                    else
                    {
                        Attack(subject, withWord, wSubject);
                    }
                    moves++;
                    break;

                case Commands.Inventory:
                    if (Player.Inventory.Count() == 0)
                    {
                        Output.WriteLine("You are empty handed.");
                    }
                    else
                    {
                        Output.WriteLine("You are carrying:");
                        foreach (Item item in Player.Inventory)
                        {
                            Output.WriteLine(item.InventoryDescription);
                        }
                    }
                    moves++;
                    break;

                default:
                    Output.WriteLine("Unknown command.");
                    break;
            }

            if (ReferenceEquals(previousRoom, Player.CurrentRoom) == false)
            {
                Look();
            }

            Output.WriteLine($"{Player.CurrentRoom}");
            if (enemyAttackKey == true)
            {
                foreach (Enemy enemy in Player.CurrentRoom.AliveInventory)
                {
                    Output.WriteLine($"{enemy.Name} attacks you and does {enemy.EnemyDamage} damage!");
                    playerHealth -= enemy.EnemyDamage;
                    if (playerHealth <= 0)
                    {
                        playerHealth = 0;
                        Output.WriteLine("You died.");
                        IsRunning = false;
                        Output.WriteLine("Thank you for playing!");

                    }
                    else
                    {
                        Output.WriteLine($"Your current health is {playerHealth}.");
                    }
                }
            }
            enemyAttackKey = true;
        }

        private void Score()
        {
            Output.WriteLine($"Your score is {rewardScore}, in {moves} moves");
        }

        private void Reward()
        {
            rewardScore++;
        }
        private void Look()
        {
            Output.WriteLine(Player.CurrentRoom.Description);
            foreach (Item item in Player.CurrentRoom.Inventory)
            {
                Output.WriteLine(item.LookDescription);
            }
            foreach (Enemy enemy in Player.CurrentRoom.AliveInventory)
            {
                Output.WriteLine(enemy.LookDescription);
            }
        }

        private void Take(string itemName)
        {
            Item itemToTake = Player.CurrentRoom.Inventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (itemToTake == null)
            {
                Output.WriteLine("You can't see any such thing.");
            }
            else
            {
                Player.AddItemToInventory(itemToTake);
                Player.CurrentRoom.RemoveItemFromInventory(itemToTake);
                Output.WriteLine("Taken.");
            }
        }

        private void Drop(string itemName)
        {
            Item itemToDrop = Player.Inventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (itemToDrop == null)
            {
                Output.WriteLine("You can't see any such thing.");
            }
            else
            {
                Player.CurrentRoom.AddItemToInventory(itemToDrop);
                Player.RemoveItemFromInventory(itemToDrop);
                Output.WriteLine("Dropped.");
            }
        }

        private void Attack(string enemyName, string withCheck, string weaponName)
        {
            Enemy enemyToAttack = Player.CurrentRoom.AliveInventory.FirstOrDefault(enemy => string.Compare(enemy.Name, enemyName, ignoreCase: true) == 0);
            Item weaponToUse = Player.Inventory.FirstOrDefault(item => string.Compare(item.Name, weaponName, ignoreCase: true) == 0);
            if (enemyToAttack == null)
            {
                Output.WriteLine("You can't see any such thing.");
            }
            else
            {
                if (withCheck != "with")
                {
                    Output.WriteLine("This command requires the word with after the enemy name.");
                }
                else
                {
                    if (weaponToUse == null || weaponToUse.IsWeapon == false)
                    {
                        Output.WriteLine("You can't attack with that.");
                    }
                    else
                    {
                        enemyToAttack.EnemyHealth -= weaponToUse.WeaponDamage;
                        Output.WriteLine($"You attacked the {enemyName} with the {weaponName} and did {weaponToUse.WeaponDamage} damage!");
                        if (enemyToAttack.EnemyHealth >= 1)
                        {
                            Output.WriteLine($"The {enemyName} now has {enemyToAttack.EnemyHealth} health!");
                        }
                        else
                        {
                            Output.WriteLine($"You have slain the {enemyName}!");
                            Player.CurrentRoom.RemoveEnemyFromInventory(enemyToAttack);
                        }
                    }
                }
            }
        }

        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}