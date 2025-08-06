using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<string, IObjectPool<GameObject>> poolDic;
    private Dictionary<string, Transform> parentDic;
    private Dictionary<string, float> lastUseTimeDic;

    private Transform parent;

    private Coroutine poolCleanupRoutine;   
    private YieldInstruction cleanUpDelay;

    private const float poolCleanupTime = 60;
    private const float poolCleanupDelay = 30;

    public void Start()
    {
        cleanUpDelay = new WaitForSeconds(poolCleanupDelay);

        ResetPool();
        poolCleanupRoutine = StartCoroutine(PoolCleanupRoutine());

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void ResetPool()
    {
        poolDic = new();
        parentDic = new();
        lastUseTimeDic = new();

        parent = new GameObject("Pools").transform;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetPool();
    }

    IEnumerator PoolCleanupRoutine()
    {
        while (true)
        {
            yield return cleanUpDelay;

            float now = Time.time;
            List<string> removePoolKeys = new List<string>();

            foreach (var kvp in poolDic)
            {
                string key = kvp.Key;

                if (lastUseTimeDic.TryGetValue(key, out float lastTime))
                {
                    if (now - lastTime > poolCleanupTime)
                    {
                        removePoolKeys.Add(key);
                    }
                }
            }

            foreach (var value in removePoolKeys)
            {
                poolDic.Remove(value);

                if (parentDic[value].gameObject != null)
                    Destroy(parentDic[value].gameObject);

                parentDic.Remove(value);
                lastUseTimeDic.Remove(value);
            }
        }
    }

    public void StopPoolCleanupRoutine()
    {
        StopCoroutine(poolCleanupRoutine);
        poolCleanupRoutine = null;
    }

    private IObjectPool<GameObject> GetOrCreatePool(string name, GameObject prefab)
    {
        if(poolDic.ContainsKey(name))
            return poolDic[name];

        Transform root = new GameObject($"{name} Pool").transform;
        root.parent = parent;
        parentDic.Add(name, root);

        ObjectPool<GameObject> pool = new ObjectPool<GameObject>
        (
            createFunc: () =>
            {
                GameObject obj = Instantiate(prefab);
                obj.name = name;
                obj.transform.parent = root;
                lastUseTimeDic[name] = Time.time;
                return obj;
            },
            actionOnGet: (GameObject go) =>
            {
                go.transform.parent = null;
                go.SetActive(true);
                lastUseTimeDic[name] = Time.time;
            },
            actionOnRelease: (GameObject go) =>
            {
                go.transform.parent = root;
                go.SetActive(false);
            },
            actionOnDestroy: (GameObject go) =>
            {
                Destroy(go);
            },
            maxSize: 10
        );

        poolDic.Add(name, pool);
        return pool;
    }

    public T Get<T> (T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
    {
        GameObject go = original as GameObject;
        string name = go.name;

        var pool = GetOrCreatePool(name, go);

        go = pool.Get();

        if (parent != null)
            go.transform.SetParent(parent, false);

        go.transform.localPosition = position;
        go.transform.rotation = rotation;

        return go as T;
    }

    public T Get<T>(T original, Vector3 position, Quaternion rotation) where T : Object
    {
        return Get<T>(original, position, rotation, null);
    }

    public T Get<T>(T original, Vector3 position) where T : Object
    {
        return Get<T>(original, position, Quaternion.identity);
    }

    public T Get<T>(T original, Vector3 position, Transform parent) where T : Object
    {
        return Get<T>(original, position, Quaternion.identity, parent);
    }

    public void Release<T> (T original) where T : Object
    {
        GameObject obj = original as GameObject;
        string name = obj.name;

        if (!poolDic.ContainsKey(name) && !obj.activeSelf)
            return;

        poolDic[name].Release(obj);
    }

    public void Release<T>(T original, float delay) where T : Object
    {
        StartCoroutine(DelayRelease(original, delay));
    }

    private IEnumerator DelayRelease<T>(T original, float delay) where T : Object
    {
        yield return new WaitForSeconds(delay);

        GameObject obj = original as GameObject;

        if (obj == null || !obj.activeSelf) yield break;

        string name = obj.name;

        if (!poolDic.ContainsKey(name) && !obj.activeSelf)
            yield break;

        poolDic[name].Release(obj);
    }

    public bool ContainsKey(string name) => poolDic.ContainsKey(name);
}
