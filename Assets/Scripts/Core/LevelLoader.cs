using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
   public class LevelLoader : MonoBehaviour
   {
      private bool _isLoading;
      private string _loadingScene = string.Empty;
      public void LoadLevel(string levelSceneName)
      {
         if (_isLoading) return;

         StartCoroutine(LoadLevelRoutine(levelSceneName));
      }

      public void Reload()
      {
         var scene = SceneManager.GetActiveScene();
         SceneManager.LoadScene(scene.name);
      }

      private IEnumerator LoadLevelRoutine(string levelSceneName)
      {
         _isLoading = true;
         _loadingScene = levelSceneName;
         
         var asyncLoad = SceneManager.LoadSceneAsync(levelSceneName);
      
         while (asyncLoad is { isDone: false })
            yield return null;

         EndLoading();
      }

      private void OnDestroy() => EndLoading();

      private void EndLoading()
      {
         MusicManager.SceneLoaded?.Invoke(_loadingScene);

         _isLoading = false;
      }
   }
}
