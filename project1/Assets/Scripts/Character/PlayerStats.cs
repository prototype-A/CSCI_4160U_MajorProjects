using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public Class playerClass;

    private int health;
    private int maxHealth;
    private int sp;
    private int maxSp;

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
        } else if (this.health > this.maxHealth) {
            // Overhealed
            this.health = this.maxHealth;
        }
    }
}
