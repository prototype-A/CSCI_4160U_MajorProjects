using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticGun : Gun {

    new void Update() {
        if (!gui.menu.activeSelf) {
            // Hold down for Automatic Fire
            if ((Input.GetButton("Fire") && fireMode == GameSystem.FireMode.Auto) ||
                (Input.GetButtonDown("Fire") && fireMode == GameSystem.FireMode.Single)) {
                Fire();
            } else {
                StopFiring();
            }

            // Other functions
            base.Update();
        }
    }
}
