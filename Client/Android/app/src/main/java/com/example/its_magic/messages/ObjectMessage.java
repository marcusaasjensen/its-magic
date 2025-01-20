package com.example.its_magic.messages;

import android.util.Log;

import androidx.annotation.NonNull;

import org.json.JSONObject;

public class ObjectMessage extends Message {
    private static final String TAG = "Object Message";
    private final String targetObject;

    public ObjectMessage(String clientId, String recipientId, String type, String targetObject) {
        super(clientId, recipientId, type);
        this.targetObject = targetObject;
    }


    @Override
    public JSONObject toJson() {
        JSONObject jsonMessage = super.toJson();
        try {
            jsonMessage.put("targetObject", targetObject);
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
