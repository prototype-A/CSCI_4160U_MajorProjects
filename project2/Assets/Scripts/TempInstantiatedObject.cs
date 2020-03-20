using UnityEngine;

public class TempInstantiatedObject : MonoBehaviour {

    private float timeCreated;
    public float duration;

    protected void Start() {
        this.timeCreated = Time.time;
    }

    void Update() {
        // Remove this bullet casing after its duration
        if (Time.time - duration >= timeCreated) {
            Destroy(gameObject);
        }
    }
}
