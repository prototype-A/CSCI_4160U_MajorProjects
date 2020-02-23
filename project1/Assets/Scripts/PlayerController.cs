using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;
    private SpriteRenderer sprite;
    private Animator animator;

    [SerializeField] private float runSpeed = 0.1f;
    private float horizontalMovement = 0.0f;
    private float verticalMovement = 0.0f;
    private float prevHMovement = 0.0f;
    private float prevVMovement = 0.0f;

    // Start is called before the first frame update
    void Start() {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        // Get horizontal movement input
        float hInput = Input.GetAxis("Horizontal");
        if (hInput != 0) {
            if (hInput > 0) {
                // Move right
                horizontalMovement = runSpeed;

                // Change sprite to face right
                sprite.sprite = rightSprite;
            } else if (hInput < 0) {
                // Move left
                horizontalMovement = -runSpeed;

                // Change sprite to face left
                sprite.sprite = leftSprite;
            }
            // Only move left/right or up/down at once
            verticalMovement = 0.0f;
        } else {
            horizontalMovement = 0.0f;
        }

        // Get vertical movement input
        float vInput = Input.GetAxis("Vertical");
        if (vInput != 0) {
            if (vInput > 0) {
                // Move up
                verticalMovement = runSpeed;

                // Change sprite to face up
                sprite.sprite = upSprite;
            } else if (vInput < 0) {
                // Move down
                verticalMovement = -runSpeed;

                // Change sprite to face down
                sprite.sprite = downSprite;
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
