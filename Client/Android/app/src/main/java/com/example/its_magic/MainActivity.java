package com.example.its_magic;

import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.animation.Animation;
import android.view.animation.AnimationUtils;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

public class MainActivity extends AppCompatActivity implements SensorCallback {
    private static final String TAG = "MainActivity";
    private WebSocketManager webSocketManager;
    private boolean isActive = false;

    private BaseSensor lightSensor;
    private BaseSensor breathSensor;
    private BaseSensor brightnessSensor;

    private TextView lightTextView;
    private TextView breathTextView;
    private TextView brightnessTextView;
    private ImageView blowEffect;
    private Animation animation;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.main_layout);

        try {
            initializeViews();
            initializeSensors();
        } catch (Exception e) {
            Log.e(TAG, "Error in onCreate", e);
            Toast.makeText(this, "Error initializing app", Toast.LENGTH_LONG).show();
        }
    }

    private void initializeViews() {
        lightTextView = findViewById(R.id.lightTextView);
        breathTextView = findViewById(R.id.breathTextView);
        brightnessTextView = findViewById(R.id.brightnessTextView);
        blowEffect = findViewById(R.id.blowEffect);
        animation = AnimationUtils.loadAnimation(this, R.anim.blow_effect);
    }

    private void initializeSensors() {
        lightSensor = new LightSensor(this, this);
        breathSensor = new BreathSensor(this, this);
        brightnessSensor = new ScreenBrightnessSensor(this, this);
    }

    private void startAllSensors() {
        try {
            if (lightSensor != null) lightSensor.start();
            if (breathSensor != null) breathSensor.start();
            if (brightnessSensor != null) brightnessSensor.start();
        } catch (Exception e) {
            Log.e(TAG, "Error starting sensors", e);
        }
    }

    private void stopAllSensors() {
        try {
            if (lightSensor != null) lightSensor.stop();
            if (breathSensor != null) breathSensor.stop();
            if (brightnessSensor != null) brightnessSensor.stop();
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
            if (breathSensor != null) breathSensor.cleanup();
            if (brightnessSensor != null) brightnessSensor.cleanup();

            lightSensor = null;
            breathSensor = null;
            brightnessSensor = null;
        } catch (Exception e) {
            Log.e(TAG, "Error cleaning up sensors", e);
        }
    }

    @Override
    public void onValueChanged(String sensorName, String value) {
        if (!isActive) return;

        runOnUiThread(() -> {
            try {
                switch (sensorName) {
                    case "light":
                        if (lightTextView != null) lightTextView.setText("Light Level: " + value);
                        break;
                    case "breath":
                        if (breathTextView != null) {
                            breathTextView.setText(value);
                            if (value.equals("Breath detected!")) {
                                showBlowEffect();
                            }
                        }
                        break;
                    case "brightness":
                        if (brightnessTextView != null) brightnessTextView.setText("Screen Brightness: " + value);
                        break;
                }
            } catch (Exception e) {
                Log.e(TAG, "Error updating UI", e);
            }
        });
    }

    @Override
    public void onError(String sensorName, String error) {
        if (!isActive) return;

        runOnUiThread(() -> {
            Toast.makeText(this, sensorName + " error: " + error, Toast.LENGTH_SHORT).show();
        });
    }

    private void showBlowEffect() {
        if (blowEffect != null && animation != null) {
            blowEffect.setVisibility(View.VISIBLE);
            blowEffect.startAnimation(animation);
        }
    }
}
