using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public struct ObjectPoolChance {
        public GameObject obj;

        public float chance;
    }
    private static Dictionary<string, ObjectPool> instances = new Dictionary<string, ObjectPool>();

    public string allowedTag;
    public List<ObjectPoolChance> objects;
    public List<GameObject> objectPool;
    public int capacity = 20;
    public bool expandIfEmpty = false;
    void Awake()
    {
        if(instances.ContainsKey(allowedTag)) {
            instances.Remove(allowedTag);
        }

        instances.Add(allowedTag, this);
        objectPool = new List<GameObject>(capacity);
        for(int i = 0; i < capacity; ++i) {
            objectPool.Add(CreateObject());
        }
    }

    public GameObject CreateObject() {
            GameObject chosenObject = objects[0].obj;
            foreach(ObjectPoolChance opChance in objects) {
                float chance = Random.Range(0f, 1f);
                if(chance < opChance.chance) {
                    chosenObject = opChance.obj;
                    break;
                }
            }
            GameObject obj = Instantiate(chosenObject, transform);
            obj.SetActive(false);

            return obj;
    }

    public GameObject Pop() {
        if(objectPool.Count == 0) {
            if(expandIfEmpty) {
                objectPool.Add(CreateObject());
                capacity++;
            } else {
                return null;
            }
        }
        GameObject obj = objectPool[0];
        objectPool.Remove(obj);
        obj.SetActive(true);
        obj.transform.parent = null;
        return obj;
    }

    public void Push(GameObject obj) {
        if(obj.tag.Equals(allowedTag)) {
            obj.transform.parent = transform;
            objectPool.Add(obj);
            obj.SetActive(false);
            return;
        }
        
        Debug.Log("This item can't be added: " + obj.tag + " to " + allowedTag);
    }

    public GameObject PopRandom() {
        if(objectPool.Count == 0) {
            if(expandIfEmpty) {
                objectPool.Add(CreateObject());
                capacity++;
            } else {
                return null;
            }
        }
        GameObject obj = objectPool[Random.Range(0, objectPool.Count)];
        objectPool.Remove(obj);
        obj.SetActive(true);
        obj.transform.parent = null;
        return obj;
    }

    public static ObjectPool GetObjectPool(string tag) {
        if(!instances.ContainsKey(tag)) return null;
        return instances[tag];
    }
}
