using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticGun : Gun {

    new void Update() {
        if (gui != null && !gui.menu.activeSelf && !gui.systemMenu.IsActive()) {
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

    public override bool Use() {
        return false;
    }
}
