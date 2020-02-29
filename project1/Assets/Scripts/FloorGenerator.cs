using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorGenerator : MonoBehaviour {

    [Range(0, 100)]
    public int initChance;

    [Range(0, 8)]
    public int birthLimit;

    [Range(0, 8)]
    public int deathLimit;

    [Range(1, 10)]
    public int numSimulations;

    public int width = 60;
    public int height = 40;
    public int[,] terrainMap;

    private enum TileType { Chasm = 0, Floor = 1, Door = 2, Treasure = 3 }
    public Tilemap floorTilemap;
    public Tilemap treasureTilemap;
    public Tilemap doorTilemap;
    public Tilemap collisionTilemap;
    public Tile floorTile1;
    public Tile floorTile2;
    public Tile floorTile3;
    public Tile floorTile4;
    public Tile floorTile5;
    public Tile collisionTile;
    public Tile doorTile;
    public Tile treasureTile;

    [Range(0, 100)]
    public int tile1Chance = 0;
    [Range(0, 100)]
    public int tile2Chance = 0;
    [Range(0, 100)]
    public int tile3Chance = 0;
    [Range(0, 100)]
    public int tile4Chance = 0;
    [Range(0, 100)]
    public int tile5Chance = 0;

    private System.Random rngesus;

    void Start() {
        rngesus = new System.Random();
    }

    public void GenerateNewFloor() {
        GameData.floorLevel++;
        GenerateMap();
        GameData.playerSprite.GetComponent<PlayerController>().gui.GetComponent<MenuButtonController>().UpdateHUD();
    }

    public void GenerateMap() {
        ClearMap();

        // Initialize and generate map of cave floor if not initialized yet
        if (terrainMap == null) {
            terrainMap = new int[width, height];
            InitPos();
        }

        for (int i = 0; i < numSimulations; i++) {
            terrainMap = GenTilePos(terrainMap);
        }

        // Randomly determine where to put a treasure chest on the map
        PutTileAtRandomFloorTile((int)TileType.Treasure);

        // Randomly determine where to put tile to move player to next floor down
        PutTileAtRandomFloorTile((int)TileType.Door);

        // Place tiles on tilemap
        FillTiles();
    }

    public void FillTiles() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (terrainMap[x, y] == (int)TileType.Floor) {
                    // Cave floor
                    floorTilemap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), GetRandomFloorTile());
                } else if (terrainMap[x, y] == (int)TileType.Chasm) {
                    // Chasm
                    collisionTilemap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), collisionTile);
                } else if (terrainMap[x, y] == (int)TileType.Door) {
                    // Tile to move to next floor
                    floorTilemap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), floorTile1);
                    doorTilemap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), doorTile);
                } else if (terrainMap[x, y] == (int)TileType.Treasure) {
                    // Treasure tile
                    floorTilemap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), floorTile1);
                    treasureTilemap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), treasureTile);
                }
            }
        }
    }

    private Tile GetRandomFloorTile() {
        // Get a random floor tile
        int totalChance = tile1Chance +
                            ((floorTile2 != null) ? tile2Chance : 0) +
                            ((floorTile3 != null) ? tile3Chance : 0) +
                            ((floorTile4 != null) ? tile4Chance : 0) +
                            ((floorTile5 != null) ? tile5Chance : 0);
        int randNum = rngesus.Next(totalChance + 1);

        if (randNum > -1 && randNum <= tile1Chance) {
            return floorTile1;
        } else if (floorTile2 != null &&
                    randNum > tile1Chance && randNum <= tile1Chance + tile2Chance) {
            return floorTile2;
        } else if (floorTile3 != null &&
                    randNum > tile2Chance && randNum <= (tile1Chance + tile2Chance + tile3Chance)) {
            return floorTile3;
        } else if (floorTile4 != null &&
                    randNum > tile3Chance && randNum <= (tile1Chance + tile2Chance + tile3Chance + tile4Chance)) {
            return floorTile4;
        } else if (floorTile5 != null &&
                    randNum > tile4Chance && randNum <= (tile1Chance + tile2Chance + tile3Chance + tile4Chance + tile5Chance)) {
            return floorTile5;
        }

        // Use tile 1 as default tile
        return floorTile1;
    }

    private void PutTileAtRandomFloorTile(int tileType) {
        // Randomly place the tile on a floor tile
        bool placed = false;
        do {
            int w = rngesus.Next(this.width);
            int h = rngesus.Next(this.height);
            if (terrainMap[w, h] == (int)TileType.Floor) {
                terrainMap[w, h] = tileType;
                placed = true;
            }
        } while (!placed);
    }

    private int[,] GenTilePos(int[,] oldMap) {
        int[,] newMap = new int[width, height];
        int neighbour;
        BoundsInt myB = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                neighbour = 0;
                foreach (var b in myB.allPositionsWithin) {
                    if (b.x == 0 && b.y == 0) continue;
                    if (x + b.x >= 0 && x+b.x < width && y+b.y >= 0 && y+b.y < height) {
                        neighbour += oldMap[x + b.x, y + b.y];
                    } else {
                        neighbour++;
                    }
                }

                if (oldMap[x, y] == (int)TileType.Floor) {
                    if (neighbour < deathLimit) {
                        newMap[x, y] = (int)TileType.Chasm;
                    } else {
                        newMap[x, y] = (int)TileType.Floor;
                    }
                }

                if (oldMap[x, y] == (int)TileType.Chasm) {
                    if (neighbour > birthLimit) {
                        newMap[x, y] = (int)TileType.Floor;
                    } else {
                        newMap[x, y] = (int)TileType.Chasm;
                    }
                }
            }
        }

        return newMap;
    }

    private void InitPos() {
        // Randomly determine which tiles of the tilemap will be the cave floor
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                terrainMap[x, y] = Random.Range(1, 101) < initChance ? (int)TileType.Floor : (int)TileType.Chasm;
            }
        }
    }

    private void ClearMap(bool complete = false) {
        // Clear all tiles from the tilemaps
        floorTilemap.ClearAllTiles();
        collisionTilemap.ClearAllTiles();

        if (complete) {
            terrainMap = null;
        }
    }
}
