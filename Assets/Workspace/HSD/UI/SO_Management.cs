using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace Editor
{
    public class SO_Management : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        private List<TankData> tankDataList;
        private ScrollView scrollView;

        [MenuItem("TankEditor/TankData")]
        public static void ShowEditor()
        {
            var window = GetWindow<SO_Management>();
            window.titleContent = new GUIContent("TankData_Editor");
        }

        public void CreateGUI()
        {
            scrollView = new ScrollView();
            rootVisualElement.Add(scrollView);

            LoadTankData();
            CreateTankEditors();
        }

        void LoadTankData()
        {
            tankDataList = new List<TankData>();
            string[] guids = AssetDatabase.FindAssets("t:TankData");

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TankData data = AssetDatabase.LoadAssetAtPath<TankData>(path);
                tankDataList.Add(data);
            }
        }

        void CreateTankEditors()
        {
            var groupedData = new Dictionary<string, List<TankData>>();

            foreach (var tank in tankDataList)
            {
                if (!groupedData.ContainsKey(tank.tankName))
                    groupedData[tank.tankName] = new List<TankData>();

                groupedData[tank.tankName].Add(tank);
            }

            foreach (var kvp in groupedData)
            {
                string tankName = kvp.Key;
                List<TankData> tanks = kvp.Value.OrderBy(t => t.level).ToList();
                TankData mainTank = tanks[0];

                var groupBox = new Box();
                groupBox.style.flexDirection = FlexDirection.Row;   // Box의 정렬 방향 설정
                groupBox.style.marginBottom = 16;
                groupBox.style.paddingBottom = 8;
                groupBox.style.height = 200;

                // 왼쪽 아이콘 미리보기
                var iconImage = new Image
                {
                    style =
                    {
                        width = 128,
                        height = 128,
                        marginRight = 16
                    },
                    image = mainTank.icon ? mainTank.icon.texture : null
                };

                // 왼쪽 아이콘 ObjectField (InspectorField)
                var iconField = new ObjectField
                {
                    objectType = typeof(Sprite),
                    value = mainTank.icon,
                    allowSceneObjects = false
                };

                // 값 변환 이벤트 구독
                iconField.RegisterValueChangedCallback(evt =>
                {
                    Sprite newIcon = (Sprite)evt.newValue;

                    foreach (var tank in tanks) // 해당 그룹에만 적용
                    {
                        tank.icon = newIcon;
                        EditorUtility.SetDirty(tank);
                    }

                    iconImage.image = newIcon != null ? newIcon.texture : null;
                });


                var iconContainer = new VisualElement();
                iconContainer.style.flexDirection = FlexDirection.Column; // 방향
                iconContainer.Add(iconImage);
                iconContainer.Add(iconField);

                groupBox.Add(iconContainer);

                // 오른쪽 패널
                var rightPanel = new VisualElement();
                rightPanel.style.flexDirection = FlexDirection.Row; // 방향
                rightPanel.style.flexGrow = 1;
                rightPanel.Add(new Label($"TANK NAME: {tankName}"));

                foreach (var tank in tanks)
                {
                    var tankBox = new Box();
                    tankBox.style.marginBottom = 8;
                    tankBox.style.paddingBottom = 4;
                    tankBox.style.borderBottomWidth = 1;
                    tankBox.style.flexShrink = 0;
                    tankBox.style.width = 320;
                    tankBox.style.height = 320;
                    tankBox.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f);

                    var titleLabel = new Label($"{tank.tankName} - Level {tank.level}");
                    titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                    titleLabel.style.color = Color.cyan;
                    titleLabel.style.marginBottom = 4;
                    tankBox.Add(titleLabel);

                    // 각 필드 생성 및 필드의 ChangeEvent 등록
                    var damageField = new FloatField("Damage") { value = tank.damage };
                    damageField.RegisterValueChangedCallback(evt => { tank.damage = evt.newValue; EditorUtility.SetDirty(tank); });
                    tankBox.Add(damageField);

                    var maxMoveField = new FloatField("Max Move") { value = tank.maxMove };
                    maxMoveField.RegisterValueChangedCallback(evt => { tank.maxMove = evt.newValue; EditorUtility.SetDirty(tank); });
                    tankBox.Add(maxMoveField);

                    var speedField = new FloatField("Speed") { value = tank.speed };
                    speedField.RegisterValueChangedCallback(evt => { tank.speed = evt.newValue; EditorUtility.SetDirty(tank); });
                    tankBox.Add(speedField);

                    var hpField = new FloatField("Max HP") { value = tank.maxHp };
                    hpField.RegisterValueChangedCallback(evt => { tank.maxHp = evt.newValue; EditorUtility.SetDirty(tank); });
                    tankBox.Add(hpField);

                    var rankField = new EnumField("Rank", tank.rank);
                    rankField.RegisterValueChangedCallback(evt => { tank.rank = (TankRank)evt.newValue; EditorUtility.SetDirty(tank); });
                    tankBox.Add(rankField);

                    var tankIconField = new ObjectField("Icon")
                    {
                        objectType = typeof(Sprite),
                        value = tank.icon,
                        allowSceneObjects = false
                    };
                    tankIconField.RegisterValueChangedCallback(evt =>
                    {
                        tank.icon = (Sprite)evt.newValue;
                        EditorUtility.SetDirty(tank);
                    });
                    tankBox.Add(tankIconField);

                    var levelField = new IntegerField("Level") { value = tank.level };
                    levelField.RegisterValueChangedCallback(evt =>
                    {
                        tank.level = evt.newValue;
                        EditorUtility.SetDirty(tank);
                    });
                    tankBox.Add(levelField);

                    var countField = new IntegerField("Count") { value = tank.count };
                    countField.RegisterValueChangedCallback(evt =>
                    {
                        tank.count = evt.newValue;
                        EditorUtility.SetDirty(tank);
                    });
                    tankBox.Add(countField);

                    rightPanel.Add(tankBox);
                }

                groupBox.Add(rightPanel);
                scrollView.Add(groupBox);
            }
        }
    }
}