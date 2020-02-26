using System.Collections;

﻿[System.Serializable]
public class Enemy {
    public string name;
    public string sprite;
    public int level;
    public int hpBase;
    public int hpPerLvl;
    public int dmgBase;
    public int dmgPerLvl;
    public ItemDrop[] drops;

    public int GetDamage() {
        return this.dmgBase + (this.dmgPerLvl * this.level);
    }

    public string[] GetDrops() {
        ArrayList droppedItems = new ArrayList();
        System.Random rngesus = new System.Random();
        foreach (ItemDrop drop in drops) {
            if (rngesus.Next(101) <= drop.chance) {
                droppedItems.Add(drop.name);
            }
        }

        return (string[])droppedItems.ToArray(typeof(string));
    }

    public void SetLevel(int playerLevel) {
        // Randomly set enemy level between 10 levels lower or high than player
        int rangeLow = ((playerLevel - 10) < 0) ? 1 : playerLevel - 10;
        int rangeHigh = (playerLevel + 10);
        System.Random rngesus = new System.Random();
        this.level = rngesus.Next(rangeLow, rangeHigh + 1);
    }
}
