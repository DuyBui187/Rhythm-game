using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TapEffectSpawner : MonoBehaviour
{
    public static TapEffectSpawner instance;

    [Header("Effect Configuration")]
    [SerializeField] private List<GameObject> effectPrefabs; // Danh sách các prefab hiệu ứng
    [SerializeField] private Transform[] effectSpawnPositions; // Các vị trí để spawn hiệu ứng
    [SerializeField] private Transform effectParentTransform; // Transform cha để chứa các hiệu ứng

    [Header("Object Pooling")]
    private Dictionary<string, ObjectPool<GameObject>> effectPools; // Pool cho các hiệu ứng

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        effectPools = new Dictionary<string, ObjectPool<GameObject>>();

        // Tạo pool cho từng prefab
        foreach (var prefab in effectPrefabs)
        {
            string effectKey = prefab.name;
            effectPools[effectKey] = new ObjectPool<GameObject>(
                () => CreateEffectInstance(prefab),
                OnEffectGet,
                OnEffectRelease,
                OnEffectDestroy);
        }
    }

    private GameObject CreateEffectInstance(GameObject prefab)
    {
        return Instantiate(prefab, effectParentTransform);
    }

    private void OnEffectGet(GameObject effect)
    {
        effect.SetActive(true);
    }

    private void OnEffectRelease(GameObject effect)
    {
        effect.SetActive(false);
    }

    private void OnEffectDestroy(GameObject effect)
    {
        Destroy(effect);
    }

    public void SpawnEffect(string effectName, int spawnAreaID)
    {
        if (!effectPools.ContainsKey(effectName))
        {
            Debug.LogWarning($"Effect '{effectName}' not found in the pool.");
            return;
        }

        GameObject effectInstance = effectPools[effectName].Get();

        switch (spawnAreaID)
        {
            case 1:
                effectInstance.transform.position = effectSpawnPositions[0].position;
                break;

            case 2:
                effectInstance.transform.position = effectSpawnPositions[1].position;
                break;
        }

        StartCoroutine(ReleaseEffectAfterDelay(effectName, effectInstance));
    }

    private IEnumerator ReleaseEffectAfterDelay(string effectName, GameObject effectInstance)
    {
        yield return new WaitForSeconds(2f);

        if (effectPools.ContainsKey(effectName))
            effectPools[effectName].Release(effectInstance);
    }
}
