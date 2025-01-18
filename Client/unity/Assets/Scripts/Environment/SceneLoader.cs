using System;
using Client;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Environment
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private UnityEvent onSceneTransition;
        public void LoadRemoteScene(string remoteSceneName)
        {
            if (string.IsNullOrEmpty(remoteSceneName)) return;
            var sceneMessage = new SceneMessage { sceneName = remoteSceneName };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(sceneMessage));
        }

        public void LoadScene(string sceneName)
        {
            if (sceneName == "HouseSideScene")
            {
                var sceneMessageAndroid = new SceneMessage
                {
                    clientId = "TopView",
                    recipientId = "Android",
                    sceneName = "workshop",
                };
                WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(sceneMessageAndroid));
            }

            if (sceneName == "ForestSideScene")
            {
                var sceneMessageAndroid = new SceneMessage
                {
                    clientId = "TopView",
                    recipientId = "Android",
                    sceneName = "forest",
                };
                WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(sceneMessageAndroid));
            }
            
            SceneController.Instance.TransitionToScene(sceneName);
        }

        public void LoadSceneFromMessage(string message)
        {
            if (message == null)
            {
                return;
            }

            var sceneMessage = JsonUtility.FromJson<SceneMessage>(message);
            if (sceneMessage is not { type: "Scene" })
            {
                return;
            }
            
            onSceneTransition.Invoke();
            SceneController.Instance.TransitionToScene(sceneMessage.sceneName);
        }
    }
}