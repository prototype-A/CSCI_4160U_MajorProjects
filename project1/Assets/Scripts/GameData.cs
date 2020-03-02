using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class GameData {
    // JSON
    public static TextAsset classJson = Resources.Load("Json/Classes") as TextAsset;
    public static TextAsset enemyJson = Resources.Load("Json/Enemies") as TextAsset;
    public static TextAsset consumablesJson = Resources.Load("Json/Consumables") as TextAsset;
    public static TextAsset weaponJson = Resources.Load("Json/Weapons") as TextAsset;

    // Enemies
    public static Enemy[] enemies = JsonUtility.FromJson<Enemies>(enemyJson.text).enemies;

    // Player
    public static Classes playableClasses = JsonUtility.FromJson<Classes>(classJson.text);
    public static Class[] playerClasses = playableClasses.classes;
    public static PlayerData playerData;
    public static bool inBattle;
    public static Transform playerSprite;
    public static InventoryManager inventoryManager;
    public static GameSave gameSave;

    // Map
    public static int floorLevel = 0;
    public static Transform map;

    // Items
    public static Dictionary<string, Consumable> consumables = JsonUtility.FromJson<Consumables>(consumablesJson.text).GetDict();
    public static Dictionary<string, Item> weapons = JsonUtility.FromJson<Weapons>(weaponJson.text).GetDict();
    public static Prefix[] weaponPrefixes = JsonUtility.FromJson<Weapons>(weaponJson.text).prefixes;


    public static void CreateNewData(string chosenClass) {
        playerData = new PlayerData(chosenClass);
        floorLevel = 0;
    }

    public static GameSave CreateSaveGame() {
        // Create game save to be written to file
        GameObject[] rootObjects = GetSceneRootObjects();
        GameObject player = null;
        FloorGenerator floorGenerator = null;
        for (int obj = 0; obj < rootObjects.Length; obj++) {
            if (rootObjects[obj].name == "Player") {
                player = rootObjects[obj].gameObject;
            } else if (rootObjects[obj].name == "Map") {
                floorGenerator = rootObjects[obj].gameObject.GetComponent<FloorGenerator>();
            }
        }
        Vector3 currPlayerPos = player.transform.position;
        float[] playerPos = {currPlayerPos.x, currPlayerPos.y, currPlayerPos.z};
        GameSave gameSave = new GameSave(playerData, floorLevel, playerPos, floorGenerator.terrainMap);

        return gameSave;
    }

    public static void LoadSaveData() {
        // Get objects at root of scene
        GameObject[] rootObjects = GetSceneRootObjects();
        for (int i = 0; i < rootObjects.Length; i++) {
            if (rootObjects[i].name == "Map") {
                map = rootObjects[i].transform;
            } else if (rootObjects[i].name == "Player") {
                playerSprite = rootObjects[i].transform;
            }
        }

        if (gameSave != null) {
            // Restore game savedata
            playerData = gameSave.playerData;
            floorLevel = gameSave.floorLevel;
            playerSprite.transform.Find("GUI").gameObject.GetComponent<MenuButtonController>().UpdateHUD();
            playerSprite.transform.Find("GUI").gameObject.GetComponent<InventoryManager>().LoadInventory(playerData.items);
            map.Find("Ground").gameObject.GetComponent<Tilemap>().ClearAllTiles();
            map.Find("Chasm").gameObject.GetComponent<Tilemap>().ClearAllTiles();
            map.Find("Treasure").gameObject.GetComponent<Tilemap>().ClearAllTiles();
            map.Find("Door").gameObject.GetComponent<Tilemap>().ClearAllTiles();
            FloorGenerator floorGenerator = map.gameObject.GetComponent<FloorGenerator>();
            floorGenerator.terrainMap = gameSave.terrainMap;
            floorGenerator.FillTiles();
            playerSprite.position = new Vector3(gameSave.playerPos[0], gameSave.playerPos[1], gameSave.playerPos[2]);
            gameSave = null;
        } else {
            // Generate a map if there is no savedata to load
            map.GetComponent<FloorGenerator>().GenerateNewFloor();
        }
    }

    public static Item GetItemByName(string name, int amount) {
        if (consumables.ContainsKey(name)) {
            return new Consumable(GetConsumable(name), amount);
        }

        return GenerateWeapon(name);
    }

    public static Consumable GetConsumable(string name) {
        return GameData.consumables[name];
    }

    public static Weapon GenerateWeapon(string name) {
        // Randomly generate a weapon
        System.Random rngesus = new System.Random();
        Item baseWeapon = GameData.weapons[name];
        Prefix prefix = GameData.weaponPrefixes[rngesus.Next(GameData.weaponPrefixes.Length)];
        if (rngesus.Next(101) > prefix.chance) {
            // Chance of not obtaining boosted gear
            prefix = null;
        }

        return new Weapon(baseWeapon, prefix);
    }

    public static GameObject[] GetSceneRootObjects() {
        // Returns an array of GameObjects in the root of the current scene
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();;
    }
}
