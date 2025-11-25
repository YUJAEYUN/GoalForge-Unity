using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Prefabs")]
    [SerializeField] private GameObject speedBoostPrefab;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject megaBallPrefab;
    [SerializeField] private GameObject trapBombPrefab;
    [SerializeField] private GameObject windCursePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private int maxItemsOnField = 2;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-8f, -4f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(8f, 4f);

    private List<GameObject> activeItems = new List<GameObject>();
    private List<GameObject> availablePrefabs = new List<GameObject>();
    private float nextSpawnTime;

    public void Setup(List<GameObject> items)
    {
        availablePrefabs = new List<GameObject>(items);
        Debug.Log($"ItemSpawner setup with {availablePrefabs.Count} items.");
    }

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        activeItems.RemoveAll(item => item == null);

        if (Time.time >= nextSpawnTime && activeItems.Count < maxItemsOnField)
        {
            SpawnRandomItem();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnRandomItem()
    {
        GameObject prefabToSpawn = GetRandomItemPrefab();
        if (prefabToSpawn == null) return;

        Vector2 spawnPosition = GetRandomSpawnPosition();
        GameObject newItem = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        newItem.transform.parent = transform;
        newItem.transform.localScale = Vector3.zero;
        activeItems.Add(newItem);

        StartCoroutine(SpawnItemAnimation(newItem));
    }

    System.Collections.IEnumerator SpawnItemAnimation(GameObject item)
    {
        if (item == null) yield break;

        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 targetScale = Vector3.one * 0.8f;

        while (elapsed < duration)
        {
            if (item == null) yield break;

            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float scale = Mathf.Lerp(0f, 1f, Mathf.Sin(progress * Mathf.PI * 0.5f));
            item.transform.localScale = targetScale * scale;

            yield return null;
        }

        if (item != null)
        {
            item.transform.localScale = targetScale;
        }
    }

    GameObject GetRandomItemPrefab()
    {
        if (availablePrefabs == null || availablePrefabs.Count == 0)
        {
            // Fallback to inspector assigned references if Setup wasn't called or list is empty
            availablePrefabs = new List<GameObject>();
            if (speedBoostPrefab != null) availablePrefabs.Add(speedBoostPrefab);
            if (shieldPrefab != null) availablePrefabs.Add(shieldPrefab);
            if (megaBallPrefab != null) availablePrefabs.Add(megaBallPrefab);
            if (trapBombPrefab != null) availablePrefabs.Add(trapBombPrefab);
            if (windCursePrefab != null) availablePrefabs.Add(windCursePrefab);
        }

        if (availablePrefabs.Count == 0) return null;

        return availablePrefabs[Random.Range(0, availablePrefabs.Count)];
    }

    Vector2 GetRandomSpawnPosition()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector2(x, y);
    }

    public void ClearAllItems()
    {
        foreach (GameObject item in activeItems)
        {
            if (item != null)
                Destroy(item);
        }
        activeItems.Clear();
    }
}

