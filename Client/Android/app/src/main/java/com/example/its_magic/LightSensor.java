package com.example.its_magic;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.util.Log;

public class LightSensor extends BaseSensor implements SensorEventListener {
    private static final String TAG = "LightSensor";
    private final SensorManager sensorManager;
    private final Sensor lightSensor;
    private boolean isNightZone = false;
    private boolean isDayZone = false;

    public LightSensor(Context context, SensorCallback callback) {
        super(context, callback);
        sensorManager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);
        lightSensor = sensorManager.getDefaultSensor(Sensor.TYPE_LIGHT);
    }

    @Override
    protected String getSensorType() {
        return "light";
    }

    @Override
    public void start() {
        if (lightSensor != null) {
            sensorManager.registerListener(this, lightSensor, SensorManager.SENSOR_DELAY_NORMAL);
        }
    }

    @Override
    public void stop() {
        if (sensorManager != null) {
            sensorManager.unregisterListener(this);
        }
    }

    @Override
    public void cleanup() {
        Log.d(TAG, "Cleaning up LightSensor");
        stop();
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        try {
            float lightLevel = event.values[0];
            String value = String.format("%.1f", lightLevel);

            if (lightLevel <= 150) {
                callback.onValueChanged("light", value + " lx");
                if (!isNightZone) {
                    isNightZone = true;
                    isDayZone = false;
                    sendToServer(value);
                }
            } else if (lightLevel > 150) {
                callback.onValueChanged("light", value + " lx");
                if (!isDayZone) {
                    isDayZone = true;
                    isNightZone = false;
                    sendToServer(value);
                }
            }
        } catch (Exception e) {
            Log.e(TAG, "Error in onSensorChanged", e);
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {
        // Non utilisé
    }
}
