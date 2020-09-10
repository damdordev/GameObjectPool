using System;
using Damdor;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Damdor
{
    public class GameObjectPoolTest
    {
        private class PrefabData
        {
            public GameObject gameObject;
            public Texture2D texture;
            public Sprite sprite;
        }
        
        private GameObjectPool pool;
        private PrefabData prefab1;
        private PrefabData prefab2;
        
        [SetUp]
        public void Setup()
        {
            pool = CreatePool();
            prefab1 = CreatePrefab();
            prefab2 = CreatePrefab();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyPool(pool);
            DestroyPrefab(prefab1);
            DestroyPrefab(prefab2);
        }

        [Test]
        public void TestIfPopReturnsGameObjectInstantiatedFromCorrectPrefab()
        {
            var obj = pool.Pop(prefab1.gameObject);
            Assert.IsTrue(IsCreatedFromSpritePrefab(obj, prefab1));
        }

        [Test]
        public void TestIfTwoPopsReturnsDifferentGameObjects()
        {
            var object1 = pool.Pop(prefab1.gameObject);
            var object2 = pool.Pop(prefab1.gameObject);

            Assert.AreNotEqual(object1, object2);
        }

        [Test]
        public void TestIsPopReturnsTheSameObjectAfterPush()
        {
            var object1 = pool.Pop(prefab1.gameObject);
            pool.Push(object1);
            var object2 = pool.Pop(prefab1.gameObject);
            
            Assert.AreEqual(object1, object2);
        }

        [Test]
        public void TestIfPopReturnsDifferentObjectsForDifferentPrefabs()
        {
            var object1 = pool.Pop(prefab1.gameObject);
            pool.Push(object1);
            var object2 = pool.Pop(prefab2.gameObject);
            
            Assert.AreNotEqual(object1, object2);
        }

        [Test]
        public void TestIfOnCreateIsFired()
        {
            var invocation = new UnityEventTester<GameObject>(pool.onCreate);
            var object1 = pool.Pop(prefab1.gameObject);
            
            Assert.AreEqual(1, invocation.InvocationCount);
            Assert.AreEqual(object1, invocation.InvocationParameters[0]);
        }

        [Test]
        public void TestIfOnCreateIsFiredOnlyOnceForTheSamePrefab()
        {
            var invocation = new UnityEventTester<GameObject>(pool.onCreate);
            var object1 = pool.Pop(prefab1.gameObject);
            pool.Push(object1);
            pool.Pop(prefab1.gameObject);
            
            Assert.AreEqual(1, invocation.InvocationCount);
        }

        [Test]
        public void TestIfOnCreateIsFiredTwiceForTwoPops()
        {
            var invocation = new UnityEventTester<GameObject>(pool.onCreate);
            var object1 = pool.Pop(prefab1.gameObject);
            var object2 = pool.Pop(prefab1.gameObject);
            
            Assert.AreEqual(2, invocation.InvocationCount);
            Assert.AreEqual(object1, invocation.InvocationParameters[0]);
            Assert.AreEqual(object2, invocation.InvocationParameters[1]);
        }

        [Test]
        public void TestIfOnCreateIsFiredTwiceForTwoPrefabs()
        {
            var invocation = new UnityEventTester<GameObject>(pool.onCreate);
            var object1 = pool.Pop(prefab1.gameObject);
            pool.Push(object1);
            var object2 = pool.Pop(prefab2.gameObject);
            
            Assert.AreEqual(2, invocation.InvocationCount);
            Assert.AreEqual(object1, invocation.InvocationParameters[0]);
            Assert.AreEqual(object2, invocation.InvocationParameters[1]);
        }

        [Test]
        public void TestPushWithGameObjectNotCreatedByPool()
        {
            var object1 = new GameObject();
            Assert.Throws<ArgumentException>(() => pool.Push(object1));
        }
        
        [Test]
        public void TestPushWithGameObjectCreatedByOtherPool()
        {
            var pool2 = CreatePool();
            var object1 = pool2.Pop(prefab1.gameObject);
            
            Assert.Throws<ArgumentException>(() => pool.Push(object1));
            
            DestroyPool(pool2);
        }
        
        [Test]
        public void TestPushTheSameObjectTwice()
        {
            var object1 = pool.Pop(prefab1.gameObject);
            Assert.Throws<ArgumentException>(() =>
            {
                pool.Push(object1);
                pool.Push(object1);
            });
        }

        [Test]
        public void TestPopNull()
        {
            Assert.Throws<ArgumentException>(() => pool.Pop(null));
        }

        [Test]
        public void TestIfOnPopIsInvoked()
        {
            var invocation = new UnityEventTester<GameObject>(pool.onPop);
            var object1 = pool.Pop(prefab1.gameObject);
            
            Assert.AreEqual(1, invocation.InvocationCount);
            Assert.AreEqual(object1, invocation.InvocationParameters[0]);
        }

        [Test]
        public void TestIfOnPopIsInvokedTwiceForTwoPopTheSameObject()
        {
            var invocation = new UnityEventTester<GameObject>(pool.onPop);
            var object1 = pool.Pop(prefab1.gameObject);
            pool.Push(object1);
            pool.Pop(prefab1.gameObject);
            
            Assert.AreEqual(2, invocation.InvocationCount);
            Assert.AreEqual(object1, invocation.InvocationParameters[0]);
            Assert.AreEqual(object1, invocation.InvocationParameters[1]);
        }

        [Test]
        public void TestIfOnPushIsNotInvokedBeforePush()
        {
            var invocation = new UnityEventTester<GameObject>(pool.onPush);
            pool.Pop(prefab1.gameObject);
            
            Assert.AreEqual(0, invocation.InvocationCount);
        }

        [Test]
        public void TestIfOnPushIsInvoked()
        {
            var invocation = new UnityEventTester<GameObject>(pool.onPush);
            var object1 = pool.Pop(prefab1.gameObject);
            pool.Push(object1);
            
            Assert.AreEqual(1, invocation.InvocationCount);
            Assert.AreEqual(object1, invocation.InvocationParameters[0]);
        }

        [Test]
        public void TestIfOnPushIsInvokedTwiceForTwoPopTheSameObject()
        {
            var invocation = new UnityEventTester<GameObject>(pool.onPush);
            var object1 = pool.Pop(prefab1.gameObject);
            pool.Push(object1);
            pool.Pop(prefab1.gameObject);
            pool.Push(object1);
            
            Assert.AreEqual(2, invocation.InvocationCount);
            Assert.AreEqual(object1, invocation.InvocationParameters[0]);
            Assert.AreEqual(object1, invocation.InvocationParameters[1]);
        }

        [Test]
        public void TestIfPoolSetSelfAsGrandparentForNewCreatedObjects()
        {
            var object1 = pool.Pop(prefab1.gameObject);
            
            Assert.AreEqual(pool.transform, object1.transform.parent.parent);
        }
        
        [Test]
        public void TestIfPoolSetSelfAsGrandparentAfterPush()
        {
            var object1 = pool.Pop(prefab1.gameObject);
            object1.transform.parent = null;
            pool.Push(object1);
            
            Assert.AreEqual(pool.transform, object1.transform.parent.parent);
        }

        [Test]
        public void TestIfPoolSetNewCreatedObjectAsInactive()
        {
            var object1 = pool.Pop(prefab1.gameObject);
            
            Assert.IsFalse(object1.activeInHierarchy);
        }
        
        [Test]
        public void TestIfPoolSetObjectInactiveAfterPush()
        {
            var object1 = pool.Pop(prefab1.gameObject);
            object1.transform.parent = null;
            object1.SetActive(true);
            pool.Push(object1);
            
            Assert.IsFalse(object1.activeInHierarchy);
        }
        
        private static GameObjectPool CreatePool()
        {
            var gameObject = new GameObject();
            return gameObject.AddComponent<GameObjectPool>();
        }

        private static void DestroyPool(GameObjectPool pool)
        {
            Object.Destroy(pool.gameObject);
        }

        private static PrefabData CreatePrefab()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var data = new PrefabData();

            data.texture = new Texture2D(100, 100, TextureFormat.RGBA32, false);
            data.sprite = Sprite.Create(data.texture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f), 1f);
            data.gameObject = new GameObject("spritePrefab");
            data.gameObject.AddComponent<SpriteRenderer>().sprite = data.sprite;

            return data;
        }

        private static void DestroyPrefab(PrefabData data)
        {
            Object.Destroy(data.gameObject);
            Object.Destroy(data.sprite);
            Object.Destroy(data.texture);
        }
        
        private static bool IsCreatedFromSpritePrefab(GameObject gameObject, PrefabData data)
        {
            return gameObject.GetComponent<SpriteRenderer>()?.sprite == data.sprite;
        }
        
    }
}