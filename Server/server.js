const WebSocket = require('ws');
const readline = require('readline');

// Démarrer un serveur WebSocket sur le port 8080
const wss = new WebSocket.Server({ port: 8080 });

wss.on('connection', (ws) => {
    console.log('Client connecté');
    console.log('Nombre de clients connectés :', wss.clients.size);

    // Envoyer un message au client dès la connexion
    ws.send('Hello Client!');

    // Réception des messages depuis le client
    ws.on('message', (message) => {
        if(message === ''){
            console.log('Client a envoyé un message vide');
            return;
        }
        console.log(`Message reçu du client : ${message}`);
        broadcastMessage(message.toString()); // Envoyer le message à tous les clients
    });

    // Gestion de la déconnexion
    ws.on('close', () => {
        console.log('Client déconnecté');
    });
});

// Lire les entrées utilisateur depuis le terminal
const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

console.log('Serveur WebSocket en écoute sur ws://localhost:8080');

// Fonction pour envoyer un message à tous les clients connectés
const broadcastMessage = (message) => {
    wss.clients.forEach((client) => {
        if (client.readyState === WebSocket.OPEN) {
            console.log(message);
            client.send(message);
        }
    });
};

// Écouter les entrées utilisateur
rl.on('line', (input) => {
    console.log(`Vous avez tapé : ${input}`);
    broadcastMessage(input); // Envoyer le message à tous les clients
});
