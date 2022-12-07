﻿namespace Zork.Common
{
    public class Item
    {
        public string Name { get; }

        public string LookDescription { get; }

        public string InventoryDescription { get; }

        public bool IsWeapon { get; }

        public int WeaponDamage { get; }

        public Item(string name, string lookDescription, string inventoryDescription, bool isWeapon, int weaponDamage)
        {
            Name = name;
            LookDescription = lookDescription;
            InventoryDescription = inventoryDescription;
            IsWeapon = isWeapon;
            WeaponDamage = weaponDamage;
        }

        public override string ToString() => Name;
    }
}