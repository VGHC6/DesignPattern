using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolTest : MonoBehaviour
{
    public ObjectPoolManager poolManager;
    public GameObject testPrefab;
    public int spawnCount = 10;
    public float spawnRadius = 5f;
    public float returnDelay = 2f;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private float timer;

    void Start()
    {
        if (poolManager == null)
        {
            poolManager = FindObjectOfType<ObjectPoolManager>();
            if (poolManager == null)
            {
                GameObject go = new GameObject("ObjectPoolManager");
                poolManager = go.AddComponent<ObjectPoolManager>();
            }
        }

        if (testPrefab == null)
        {
            testPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testPrefab.name = "TestCube";
            testPrefab.transform.position = new Vector3(9999, 9999, 9999);
            testPrefab.SetActive(false);
        }

        Debug.Log("=== 对象池测试启动 ===");
        Debug.Log("按 A 键：批量生成对象");
        Debug.Log("按 R 键：回收所有对象");
        Debug.Log("按 1 键：生成单个对象");
        Debug.Log("按 2 键：回收最后一个对象");
        Debug.Log("按 T 键：自动循环测试（生成->延时回收）");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnMultipleObjects();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReturnAllObjects();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnSingleObject();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ReturnLastObject();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(AutoCycleTest());
        }

        PrintPoolStatus();
    }

    void SpawnSingleObject()
    {
        Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
        randomPos.y = 0;
        Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360), 0);

        GameObject obj = poolManager.SpawnPoolObject(testPrefab, randomPos, randomRot);
        spawnedObjects.Add(obj);

        Debug.Log($"生成对象: {obj.name}, 实例ID: {obj.GetInstanceID()}, 当前存活: {spawnedObjects.Count}");
    }

    void SpawnMultipleObjects()
    {
        Debug.Log($"--- 批量生成 {spawnCount} 个对象 ---");
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnSingleObject();
        }
    }

    void ReturnAllObjects()
    {
        Debug.Log($"--- 回收全部 {spawnedObjects.Count} 个对象 ---");
        foreach (var obj in spawnedObjects)
        {
            if (obj != null && obj.activeSelf)
            {
                poolManager.ReturnObjectToPool(obj);
                Debug.Log($"回收对象: {obj.name}, 实例ID: {obj.GetInstanceID()}");
            }
        }
        spawnedObjects.Clear();
    }

    void ReturnLastObject()
    {
        if (spawnedObjects.Count > 0)
        {
            GameObject lastObj = spawnedObjects[spawnedObjects.Count - 1];
            spawnedObjects.RemoveAt(spawnedObjects.Count - 1);

            if (lastObj != null && lastObj.activeSelf)
            {
                poolManager.ReturnObjectToPool(lastObj);
                Debug.Log($"回收最后一个对象: {lastObj.name}, 实例ID: {lastObj.GetInstanceID()}, 剩余存活: {spawnedObjects.Count}");
            }
        }
        else
        {
            Debug.LogWarning("没有可回收的对象");
        }
    }

    IEnumerator AutoCycleTest()
    {
        Debug.Log("=== 开始自动循环测试 ===");

        while (true)
        {
            Debug.Log("--- [自动测试] 生成阶段 ---");
            SpawnMultipleObjects();
            yield return new WaitForSeconds(returnDelay);

            Debug.Log("--- [自动测试] 回收阶段 ---");
            ReturnAllObjects();
            yield return new WaitForSeconds(returnDelay);
        }
    }

    void PrintPoolStatus()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("=== 对象池状态 ===");
            foreach (var pool in GetPoolInfos())
            {
                Debug.Log($"对象类型: {pool.lookupString}, 池内闲置数量: {pool.inactiveObjects.Count}");
            }
            Debug.Log($"当前场景存活对象: {spawnedObjects.Count}");
        }
    }

    List<PoolObjectInfo> GetPoolInfos()
    {
        var field = typeof(ObjectPoolManager).GetField("poolObjectInfos",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        return (List<PoolObjectInfo>)field.GetValue(null);
    }
}