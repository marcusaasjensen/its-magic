const WebSocket = require('ws');

// Connexion au serveur WebSocket
const ws = new WebSocket('ws://localhost:8080');

// Quand la connexion est établie
ws.on('open', () => {
    console.log('Connecté au serveur');

    // Envoyer un message au serveur
    ws.send('Hello Server!');
});

// Quand un message est reçu du serveur
ws.on('message', (message) => {
    console.log(`Message reçu du serveur : ${message}`);
});
