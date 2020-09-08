using Damdor;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Damdor
{
    public class GameObjectPoolTest
    {
        private GameObjectPool pool;
        private GameObject prefab1;
        private GameObject prefab2;

        [SetUp]
        public void Setup()
        {
            var gameObject = new GameObject();
            pool = gameObject.AddComponent<GameObjectPool>();
         
            prefab1 = new GameObject("prefab1");
            prefab2 = new GameObject("prefab2");
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(pool.gameObject);
            GameObject.Destroy(prefab1);
            GameObject.Destroy(prefab2);
        }
        
    }
}