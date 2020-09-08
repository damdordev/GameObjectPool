using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace Damdor
{
    public class GameObjectPool : MonoBehaviour
    {
        public UnityEvent<GameObject> onCreate = new UnityEvent<GameObject>();
        public UnityEvent<GameObject> onPop = new UnityEvent<GameObject>();
        public UnityEvent<GameObject> onPush = new UnityEvent<GameObject>();

        public GameObject Pop(GameObject prefab) => null;
        public T Pop<T>(T prefab) where T : Component => null;
        public T Pop<T>(GameObject prefab) where T : Component => null;
        public void Push(GameObject gameObjectToPush) {}

        public void Clear() { }
        public void Clear(GameObject prefab) { }
    }
}