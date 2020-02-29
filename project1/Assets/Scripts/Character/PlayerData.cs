using System.Collections.Generic;

[System.Serializable]
public class PlayerData {

    public Class playerClass;
    public Inventory playerInventory;

    public int level;
    public float health;
    public int maxHealth;
    public float sp;
    public int maxSp;
    public float exp;
    public int maxExp;

    public int str;
    public int con;
    public int spr;

    public PlayerData(string chosenClass) {
        playerClass = GameData.playableClasses.GetDict()[chosenClass];
        playerInventory = new Inventory();
        this.level = 1;
        this.exp = 0;
        UpdateStats(true);
    }

    public bool GainExp(float exp) {
        this.exp += exp;
        if (this.exp >= this.maxExp) {
            // Level up
            this.exp -= this.level * 10;
            this.level++;
            UpdateStats(true);
            return true;
        }

        return false;
    }

    public void UpdateStats(bool fullHeal = false) {
        this.str = playerClass.str + ((this.level - 1) * playerClass.strPerLvl);
        this.con = playerClass.con + ((this.level - 1) * playerClass.conPerLvl);
        this.spr = playerClass.spr + ((this.level - 1) * playerClass.sprPerLvl);

        this.maxHealth = playerClass.hpBase + this.con;
        this.maxSp = playerClass.spBase + this.spr;

        this.maxExp = this.level * 10;

        if (fullHeal) {
            this.health = maxHealth;
            this.sp = maxSp;
        }
    }

    public int GetDamage() {
        return this.level + this.str;
    }

    public bool CanUseSkill(Skill skill) {
        if (skill.costType == "SP") {
            return this.sp >= skill.cost;
        }

        return false;
    }

    public int UseSkill(Skill skill) {
        if (this.CanUseSkill(skill)) {
            // Pay the skill cost
            if (skill.costType == "SP") {
                this.sp -= skill.cost;
            }

            // Calculate skill damage based on type
            float damage = 0;
            if (skill.dmgType == "Physical") {
                damage = this.str * skill.dmgMultiplier * skill.hits;
            } else if (skill.dmgType == "Magic") {
                damage = this.spr * skill.dmgMultiplier * skill.hits;
            }

            return (int)damage;
        }

        return -1;
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
