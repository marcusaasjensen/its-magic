package com.example.its_magic.messages;

import android.util.Log;

import androidx.annotation.NonNull;

import org.json.JSONObject;

public class FireMessage extends Message {
    private static final String TAG = "Object Message";
    private final float fireIntensity;

    public FireMessage(String clientId, String recipientId, String type, float fireIntensity) {
        super(clientId, recipientId, type);
        this.fireIntensity = fireIntensity;
    }

    @Override
    public JSONObject toJson() {
        JSONObject jsonMessage = super.toJson();
        try {
            jsonMessage.put("fireIntensity", fireIntensity);
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
