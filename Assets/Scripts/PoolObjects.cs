using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ObjectsToPool {
    public GameObject obj;
    public int count;
    //public int weight;
}

public class PoolObjects : MonoBehaviour {
    public static PoolObjects Instance;
    private List<GameObject> pooledObjects;
    public ObjectsToPool[] objectsToPool;

    void Awake() {
        Instance = this;

        pooledObjects = new List<GameObject>();
        GameObject tmp;

        foreach(ObjectsToPool entry in objectsToPool) {
            for (int i = 0; i < entry.count; i++) {
                tmp = Instantiate(entry.obj);
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }
    }

    public GameObject GetPooledObject(Enum objectType) {
        //Debug.Log($"enemyType {enemyType}");
        int counter = 0;
        foreach (ObjectsToPool element in objectsToPool) {
            if (element.obj.name.ToUpper() == objectType.ToString()) {
                for (int i = 0; i < element.count; i++) {
                    if (!pooledObjects[counter+i].activeInHierarchy) {
                        //pooledObjects[i].GetComponent<Enemy>().TargetIndex = 0;
                        return pooledObjects[counter+i];
                    }
                }
                throw new Exception("All objects in pool are active, no object to return");
            }
            counter += element.count;
        }
        throw new Exception($"{objectType} Not found in pool");
    }

    //public GameObject GetPooledObject()
    //{
    //    for (int i = 0; i < enemyAmountToPool; i++)
    //    {
    //        if (!pooledObjects[i].activeInHierarchy)
    //        {
    //            pooledObjects[i].GetComponent<Enemy>().TargetIndex = 0;
    //            //Debug.Log($"Enemy {i} target index - {pooledObjects[i].GetComponent<Enemy>().TargetIndex}");
    //            return pooledObjects[i];
    //        }
    //    }
    //    return null;
    //}

    //public GameObject GetPooledObject()
    //{
    //    for (int i = 0; i < bulletAmountToPool; i++)
    //    {
    //        if (!pooledBulletObjects[i].activeInHierarchy)
    //        {
    //            return pooledBulletObjects[i];
    //        }
    //    }
    //    return null;
    //}
}
