using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticGun : Gun {

    void Update() {
        // Hold down for Automatic Fire
        if (Input.GetButton("Fire")) {
            Fire();
        } else {
            StopFiring();
        }

        // Other functions
        base.Update();
    }
}
