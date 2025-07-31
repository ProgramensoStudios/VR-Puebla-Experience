using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabsToCreate; 
    [SerializeField] private int initialPoolSize = 10;
    private readonly List<GameObject> _createdObjects = new List<GameObject>();

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        if (prefabsToCreate == null || prefabsToCreate.Count == 0)
        {
            return;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            int prefabIndex = i % prefabsToCreate.Count;
            GameObject obj = Instantiate(prefabsToCreate[prefabIndex], transform);
            obj.SetActive(false);
            _createdObjects.Add(obj);
        }
    }

    public GameObject AskForObject(Transform pos)
    {
        if (pos == null)
        {
            return null;
        }

       
        for (var index = 0; index < _createdObjects.Count; index++)
        {
            var obj = _createdObjects[index];
            if (obj.activeInHierarchy) continue;
            SetupObject(obj, pos);
            return obj;
        }
        
        if (prefabsToCreate.Count == 0)
        {
            return null;
        }

        int prefabIndex = Random.Range(0, prefabsToCreate.Count);
        GameObject newObject = Instantiate(prefabsToCreate[prefabIndex], transform);
        _createdObjects.Add(newObject);
        SetupObject(newObject, pos);
        return newObject;
    }

    private void SetupObject(GameObject obj, Transform position)
    {
        obj.transform.position = position.position;
        obj.transform.rotation = position.rotation;
        obj.SetActive(true);
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(false);
        }
    }
}
