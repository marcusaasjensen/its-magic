﻿using System;
using Client;
using Managers;
using UnityEngine;
using UnityEngine.Rendering;

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
            SceneController.Instance.TransitionToScene(sceneName);
            //UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
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

            //UnityEngine.SceneManagement.SceneManager.LoadScene(sceneMessage.sceneName);
            SceneController.Instance.TransitionToScene(sceneMessage.sceneName);
        }

        public void StartGame(string message)
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
            var sceneMessage = new SceneMessage()
            {
                clientId = "TopView",
                recipientId = "Android",
                sceneName = "forest",
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(sceneMessage));
        }
    }
}