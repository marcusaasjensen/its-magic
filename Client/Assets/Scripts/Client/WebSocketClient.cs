using System.Collections.Generic;
using UnityEngine;
using Utils;
using WebSocketSharp;

namespace Client
{
    public class WebSocketClient : MonoBehaviourSingleton<WebSocketClient>
    {
        //[SerializeField] private string serverAddress = "ws://<ipconfig>:8080";
    
        private ServerConfig _serverConfig;
        private WebSocket _ws;
        //public TMP_InputField messageInput;
        //public Button sendButton;
        //public TMP_Text receivedMessagesText;
        private string receivedMessages = "";

        // File d'attente pour les messages reçus
        public readonly Queue<string> messageQueue = new();
        
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

                lock (messageQueue)
                {
                    messageQueue.Enqueue(e.Data);
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

            //sendButton.onClick.AddListener(() => SendMessageToServer(messageInput.text));
        }

        private void Update()
        {
            lock (messageQueue)
            {
                while (messageQueue.Count > 0)
                {
                    string message = messageQueue.Dequeue();
                    Debug.Log("Message reçu du serveur : " + message);
                    //AddMessageToUI(message);
                }
            }
        }

        public void SendMessageToServer(string message)
        {
            // string message = messageInput.text;

            if (!string.IsNullOrEmpty(message))
            {
                _ws.Send(message);
                Debug.Log("Message envoyé au serveur : " + message);

                //messageInput.text = "";
            }
            else
            {
                Debug.Log("La boîte de texte est vide.");
            }
        }

        /*void AddMessageToUI(string message)
    {
        // Ajoute le message reçu à la chaîne
        receivedMessages += message + "\n";
        Debug.Log("Messages reçus : " + receivedMessages);

        // Met à jour le texte dans l'UI
        if (receivedMessagesText != null)
        {
            receivedMessagesText.text = receivedMessages;
        }
    }*/

        private void OnDestroy()
        {
            if (_ws != null)
            {
                _ws.Close();
            }
        }
    }
}
