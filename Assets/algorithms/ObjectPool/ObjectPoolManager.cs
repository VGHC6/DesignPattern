using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    //对象池信息
    private static List<PoolObjectInfo> poolObjectInfos = new List<PoolObjectInfo>();

    //生成对象
    public GameObject SpawnPoolObject(GameObject objectSpawn, Vector3 spwanPosition, Quaternion spawnRotation)
    {
        //查找对象池信息
        PoolObjectInfo pool = poolObjectInfos.Find(x => x.lookupString == objectSpawn.name);
        if (pool == null)
        {
            //创建对象池信息
            pool = new PoolObjectInfo();
            pool.lookupString = objectSpawn.name;
            poolObjectInfos.Add(pool);
        }

        //获取非活动对象
        GameObject spwanObject = pool.inactiveObjects.FirstOrDefault();
        if (spwanObject == null)
        {
            //没有非活动对象，创建新对象
            spwanObject = Instantiate(objectSpawn, spwanPosition, spawnRotation);
        }
        else
        {
            //存在非活动对象，设置旋转，位置
            spwanObject.transform.position = spwanPosition;
            spwanObject.transform.rotation = spawnRotation;
            //移除非活动对象
            pool.inactiveObjects.Remove(spwanObject);
            //添加到活动对象
            spwanObject.SetActive(true);
        }
        return spwanObject;
    }

    //销毁对象
    public void ReturnObjectToPool(GameObject obj)
    {
        //去掉对象后面的(clone)
        string goName = obj.name.Substring(0, obj.name.Length - 7);

        //查找对象池信息
        PoolObjectInfo pool = poolObjectInfos.Find(x => x.lookupString == goName);
        if (pool == null)
        {
            Debug.LogWarning($"对象池中没有{goName}的池信息");
            return;
        }
        else
        {
            //添加到非活动对象
            obj.SetActive(false);
            pool.inactiveObjects.Add(obj);            
        }
    }
}


//对象池信息
public class PoolObjectInfo
{
    public string lookupString;
    //非活动对象
    public List<GameObject> inactiveObjects = new List<GameObject>();
}