[System.Serializable]
public class PlayerStats {

    public Class playerClass;

    private int level;
    private int health;
    private int maxHealth;
    private int sp;
    private int maxSp;

    private int str;
    private int con;
    private int spr;

    public void UpdateStats() {
        this.str = playerClass.str;
        this.con = playerClass.con;
        this.spr = playerClass.spr;
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
