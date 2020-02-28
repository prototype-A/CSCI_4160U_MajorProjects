public static class ItemGenerator {

    // Item database

    public static Consumable CreateConsumable(string name) {
        return GameData.consumables[name];
    }

    public static Weapon GenerateWeapon(string name) {
        // Randomly generate a weapon
        System.Random rngesus = new System.Random();
        Item baseWeapon = GameData.weapons[name];
        Prefix prefix = GameData.weaponPrefixes[rngesus.Next(GameData.weaponPrefixes.Length)];
        if (rngesus.Next(101) > prefix.chance) {
            // Chance of not obtaining boosted gear
            prefix = null;
        }

        return new Weapon(baseWeapon, prefix);
    }
}
