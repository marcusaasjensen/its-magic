package com.example.its_magic.messages;

import android.util.Log;

import androidx.annotation.NonNull;

import org.json.JSONObject;

public class SceneMessage extends Message {
    private static final String TAG = "Scene Message";
    private final String sceneName;

    public SceneMessage(String clientId, String recipientId, String type, String sceneName) {
        super(clientId, recipientId, type);
        this.sceneName = sceneName;
    }

    @Override
    public JSONObject toJson() {
        JSONObject jsonMessage = super.toJson();
        try {
            jsonMessage.put("sceneName", sceneName);
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
