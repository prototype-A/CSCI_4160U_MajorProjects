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

    // Update is called once per frame
    void Update() {
        // Player's turn
        if (playersTurn) {
            // Key press
            if (Input.GetButtonDown("Up")) {
                if (currAction > 0) {
                    prevAction = currAction;
                    currAction--;

                    // Update currently selected action
                    SetActiveAction();
                }
            } else if (Input.GetButtonDown("Down")) {
                if (currAction < actionsText.Length - 1) {
                    prevAction = currAction;
                    currAction++;

                    // Update currently selected action
                    SetActiveAction();
                }
            } else if (Input.GetButtonDown("Submit")) {
                // Action
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
                        Debug.Log("Skill");
                        break;
                    case Actions.Item:
                        Debug.Log("Items");
                        break;
                    case Actions.Run:
                        Debug.Log("Run");
                        break;
                }

                playersTurn = !playersTurn;
            }
        }
    }

    private void ShowSkills() {

    }

    private void ShowItems() {

    }

    private void Run() {
        SetLogText("Successfully escaped!");
        Invoke("UnloadScene", WAIT_TIME);
    }

    private void UnloadScene() {
        SceneManager.UnloadSceneAsync("EnemyBattle");
        GameData.inBattle = false;
    }

    private void EnemyMove() {
        if (this.enemy.hp <= 0) {
            // Unload battle scene when enemy dies
            SetLogText("The " + enemyLabel.text + " died!");
            Invoke("UnloadScene", WAIT_TIME);
        } else {
            EnemyAttack();
        }
    }

    private void EnemyAttack() {
        // Enemy makes move (attack)
        SetLogText(this.enemy.name + " attacks you!\n" +
                    "You took " + GameData.playerStats.TakeDamage(this.enemy.Attack()) +
                    " damage!");
        UpdateUI();
        // Switch turns
        playersTurn = !playersTurn;
    }

    private void UpdateUI() {
        // Update enemy UI
        float enemyHpBarOffset = 175 - (this.enemy.hp / this.enemy.maxHp) * 250;
        SetRight(this.enemyHpBar, enemyHpBarOffset);

        // Update own UI
        this.levelLabel.text = "Level " + GameData.playerStats.level;
        this.hpLabel.text = GameData.playerStats.health + "/" + GameData.playerStats.maxHealth;
        this.spLabel.text = GameData.playerStats.sp + "/" + GameData.playerStats.maxSp;
        float hpBarOffset = 120 - (GameData.playerStats.health / GameData.playerStats.maxHealth) * 180;
        float spBarOffset = 120 - (GameData.playerStats.sp / GameData.playerStats.maxSp) * 180;
        SetRight(this.hpBar, hpBarOffset);
        SetRight(this.spBar, spBarOffset);
    }

    private void SetLeft(RectTransform rect, float left) {
        rect.offsetMin = new Vector2(left, rect.offsetMin.y);
    }

    private void SetRight(RectTransform rect, float right) {
        rect.offsetMax = new Vector2(-right, rect.offsetMax.y);
    }

    private void SetTop(RectTransform rect, float top) {
        rect.offsetMax = new Vector2(rect.offsetMax.x, -top);
    }

    private void SetBottom(RectTransform rect, float bottom) {
        rect.offsetMin = new Vector2(rect.offsetMin.x, bottom);
    }

    private float GetLeft(RectTransform rect) {
        return rect.offsetMin.x;
    }

    private float GetRight(RectTransform rect) {
        return -rect.offsetMax.x;
    }

    private float GetTop(RectTransform rect) {
        return -rect.offsetMax.y;
    }

    private float GetBottom(RectTransform rect) {
        return rect.offsetMin.y;
    }

    private void SetActiveAction() {
        // Select active action
        actionsText[currAction].text = ">";
        // Unselect previous action
        actionsText[prevAction].text = "";
    }

    private void SetLogText(string text) {
        this.logText.text = text;
    }

}
