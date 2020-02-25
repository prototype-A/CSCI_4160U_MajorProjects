[System.Serializable]
public class Weapon : Item {

    public string prefix { get; }
    public string color { get; }

    public Weapon(string name, string desc) : base(name, desc) {}

    public Weapon(Item baseWeapon, Prefix prefix) : base(baseWeapon.name, baseWeapon.desc) {
        if (prefix != null) {
            this.prefix = prefix.prefix;
            this.color = prefix.color;
        }
    }

    public string GetWeaponName() {
        return this.prefix + this.name;
    }
}
