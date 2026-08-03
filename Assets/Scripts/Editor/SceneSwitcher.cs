#region

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

#endregion

namespace Editor
{
    public static class SceneSwitcher
    {
        private const string InitialScene = "Assets/Scenes/Initial.unity";
        private const string SplashScene = "Assets/Scenes/SplashScreen.unity";
        private const string GameScene = "Assets/Scenes/Game.unity";

        [MainToolbarElement("SceneSwitcher/Initial", defaultDockPosition = MainToolbarDockPosition.Right, defaultDockIndex = 0)]
        private static MainToolbarElement CreateInitialButton() =>
            CreateSceneButton("Initial", "Load Initial Scene", InitialScene);

        [MainToolbarElement("SceneSwitcher/Splash", defaultDockPosition = MainToolbarDockPosition.Right, defaultDockIndex = 1)]
        private static MainToolbarElement CreateSplashButton() =>
            CreateSceneButton("Splash", "Load Splash Screen Scene", SplashScene);

        [MainToolbarElement("SceneSwitcher/Game", defaultDockPosition = MainToolbarDockPosition.Right, defaultDockIndex = 2)]
        private static MainToolbarElement CreateGameButton() =>
            CreateSceneButton("GameScene", "Load Game Scene", GameScene);

        private static MainToolbarElement CreateSceneButton(string label, string tooltip, string scenePath) =>
            new MainToolbarButton(new MainToolbarContent(label, tooltip), () => OpenScene(scenePath));

        private static void OpenScene(string scenePath)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            // Hold Shift while clicking to load the scene additively.
            var additive = (Event.current?.modifiers & EventModifiers.Shift) != 0;
            EditorSceneManager.OpenScene(scenePath, additive ? OpenSceneMode.Additive : OpenSceneMode.Single);
        }
    }
}
