using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace SOEditor
{
    /// <summary>ScriptableObject 데이터 관리 인터페이스</summary>
    public interface ISODataProvider
    {
        List<Type> GetAvailableSOTypes();
        List<ScriptableObject> LoadAssetsOfType(Type type);
        List<FieldInfo> GetGroupableFields(Type type);
        void RefreshCache();
    }

    /// <summary>UI 렌더링 인터페이스</summary>
    public interface ISOUIRenderer
    {
        void RenderFlatUI(ScrollView container, List<ScriptableObject> assets, Type selectedType);
        void RenderGroupedUI(ScrollView container, IEnumerable<IGrouping<string, ScriptableObject>> groups,
                           Type selectedType, bool showIcons, Color[] colors, string groupFieldName, Action onGroupFieldChanged);
        VisualElement CreateSOEditorBox(ScriptableObject so, Type selectedType, string groupFieldName, bool isGrouping, Action onGroupFieldChanged = null);
        VisualElement CreateFieldElement(FieldInfo field, ScriptableObject so, bool isGroupingField, Action onGroupFieldChanged);
    }

    /// <summary>그룹 관리 인터페이스</summary>
    public interface ISOGroupManager
    {
        IEnumerable<IGrouping<string, ScriptableObject>> GroupAssets(List<ScriptableObject> assets, string fieldName, Type type);
        Sprite GetFirstSpriteFromGroup(IGrouping<string, ScriptableObject> group, Type type);
        void UpdateGroupSprites(IGrouping<string, ScriptableObject> group, Sprite newSprite, Type type);
    }

    /// <summary>스프라이트 관리 인터페이스</summary>
    public interface ISOSpriteManager
    {
        Sprite GetFirstSprite(ScriptableObject so, Type type);
        void UpdateSprite(ScriptableObject so, Sprite newSprite, Type type);
        List<FieldInfo> GetSpriteFields(Type type);
    }
}
