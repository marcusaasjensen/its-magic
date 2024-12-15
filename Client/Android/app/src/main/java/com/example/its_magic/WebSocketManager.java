package com.example.its_magic;

import android.content.Context;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;

import org.java_websocket.client.WebSocketClient;
import org.java_websocket.handshake.ServerHandshake;
import org.json.JSONObject;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.net.URI;
import java.net.URISyntaxException;

public class WebSocketManager {
    private final String clientId = "Android";
    private static final String TAG = "WebSocketManager";
    private static WebSocketManager instance;
    private WebSocketClient webSocketClient;
    private static final String WEBSOCKET_URL_BASE  = "ws://";
    private final Object lock = new Object();
    private SpeakerSensor speakerSensor;
    private String serverIp;
    private int serverPort;
    private String clientType;

    private WebSocketManager(Context context) {
        this.speakerSensor = new SpeakerSensor(context);
        JSONObject config = ConfigReader.readConfig(context);
        if (config != null) {
            this.serverIp = config.optString("serverIp", "192.168.1.1");  // Valeur par défaut
            this.serverPort = config.optInt("serverPort", 8080);  // Valeur par défaut
            this.clientType = config.optString("clientType", "Android");  // Valeur par défaut
        }
        initWebSocket();

    }

    public String getClientId() {
        return clientId;
    }

    public static synchronized WebSocketManager getInstance(Context context) {
        if (instance == null) {
            instance = new WebSocketManager(context);
        }
        return instance;
    }

    private void initWebSocket() {
        try {
            String webSocketUrl = WEBSOCKET_URL_BASE + serverIp + ":" + serverPort + "?clientType=" + clientType;
            URI serverUri = new URI(webSocketUrl);
            createWebSocketClient(serverUri);
        } catch (URISyntaxException e) {
            Log.e(TAG, "Error creating WebSocket URI", e);
        }
    }

    private void createWebSocketClient(URI serverUri) {
        synchronized (lock) {
            if (webSocketClient != null) {
                try {
                    webSocketClient.close();
                } catch (Exception e) {
                    Log.e(TAG, "Error closing existing WebSocket", e);
                }
                webSocketClient = null;
            }

            webSocketClient = new WebSocketClient(serverUri) {
                @Override
                public void onOpen(ServerHandshake handshakeData) {
                    Log.i(TAG, "WebSocket connection opened successfully");
                }

                @Override
                public void onMessage(String message) {
                    Log.d(TAG, "Received message from server: " + message);
                    try {
                        JSONObject jsonMessage = new JSONObject(message);
                        String clientId = jsonMessage.getString("clientId");
                        String type = jsonMessage.getString("type");

                        if ("TopView".equals(clientId) && "vibrate".equals(type)) {
                            speakerSensor.vibratePhone();
                        }
                    } catch (Exception e) {
                        Log.e(TAG, "Error parsing message", e);
                    }
                }

                @Override
                public void onClose(int code, String reason, boolean remote) {
                    Log.e(TAG, "WebSocket closed with code: " + code + " reason: " + reason);
                    reconnectWithDelay();
                }

                @Override
                public void onError(Exception ex) {
                    Log.e(TAG, "WebSocket error occurred: " + ex.getMessage());
                    if (ex instanceof java.net.ConnectException) {
                        Log.e(TAG, "Connection refused. Check that the server is running and accessible");
                    } else if (ex instanceof java.net.SocketTimeoutException) {
                        Log.e(TAG, "Connection timed out. Check your network connection");
                    }
                    StringWriter sw = new StringWriter();
                    ex.printStackTrace(new PrintWriter(sw));
                    Log.e(TAG, "Stack trace complète: " + sw);
                }
            };

            try {
                Log.d(TAG, "Attempting to connect to WebSocket at: " + serverUri);
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
                    String recipientId = "TopView";
                    JSONObject jsonMessage = new JSONObject();
                    jsonMessage.put("clientId", clientId);
                    jsonMessage.put("type", value);
                    jsonMessage.put("recipientId", recipientId);

                    webSocketClient.send(jsonMessage.toString());
                    Log.d(TAG, "Sent sensor data: " + jsonMessage);
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
        }, 5000);
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