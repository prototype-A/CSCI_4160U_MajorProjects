using UnityEngine;

public class RotateToFaceCamera : MonoBehaviour {

    // Update is called once per frame
    void Update() {
        // Rotate UI on Y-axis only to face camera direction
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }
}
