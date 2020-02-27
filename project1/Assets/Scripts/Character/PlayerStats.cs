using System.Collections.Generic;

[System.Serializable]
public class PlayerStats {

    public Class playerClass;

    public int level;
    public float health;
    public int maxHealth;
    public int sp;
    public int maxSp;
    public float exp;

    private int str;
    private int con;
    private int spr;

    public PlayerStats(string chosenClass) {
        playerClass = GameData.playableClasses.GetDict()[chosenClass];
        this.level = 1;
        UpdateStats(true);
    }

    public void AddExp(float exp) {
        this.exp += exp;
        if (this.exp > this.level * 10) {
            // Level up
            this.level++;
            this.exp -= this.level * 10;
            UpdateStats(true);
        }
    }

    public void UpdateStats(bool fullHeal = false) {
        this.str = playerClass.str + ((this.level - 1) * playerClass.strPerLvl);
        this.con = playerClass.con + ((this.level - 1) * playerClass.conPerLvl);
        this.spr = playerClass.spr + ((this.level - 1) * playerClass.sprPerLvl);

        this.maxHealth = playerClass.hpBase + this.con;
        this.maxSp = playerClass.spBase + this.spr;

        if (fullHeal) {
            this.health = maxHealth;
            this.sp = maxSp;
        }
    }

    public int GetDamage() {
        return this.level + this.str;
    }

    public int TakeDamage(int damage) {
        // Calculate damage taken
        int damageTaken = damage - this.con;
        if (damageTaken <= 0) {
            // Always take at least 1 damage
            damageTaken = 1;
        }

        // Take damage
        this.health -= damageTaken;

        if (this.health < 0) {
            // Dead
            this.health = 0;
        } else if (this.health > this.maxHealth) {
            // Overhealed
            this.health = this.maxHealth;
        }

        return damageTaken;
    }
}
