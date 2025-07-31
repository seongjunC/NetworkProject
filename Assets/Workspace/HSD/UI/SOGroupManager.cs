using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SOGroupManager : ISOGroupManager
    {
        private readonly ISOSpriteManager spriteManager;

        public SOGroupManager(ISOSpriteManager spriteManager)
        {
            this.spriteManager = spriteManager;
        }

        public IEnumerable<IGrouping<string, ScriptableObject>> GroupAssets(List<ScriptableObject> assets, string fieldName, Type type)
        {
            return assets.GroupBy(so =>
            {
                var field = type.GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                return field?.GetValue(so)?.ToString() ?? "Undefined";
            });
        }

        public Sprite GetFirstSpriteFromGroup(IGrouping<string, ScriptableObject> group, Type type)
        {
            Debug.Log($"GetFirstSpriteFromGroup called for group: {group.Key}, count: {group.Count()}");

            // 그룹의 모든 에셋을 순회하면서 첫 번째 스프라이트 찾기
            foreach (var asset in group)
            {
                var sprite = spriteManager.GetFirstSprite(asset, type);
                if (sprite != null)
                {
                    Debug.Log($"Found sprite: {sprite.name} in asset: {asset.name}");
                    return sprite;
                }
            }

            Debug.Log($"No sprite found in group: {group.Key}");
            return null;
        }

        public void UpdateGroupSprites(IGrouping<string, ScriptableObject> group, Sprite newSprite, Type type)
        {
            Debug.Log($"UpdateGroupSprites called for group: {group.Key}, newSprite: {(newSprite != null ? newSprite.name : "null")}");

            foreach (var asset in group)
            {
                spriteManager.UpdateSprite(asset, newSprite, type);
            }

            // 전체 저장은 한 번만
            AssetDatabase.SaveAssets();
            Debug.Log($"Group update completed for: {group.Key}");
        }
    }
}