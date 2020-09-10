using UnityEngine;

namespace Damdor
{
    public class PooledObject : MonoBehaviour
    {
        public GameObjectPool Pool => pool;
        public GameObject Prefab => prefab;

        [SerializeField]
        private GameObjectPool pool;
        [SerializeField]
        private GameObject prefab;

        public void Setup(GameObjectPool pool, GameObject prefab)
        {
            this.pool = pool;
            this.prefab = prefab;
        }
    }
}