using UnityEngine;

public class BulletCasing : TempInstantiatedObject {

    new void Start() {
        // Don't let player collide with spent bullet casings
        gameObject.layer = LayerMask.NameToLayer("Casings");

        base.Start();
    }
}
