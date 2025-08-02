using SOEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace SOEditor
{
    /// <summary>메인 에디터 윈도우 - 의존성 주입으로 구성된 컨트롤러</summary>
    public class SO_Management : EditorWindow
    {
        #region Dependencies
        private ISODataProvider dataProvider;
        private ISOUIRenderer uiRenderer;
        private ISOGroupManager groupManager;
        private ISOSpriteManager spriteManager;
        #endregion

        #region UI Elements
        private ScrollView scrollView;
        private List<Type> soTypes;
        private DropdownField soDropdown;
        private Type selectedType;
        private Toggle groupToggle;
        private Toggle viewIconToggle;
        private DropdownField groupByFieldDropdown;
        private string selectedGroupFieldName;

        // 이전 상태 추적용
        private Type previousSelectedType;
        private bool isTypeChanged = false;
        #endregion

        #region Data
        private List<ScriptableObject> loadedAssets = new List<ScriptableObject>();
        #endregion

        [MenuItem("Scriptable Editor/Scriptable Management")]
        public static void ShowEditor()
        {
            var window = GetWindow<SO_Management>();
            window.titleContent = new GUIContent("Scriptable Management");
        }

        private void InitializeDependencies()
        {
            spriteManager = new SOSpriteManager();
            dataProvider = new SODataProvider();
            groupManager = new SOGroupManager(spriteManager);
            uiRenderer = new SOUIRenderer();

            ((SOUIRenderer)uiRenderer).SetManagers(groupManager);
        }

        public void CreateGUI()
        {
            InitializeDependencies();

            var mainContainer = new VisualElement();
            mainContainer.style.flexGrow = 1;
            mainContainer.style.paddingTop = 10;
            mainContainer.style.paddingLeft = 10;
            mainContainer.style.paddingRight = 10;
            mainContainer.style.paddingBottom = 10;

            var controlsArea = new VisualElement();
            controlsArea.style.marginBottom = 15;

            SetupGroupOptionsUI(controlsArea);
            PopulateSOTypeDropdown(controlsArea);

            scrollView = new ScrollView();
            scrollView.style.flexDirection = FlexDirection.Column;
            scrollView.style.flexGrow = 1;

            mainContainer.Add(controlsArea);
            mainContainer.Add(scrollView);
            rootVisualElement.Add(mainContainer);
        }

        void SetupGroupOptionsUI(VisualElement parent)
        {
            var controlsContainer = new VisualElement();
            controlsContainer.style.flexDirection = FlexDirection.Row;
            controlsContainer.style.paddingBottom = 10;

            groupToggle = new Toggle("Enable Grouping");
            groupToggle.RegisterValueChangedCallback(evt =>
            {
                viewIconToggle.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                if (selectedType != null && scrollView != null)
                {
                    isTypeChanged = true; // 그룹 모드 변경 시 완전 재생성
                    LoadAndRenderSOInstances();
                }
            });

            viewIconToggle = new Toggle("View Icons");
            viewIconToggle.style.display = DisplayStyle.None;
            viewIconToggle.RegisterValueChangedCallback(evt =>
            {
                if (selectedType != null && groupToggle.value && scrollView != null)
                {
                    isTypeChanged = true; // 뷰 모드 변경 시 완전 재생성
                    LoadAndRenderSOInstances();
                }
            });

            groupByFieldDropdown = new DropdownField("Group By Field", new List<string>(), 0);
            groupByFieldDropdown.RegisterValueChangedCallback(evt =>
            {
                selectedGroupFieldName = evt.newValue;
                if (selectedType != null && groupToggle.value && scrollView != null)
                {
                    isTypeChanged = true; // 그룹 기준 변경 시 완전 재생성
                    LoadAndRenderSOInstances();
                }
            });

            controlsContainer.Add(groupToggle);
            controlsContainer.Add(viewIconToggle);
            parent.Add(controlsContainer);
            parent.Add(groupByFieldDropdown);
        }

        void PopulateSOTypeDropdown(VisualElement parent)
        {
            soTypes = dataProvider.GetAvailableSOTypes();
            var typeNames = soTypes.Select(t => t.Name).ToList();
            soDropdown = new DropdownField("ScriptableObject Type", typeNames, 0);

            parent.Add(soDropdown);

            if (soTypes.Count > 0)
            {
                selectedType = soTypes[0];
                previousSelectedType = selectedType;
                soDropdown.value = soTypes[0].Name;

                soDropdown.RegisterValueChangedCallback(evt =>
                {
                    var newSelectedType = soTypes[typeNames.IndexOf(evt.newValue)];
                    isTypeChanged = (newSelectedType != selectedType);
                    selectedType = newSelectedType;
                    PopulateGroupByFields();
                    LoadAndRenderSOInstances();
                });

                PopulateGroupByFields();
                rootVisualElement.schedule.Execute(() =>
                {
                    isTypeChanged = true; // 초기 로드 시에는 완전 재생성
                    LoadAndRenderSOInstances();
                });
            }
            else
            {
                parent.Add(new Label("Resources 폴더에 존재하는 ScriptableObject 타입이 없습니다."));
            }
        }

        void PopulateGroupByFields()
        {
            if (selectedType == null || groupByFieldDropdown == null)
                return;

            var fields = dataProvider.GetGroupableFields(selectedType);
            var fieldNames = fields.Select(f => f.Name).ToList();
            groupByFieldDropdown.choices = fieldNames;

            if (fieldNames.Count > 0)
            {
                groupByFieldDropdown.value = fieldNames[0];
                selectedGroupFieldName = fieldNames[0];
            }
            else
            {
                groupByFieldDropdown.value = null;
                selectedGroupFieldName = null;
            }
        }

        void LoadAndRenderSOInstances()
        {
            if (scrollView == null) return;

            // 타입이 변경되었거나 UI 모드가 변경된 경우에만 완전히 Clear
            if (isTypeChanged)
            {
                scrollView.Clear();
                loadedAssets.Clear();
                // UIRenderer의 캐시도 초기화
                if (uiRenderer is SOUIRenderer renderer)
                {
                    // 리플렉션으로 private 메서드 호출하거나, public 메서드로 만들어야 함
                    var clearCacheMethod = typeof(SOUIRenderer).GetMethod("ClearUICache",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    clearCacheMethod?.Invoke(renderer, null);
                }
                isTypeChanged = false;
            }

            if (selectedType == null) return;

            loadedAssets = dataProvider.LoadAssetsOfType(selectedType);
            Debug.Log(loadedAssets.Count);

            if (loadedAssets == null || loadedAssets.Count == 0)
            {
                scrollView.Add(new Label("Resources 폴더 내에 해당 타입의 에셋이 없습니다."));
                return;
            }

            if (groupToggle.value && !string.IsNullOrEmpty(selectedGroupFieldName))
            {
                var grouped = groupManager.GroupAssets(loadedAssets, selectedGroupFieldName, selectedType);
                uiRenderer.RenderGroupedUI(scrollView, grouped, selectedType, viewIconToggle.value,
                                         new Color[] {
                                             new Color(0.8f, 0.4f, 0.4f), new Color(0.4f, 0.8f, 0.4f),
                                             new Color(0.4f, 0.4f, 0.8f), new Color(0.8f, 0.8f, 0.4f)
                                         }, selectedGroupFieldName, LoadAndRenderSOInstances);
            }
            else
            {
                uiRenderer.RenderFlatUI(scrollView, loadedAssets, selectedType);
            }

            previousSelectedType = selectedType;
        }

        [MenuItem("Scriptable Editor/Refresh Scriptable Cache")]
        public static void RefreshCache()
        {
            var window = GetWindow<SO_Management>();
            window.dataProvider?.RefreshCache();
            if (window.selectedType != null)
            {
                window.isTypeChanged = true; // 캐시 리프레시 시 완전 재생성
                window.LoadAndRenderSOInstances();
            }
        }
    }
}