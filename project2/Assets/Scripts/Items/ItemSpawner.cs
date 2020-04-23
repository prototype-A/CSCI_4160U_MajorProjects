using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {

    public int respawnTime = 0;
    public Item[] itemPrefabs;
    public Item spawnedItem;

    // Start is called before the first frame update
    void Start() {
        // Spawn initial item
        SpawnItem();
        StartCoroutine(SpawnItemAfterPeriod());
    }

    private void SpawnItem() {
        spawnedItem = Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)],
                                    transform.position,
                                    Quaternion.Euler(0, Random.Range(0, 360), 0),
                                    transform);
        spawnedItem.gameObject.layer = LayerMask.NameToLayer("Items");
        spawnedItem.spawnPoint = this;
    }

    private IEnumerator SpawnItemAfterPeriod() {
        while (true) {
            yield return new WaitForSeconds(respawnTime);
            if (spawnedItem == null) {
                SpawnItem();
            }
        }
    }
}
