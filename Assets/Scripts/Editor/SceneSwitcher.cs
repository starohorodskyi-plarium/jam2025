#region

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#endregion

namespace Editor
{
    [InitializeOnLoad]
    public class SceneSwitchLeftButton
    {
        static SceneSwitchLeftButton()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }


        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent(" Initial ", "Load Initial Scene"), EditorStyles.miniButtonLeft))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(
                    "Assets/Scenes/Initial.unity",
                    (Event.current.modifiers & EventModifiers.Shift) != 0 ? OpenSceneMode.Additive : OpenSceneMode.Single);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.Space(4);

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent(" Splash ", "Load Splash Screen Scene"), EditorStyles.miniButtonMid))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(
                    "Assets/Scenes/SplashScreen.unity",
                    (Event.current.modifiers & EventModifiers.Shift) != 0 ? OpenSceneMode.Additive : OpenSceneMode.Single);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.Space(4);

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent(" GameScene ", "Load Game Scene"), EditorStyles.miniButtonRight))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(
                    "Assets/Scenes/Game.unity",
                    (Event.current.modifiers & EventModifiers.Shift) != 0 ? OpenSceneMode.Additive : OpenSceneMode.Single);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
    }
}
