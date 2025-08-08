using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SOEditor
{
    /// <summary>UI 렌더러</summary>
    public class SOUIRenderer : ISOUIRenderer
    {
        private readonly Color[] groupColors = {
            new Color(0.8f, 0.4f, 0.4f), new Color(0.4f, 0.8f, 0.4f), new Color(0.4f, 0.4f, 0.8f),
            new Color(0.8f, 0.8f, 0.4f), new Color(0.8f, 0.4f, 0.8f), new Color(0.4f, 0.8f, 0.8f),
            new Color(0.9f, 0.6f, 0.3f), new Color(0.6f, 0.9f, 0.6f),
        };

        // 의존성 주입을 위한 매니저들
        private ISOGroupManager groupManager;

        // UI 재생성 방지를 위한 캐시
        private Dictionary<string, ObjectField> groupIconFields = new Dictionary<string, ObjectField>();
        private Dictionary<string, VisualElement> groupIconDisplays = new Dictionary<string, VisualElement>();
        private ScrollView currentContainer; // 현재 컨테이너 참조 유지

        // 매니저를 설정하는 메서드 추가
        public void SetManagers(ISOGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        public void RenderFlatUI(ScrollView container, List<ScriptableObject> assets, Type selectedType)
        {
            // 캐시 초기화
            ClearUICache();
            currentContainer = container;

            foreach (var asset in assets)
            {
                container.Add(CreateSOEditorBox(asset, selectedType, null, false, null));
            }
        }

        public void RenderGroupedUI(ScrollView container, IEnumerable<IGrouping<string, ScriptableObject>> groups,
                                  Type selectedType, bool showIcons, Color[] colors, string groupFieldName, Action onGroupFieldChanged)
        {
            // 캐시 초기화는 하지 않고 현재 컨테이너만 업데이트
            currentContainer = container;

            int groupIndex = 0;
            foreach (var group in groups)
            {
                var groupColor = colors[groupIndex % colors.Length];

                if (showIcons)
                    CreateGroupWithIcon(container, group, groupColor, selectedType, groupFieldName, onGroupFieldChanged);
                else
                    CreateGroupWithoutIcon(container, group, groupColor, selectedType, groupFieldName, onGroupFieldChanged);

                groupIndex++;
            }
        }

        /// <summary>UI 캐시 초기화 - 타입이 바뀔 때만 호출</summary>
        public void ClearUICache()
        {
            groupIconFields.Clear();
            groupIconDisplays.Clear();
        }

        /// <summary>부분적 UI 업데이트 - 아이콘 변경 시에만 필요한 부분만 업데이트</summary>
        private void UpdateDataContainersOnly(ScrollView container, IEnumerable<IGrouping<string, ScriptableObject>> groups,
                                            Type selectedType, string groupFieldName, Action onGroupFieldChanged)
        {
            // 기존 그룹 컨테이너들을 찾아서 데이터 부분만 업데이트
            var groupContainers = container.Query<VisualElement>()
                .Where(ve => ve.style.flexDirection == FlexDirection.Row && ve.childCount >= 2)
                .ToList();

            int groupIndex = 0;
            foreach (var group in groups)
            {
                if (groupIndex < groupContainers.Count)
                {
                    var groupContainer = groupContainers[groupIndex];
                    var dataContainer = groupContainer.ElementAt(1) as ScrollView; // 두 번째 자식이 데이터 컨테이너

                    if (dataContainer != null)
                    {
                        dataContainer.Clear();
                        foreach (var asset in group)
                        {
                            var assetBox = CreateSOEditorBox(asset, selectedType, groupFieldName, true, onGroupFieldChanged);
                            assetBox.style.width = 250;
                            assetBox.style.marginRight = 10;
                            dataContainer.Add(assetBox);
                        }
                    }
                }
                groupIndex++;
            }
        }

        public VisualElement CreateSOEditorBox(ScriptableObject so, Type selectedType, string groupFieldName, bool isGrouping, Action onGroupFieldChanged = null)
        {
            var box = new Box();
            StyleSOEditorBox(box);

            var nameLabel = new Label(so.name);
            StyleNameLabel(nameLabel);
            box.Add(nameLabel);

            var fields = selectedType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                bool isGroupingField = field.Name == groupFieldName && isGrouping;
                var fieldElement = CreateFieldElement(field, so, isGroupingField, onGroupFieldChanged);
                if (fieldElement != null)
                    box.Add(fieldElement);
            }

            return box;
        }

        public VisualElement CreateFieldElement(FieldInfo field, ScriptableObject so, bool isGroupingField, Action onGroupFieldChanged)
        {
            object value = field.GetValue(so);

            if (field.FieldType == typeof(int))
                return CreateIntField(field, so, value, isGroupingField, onGroupFieldChanged);
            else if (field.FieldType == typeof(float))
                return CreateFloatField(field, so, value);
            else if (field.FieldType == typeof(string))
                return CreateStringField(field, so, value, isGroupingField, onGroupFieldChanged);
            else if (field.FieldType.IsEnum)
                return CreateEnumField(field, so, value, isGroupingField, onGroupFieldChanged);
            else if (field.FieldType == typeof(Sprite))
                return CreateSpriteField(field, so, value);
            else if (typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType))
                return CreateObjectField(field, so, value);

            return null;
        }

        #region Private UI Creation Methods
        private void CreateGroupWithIcon(ScrollView container, IGrouping<string, ScriptableObject> group,
                                       Color groupColor, Type selectedType, string groupFieldName, Action onGroupFieldChanged)
        {
            var groupKey = group.Key;

            // 기존 그룹 컨테이너가 있는지 확인 (UQueryBuilder 문제 회피)
            VisualElement existingGroupContainer = null;
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.ElementAt(i);
                if (child.name == $"group_{groupKey}")
                {
                    existingGroupContainer = child;
                    break;
                }
            }

            if (existingGroupContainer != null)
            {
                // 기존 그룹이 있으면 데이터 부분만 업데이트
                var existingDataContainer = existingGroupContainer.ElementAt(1) as ScrollView;
                if (existingDataContainer != null)
                {
                    existingDataContainer.Clear();
                    foreach (var asset in group)
                    {
                        var assetBox = CreateSOEditorBox(asset, selectedType, groupFieldName, true, onGroupFieldChanged);
                        assetBox.style.width = 250;
                        assetBox.style.marginRight = 10;
                        existingDataContainer.Add(assetBox);
                    }
                }
                return;
            }

            // 새 그룹 컨테이너 생성
            var groupContainer = new VisualElement();
            groupContainer.name = $"group_{groupKey}"; // 식별을 위한 이름 설정
            groupContainer.style.flexDirection = FlexDirection.Row;
            groupContainer.style.marginBottom = 20;
            groupContainer.style.minHeight = 200;

            var iconContainer = CreateIconContainer(group, groupColor, selectedType, groupFieldName, onGroupFieldChanged);
            var dataContainer = CreateDataContainer(group, selectedType, groupFieldName, onGroupFieldChanged);

            groupContainer.Add(iconContainer);
            groupContainer.Add(dataContainer);
            container.Add(groupContainer);
        }

        private void CreateGroupWithoutIcon(ScrollView container, IGrouping<string, ScriptableObject> group,
                                          Color groupColor, Type selectedType, string groupFieldName, Action onGroupFieldChanged)
        {
            var groupHeader = new Label($"Group: {group.Key}");
            StyleGroupHeader(groupHeader, groupColor);
            container.Add(groupHeader);

            var horizontalContainer = new ScrollView(ScrollViewMode.Horizontal);
            horizontalContainer.style.flexDirection = FlexDirection.Row;
            horizontalContainer.style.marginBottom = 20;

            foreach (var asset in group)
            {
                var assetBox = CreateSOEditorBox(asset, selectedType, groupFieldName, true, onGroupFieldChanged);
                assetBox.style.width = 250;
                assetBox.style.marginRight = 10;
                horizontalContainer.Add(assetBox);
            }

            container.Add(horizontalContainer);
        }

        private VisualElement CreateIconContainer(IGrouping<string, ScriptableObject> group, Color groupColor,
                                                Type selectedType, string groupFieldName, Action onGroupFieldChanged)
        {
            var groupKey = group.Key;
            var iconContainer = new VisualElement();
            iconContainer.style.width = 200;
            iconContainer.style.paddingRight = 15;
            iconContainer.style.paddingTop = 10;

            var groupHeader = new Label($"Group: {group.Key}");
            StyleGroupHeader(groupHeader, groupColor);
            iconContainer.Add(groupHeader);

            // 아이콘 표시 영역 - 캐시된 것이 있으면 재사용
            VisualElement iconDisplay;
            if (groupIconDisplays.ContainsKey(groupKey))
            {
                iconDisplay = groupIconDisplays[groupKey];
            }
            else
            {
                iconDisplay = new VisualElement();
                iconDisplay.style.width = 128;
                iconDisplay.style.height = 128;
                iconDisplay.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
                iconDisplay.style.marginBottom = 10;
                groupIconDisplays[groupKey] = iconDisplay;
            }

            // 그룹의 첫 번째 스프라이트 가져오기
            Sprite groupSprite = null;
            if (groupManager != null)
            {
                groupSprite = groupManager.GetFirstSpriteFromGroup(group, selectedType);

                if (groupSprite != null)
                {
                    iconDisplay.style.backgroundImage = new StyleBackground(AssetPreview.GetAssetPreview(groupSprite));
                }
            }

            iconContainer.Add(iconDisplay);

            // 아이콘 변경 필드 - 캐시된 것이 있으면 재사용
            ObjectField iconField;
            if (groupIconFields.ContainsKey(groupKey))
            {
                iconField = groupIconFields[groupKey];
                iconField.value = groupSprite; // 값만 업데이트
            }
            else
            {
                iconField = new ObjectField("Group Icon")
                {
                    objectType = typeof(Sprite),
                    value = groupSprite,
                    allowSceneObjects = false
                };

                // 아이콘 변경 콜백 등록 (한 번만)
                iconField.RegisterValueChangedCallback(evt =>
                {
                    var newSprite = evt.newValue as Sprite;
                    Debug.Log($"Icon changed for group {groupKey}: {(newSprite != null ? newSprite.name : "null")}");

                    if (groupManager != null)
                    {
                        // 그룹 내 모든 오브젝트의 스프라이트 업데이트
                        groupManager.UpdateGroupSprites(group, newSprite, selectedType);

                        // 아이콘 표시 즉시 업데이트
                        var cachedIconDisplay = groupIconDisplays[groupKey];
                        if (newSprite != null)
                        {
                            cachedIconDisplay.style.backgroundImage = new StyleBackground(AssetPreview.GetAssetPreview(newSprite));
                        }
                        else
                        {
                            cachedIconDisplay.style.backgroundImage = null;
                        }

                        // 데이터 부분만 부분적으로 업데이트 (전체 UI 재생성 방지)
                        onGroupFieldChanged?.Invoke();
                    }
                });

                groupIconFields[groupKey] = iconField;
            }

            iconContainer.Add(iconField);
            return iconContainer;
        }

        private VisualElement CreateDataContainer(IGrouping<string, ScriptableObject> group, Type selectedType,
                                                string groupFieldName, Action onGroupFieldChanged)
        {
            var dataContainer = new ScrollView(ScrollViewMode.Horizontal);
            dataContainer.style.flexGrow = 1;
            dataContainer.style.flexDirection = FlexDirection.Row;

            foreach (var asset in group)
            {
                var assetBox = CreateSOEditorBox(asset, selectedType, groupFieldName, true, onGroupFieldChanged);
                assetBox.style.width = 250;
                assetBox.style.marginRight = 10;
                dataContainer.Add(assetBox);
            }

            return dataContainer;
        }

        private VisualElement CreateIntField(FieldInfo field, ScriptableObject so, object value, bool isGroupingField, Action onGroupFieldChanged)
        {
            int intValue = value != null ? (int)value : 0;
            var intField = new IntegerField(field.Name) { value = intValue };

            intField.RegisterValueChangedCallback(evt =>
            {
                field.SetValue(so, evt.newValue);
                EditorUtility.SetDirty(so);
                if (isGroupingField) onGroupFieldChanged?.Invoke();
            });
            return intField;
        }

        private VisualElement CreateFloatField(FieldInfo field, ScriptableObject so, object value)
        {
            float floatValue = value != null ? (float)value : 0f;
            var floatField = new FloatField(field.Name) { value = floatValue };

            floatField.RegisterValueChangedCallback(evt =>
            {
                field.SetValue(so, evt.newValue);
                EditorUtility.SetDirty(so);
            });
            return floatField;
        }

        private VisualElement CreateStringField(FieldInfo field, ScriptableObject so, object value, bool isGroupingField, Action onGroupFieldChanged)
        {
            string stringValue = value as string ?? "";
            var textField = new TextField(field.Name) { value = stringValue };

            textField.RegisterValueChangedCallback(evt =>
            {
                field.SetValue(so, evt.newValue);
                EditorUtility.SetDirty(so);
                if (isGroupingField) onGroupFieldChanged?.Invoke();
            });
            return textField;
        }

        private VisualElement CreateEnumField(FieldInfo field, ScriptableObject so, object value, bool isGroupingField, Action onGroupFieldChanged)
        {
            Enum enumValue = value as Enum ?? (Enum)Enum.GetValues(field.FieldType).GetValue(0);
            var enumField = new EnumField(field.Name, enumValue);

            enumField.RegisterValueChangedCallback(evt =>
            {
                field.SetValue(so, evt.newValue);
                EditorUtility.SetDirty(so);
                if (isGroupingField) onGroupFieldChanged?.Invoke();
            });
            return enumField;
        }

        private VisualElement CreateSpriteField(FieldInfo field, ScriptableObject so, object value)
        {
            var spriteField = new ObjectField(field.Name)
            {
                objectType = typeof(Sprite),
                value = value as Sprite,
                allowSceneObjects = false
            };

            spriteField.RegisterValueChangedCallback(evt =>
            {
                field.SetValue(so, evt.newValue);
                EditorUtility.SetDirty(so);
            });
            return spriteField;
        }

        private VisualElement CreateObjectField(FieldInfo field, ScriptableObject so, object value)
        {
            var objectField = new ObjectField(field.Name)
            {
                objectType = field.FieldType,
                value = value as UnityEngine.Object,
                allowSceneObjects = false
            };

            objectField.RegisterValueChangedCallback(evt =>
            {
                field.SetValue(so, evt.newValue);
                EditorUtility.SetDirty(so);
            });
            return objectField;
        }

        private void StyleSOEditorBox(Box box)
        {
            box.style.marginBottom = 8;
            box.style.paddingBottom = 4;
            box.style.paddingLeft = 8;
            box.style.paddingRight = 8;
            box.style.paddingTop = 8;
            box.style.borderBottomWidth = 1;
            box.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f);
            box.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
        }

        private void StyleNameLabel(Label nameLabel)
        {
            nameLabel.style.fontSize = 14;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            nameLabel.style.marginBottom = 8;
        }

        private void StyleGroupHeader(Label groupHeader, Color groupColor)
        {
            groupHeader.style.fontSize = 18;
            groupHeader.style.color = groupColor;
            groupHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            groupHeader.style.marginBottom = 10;
            groupHeader.style.marginTop = 10;
        }
        #endregion
    }
}