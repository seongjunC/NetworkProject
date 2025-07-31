using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Editor
{
    public class SODataProvider : ISODataProvider
    {
        private Dictionary<Type, List<ScriptableObject>> typeInstanceCache = new();

        public List<Type> GetAvailableSOTypes()
        {
            Assembly userAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp");

            return userAssembly?.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ScriptableObject)) && !t.IsAbstract)
                .Where(t => HasResourcesInstanceOfType(t))
                .ToList() ?? new List<Type>();
        }

        public List<ScriptableObject> LoadAssetsOfType(Type type)
        {
            Debug.Log($"LoadAssetsOfType called for: {type.Name}");

            // ĳ�ÿ� �ְ� ������� �ʴٸ� ��ȯ
            if (typeInstanceCache.ContainsKey(type) && typeInstanceCache[type].Count > 0)
            {
                var cached = typeInstanceCache[type];
                Debug.Log($"Returning cached result: {cached.Count}");
                return cached;
            }

            Debug.Log("Loading from AssetDatabase...");
            var result = new List<ScriptableObject>();
            string[] guids = AssetDatabase.FindAssets($"t:{type.Name}");
            Debug.Log($"Found GUIDs: {guids.Length}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log($"Checking path: {path}");
                if (!path.Contains("/Resources/")) continue;

                var asset = AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;
                if (asset != null)
                {
                    result.Add(asset);
                    Debug.Log($"Added asset: {asset.name}");
                }
            }

            Debug.Log($"Final result count: {result.Count}");

            // ����� ���� ���� ĳ�ÿ� ����
            if (result.Count > 0)
            {
                typeInstanceCache[type] = result;
            }

            return result;
        }

        public List<FieldInfo> GetGroupableFields(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.FieldType == typeof(string) || f.FieldType.IsEnum || f.FieldType == typeof(int))
                .ToList();
        }

        public void RefreshCache()
        {
            typeInstanceCache.Clear();
        }

        private bool HasResourcesInstanceOfType(Type type)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{type.Name}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("/Resources/"))
                    return true;
            }
            return false;
        }
    }
}