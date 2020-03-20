using UnityEngine;

﻿[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject {

    public string name;
    public string desc;
    public Types.ItemType itemType;
    public Vector2 size;
    public Texture srcImg;

}
