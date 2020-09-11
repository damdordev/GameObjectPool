using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Damdor
{
    public class GameObjectPool : MonoBehaviour
    {
        public UnityEvent<GameObject> onCreate = new UnityEvent<GameObject>();
        public UnityEvent<GameObject> onPop = new UnityEvent<GameObject>();
        public UnityEvent<GameObject> onPush = new UnityEvent<GameObject>();

        private readonly Dictionary<GameObject, List<PooledObject>> cache = new Dictionary<GameObject, List<PooledObject>>();
        private Transform root;

        public GameObject Pop(GameObject prefab)
        {
            if (root == null)
            {
                var rootGameObject = new GameObject("root");
                rootGameObject.transform.parent = transform;
                root = rootGameObject.transform;
                rootGameObject.SetActive(false);
                rootGameObject.SetActive(false);
            }
            
            if (prefab == null)
            {
                throw new ArgumentException("Trying to pop null");
            }

            if (!cache.ContainsKey(prefab))
            {
                cache[prefab] = new List<PooledObject>();
            }

            GameObject result;
            if (cache[prefab].Count == 0)
            {
                result = Instantiate(prefab, root);
                var pooledObject = result.AddComponent<PooledObject>();
                pooledObject.Setup(this, prefab);
                onCreate.Invoke(result);
            }
            else
            {
                var index = cache[prefab].Count - 1;
                var pooledObject = cache[prefab][index];
                cache[prefab].RemoveAt(index);
                result = pooledObject.gameObject;
            }

            onPop?.Invoke(result);
            return result;
        }

        public void Push(GameObject gameObjectToPush)
        {
            var poolGameObject = gameObjectToPush.GetComponent<PooledObject>();
            if (poolGameObject == null)
            {
                throw new ArgumentException("Trying to push object not created by pool");
            }

            if (poolGameObject.Pool != this)
            {
                throw new ArgumentException("Trying to push object created by other pool");
            }

            if (poolGameObject.Prefab == null)
            {
                throw new ArgumentException("Trying to push object created from null prefab");
            }

            if (cache[poolGameObject.Prefab].Contains(poolGameObject))
            {
                throw new ArgumentException("Trying to push the same object twice");
            }
            
            cache[poolGameObject.Prefab].Add(poolGameObject);
            poolGameObject.transform.parent = root;
            onPush?.Invoke(gameObjectToPush);
        }

    }
}