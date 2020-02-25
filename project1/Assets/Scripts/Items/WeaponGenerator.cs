using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour {

    public TextAsset weaponsJson;
    private Weapons weapons;
    private System.Random rngesus;

    // Start is called before the first frame update
    void Start() {
        weapons = JsonUtility.FromJson<Weapons>(weaponsJson.text);
        rngesus = new System.Random();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Weapon: " + GenerateWeapon().GetWeaponName());
            GenerateWeapon();
        }
    }

    public Weapon GenerateWeapon() {
        // Randomly generate a weapon
        Item baseWeapon = weapons.weapons[this.rngesus.Next(weapons.weapons.Length)];
        Prefix prefix = weapons.prefixes[this.rngesus.Next(weapons.prefixes.Length)];
        if (this.rngesus.Next(101) > prefix.chance) {
            // Chance of not obtaining boosted gear
            prefix = null;
        }
        
        return new Weapon(baseWeapon, prefix);
    }
}
