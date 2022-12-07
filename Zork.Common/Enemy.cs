namespace Zork.Common
{
    public class Enemy
    {
        public string Name { get; }

        public string LookDescription { get; }

        public int EnemyHealth { get; set; }

        public int EnemyDamage { get; set; }

        public Enemy(string name, string lookDescription, int enemyHealth, int enemyDamage)
        {
            Name = name;
            LookDescription = lookDescription;
            EnemyHealth = enemyHealth;
            EnemyDamage = enemyDamage;
        }

        public override string ToString() => Name;
    }
}
