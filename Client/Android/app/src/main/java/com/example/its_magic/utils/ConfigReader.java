package com.example.its_magic.utils;

import android.content.Context;
import android.util.Log;

import org.json.JSONObject;

import java.io.InputStream;
import java.nio.charset.StandardCharsets;

public class ConfigReader {

    private static final String TAG = "ConfigReader";

    public static JSONObject readConfig(Context context) {
        try {
            InputStream inputStream = context.getAssets().open("config.json");

            int size = inputStream.available();
            byte[] buffer = new byte[size];
            inputStream.read(buffer);
            inputStream.close();

            String jsonString = new String(buffer, StandardCharsets.UTF_8);

            return new JSONObject(jsonString);

        } catch (Exception e) {
            Log.e(TAG, "Error reading config file", e);
            return null;
        }
    }
}

