using UnityEngine;

﻿[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemInfo : ScriptableObject {

    public new string name;
    public string desc;
    public GameSystem.ItemType itemType;
    public Vector2 size;
    public Sprite srcImg;
    public GameObject prefab;

}
