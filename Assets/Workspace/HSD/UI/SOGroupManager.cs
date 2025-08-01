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
            // �׷��� ��� ������ ��ȸ�ϸ鼭 ù ��° ��������Ʈ ã��
            foreach (var asset in group)
            {
                var sprite = spriteManager.GetFirstSprite(asset, type);
                if (sprite != null)
                {
                    return sprite;
                }
            }

            return null;
        }

        public void UpdateGroupSprites(IGrouping<string, ScriptableObject> group, Sprite newSprite, Type type)
        {
            foreach (var asset in group)
            {
                spriteManager.UpdateSprite(asset, newSprite, type);
            }

            // ��ü ������ �� ����
            AssetDatabase.SaveAssets();
        }
    }
}