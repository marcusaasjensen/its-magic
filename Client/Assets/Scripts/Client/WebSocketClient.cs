using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;


public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws; // Instance WebSocket
    public TMP_InputField messageInput; // Référence à la boîte de texte pour entrer le message
    public Button sendButton; // Bouton pour envoyer un message

    void Start()
    {
        // Initialiser le WebSocket avec l'adresse de votre serveur
        ws = new WebSocket("ws://192.168.1.28:8080"); //mettre bien votre ip

        // Événements pour gérer les messages reçus et la connexion
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message reçu du serveur : " + e.Data);
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

    void OnDestroy()
    {
        // Fermer la connexion lorsque l'objet est détruit
        if (ws != null)
        {
            ws.Close();
        }
    }
}
