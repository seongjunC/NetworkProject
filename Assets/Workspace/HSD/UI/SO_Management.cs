using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SO_Management : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/SO_Management")]
    public static void ShowExample()
    {
        SO_Management wnd = GetWindow<SO_Management>();
        wnd.titleContent = new GUIContent("SO_Management");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
    }
}
