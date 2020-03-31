using UnityEngine;
using TMPro;

public class ItemInfoPanel : MonoBehaviour {

    public GameObject nameText;
    public GameObject infoText;
    public GameObject descText;

    public void SetName(string name) {
        infoText.SetActive(false);
        nameText.GetComponent<TextMeshProUGUI>().text = name;
    }

    public void SetInfo(string info) {
        infoText.SetActive(true);
        infoText.GetComponent<TextMeshProUGUI>().text = info;
    }

    public void SetDesc(string desc) {
        descText.GetComponent<TextMeshProUGUI>().text = desc;
    }
}
