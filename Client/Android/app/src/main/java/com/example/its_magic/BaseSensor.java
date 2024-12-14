package com.example.its_magic;

import android.content.Context;
import android.util.Log;

public abstract class BaseSensor {
    protected Context context;
    protected SensorCallback callback;
    protected WebSocketManager webSocketManager;
    private static final String TAG = "BaseSensor";

    public BaseSensor(Context context, SensorCallback callback) {
        this.context = context;
        this.callback = callback;
        try {
            this.webSocketManager = WebSocketManager.getInstance(context);
        } catch (Exception e) {
            Log.e(TAG, "Error initializing WebSocketManager", e);
        }
    }

    protected void sendToServer(String value) {
        try {
            if (webSocketManager != null) {
                webSocketManager.sendSensorData(getSensorType(), value);
            }
        } catch (Exception e) {
            Log.e(TAG, "Error sending to server: " + e.getMessage());
        }
    }

    protected abstract String getSensorType();
    public abstract void start();
    public abstract void stop();
    public abstract void cleanup();
}
