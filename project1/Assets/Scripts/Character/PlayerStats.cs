using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    // Health/Sp
    private int health = 100;
    private int maxHealth = 100;
    private int sp = 30;
    private int maxSp = 30;

    // Stats


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void TakeDamage(int damageTaken) {
        this.health -= damageTaken;

        if (this.health < 0) {
            // Dead
            this.health = 0;
        } else if (this.health > maxHealth) {
            // Overhealed
            this.health = maxHealth;
        }
    }
}
