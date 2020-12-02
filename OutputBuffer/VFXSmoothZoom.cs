using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.VFX.UI; 

public class SmoothZoom : EditorWindow
{
    public bool zoomEnabled = true;
    public float minScale = 0.125f;
    public float maxScale = 4.0f;
    public float scaleStep = 0.07f; 
    public float referenceScale = 1.0f;

    [MenuItem ("Window/SmoothZoom")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SmoothZoom));
    }

    void OnGUI()
    {
        //Debug.Log("SmoothZoom OnGUI run");
        
        GUILayout.Label("VFX Graph View Settings", EditorStyles.boldLabel);
        
        zoomEnabled = EditorGUILayout.BeginToggleGroup("Zoom Override", zoomEnabled);
            minScale = EditorGUILayout.Slider("Minimum Scale", minScale, 0, 1);
            maxScale = EditorGUILayout.Slider("Maximum Scale", maxScale, 1, 16);
            scaleStep = EditorGUILayout.Slider("Scale Step", scaleStep, 0, 1);
            referenceScale = EditorGUILayout.Slider("Reference Scale", referenceScale, 0, 4);
        EditorGUILayout.EndToggleGroup ();

        VFXViewWindow[] allWindows = Resources.FindObjectsOfTypeAll<VFXViewWindow>();
        foreach (VFXViewWindow thisWindow in allWindows)
        {
            //Debug.Log(thisWindow);
            VisualElement root = thisWindow.GetRootVisualElement();
            VFXView view = root.Q<VFXView>();

            if (view is null)
            {
                //Debug.Log("No VFX Graph views to override");
            } 
            else
            {
                if (zoomEnabled)
                {
                    view.SetupZoom(minScale, maxScale, scaleStep, referenceScale);
                } 
                else
                {
                    view.SetupZoom(0.125f, 4.0f, 0.07f, 1.0f);
                }
                //Debug.Log(System.String.Format("{0}, {1}, {2}, {3}", view.minScale, view.maxScale, view.scaleStep, view.referenceScale));
            }
        }
    }
}