package com.example.its_magic;

import android.app.Activity;
import android.content.Context;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;

import com.example.its_magic.activities.ActivitySwitcher;
import com.example.its_magic.activities.BagActivity;
import com.example.its_magic.activities.BreathSensorActivity;
import com.example.its_magic.activities.ForestActivity;
import com.example.its_magic.activities.LightSensorActivity;
import com.example.its_magic.activities.WorkshopActivity;
import com.example.its_magic.messages.Message;
import com.example.its_magic.messages.SensorMessage;
import com.example.its_magic.sensors.SpeakerSensor;
import com.example.its_magic.utils.ConfigReader;

import org.java_websocket.client.WebSocketClient;
import org.java_websocket.handshake.ServerHandshake;
import org.json.JSONObject;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.List;

public class WebSocketManager {
    public static final String CLIENT_ID = "Android";
    public static final List<String> RECIPIENT_ID = List.of("TopView", "SideView");
    private static final String TAG = "WebSocketManager";
    private static WebSocketManager instance;
    private WebSocketClient webSocketClient;
    private static final String WEBSOCKET_URL_BASE = "ws://";
    private final Object lock = new Object();
    private SpeakerSensor speakerSensor;
    private String serverIp;
    private int serverPort;
    private String clientType;
    private final Context context;

    private WebSocketManager(Context context) {
        this.context = context;
        this.speakerSensor = new SpeakerSensor(context);
        JSONObject config = ConfigReader.readConfig(context);
        if (config != null) {
            this.serverIp = config.optString("serverIp", "192.168.205.187");
            this.serverPort = config.optInt("serverPort", 8080);
            this.clientType = config.optString("clientType", "Android");
        }
        initWebSocket();

    }

    public static synchronized WebSocketManager getInstance(Context newContext) {
        instance = new WebSocketManager(newContext);
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

                        if (clientId.equals("TopView")) {
                            switch (type) {
                                case "Vibrate":
                                    speakerSensor.vibratePhone();
                                    break;
                                case "SwitchObject":
                                    String object = jsonMessage.getString("objectName");
                                    if (object.equals("Bag")) {
                                        String scene = jsonMessage.getString("objectScene");
                                        if (scene.equals("forest")) {
                                            ActivitySwitcher.switchActivityWithExtras(context.getApplicationContext(), BagActivity.class, R.drawable.forest_background);
                                            speakerSensor.vibratePhone();
                                        } else {
                                            ActivitySwitcher.switchActivityWithExtras(context.getApplicationContext(), BagActivity.class, R.drawable.workshop_background);
                                            speakerSensor.vibratePhone();
                                        }
                                    } else if (object.equals("bellows")) {
                                        ActivitySwitcher.switchActivity(context.getApplicationContext(), BreathSensorActivity.class);
                                        speakerSensor.vibratePhone();
                                    } else {
                                        ActivitySwitcher.switchActivity(context.getApplicationContext(), LightSensorActivity.class);
                                        speakerSensor.vibratePhone();
                                    }
                                    break;
                                case "Scene":
                                    String scene = jsonMessage.getString("sceneName");
                                    if (scene.equals("forest")) {
                                        ActivitySwitcher.switchActivity(context.getApplicationContext(), ForestActivity.class);
                                        speakerSensor.vibratePhone();
                                    } else {
                                        ActivitySwitcher.switchActivity(context.getApplicationContext(), WorkshopActivity.class);
                                        speakerSensor.vibratePhone();
                                    }
                                    break;
                                case "AddItem":
                                    int objectId = jsonMessage.getInt("objectId");
                                    if (context instanceof Activity) {
                                        speakerSensor.vibratePhone();
                                        ((Activity) context).runOnUiThread(() -> {
                                            BagActivity bagActivity = (BagActivity) context;
                                            bagActivity.addItemInBag(objectId);
                                        });
                                    }
                                    break;
                            }
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
                    for (String recipientId : RECIPIENT_ID) {
                        SensorMessage message = new SensorMessage(CLIENT_ID, recipientId, sensorType, value);
                        webSocketClient.send(message.toString());
                        Log.d(TAG, "Sent sensor data to " + recipientId + ": " + message);
                    }
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

    public void sendDataToServer(Message message) {
        synchronized (lock) {
            if (webSocketClient != null && webSocketClient.isOpen()) {
                try {
                    webSocketClient.send(message.toString());
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