package com.example.its_magic;

import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import org.java_websocket.client.WebSocketClient;
import org.java_websocket.handshake.ServerHandshake;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.net.URI;
import java.net.URISyntaxException;

public class WebSocketManager {
    private static final String TAG = "WebSocketManager";
    private static WebSocketManager instance;
    private WebSocketClient webSocketClient;
    private static final String WEBSOCKET_URL = "ws://192.168.229.31:8080";
    private final Object lock = new Object(); // Pour la synchronisation

    private WebSocketManager() {
        initWebSocket();
    }

    public static synchronized WebSocketManager getInstance() {
        if (instance == null) {
            instance = new WebSocketManager();
        }
        return instance;
    }

    private void initWebSocket() {
        try {
            URI serverUri = new URI(WEBSOCKET_URL);
            createWebSocketClient(serverUri);
        } catch (URISyntaxException e) {
            Log.e(TAG, "Error creating WebSocket URI", e);
        }
    }

    private void createWebSocketClient(URI serverUri) {
        synchronized (lock) {
            // Si un client existe déjà, on le ferme proprement
            if (webSocketClient != null) {
                try {
                    webSocketClient.close();
                } catch (Exception e) {
                    Log.e(TAG, "Error closing existing WebSocket", e);
                }
                webSocketClient = null;
            }

            // Création d'un nouveau client
            webSocketClient = new WebSocketClient(serverUri) {
                @Override
                public void onOpen(ServerHandshake handshakedata) {
                    Log.i(TAG, "WebSocket connection opened successfully");
                }

                @Override
                public void onMessage(String message) {
                    Log.d(TAG, "Received message from server: " + message);
                }

                @Override
                public void onClose(int code, String reason, boolean remote) {
                    Log.e(TAG, "WebSocket closed with code: " + code + " reason: " + reason);
                    // Tentative de reconnexion après un délai
                    reconnectWithDelay();
                }

                @Override
                public void onError(Exception ex) {
                    Log.e(TAG, "WebSocket error occurred: " + ex.getMessage());
                    if (ex instanceof java.net.ConnectException) {
                        Log.e(TAG, "Connection refused. Vérifiez que le serveur est en cours d'exécution et accessible");
                    } else if (ex instanceof java.net.SocketTimeoutException) {
                        Log.e(TAG, "Connection timed out. Vérifiez votre connexion réseau");
                    }
                    StringWriter sw = new StringWriter();
                    ex.printStackTrace(new PrintWriter(sw));
                    Log.e(TAG, "Stack trace complète: " + sw.toString());
                }
            };

            try {
                Log.d(TAG, "Attempting to connect to WebSocket at: " + serverUri.toString());
                webSocketClient.connect();
            } catch (Exception e) {
                Log.e(TAG, "Exception while connecting to WebSocket", e);
            }
        }
    }

    public void sendSensorData(String sensorType, String value) {
        synchronized (lock) {
            if (webSocketClient != null && webSocketClient.isOpen()) {
                try {
                    String message = String.format(
                            "{\"type\":\"%s\",\"value\":\"%s\",\"timestamp\":%d}",
                            sensorType, value, System.currentTimeMillis()
                    );
                    webSocketClient.send(message);
                    Log.d(TAG, "Sent sensor data: " + message);
                } catch (Exception e) {
                    Log.e(TAG, "Error sending sensor data", e);
                    reconnectWithDelay();
                }
            } else {
                Log.w(TAG, "WebSocket is not connected. Attempting to reconnect...");
                reconnectWithDelay();
            }
        }
    }

    private void reconnectWithDelay() {
        new Handler(Looper.getMainLooper()).postDelayed(() -> {
            synchronized (lock) {
                if (webSocketClient == null || !webSocketClient.isOpen()) {
                    Log.d(TAG, "Attempting to reconnect...");
                    initWebSocket();
                }
            }
        }, 5000); // 5 secondes de délai
    }

    public void cleanup() {
        synchronized (lock) {
            if (webSocketClient != null) {
                try {
                    webSocketClient.close();
                } catch (Exception e) {
                    Log.e(TAG, "Error during cleanup", e);
                }
                webSocketClient = null;
            }
        }
    }
}