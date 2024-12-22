package com.example.its_magic.activities;

import android.graphics.drawable.Drawable;
import android.graphics.drawable.GradientDrawable;
import android.graphics.drawable.TransitionDrawable;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.res.ResourcesCompat;

import com.example.its_magic.BaseSensor;
import com.example.its_magic.LightSensor;
import com.example.its_magic.R;
import com.example.its_magic.SensorCallback;
import com.example.its_magic.WebSocketManager;
import com.example.its_magic.fireflies.FireflyView;
import com.example.its_magic.utils.SetupHelper;

public class LightSensorActivity extends AppCompatActivity implements SensorCallback {
    private static final String TAG = "LightSensorActivity";
    private WebSocketManager webSocketManager;
    private boolean isActive = false;

    private BaseSensor lightSensor;

    private TextView lightTextView;

    private ImageView sunCycleImageView;
    private ImageView starType;
    private FireflyView fireflyView;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.light_sensor_scene_layout);

        try {
            SetupHelper.fullScreen(this);
            initializeViews();
            initializeSensors();
        } catch (Exception e) {
            Log.e(TAG, "Error in onCreate", e);
            Toast.makeText(this, "Error initializing app", Toast.LENGTH_LONG).show();
        }
    }

    private void initializeViews() {
        sunCycleImageView = findViewById(R.id.sunCycleImg);
        lightTextView = findViewById(R.id.lightLevel);
        fireflyView = findViewById(R.id.fireflies);
        starType = findViewById(R.id.starType);
    }

    private void initializeSensors() {
        lightSensor = new LightSensor(this, this);
    }

    private void startAllSensors() {
        try {
            if (lightSensor != null) lightSensor.start();
        } catch (Exception e) {
            Log.e(TAG, "Error starting sensors", e);
        }
    }

    private void stopAllSensors() {
        try {
            if (lightSensor != null) lightSensor.stop();
        } catch (Exception e) {
            Log.e(TAG, "Error stopping sensors", e);
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        Log.d(TAG, "onResume");
        isActive = true;
        startAllSensors();
    }

    @Override
    protected void onPause() {
        super.onPause();
        Log.d(TAG, "onPause");
        isActive = false;
        stopAllSensors();
    }

    @Override
    protected void onDestroy() {
        Log.d(TAG, "onDestroy");
        super.onDestroy();
        cleanupSensors();
        if (webSocketManager != null) {
            webSocketManager.cleanup();
        }
    }

    private void cleanupSensors() {
        try {
            if (lightSensor != null) lightSensor.cleanup();
            lightSensor = null;
        } catch (Exception e) {
            Log.e(TAG, "Error cleaning up sensors", e);
        }
    }

    @Override
    public void onValueChanged(String sensorName, String value) {
        if (!isActive) return;

        runOnUiThread(() -> {
            try {
                if (sensorName.equals("Light")) {
                    if (lightTextView != null) {
                        lightTextView.setText("Light Level: " + value);
                    }

                    if (sunCycleImageView != null) {
                        try {
                            String cleanedValue = value.replace(",", ".").replace(" lx", "").trim();
                            float lightValue = Float.parseFloat(cleanedValue);

                            float rotationAngle;
                            if (lightValue >= 0 && lightValue <= 180) {
                                rotationAngle = mapValue(lightValue, 0, 180, 0, 180);
                                changeBackgroundGradient(R.drawable.night_background_gradiant);
                                fireflyView.setVisibility(View.VISIBLE);
                                starType.setImageResource(R.drawable.moon);

                            } else {
                                rotationAngle = mapValue(lightValue, 180, 360, 180, 360);
                                changeBackgroundGradient(R.drawable.day_background_gradiant);
                                fireflyView.setVisibility(View.INVISIBLE);
                                starType.setImageResource(R.drawable.sun);
                            }

                            sunCycleImageView.setRotation(rotationAngle);

                        } catch (NumberFormatException e) {
                            Log.e(TAG, "Invalid light value: " + value, e);
                        }
                    }
                }
            } catch (Exception e) {
                Log.e(TAG, "Error updating UI", e);
            }
        });

    }

    @Override
    public void onError(String sensorName, String error) {
        if (!isActive) return;

        runOnUiThread(() -> Toast.makeText(this, sensorName + " error: " + error, Toast.LENGTH_SHORT).show());
    }

    private float mapValue(float value, float fromLow, float fromHigh, float toLow, float toHigh) {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }

    private void changeBackgroundGradient(int gradientResourceId) {
        GradientDrawable gradient = (GradientDrawable) ResourcesCompat.getDrawable(getResources(), gradientResourceId, null);

        TransitionDrawable transitionDrawable = new TransitionDrawable(new Drawable[]{
                getWindow().getDecorView().getBackground(),
                gradient
        });

        getWindow().getDecorView().setBackground(transitionDrawable);
        transitionDrawable.startTransition(1000);
    }
}
