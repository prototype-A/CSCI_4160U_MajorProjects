using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleInterface : MonoBehaviour {

    public TextAsset enemyJson;
    private Enemy[] enemies;
    private SpriteRenderer enemySprite;
    private Transform enemyHpBar;
    private TextMeshProUGUI enemyHpText;

    private TextMeshProUGUI[] actionsText;
    private int prevAction = 0;
    private int currAction = 0;

    private System.Random rngesus;

    // Start is called before the first frame update
    void Start() {
        enemies = JsonUtility.FromJson<Enemies>(enemyJson.text).enemies;
        enemySprite = transform.Find("Enemy").Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
        enemyHpBar = transform.Find("Enemy").Find("HPBar").Find("HPBar");
        enemyHpText = transform.Find("Enemy").Find("HPBar").Find("HPLabel").gameObject.GetComponent<TextMeshProUGUI>();

        int numActions = transform.Find("ActionPanel").Find("Actions").childCount;
        actionsText = new TextMeshProUGUI[numActions];
        for (int action = 0; action < numActions; action++) {
            actionsText[action] = transform.Find("ActionPanel").Find("Actions").Find("Action" + (action + 1)).gameObject.GetComponent<TextMeshProUGUI>();
        }

        rngesus = new System.Random();

        GenerateEnemy();
    }

    // Update is called once per frame
    void Update() {
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
        }


    }

    private void GenerateEnemy() {
        // Generate a random enemy to fight
        Enemy enemy = enemies[rngesus.Next(enemies.Length)];
        int level = rngesus.Next(5);

        Debug.Log("Enemy: Lv." + level + " " + enemy.name);
    }

    private void SetActiveAction() {
        // Select active action
        actionsText[currAction].text = ">";

        // Unselect previous action
        actionsText[prevAction].text = "";
    }
}
