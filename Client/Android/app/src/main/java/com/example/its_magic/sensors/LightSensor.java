package com.example.its_magic.sensors;

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
    private float lastLightLevel = -1f;


    public LightSensor(Context context, SensorCallback callback) {
        super(context, callback);
        sensorManager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);
        lightSensor = sensorManager.getDefaultSensor(Sensor.TYPE_LIGHT);
    }

    @Override
    protected String getSensorType() {
        return "Light";
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

            if (lastLightLevel < 0 || Math.abs(lightLevel - lastLightLevel) >= 20f) {
                lastLightLevel = lightLevel;

                float minLightValue = 0f;
                float maxLightValue = 360f;

                float normalizedLightLevel = (lightLevel - minLightValue) / (maxLightValue - minLightValue);
                normalizedLightLevel = Math.max(0f, Math.min(1f, normalizedLightLevel));

                String normalizedValue = String.format("%.2f", normalizedLightLevel);

                callback.onValueChanged("Light", value + " lx");
                sendToServer(normalizedLightLevel);
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
