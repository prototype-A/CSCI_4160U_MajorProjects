using System.Collections;
using System.Collections.Generic;

﻿[System.Serializable]
public class Enemy {
    public string name;
    public int level;
    public float hp;
    public int maxHp;
    public int hpBase;
    public int hpPerLvl;
    public int dmgBase;
    public int dmgPerLvl;
    public ItemDrop[] drops;
    private System.Random rngesus;

    public Enemy() {
        this.rngesus = new System.Random();
    }

    public int Attack() {
        return this.GetDamage();
    }

    public int GetDamage() {
        return this.dmgBase + (this.dmgPerLvl * (this.level - 1));
    }

    public Dictionary<string, int> GetDrops() {
        Dictionary<string, int> droppedItems = new Dictionary<string, int>();
        foreach (ItemDrop drop in drops) {
            if (rngesus.Next(101) <= drop.chance) {
                // Item dropped
                if (droppedItems.ContainsKey(drop.name)) {
                    droppedItems[drop.name]++;
                } else {
                    droppedItems.Add(drop.name, 1);
                }
            }
        }

        return droppedItems;
    }

    public float GetExpGain() {
        return this.level * 5;
    }

    public void SetStats(int playerLevel) {
        // Randomly set enemy level between 5 levels lower or high than player
        int range = 5;
        int rangeLow = ((playerLevel - range) <= 0) ? 1 : playerLevel - range;
        int rangeHigh = playerLevel + range;
        this.level = rngesus.Next(rangeLow, rangeHigh + 1);
        this.maxHp = this.hpBase + (this.hpPerLvl * (this.level - 1));
        this.hp = maxHp;
    }

    public int TakeDamage(int damageTaken) {
        // Take damage
        this.hp -= damageTaken;

        if (this.hp < 0) {
            // Dead
            this.hp = 0;
        } else if (this.hp > this.hpBase) {
            // Overhealed
            this.hp = this.hpBase;
        }

        return damageTaken;
    }
}
