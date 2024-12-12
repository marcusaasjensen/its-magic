const WebSocket = require('ws');
const readline = require('readline');

// Démarrer un serveur WebSocket sur le port 8080
const wss = new WebSocket.Server({ port: 8080 });

// Un objet pour stocker les clients connectés avec leur tag
const clients = new Map();

wss.on('connection', (ws, req) => {
    console.log('Nouveau client connecté');
    
    // Analyser les paramètres de l'URL pour obtenir le clientType
    const urlParams = new URLSearchParams(req.url.slice(1));
    const clientType = urlParams.get('clientType');
    
    if (clientType) {
        clients.set(clientType, ws);
        console.log(`Client enregistré avec l'ID : ${clientType}`);
    }

    // Réception des messages depuis le client
    ws.on('message', (message) => {
        try {
            const parsedMessage = JSON.parse(message);
            console.log('Message reçu :', parsedMessage);

            // Vérifier que le message a la bonne structure
            if (!parsedMessage.clientId || !parsedMessage.type) {
                console.log('Structure de message invalide');
                return;
            }

            // Si le message a un destinataire spécifique
            if (parsedMessage.recipientId) {
                const recipientWs = clients.get(parsedMessage.recipientId);
                if (recipientWs && recipientWs.readyState === WebSocket.OPEN) {
                    recipientWs.send(message.toString());  // Renvoie le message tel quel
                    console.log(`Message transmis de ${parsedMessage.clientId} vers ${parsedMessage.recipientId}`);
                } else {
                    console.log(`Client destinataire ${parsedMessage.recipientId} non trouvé ou déconnecté`);
                }
            } 
            // Sinon, broadcast à tous les clients sauf l'émetteur
            else {
                clients.forEach((client, id) => {
                    if (client !== ws && client.readyState === WebSocket.OPEN) {
                        client.send(message.toString());
                        console.log(`Message broadcast de ${parsedMessage.clientId} vers ${id}`);
                    }
                });
            }

        } catch (error) {
            console.log('Erreur de parsing du message :', error);
        }
    });

    // Gestion de la déconnexion
    ws.on('close', () => {
        for (const [clientId, clientWs] of clients.entries()) {
            if (clientWs === ws) {
                clients.delete(clientId);
                console.log(`Client déconnecté avec l'ID : ${clientId}`);
                break;
            }
        }
    });
});

// Configuration de l'interface de ligne de commande
const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

console.log('Serveur WebSocket en écoute sur ws://localhost:8080');

// Écouter les entrées utilisateur pour le broadcast manuel depuis le serveur
rl.on('line', (input) => {
    const serverMessage = {
        clientId: 'server',
        type: 'ServerMessage',
        message: input
    };
    // Broadcast à tous les clients
    clients.forEach((client) => {
        if (client.readyState === WebSocket.OPEN) {
            client.send(JSON.stringify(serverMessage));
        }
    });
});