using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private GameObject example;
    private Transform storage;

    private Queue<GameObject> poolOfObjects;
    public ObjectPool(int Index, GameObject Example, Transform Storage)
    {
        example = Example;
        storage = Storage;

        poolOfObjects = new Queue<GameObject>();


        for (int i = 0; i < Index; i++)
        {
            GameObject _object = Instantiate(Example, Vector3.zero, Quaternion.identity, Storage);
            _object.SetActive(false);
            poolOfObjects.Enqueue(_object);
        }
    }

    public ObjectPool(int Index, GameObject Example)
    {
        example = Example;
        storage = null;

        poolOfObjects = new Queue<GameObject>();


        for (int i = 0; i < Index; i++)
        {
            GameObject _object = Instantiate(Example);
            _object.SetActive(false);
            poolOfObjects.Enqueue(_object);
        }
    }

    public GameObject GetObject()
    {
        if (poolOfObjects.Count > 0) return poolOfObjects.Dequeue();

        print("instantiated new object of type: queue is full");
        GameObject _object = null;
        if (storage == null)
        {
            _object = Instantiate(example);
        }
        else
        {
            _object = Instantiate(example, storage);
        }

        _object.SetActive(false);

        return _object;
    }

    public GameObject GetObject(bool isActiveBeforeTaken)
    {
        if (poolOfObjects.Count > 0)
        {
            GameObject result = poolOfObjects.Dequeue();
            result.SetActive(isActiveBeforeTaken);
            return result;
        }


        print("instantiated new object of type: queue is full");
        GameObject _object = null;
        if (storage == null)
        {
            Instantiate(example);
        }
        else
        {
            Instantiate(example, storage);
        }

        _object.SetActive(isActiveBeforeTaken);

        return _object;
    }

    public void ReturnObject(GameObject _object)
    {
        _object.SetActive(false);
        poolOfObjects.Enqueue(_object);
    }
}
