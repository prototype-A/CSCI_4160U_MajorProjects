using UnityEngine;

public class SystemMenu : MonoBehaviour {

    public GameObject systemMenu;
    public GameObject saveLoadMenu;
    public GameObject settingsMenu;


    public void Back(GameObject currMenu) {
        currMenu.SetActive(false);
        systemMenu.SetActive(true);
    }

    public void ShowSystemMenu(bool show) {
        systemMenu.SetActive(show);
    }

    public void ShowSaveMenu(bool show) {
        systemMenu.SetActive(false);
        saveLoadMenu.SetActive(show);
    }

    public void ShowLoadMenu(bool show) {
        systemMenu.SetActive(false);
        saveLoadMenu.SetActive(show);
    }

    public void ShowSettingsMenu(bool show) {
        systemMenu.SetActive(false);
        settingsMenu.SetActive(show);
    }

    public void Quit() {
        Application.Quit(0);
    }
}
