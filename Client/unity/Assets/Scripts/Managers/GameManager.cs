using Client;
using Data;
using UnityEngine;
using Utils;

namespace Managers
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private GlobalData globalData;
        [SerializeField] private bool resetDataOnStart;

        protected override void Awake()
        {
            base.Awake();
            
            if (resetDataOnStart)
            {
                globalData.ResetData();
            }
            
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        
        public void StartTopViewGame(string message)
        {
            if (message == null)
            {
                return;
            }

            var startGameMessage = JsonUtility.FromJson<SceneMessage>(message);
            if (startGameMessage is not { type: "StartGame" })
            {
                return;
            }

            SceneController.Instance.TransitionToScene(startGameMessage.sceneName);
            var sceneMessage = new SceneMessage
            {
                clientId = "TopView",
                recipientId = "Android",
                sceneName = "workshop",
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(sceneMessage));
        }
        
        public void StartSideViewGame(string message)
        {
            if (message == null)
            {
                return;
            }

            var startGameMessage = JsonUtility.FromJson<SceneMessage>(message);
            if (startGameMessage is not { type: "StartGame" })
            {
                return;
            }

            SceneController.Instance.TransitionToScene(startGameMessage.sceneName);
        }
    }
}