using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace SOEditor
{
    /// <summary>스프라이트 관리자</summary>
    public class SOSpriteManager : ISOSpriteManager
    {
        public Sprite GetFirstSprite(ScriptableObject so, Type type)
        {
            Debug.Log($"GetFirstSprite called for: {so.name}");
            var spriteFields = GetSpriteFields(type);
            Debug.Log($"Found {spriteFields.Count} sprite fields");

            foreach (var field in spriteFields)
            {
                var sprite = field.GetValue(so) as Sprite;
                Debug.Log($"Field {field.Name}: {(sprite != null ? sprite.name : "null")}");
                if (sprite != null)
                    return sprite;
            }
            return null;
        }

        public void UpdateSprite(ScriptableObject so, Sprite newSprite, Type type)
        {
            Debug.Log($"UpdateSprite called for: {so.name}, newSprite: {(newSprite != null ? newSprite.name : "null")}");

            var spriteFields = GetSpriteFields(type);

            // 기존 스프라이트가 있는 필드만 업데이트
            bool updated = false;
            foreach (var field in spriteFields)
            {
                var currentSprite = field.GetValue(so) as Sprite;
                if (currentSprite != null || newSprite != null) // 기존에 스프라이트가 있거나 새 스프라이트를 설정하려는 경우
                {
                    field.SetValue(so, newSprite);
                    EditorUtility.SetDirty(so);
                    updated = true;
                    Debug.Log($"Updated field {field.Name} in {so.name}");
                }
            }

            if (updated)
            {
                AssetDatabase.SaveAssets();
                Debug.Log($"Saved changes for {so.name}");
            }
        }

        public List<FieldInfo> GetSpriteFields(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.FieldType == typeof(Sprite))
                .ToList();

            Debug.Log($"GetSpriteFields for {type.Name}: found {fields.Count} fields");
            foreach (var field in fields)
            {
                Debug.Log($"  - {field.Name}");
            }

            return fields;
        }
    }
}