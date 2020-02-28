using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleInterface : MonoBehaviour {

    // Player GUI
    private TextMeshProUGUI levelLabel;
    private RectTransform hpBar;
    private RectTransform spBar;
    private TextMeshProUGUI hpLabel;
    private TextMeshProUGUI spLabel;
    private enum Actions { Attack, Skill, Item, Run }
    private TextMeshProUGUI[] actionsText;
    private int prevAction = 0;
    private int currAction = 0;
    private TextMeshProUGUI logText;
    private readonly int WAIT_TIME = 3;

    // Enemy GUI
    private Enemy enemy;
    private TextMeshProUGUI enemyLabel;
    private RectTransform enemyHpBar;

    private bool playersTurn = true;

    private System.Random rngesus;


    // Start is called before the first frame update
    void Start() {
        // Player sprite will sometimes show up in battle scene
        GameData.playerSprite.SetActive(false);

        levelLabel = transform.Find("PlayerInfo").Find("LevelLabel").gameObject.GetComponent<TextMeshProUGUI>();
        hpBar = transform.Find("PlayerInfo").Find("Stats").Find("HPBar").Find("Bar").gameObject.GetComponent<RectTransform>();
        spBar = transform.Find("PlayerInfo").Find("Stats").Find("SPBar").Find("Bar").gameObject.GetComponent<RectTransform>();
        hpLabel = transform.Find("PlayerInfo").Find("Stats").Find("HPBar").Find("HPLabel").gameObject.GetComponent<TextMeshProUGUI>();
        spLabel = transform.Find("PlayerInfo").Find("Stats").Find("SPBar").Find("SPLabel").gameObject.GetComponent<TextMeshProUGUI>();

        enemyHpBar = transform.Find("Enemy").Find("HPBar").Find("Bar").gameObject.GetComponent<RectTransform>();
        enemyLabel = transform.Find("Enemy").Find("Name").gameObject.GetComponent<TextMeshProUGUI>();

        int numActions = transform.Find("ActionPanel").Find("Actions").childCount;
        actionsText = new TextMeshProUGUI[numActions];
        for (int action = 0; action < numActions; action++) {
            actionsText[action] = transform.Find("ActionPanel").Find("Actions").Find("Action" + (action + 1)).gameObject.GetComponent<TextMeshProUGUI>();
        }

        logText = transform.Find("LogPanel").Find("Text").gameObject.GetComponent<TextMeshProUGUI>();

        rngesus = new System.Random();

        GenerateEnemy();

        UpdateUI();
    }

    private void GenerateEnemy() {
        // Generate a random enemy to fight up to 10 levels above/below player level
        this.enemy = GameData.enemies[rngesus.Next(GameData.enemies.Length)];
        this.enemy.SetLevel(GameData.playerStats.level);
        enemyLabel.text = "Lv." + this.enemy.level + " " + this.enemy.name;
        SetLogText("Encountered a " + enemyLabel.text + "!");
    }

    public void SetActiveAction(int newAction) {
        prevAction = currAction;
        currAction = newAction;

        // Unselect previous action
        actionsText[prevAction].text = "";
        // Select active action
        actionsText[currAction].text = ">";
    }

    // Update is called once per frame
    void Update() {
        // Player's turn
        if (playersTurn) {
            // Key press
            if (Input.GetButtonDown("Up")) {
                if (currAction > 0) {
                    // Update currently selected action
                    SetActiveAction(currAction - 1);
                }
            } else if (Input.GetButtonDown("Down")) {
                if (currAction < actionsText.Length - 1) {
                    // Update currently selected action
                    SetActiveAction(currAction + 1);
                }
            } else if (Input.GetButtonDown("Submit") || Input.GetMouseButtonDown(0)) {
                // 'Enter' pressed or mouse left-click
                switch((Actions)currAction) {
                    case Actions.Attack:
                        // Attack enemy
                        SetLogText(this.enemy.name + " took " +
                                    this.enemy.TakeDamage(GameData.playerStats.GetDamage()) +
                                    " damage!");
                        UpdateUI();
                        Invoke("EnemyMove", WAIT_TIME);
                        break;
                    case Actions.Skill:
                        ShowSkills();
                        break;
                    case Actions.Item:
                        ShowUsableItems();
                        break;
                    case Actions.Run:
                        Run();
                        break;
                }

                playersTurn = !playersTurn;
            }
        }
    }

    private void ShowSkills() {
        Debug.Log("Skill");
        playersTurn = !playersTurn;
    }

    private void ShowUsableItems() {
        Debug.Log("Items");
        playersTurn = !playersTurn;
    }

    private void Run() {
        // Run from battle
        SetLogText("Successfully escaped!");
        Invoke("UnloadScene", WAIT_TIME);
    }

    private void EnemyMove() {
        if (this.enemy.hp <= 0) {
            SetLogText("The " + enemyLabel.text + " died!");

            // Player gains exp
            float expGain = this.enemy.GetExpGain();
            SetLogText("You gained " + expGain + " experience points!");
            if (GameData.playerStats.GainExp(expGain)) {
                // Player levelled up
                AppendLogText("\nYou levelled up!");
            }

            // Give player monster drops and unload the scene
            Invoke("GetDrops", WAIT_TIME);
        } else {
            // Enemy attacks player
            EnemyAttack();
        }
    }

    private void EnemyAttack() {
        // Enemy makes a move (attack)
        SetLogText(this.enemy.name + " attacks you!\n" +
                    "You took " + GameData.playerStats.TakeDamage(this.enemy.Attack()) +
                    " damage!");
        UpdateUI();
        // Switch turns
        playersTurn = !playersTurn;
    }

    private void GetDrops() {
        // Give player the monster drops
        Dictionary<string, int> drops = this.enemy.GetDrops();
        if (drops.Count > 0) {
            SetLogText("You obtained the following items: ");
            string[] dropNames = new string[drops.Count];
            drops.Keys.CopyTo(dropNames, 0);
            for (int i = 0; i < dropNames.Length; i++) {
                AppendLogText(drops[dropNames[i]] + "x " + dropNames[i]);
                if (i < dropNames.Length - 1) {
                    AppendLogText(", ");
                }
            }
        }

        // Unload battle scene
        Invoke("UnloadScene", WAIT_TIME + (float)(drops.Count * 0.75));
    }

    private void UnloadScene() {
        // Remove battle scene
        SceneManager.UnloadSceneAsync("EnemyBattle");
        GameData.inBattle = false;
        GameData.playerSprite.SetActive(true);
    }

    private void UpdateUI() {
        // Update enemy UI
        Utils.SetRectRight(this.enemyHpBar, Utils.CalculateRectRight(this.enemy.hp, this.enemy.maxHp, 175, -75));

        // Update own UI
        this.levelLabel.text = "Level " + GameData.playerStats.level;
        this.hpLabel.text = GameData.playerStats.health + "/" + GameData.playerStats.maxHealth;
        this.spLabel.text = GameData.playerStats.sp + "/" + GameData.playerStats.maxSp;
        Utils.SetRectRight(this.hpBar, Utils.CalculateRectRight(GameData.playerStats.health, GameData.playerStats.maxHealth, 120, -60));
        Utils.SetRectRight(this.spBar, Utils.CalculateRectRight(GameData.playerStats.sp, GameData.playerStats.maxSp, 120, -60));
    }

    private void SetLogText(string text) {
        // Sets the text of the battle scene log
        this.logText.text = text;
    }

    private void AppendLogText(string textToAppend) {
        // Appends textToAppend to the text of the battle scene log
        this.logText.text = GetLogText() + textToAppend;
    }

    private string GetLogText() {
        // Returns the text of the battle scene log
        return this.logText.text;
    }

}
