package com.example.its_magic.messages;

import android.util.Log;

import androidx.annotation.NonNull;

import org.json.JSONObject;

public class BreathMessage extends Message {
    private static final String TAG = "Object Message";
    private final float fireIntensity;
    private final float windIntensity;

    public BreathMessage(String clientId, String recipientId, String type, float fireIntensity, float windIntensity) {
        super(clientId, recipientId, type);
        this.fireIntensity = fireIntensity;
        this.windIntensity = windIntensity;
    }

    @Override
    public JSONObject toJson() {
        JSONObject jsonMessage = super.toJson();
        try {
            jsonMessage.put("fireIntensity", fireIntensity);
            jsonMessage.put("windIntensity", windIntensity);
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
