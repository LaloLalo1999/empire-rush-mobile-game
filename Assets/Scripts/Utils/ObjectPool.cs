using System.Collections.Generic;
using UnityEngine;

namespace EmpireRush.Utils
{
    public class ObjectPool<T> where T : Component
    {
        private readonly Queue<T> pool = new Queue<T>();
        private readonly T prefab;
        private readonly Transform parent;
        private readonly bool autoExpand;
        
        public int CountInactive => pool.Count;
        public int CountActive { get; private set; }
        public int CountTotal => CountInactive + CountActive;
        
        public ObjectPool(T prefab, Transform parent = null, int initialSize = 10, bool autoExpand = true)
        {
            this.prefab = prefab;
            this.parent = parent;
            this.autoExpand = autoExpand;
            
            for (int i = 0; i < initialSize; i++)
            {
                CreatePooledObject();
            }
        }
        
        private T CreatePooledObject()
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
            return obj;
        }
        
        public T Get()
        {
            T obj;
            
            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else if (autoExpand)
            {
                obj = CreatePooledObject();
                pool.Dequeue(); // Remove it from pool since we're returning it
            }
            else
            {
                return null;
            }
            
            obj.gameObject.SetActive(true);
            CountActive++;
            
            return obj;
        }
        
        public void Return(T obj)
        {
            if (obj == null) return;
            
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
            CountActive = Mathf.Max(0, CountActive - 1);
        }
        
        public void ReturnAll()
        {
            T[] activeObjects = Object.FindObjectsOfType<T>();
            foreach (T obj in activeObjects)
            {
                if (obj.gameObject.activeInHierarchy)
                {
                    Return(obj);
                }
            }
        }
        
        public void Clear()
        {
            while (pool.Count > 0)
            {
                T obj = pool.Dequeue();
                if (obj != null)
                {
                    Object.Destroy(obj.gameObject);
                }
            }
            CountActive = 0;
        }
        
        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                CreatePooledObject();
            }
        }
    }
    
    public class SimpleObjectPool : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 10;
        [SerializeField] private bool autoExpand = true;
        [SerializeField] private Transform poolParent;
        
        private Queue<GameObject> pool = new Queue<GameObject>();
        
        public int CountInactive => pool.Count;
        public int CountActive { get; private set; }
        public int CountTotal => CountInactive + CountActive;
        
        private void Awake()
        {
            if (poolParent == null)
                poolParent = transform;
                
            for (int i = 0; i < initialSize; i++)
            {
                CreatePooledObject();
            }
        }
        
        private GameObject CreatePooledObject()
        {
            GameObject obj = Instantiate(prefab, poolParent);
            obj.SetActive(false);
            pool.Enqueue(obj);
            return obj;
        }
        
        public GameObject Get()
        {
            GameObject obj;
            
            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else if (autoExpand)
            {
                obj = CreatePooledObject();
                pool.Dequeue(); // Remove it from pool since we're returning it
            }
            else
            {
                return null;
            }
            
            obj.SetActive(true);
            CountActive++;
            
            return obj;
        }
        
        public void Return(GameObject obj)
        {
            if (obj == null) return;
            
            obj.SetActive(false);
            obj.transform.SetParent(poolParent);
            pool.Enqueue(obj);
            CountActive = Mathf.Max(0, CountActive - 1);
        }
        
        public void ReturnAll()
        {
            GameObject[] children = new GameObject[poolParent.childCount];
            for (int i = 0; i < poolParent.childCount; i++)
            {
                children[i] = poolParent.GetChild(i).gameObject;
            }
            
            foreach (GameObject obj in children)
            {
                if (obj.activeInHierarchy)
                {
                    Return(obj);
                }
            }
        }
        
        public void Clear()
        {
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            CountActive = 0;
        }
        
        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                CreatePooledObject();
            }
        }
    }
}