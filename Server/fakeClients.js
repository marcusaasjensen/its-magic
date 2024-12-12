const WebSocket = require('ws');

// Fonction pour créer un client de test
function createTestClient(clientId, port) {
    const ws = new WebSocket(`ws://localhost:${port}?clientType=${clientId}`);

    ws.on('open', () => {
        console.log(`${clientId} connecté au serveur`);
    });

    ws.on('message', (data) => {
        const message = JSON.parse(data);
        console.log(`${clientId} a reçu:`, message);

        // Simulation des réponses en fonction du type d'action
        if (clientId === 'client2') {
            if (message.type === 'SendGreeting') {
                // Client2 répond avec un "ReceiveGreeting"
                setTimeout(() => {
                    const response = {
                        clientId: 'client2',
                        type: 'ReceiveGreeting',
                        recipientId: 'client1'
                    };
                    ws.send(JSON.stringify(response));
                    console.log(`${clientId} a exécuté ReceiveGreeting`);
                }, 1000);
            }
        } 
        else if (clientId === 'client1') {
            if (message.type === 'ReceiveGreeting') {
                // Client1 confirme avec un "ConfirmReceived"
                setTimeout(() => {
                    const confirmation = {
                        clientId: 'client1',
                        type: 'ConfirmReceived',
                        recipientId: 'client2'
                    };
                    ws.send(JSON.stringify(confirmation));
                    console.log(`${clientId} a exécuté ConfirmReceived`);
                }, 1000);
            }
        }
    });

    return ws;
}

// Créer les deux clients
const client1 = createTestClient('client1', 8080);
const client2 = createTestClient('client2', 8080);

// Attendre que les connexions soient établies avant d'envoyer le premier message
setTimeout(() => {
    // Client1 commence avec SendGreeting
    const initialMessage = {
        clientId: 'client1',
        type: 'SendGreeting',
        recipientId: 'client2'
    };
    client1.send(JSON.stringify(initialMessage));
    console.log('Client1 a exécuté SendGreeting');
}, 2000);