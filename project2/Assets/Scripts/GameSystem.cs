using System.Numerics;
using UnityEngine;

public class GameSystem : MonoBehaviour {

    // Scene names
    public static readonly string GAME_SCENE_NAME = "Game";

    // PlayerPrefs keys
    public static readonly string PLAYERNAME_KEY = "PlayerName";
    public static readonly string CROSSHAIR_SETTING_KEY = "Crosshair";

    // Types
    public enum ItemType {
        All,
        Barrel,
        Consumable,
        Gun,
        Magazine,
        Muzzle,
        Rail,
        Scope,
        Stock,
        Underbarrel
    }
    public enum FireMode {
        Auto,
        Safety,
        Single
    }

    // In-game objects
    public FPSCharacterController player;
    public Settings settings;

    // Save data (to restore)
    public static SaveData saveData = null;


    // Set player name
    public static void SetPlayerName(string name) {
        PlayerPrefs.SetString(PLAYERNAME_KEY, name);
    }

    // Restore saved data after loading game scene
    public void RestoreSavedData() {
        if (saveData != null) {
            // Restore player name
            SetPlayerName(saveData.playerName);

            // Restore player status
            player.health = saveData.playerHealth;
            player.hunger = saveData.playerHunger;
            player.thirst = saveData.playerThirst;

            // Change crosshair to saved crosshair
            settings.ChangeCrosshair(saveData.crosshair);

            // Restore player inventory items
            //player.gui.inventoryItems
        }
    }

    public SaveData BuildSaveData() {
        string playerName = PlayerPrefs.GetString(PLAYERNAME_KEY);
        UnityEngine.Vector3 pos = player.gameObject.transform.position;
        System.Numerics.Vector3 playerPos = new System.Numerics.Vector3(pos.x, pos.y, pos.z);
        int crosshair = PlayerPrefs.GetInt(CROSSHAIR_SETTING_KEY);
        SaveData saveData = new SaveData(playerName, playerPos, player.health,
                                        player.hunger, player.thirst, crosshair);

        return null;
    }
}
