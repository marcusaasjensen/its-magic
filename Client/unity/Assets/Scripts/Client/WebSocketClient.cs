using System.Collections.Generic;
using Environment;
using UnityEngine;
using UnityEngine.Events;
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
        
        protected override void Awake()
        {
            base.Awake();
            _serverConfig = ConfigLoader.LoadConfig();
            Debug.Log($"Configuration loaded: {JsonUtility.ToJson(_serverConfig)}");
        }

        private void Start()
        {
            _ws = new WebSocket($"ws://{_serverConfig.serverIp}:{_serverConfig.serverPort}");
            
            _ws.OnMessage += (sender, e) =>
            {
                // Debug.Log("Message reçu du serveur : " + e.Data);

                lock (_messageQueue)
                {
                    _messageQueue.Enqueue(e.Data);
                }
            };

            _ws.OnOpen += (sender, e) =>
            {
                Debug.Log("Connecté au serveur WebSocket.");
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
