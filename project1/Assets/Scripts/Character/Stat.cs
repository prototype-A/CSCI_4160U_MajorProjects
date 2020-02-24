public class Stat {

    private string statName { get; }
    private string statDesc { get; }
    private int statLevel { get; set; }
    private int maxStatLevel { get; }

    public Stat(string name, string desc) {
        this.statName = name;
        this.statDesc = desc;
        this.statLevel = 0;
    }

    public Stat(string name, string desc, int level) : this(name, desc) {
        this.statLevel = level;
        this.maxStatLevel = 100;
    }
}
