using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using System.Collections.Generic;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws; // Instance WebSocket
    public TMP_InputField messageInput; // Référence à la boîte de texte pour entrer le message
    public Button sendButton; // Bouton pour envoyer un message
    public TMP_Text receivedMessagesText; // Zone pour afficher les messages reçus
    private string receivedMessages = ""; // Stocker les messages reçus

    // File d'attente pour les messages reçus
    private Queue<string> messageQueue = new Queue<string>();

    void Start()
    {
        // Initialiser le WebSocket avec l'adresse de votre serveur
        ws = new WebSocket("ws://172.20.10.2:8080"); // Remplacez par votre IP serveur

        // Événements pour gérer les messages reçus et la connexion
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message reçu du serveur : " + e.Data);

            // Ajouter le message à la file d'attente
            lock (messageQueue)
            {
                messageQueue.Enqueue(e.Data);
            }
        };

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("Connecté au serveur WebSocket.");
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("Déconnecté du serveur.");
        };

        // Connecter au serveur
        ws.Connect();

        // Ajouter un listener au bouton d'envoi
        sendButton.onClick.AddListener(SendMessageToServer);
    }

    void Update()
    {
        // Traiter les messages reçus depuis la file d'attente
        lock (messageQueue)
        {
            while (messageQueue.Count > 0)
            {
                string message = messageQueue.Dequeue();
                AddMessageToUI(message);
            }
        }
    }

    void SendMessageToServer()
    {
        // Récupérer le message de la boîte de texte
        string message = messageInput.text;

        if (!string.IsNullOrEmpty(message))
        {
            // Envoyer le message au serveur
            ws.Send(message);
            Debug.Log("Message envoyé au serveur : " + message);

            // Effacer la boîte de texte après l'envoi
            messageInput.text = "";
        }
        else
        {
            Debug.Log("La boîte de texte est vide.");
        }
    }

    void AddMessageToUI(string message)
    {
        // Ajoute le message reçu à la chaîne
        receivedMessages += message + "\n";
        Debug.Log("Messages reçus : " + receivedMessages);

        // Met à jour le texte dans l'UI
        if (receivedMessagesText != null)
        {
            receivedMessagesText.text = receivedMessages;
        }
    }

    void OnDestroy()
    {
        // Fermer la connexion lorsque l'objet est détruit
        if (ws != null)
        {
            ws.Close();
        }
    }
}
