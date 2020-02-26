using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour {

    // JSONs
    public TextAsset consumablesJson;
    public TextAsset weaponsJson;

    // Item database
    private Dictionary<string, Consumable> consumables;
    private Dictionary<string, Item> weapons;
    private Prefix[] weaponPrefixes;

    private System.Random rngesus;

    // Start is called before the first frame update
    void Start() {
        consumables = JsonUtility.FromJson<Consumables>(consumablesJson.text).GetDict();
        Weapons weps = JsonUtility.FromJson<Weapons>(weaponsJson.text);
        weapons = weps.GetDict(weps.weapons);
        weaponPrefixes = weps.prefixes;
        rngesus = new System.Random();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Consumable item = CreateConsumable("Small HP Potion");
            Debug.Log("Item: " + item.name);

            Weapon weapon = GenerateWeapon("Sword");
            Debug.Log("Weapon: " + weapon.GetWeaponName());
        }
    }

    public Consumable CreateConsumable(string name) {
        return consumables[name];
    }

    public Weapon GenerateWeapon(string name) {
        // Randomly generate a weapon
        Item baseWeapon = weapons[name];
        Prefix prefix = weaponPrefixes[this.rngesus.Next(weaponPrefixes.Length)];
        if (this.rngesus.Next(101) > prefix.chance) {
            // Chance of not obtaining boosted gear
            prefix = null;
        }

        return new Weapon(baseWeapon, prefix);
    }
}
