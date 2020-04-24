using UnityEngine;

public class SystemMenu : MonoBehaviour {

    public GameObject systemMenu;
    public SaveLoad saveLoadMenu;
    public GameObject settingsMenu;


    public bool IsActive() {
        return systemMenu.activeSelf || saveLoadMenu.gameObject.activeSelf || settingsMenu.activeSelf;
    }

    public void Back(GameObject currMenu) {
        currMenu.SetActive(false);
        systemMenu.SetActive(true);
    }

    public void ShowSystemMenu(bool show) {
        systemMenu.SetActive(show);
    }

    public void ShowSaveMenu(bool show) {
        systemMenu.SetActive(!show);
        saveLoadMenu.gameObject.SetActive(show);
        saveLoadMenu.mode = SaveLoad.Mode.SAVE;
    }

    public void ShowLoadMenu(bool show) {
        systemMenu.SetActive(!show);
        saveLoadMenu.gameObject.SetActive(show);
        saveLoadMenu.mode = SaveLoad.Mode.LOAD;
    }

    public void ShowSettingsMenu(bool show) {
        systemMenu.SetActive(!show);
        settingsMenu.SetActive(show);
    }

    public void Quit() {
        Application.Quit(0);
    }
}
