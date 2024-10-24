using UnityEngine;

namespace Environment
{
    public class LevelManager : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}