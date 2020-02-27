using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    private SpriteRenderer sprite;
    private Animator animator;
    public GameObject gui;

    // Movement
    [SerializeField] private float runSpeed = 0.1f;
    private float horizontalMovement = 0.0f;
    private float verticalMovement = 0.0f;
    private float hInput;
    private float vInput;

    // Battle system
    private float battleChance = 0.0f;
    public float battleChanceModifier { get; set; } = 0.05f;

    // Start is called before the first frame update
    void Start() {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void EnterBattle() {
        if (this.battleChance >= 100.0f) {
            // Battle enemy
            this.battleChance = 0.0f;
            hInput = 0.0f;
            vInput = 0.0f;
            BattleEnemy();
        } else {
            // Increase chance for enemy battle
            System.Random rngesus = new System.Random();
            this.battleChance += rngesus.Next(11) * battleChanceModifier;
        }
    }

    private void BattleEnemy() {
        // Battle an enemy
        SceneManager.LoadScene("EnemyBattle", LoadSceneMode.Additive);
        GameData.inBattle = true;
    }

    // Update is called once per frame
    void Update() {
        if (!GameData.inBattle) {
            // Walking around
            // Key pressed
            if (Input.GetKeyDown(KeyCode.Escape)) {
                // Open/close menu
                MenuButtonController menu = gui.GetComponent<MenuButtonController>();
                if (menu.IsInMenu() && menu.IsInMenuScreen()) {
                    menu.CloseMenu();
                } else if (!(menu.IsInMenu() || menu.IsInMenuScreen())) {
                    menu.ShowMenu();
                }
            }

            // Get horizontal movement input
            hInput = Input.GetAxis("Horizontal");
            if (hInput != 0) {
                if (hInput > 0) {
                    // Move right
                    horizontalMovement = runSpeed;
                } else if (hInput < 0) {
                    // Move left
                    horizontalMovement = -runSpeed;
                }
                // Only move left/right or up/down at once
                verticalMovement = 0.0f;
            } else {
                horizontalMovement = 0.0f;
            }

            // Get vertical movement input
            vInput = Input.GetAxis("Vertical");
            if (vInput != 0) {
                if (vInput > 0) {
                    // Move up
                    verticalMovement = runSpeed;
                } else if (vInput < 0) {
                    // Move down
                    verticalMovement = -runSpeed;
                }
                // Only move left/right or up/down at once
                horizontalMovement = 0.0f;
            } else {
                verticalMovement = 0.0f;
            }

            // Change animator parameter to change animation
            //Debug.Log("Horizontal Movement: " + horizontalMovement / runSpeed);
            //Debug.Log("Vertical Movement: " + verticalMovement / runSpeed);
            animator.SetFloat("HorizontalSpeed", horizontalMovement / runSpeed);
            animator.SetFloat("VerticalSpeed", verticalMovement / runSpeed);

            // Move character
            transform.Translate(new Vector3(horizontalMovement,
                                            verticalMovement,
                                            0.0f));
        }
    }

    void FixedUpdate() {
        if (hInput != 0 || vInput != 0) {
            // Increase chance to battle enemy with every step
            EnterBattle();
        }
    }
}
