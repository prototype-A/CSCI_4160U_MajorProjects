using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorGenerator : MonoBehaviour {

    // Chance for tile to be ground
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

    public Tilemap topMap;
    public Tilemap botMap;
    public Tile topTile;
    public Tile bottomTile;


    public void generateMap() {
        clearMap(false);

        if (terrainMap == null) {
            terrainMap = new int[width, height];
            initPos();
        }

        for (int i = 0; i < numSimulations; i++) {
            terrainMap = genTilePos(terrainMap);
        }

        FillTiles();
    }

    public void FillTiles() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (terrainMap[x, y] == 1) {
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), topTile);
                } else {
                    botMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), bottomTile);
                }
            }
        }
    }

    private int[,] genTilePos(int[,] oldMap) {
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

                if (oldMap[x, y] == 1) {
                    if (neighbour < deathLimit) {
                        newMap[x, y] = 0;
                    } else {
                        newMap[x, y] = 1;
                    }
                }

                if (oldMap[x, y] == 0) {
                    if (neighbour > birthLimit) {
                        newMap[x, y] = 1;
                    } else {
                        newMap[x, y] = 0;
                    }
                }
            }
        }

        return newMap;
    }

    private void initPos() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                terrainMap[x, y] = Random.Range(1, 101) < initChance ? 1 : 0;
            }
        }
    }

    private void clearMap(bool complete) {
        topMap.ClearAllTiles();
        botMap.ClearAllTiles();

        if (complete) {
            terrainMap = null;
        }
    }
}
