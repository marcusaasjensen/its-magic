package com.example.its_magic.messages;

import android.util.Log;

import androidx.annotation.NonNull;

import org.json.JSONObject;

public class Message {
    private static final String TAG = "Message";
    protected String clientId;
    protected String recipientId;
    protected String type;

    public Message(String clientId, String recipientId, String type) {
        this.clientId = clientId;
        this.recipientId = recipientId;
        this.type = type;
    }

    public JSONObject toJson() {
        JSONObject jsonMessage = new JSONObject();
        try {
            jsonMessage.put("clientId", clientId);
            jsonMessage.put("recipientId", recipientId);
            jsonMessage.put("type", type);
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

