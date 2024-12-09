using System.Collections.Generic;
using Environment;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;  // Pour accéder à SceneManager
using Utils;
using WebSocketSharp;

namespace Client
{
    public class WebSocketClient : MonoBehaviourSingleton<WebSocketClient>
    {
        [SerializeField] private UnityEvent<string> onMessageReceived;
        
        private ServerConfig _serverConfig;
        private WebSocket _ws;
        private readonly Queue<string> _messageQueue = new();
        private string _clientTag;
        
        protected override void Awake()
        {
            base.Awake();
            _serverConfig = ConfigLoader.LoadConfig();
            Debug.Log($"Configuration loaded: {JsonUtility.ToJson(_serverConfig)}");

            // Déterminer le tag en fonction de la scène
            string sceneName = SceneManager.GetActiveScene().name;
            _clientTag = GetClientTagFromScene(sceneName);
            Debug.Log($"Client tag determined from scene '{sceneName}': {_clientTag}");
        }

        private string GetClientTagFromScene(string sceneName)
        {
            switch (sceneName)
            {
                case "TopViewScene":
                    return "TopView";
                case "VerticalScene":
                    return "VerticalView";
                default:
                    Debug.LogWarning($"Unknown scene name: {sceneName}. Using default tag.");
                    return "UnknownView";
            }
        }

        private void Start()
        {
            _ws = new WebSocket($"ws://{_serverConfig.serverIp}:{_serverConfig.serverPort}?clientType={_clientTag}");
            
            _ws.OnMessage += (sender, e) =>
            {
                lock (_messageQueue)
                {
                    _messageQueue.Enqueue(e.Data);
                }
            };

            _ws.OnOpen += (sender, e) =>
            {
                Debug.Log($"Connecté au serveur WebSocket en tant que {_clientTag}");
            };

            _ws.OnClose += (sender, e) =>
            {
                Debug.Log("Déconnecté du serveur.");
            };
        
            _ws.Connect();
        }

        private void Update()
        {
            lock (_messageQueue)
            {
                while (_messageQueue.Count > 0)
                {
                    var message = _messageQueue.Dequeue();
                    Debug.Log("Message reçu du serveur : " + message);
                    onMessageReceived.Invoke(string.Copy(message));
                }
            }
        }

        public void SendMessageToServer(string message)
        {
            if(_ws.IsAlive == false)
            {
                return;
            }
            if (!string.IsNullOrEmpty(message))
            {
                _ws.Send(message);
                Debug.Log("Message envoyé au serveur : " + message);
            }
            else
            {
                Debug.Log("La boîte de texte est vide.");
            }
        }
        
        private void OnDestroy()
        {
            if (_ws != null)
            {
                _ws.Close();
            }
        }
    }
}