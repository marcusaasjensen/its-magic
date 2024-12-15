package com.example.its_magic;

import android.content.ContentResolver;
import android.content.Context;
import android.database.ContentObserver;
import android.os.Handler;
import android.os.Looper;
import android.provider.Settings;
import android.util.Log;


public class ScreenBrightnessSensor extends BaseSensor {
    private static final String TAG = "ScreenBrightnessSensor";
    private ContentResolver contentResolver;
    private ContentObserver brightnessObserver;
    private Handler handler;
    private double deviceSpecificGamma = 1.8;

    public ScreenBrightnessSensor(Context context, SensorCallback callback) {
        super(context, callback);
        this.contentResolver = context.getContentResolver();
        this.handler = new Handler(Looper.getMainLooper());
    }

    @Override
    protected String getSensorType() {
        return "brightness";
    }

    @Override
    public void start() {
        brightnessObserver = new ContentObserver(handler) {
            @Override
            public void onChange(boolean selfChange) {
                updateBrightnessStatus();
            }
        };

        contentResolver.registerContentObserver(
                Settings.System.getUriFor(Settings.System.SCREEN_BRIGHTNESS),
                false,
                brightnessObserver
        );
        updateBrightnessStatus();
    }

    @Override
    public void stop() {
        if (brightnessObserver != null && contentResolver != null) {
            contentResolver.unregisterContentObserver(brightnessObserver);
        }
    }

    @Override
    public void cleanup() {
        Log.d(TAG, "Cleaning up ScreenBrightnessSensor");
        stop();
        handler = null;
    }

    private void updateBrightnessStatus() {
        try {
            if (contentResolver == null) return;

            int brightness = Settings.System.getInt(contentResolver, Settings.System.SCREEN_BRIGHTNESS);
            int brightnessPercentage = convertBrightnessToPercentage(brightness);
            String value = String.valueOf(brightnessPercentage);

            callback.onValueChanged("brightness", value + "%");

            if (brightnessPercentage == 100) {
                sendToServer(value);
            }
        } catch (Exception e) {
            Log.e(TAG, "Error updating brightness", e);
            callback.onError("brightness", "Error fetching brightness");
        }
    }

    private int convertBrightnessToPercentage(int systemBrightness) {
        double normalizedBrightness = (systemBrightness - 1.0) / (255.0 - 1.0);
        double gammaCorrectedBrightness = Math.pow(normalizedBrightness, 1.0 / deviceSpecificGamma);
        int percentage = (int) Math.round(gammaCorrectedBrightness * 100.0);
        return Math.max(0, Math.min(100, percentage));
    }
}
