using System;
using Client;
using UnityEngine;

namespace Environment
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadRemoteScene(string remoteSceneName)
        {
            if (string.IsNullOrEmpty(remoteSceneName)) return;
            var sceneMessage = new SceneMessage { sceneName = remoteSceneName };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(sceneMessage));
        }
        
        public void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneFromMessage(string message)
        {
            if(message == null)
            {
                return;
            }
            
            var sceneMessage = JsonUtility.FromJson<SceneMessage>(message);
            if(sceneMessage is not { type: "Scene" })
            {
                return;
            }
            
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneMessage.sceneName);
        }
    }
}