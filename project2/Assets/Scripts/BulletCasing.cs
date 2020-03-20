using UnityEngine;

public class BulletCasing : TempInstantiatedObject {

    void Start() {
        // Don't let player collide with spent bullet casings
        gameObject.layer = LayerMask.NameToLayer("Casings");

        base.Start();
    }
}
