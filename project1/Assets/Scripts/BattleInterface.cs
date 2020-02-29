using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleInterface : MonoBehaviour {

    private System.Random rngesus;
    private readonly int WAIT_TIME = 3;

    // Left panel
    private TextMeshProUGUI levelLabel;
    private RectTransform hpBar;
    private RectTransform spBar;
    private TextMeshProUGUI hpLabel;
    private TextMeshProUGUI spLabel;

    // Center panel
    private GameObject logText;
    private GameObject skillsItems;
    private TextMeshProUGUI[] skillsItemsText;
    private TextMeshProUGUI[] skillsItemsSelectors;
    private int skillItemAction = 0;
    private int prevSkillItemAction = 0;

    // Right panel
    private enum Actions { Attack, Skill, Item, Run }
    private TextMeshProUGUI[] actionsText;
    private int currAction = 0;
    private int prevAction = 0;
    private bool usingCenterPanel = false;
    private GameObject controlTransferIndicator;

    // Enemy GUI
    private Enemy enemy;
    private TextMeshProUGUI enemyLabel;
    private RectTransform enemyHpBar;

    private bool playersTurn = true;


    // Start is called before the first frame update
    void Start() {
        // Disable player sprite and cave GUI
        GameData.playerSprite.gameObject.SetActive(false);
        GameData.playerSprite.GetComponent<PlayerController>().gui.SetActive(false);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("EnemyBattle"));

        rngesus = new System.Random();
        levelLabel = transform.Find("PlayerInfo").Find("LevelLabel").gameObject.GetComponent<TextMeshProUGUI>();
        hpBar = transform.Find("PlayerInfo").Find("Stats").Find("HPBar").Find("Bar").gameObject.GetComponent<RectTransform>();
        spBar = transform.Find("PlayerInfo").Find("Stats").Find("SPBar").Find("Bar").gameObject.GetComponent<RectTransform>();
        hpLabel = transform.Find("PlayerInfo").Find("Stats").Find("HPBar").Find("HPLabel").gameObject.GetComponent<TextMeshProUGUI>();
        spLabel = transform.Find("PlayerInfo").Find("Stats").Find("SPBar").Find("SPLabel").gameObject.GetComponent<TextMeshProUGUI>();

        logText = transform.Find("LogPanel").Find("Text").gameObject;
        skillsItems = transform.Find("LogPanel").Find("Actions").gameObject;
        int numItemsSkills = skillsItems.transform.Find("Text").childCount;
        skillsItemsText = new TextMeshProUGUI[numItemsSkills];
        skillsItemsSelectors = new TextMeshProUGUI[numItemsSkills];
        for (int si = 0; si < numItemsSkills; si++) {
            skillsItemsText[si] = transform.Find("LogPanel").Find("Actions").Find("Text").Find("Action" + (si + 1)).gameObject.GetComponent<TextMeshProUGUI>();
            skillsItemsSelectors[si] = transform.Find("LogPanel").Find("Actions").Find("Actions").Find("Action" + (si + 1)).gameObject.GetComponent<TextMeshProUGUI>();
        }

        controlTransferIndicator = transform.Find("ActionPanel").Find("TransferAction").gameObject;
        int numActions = transform.Find("ActionPanel").Find("Actions").childCount;
        actionsText = new TextMeshProUGUI[numActions];
        for (int action = 0; action < numActions; action++) {
            actionsText[action] = transform.Find("ActionPanel").Find("Actions").Find("Action" + (action + 1)).gameObject.GetComponent<TextMeshProUGUI>();
        }

        enemyHpBar = transform.Find("Enemy").Find("HPBar").Find("Bar").gameObject.GetComponent<RectTransform>();
        enemyLabel = transform.Find("Enemy").Find("Name").gameObject.GetComponent<TextMeshProUGUI>();

        // Generate random enemy
        GenerateEnemy();

        UpdateUI();
    }

    private void GenerateEnemy() {
        // Generate a random enemy to fight up to 10 levels above/below player level
        this.enemy = GameData.enemies[rngesus.Next(GameData.enemies.Length)];
        this.enemy.SetLevel(GameData.playerData.level);
        enemyLabel.text = "Lv." + this.enemy.level + " " + this.enemy.name;
        SetLogText("Encountered a " + enemyLabel.text + "!");
    }

    public void SetActiveAction(int newAction) {
        if (!usingCenterPanel && playersTurn) {
            prevAction = currAction;
            currAction = newAction;

            // Unselect previous action
            actionsText[prevAction].text = "";
            // Select active action
            actionsText[currAction].text = ">";
        }
    }

    public void SetActiveCenterPanelAction(int newAction) {
        if (skillsItemsText[newAction].text != "") {
            // Only select available items/skills
            prevSkillItemAction = skillItemAction;
            skillItemAction = newAction;

            // Unselect previous action
            skillsItemsSelectors[prevSkillItemAction].text = "";
            // Select active action
            skillsItemsSelectors[skillItemAction].text = ">";
        }
    }

    // Update is called once per frame
    void Update() {
        // Player's turn
        if (playersTurn) {
            // Key press
            if (Input.GetButtonDown("Up")) {
                if (!usingCenterPanel) {
                    // Right panel actions
                    if (currAction > 0) {
                        // Update currently selected action
                        SetActiveAction(currAction - 1);
                    }
                } else {
                    // Center panel actions

                }
            } else if (Input.GetButtonDown("Down")) {
                if (!usingCenterPanel) {
                    // Right panel actions
                    if (currAction < actionsText.Length - 1) {
                        // Update currently selected action
                        SetActiveAction(currAction + 1);
                    }
                } else {
                    // Center panel actions
                    SetActiveCenterPanelAction(skillItemAction + 1);
                }
            } else if (Input.GetButtonDown("Left") && usingCenterPanel) {
                if ((skillItemAction + 1) % 2 == 0) {
                    SetActiveCenterPanelAction((skillItemAction + 1) * 2);
                }
            } else if (Input.GetButtonDown("Right") && usingCenterPanel) {
                if ((skillItemAction + 1) * 2 <= skillsItemsText.Length) {
                    SetActiveCenterPanelAction((skillItemAction + 1) * 2);
                }
            } else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) && usingCenterPanel) {
                // Return from center panel control (skill/item usage) with 'Esc' or mouse right-click
                LogControl(false);
            } else if (Input.GetButtonDown("Submit") || Input.GetMouseButtonDown(0)) {
                // 'Enter' pressed or mouse left-click
                if (!usingCenterPanel) {
                    // Right panel actions
                    switch((Actions)currAction) {
                        case Actions.Attack:
                            playersTurn = !playersTurn;
                            // Attack enemy
                            SetLogText(this.enemy.name + " took " +
                                        this.enemy.TakeDamage(GameData.playerData.GetDamage()) +
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
                } else {
                    // Center panel actions
                    if ((Actions)currAction == Actions.Skill && skillItemAction != -1) {
                        Skill skillUsed = GameData.playerData.playerClass.skills[skillItemAction];
                        int damageDealt = GameData.playerData.UseSkill(skillUsed);
                        if (damageDealt != -1) {
                            // Player has the resources to use the skill
                            LogControl(false);
                            SetLogText("Used " + skillUsed.name + " on " +
                                        this.enemy.name + "!\n" +
                                        this.enemy.name + " took " +
                                        this.enemy.TakeDamage(damageDealt) +
                                        " damage!");
                            UpdateUI();
                            Invoke("EnemyMove", WAIT_TIME);
                        }
                    }
                    playersTurn = !playersTurn;
                }
            }
        }
    }

    private void LogControl(bool control) {
        // control == true => Center panel shows skills/usable items
        // control == false => Center panel becomes log
        usingCenterPanel = control;
        logText.SetActive(!control);
        skillsItems.SetActive(control);
        controlTransferIndicator.SetActive(control);

        if (!control) {
            // Remove previous text when transferring control back
            if (skillItemAction != -1) {
                for (int i = 0; i < skillsItemsText.Length; i++) {
                    skillsItemsText[i].text = "";
                }
                skillsItemsSelectors[skillItemAction].text = "";
                skillItemAction = 0;
                prevSkillItemAction = 0;
            }
        }
    }

    private void ShowSkills() {
        Skill[] skills = GameData.playerData.playerClass.skills;
        if (skills.Length == 0) {
            // No skills to use
            skillItemAction = -1;
            prevSkillItemAction = -1;
        } else {
            // Set skill names
            for (int i = 0; i < skills.Length; i++) {
                skillsItemsText[i].text = skills[i].name + " [" + skills[i].cost + " " + skills[i].costType + "]";
            }
            SetActiveCenterPanelAction(skillItemAction);
        }

        LogControl(true);
    }

    private void ShowUsableItems() {
        Debug.Log("Items");
        playersTurn = !playersTurn;
    }

    private void Run() {
        // Run from battle
        playersTurn = !playersTurn;
        SetLogText("Successfully escaped from the " + this.enemyLabel.text + "!");
        Invoke("UnloadScene", WAIT_TIME);
    }

    private void EnemyMove() {
        if (this.enemy.hp <= 0) {
            SetLogText("The " + enemyLabel.text + " died!");

            // Player gains exp
            float expGain = this.enemy.GetExpGain();
            SetLogText("You gained " + expGain + " experience points!");
            if (GameData.playerData.GainExp(expGain)) {
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
                    "You took " + GameData.playerData.TakeDamage(this.enemy.Attack()) +
                    " damage!");
        UpdateUI();

        if (GameData.playerData.health <= 0) {
            // Player died
            AppendLogText("\nYou died!");

            Invoke("ShowDeathScene", WAIT_TIME);
        } else {
            // Switch turns
            playersTurn = !playersTurn;
        }
    }

    private void ShowDeathScene() {
        SceneManager.LoadScene("DeathScene");
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
            AppendLogText("!");
        }

        // Unload battle scene
        Invoke("UnloadScene", WAIT_TIME + (float)(drops.Count * 0.75));
    }

    private void UnloadScene() {
        // Remove battle scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("PlayGame"));
        GameData.inBattle = false;
        GameData.playerSprite.gameObject.SetActive(true);
        GameData.playerSprite.GetComponent<PlayerController>().gui.SetActive(true);
        SceneManager.UnloadSceneAsync("EnemyBattle");
    }

    private void UpdateUI() {
        // Update enemy UI
        Utils.SetRectRight(this.enemyHpBar, Utils.CalculateRectRight(this.enemy.hp, this.enemy.maxHp, 175, -75));

        // Update own UI
        this.levelLabel.text = "Level " + GameData.playerData.level;
        this.hpLabel.text = GameData.playerData.health + "/" + GameData.playerData.maxHealth;
        this.spLabel.text = GameData.playerData.sp + "/" + GameData.playerData.maxSp;
        Utils.SetRectRight(this.hpBar, Utils.CalculateRectRight(GameData.playerData.health, GameData.playerData.maxHealth, 120, -60));
        Utils.SetRectRight(this.spBar, Utils.CalculateRectRight(GameData.playerData.sp, GameData.playerData.maxSp, 120, -60));
    }

    private void SetLogText(string text) {
        // Sets the text of the battle scene log
        this.logText.GetComponent<TextMeshProUGUI>().text = text;
    }

    private void AppendLogText(string textToAppend) {
        // Appends textToAppend to the text of the battle scene log
        this.logText.GetComponent<TextMeshProUGUI>().text = GetLogText() + textToAppend;
    }

    private string GetLogText() {
        // Returns the text of the battle scene log
        return this.logText.GetComponent<TextMeshProUGUI>().text;
    }

}
