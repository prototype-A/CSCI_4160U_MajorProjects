using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

    public float hp = 100.0f;
    private int maxHp = 100;
    public RectTransform hpBar;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public bool TakeDamage(int dmg) {
        this.hp -= dmg;
        UpdateHpBar();

        // Remove model when it dies
        if (this.hp <= 0) {
            this.hp = 0;
            Destroy(gameObject);

            return true;
        }

        return false;
    }

    private void UpdateHpBar() {
        float barXPosAtZero = 0.25f;
        float newBarXPos = barXPosAtZero - (hp / maxHp) * barXPosAtZero;
        float barWidthAtFull = 5.0f;
        float newBarWidth = (hp / maxHp) * barWidthAtFull;

        hpBar.anchoredPosition3D = new Vector3(newBarXPos, hpBar.anchoredPosition3D.y, hpBar.anchoredPosition3D.z);
        hpBar.sizeDelta = new Vector2(newBarWidth, hpBar.sizeDelta.y);
    }
}
