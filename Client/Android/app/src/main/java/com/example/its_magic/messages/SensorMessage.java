package com.example.its_magic.messages;

import android.util.Log;

import androidx.annotation.NonNull;

import org.json.JSONObject;

public class SensorMessage extends Message {
    private static final String TAG = "Sensor Message";
    private final float value;

    public SensorMessage(String clientId, String recipientId, String sensorType, float value) {
        super(clientId, recipientId, sensorType);
        this.value = value;
    }

    @Override
    public JSONObject toJson() {
        JSONObject jsonMessage = super.toJson();
        try {
            jsonMessage.put("value", value);
        } catch (Exception e) {
            Log.e(TAG, "Error sending data", e);
        }
        return jsonMessage;
    }

    @NonNull
    @Override
    public String toString() {
        return toJson().toString();
    }
}
