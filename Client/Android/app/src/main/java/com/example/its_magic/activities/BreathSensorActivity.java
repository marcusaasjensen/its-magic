package com.example.its_magic.activities;


import android.os.Bundle;
import android.os.Handler;
import android.util.Log;
import android.widget.ImageView;
import android.widget.Toast;


import androidx.appcompat.app.AppCompatActivity;

import com.example.its_magic.R;
import com.example.its_magic.sensors.BreathSensor;
import com.example.its_magic.sensors.SensorCallback;
import com.example.its_magic.utils.AnimationHelper;
import com.example.its_magic.utils.SetupHelper;

public class BreathSensorActivity extends AppCompatActivity implements SensorCallback {
    private static final String TAG = "BreathSensorActivity";
    private BreathSensor breathSensor;
    private ImageView fire;
    private ImageView breath;
    private int currentFireState = 0;
    private final Handler handler = new Handler();

    private final int[] fireImages = new int[]{R.drawable.fire_off, R.drawable.fire_small, R.drawable.fire_medium, R.drawable.fire_large};

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.breath_sensor_scene_layout);

        try {
            SetupHelper.fullScreen(this);
            initializeViews();
            initSound();
            initializeSensor();
            AnimationHelper.startAnimation(breath, R.anim.blow_anim);
        } catch (Exception e) {
            Log.e(TAG, "Error in onCreate", e);
            Toast.makeText(this, "Error initializing app", Toast.LENGTH_LONG).show();
        }
        if (breathSensor != null) {
            handler.postDelayed(new Runnable() {
                @Override
                public void run() {
                    breathSensor.reduceFireIfIdle();
                    handler.postDelayed(this, 1000);
                    if (breathSensor.isBreath()) {
                        AnimationHelper.startAnimation(breath, R.anim.blow_anim);
                    }
                }
            }, 1000);

        }
    }

    private void initializeViews() {
        breath = findViewById(R.id.breath);
        fire = findViewById(R.id.fire);
    }

    private void initializeSensor() {
        breathSensor = new BreathSensor(this, this, this);
    }

    private void initSound() {
        String soundGame = "nightGame";
    }


    @Override
    protected void onStart() {
        super.onStart();
        breathSensor.start();
    }

    @Override
    protected void onResume() {
        super.onResume();
        Log.d(TAG, "onResume");
        breathSensor.start();
    }

    @Override
    protected void onPause() {
        super.onPause();
        Log.d(TAG, "onPause");
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        breathSensor.stop();
    }

    @Override
    public void onValueChanged(String sensorType, String value) {
        if ("fire".equals(sensorType)) {
            updateFireAnimation(value);
        }
    }

    @Override
    public void onError(String sensorType, String errorMessage) {
        Log.e(TAG, "Error with " + sensorType + ": " + errorMessage);
    }

    private void updateFireAnimation(String value) {
        if ("Fire off".equals(value)) {
            currentFireState = 0;
        } else if ("Small fire".equals(value)) {
            currentFireState = 100;
        } else if ("Medium fire".equals(value)) {
            currentFireState = 200;
        } else if ("Large fire".equals(value)) {
            currentFireState = 300;
        }
        fire.setBackgroundResource(fireImages[getFireStateIndex()]);
    }

    private int getFireStateIndex() {
        if (currentFireState >= 300) {
            return 3;
        } else if (currentFireState >= 200) {
            return 2;
        } else if (currentFireState >= 100) {
            return 1;
        } else {
            return 0;
        }
    }

}