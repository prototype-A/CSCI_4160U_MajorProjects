using UnityEngine;
using TMPro;

public class ItemInfoPanel : MonoBehaviour {

    public GameObject nameText;
    public GameObject descText;

    public void SetName(string name) {
        nameText.GetComponent<TextMeshProUGUI>().text = name;
    }

    public void SetDesc(string desc) {
        descText.GetComponent<TextMeshProUGUI>().text = desc;
    }
}
