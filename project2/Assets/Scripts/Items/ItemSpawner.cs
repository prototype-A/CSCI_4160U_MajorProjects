using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {

    public Item spawnedItem;
    public int respawnTime = 0;
    public Item[] itemPrefabs;
    public int[] itemSpawnChance;

    // Start is called before the first frame update
    void Start() {
        // Spawn initial item
        SpawnItem();
        StartCoroutine(SpawnItemAfterPeriod());
    }

    private void SpawnItem() {
        // Determine item to spawn
        int chance = Random.Range(0, 101);
        int itemToSpawn = 0;
        int prevItemChance = 0;
        if (chance > itemSpawnChance[itemToSpawn]) {
            for (itemToSpawn = 1; itemToSpawn < itemSpawnChance.Length - 1; itemToSpawn++) {
                prevItemChance += itemSpawnChance[itemToSpawn - 1];
                if (chance >= prevItemChance &&
                    chance < prevItemChance + itemSpawnChance[itemToSpawn]) {
                    break;
                }
            }
        }

        // Spawn item
        if (itemPrefabs[itemToSpawn] != null) {
            spawnedItem = Instantiate(itemPrefabs[itemToSpawn],
                                        transform.position,
                                        Quaternion.Euler(0, Random.Range(0, 360), 0),
                                        transform);
            spawnedItem.gameObject.layer = LayerMask.NameToLayer("Items");
            spawnedItem.spawnPoint = this;
        }
    }

    private IEnumerator SpawnItemAfterPeriod() {
        while (true) {
            yield return new WaitForSeconds(respawnTime);
            if (spawnedItem == null && itemPrefabs.Length == itemSpawnChance.Length) {
                SpawnItem();
            }
        }
    }
}
