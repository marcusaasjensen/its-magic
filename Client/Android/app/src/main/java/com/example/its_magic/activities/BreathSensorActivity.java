package com.example.its_magic.activities;


import android.os.Bundle;
import android.os.Handler;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.Toast;


import androidx.appcompat.app.AppCompatActivity;

import com.example.its_magic.R;
import com.example.its_magic.WebSocketManager;
import com.example.its_magic.messages.SendMessage;
import com.example.its_magic.sensors.BreathSensor;
import com.example.its_magic.sensors.SensorCallback;
import com.example.its_magic.utils.AnimationHelper;
import com.example.its_magic.utils.SetupHelper;
import com.example.its_magic.utils.SoundHelper;

public class BreathSensorActivity extends AppCompatActivity implements SensorCallback {
    private static final String TAG = "BreathSensorActivity";
    private WebSocketManager webSocketManager;
    private SoundHelper soundHelper;
    private BreathSensor breathSensor;
    private ImageView breath, fire;
    private FrameLayout smallFire, mediumFire, largeFire;

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
            soundHelper = new SoundHelper();
            initializeViews();
            initSound();
            initializeSensor();
            this.webSocketManager = WebSocketManager.getInstance(this);
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
        smallFire = findViewById(R.id.fireRed);
        mediumFire = findViewById(R.id.fireOrange);
        largeFire = findViewById(R.id.fireYellow);

    }

    private void initializeSensor() {
        breathSensor = new BreathSensor(this, this, this);
    }

    private void initSound() {
        soundHelper.loadSound(this, soundHelper.getTapSound(), R.raw.sfx_tap);
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
        stopAllSensors();
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        cleanupSensors();
        breathSensor.stop();
    }

    private void stopAllSensors() {
        try {
            if (breathSensor != null) breathSensor.stop();
        } catch (Exception e) {
            Log.e(TAG, "Error stopping sensors", e);
        }
    }


    private void cleanupSensors() {
        try {
            if (breathSensor != null) breathSensor.cleanup();
            breathSensor = null;
        } catch (Exception e) {
            Log.e(TAG, "Error cleaning up sensors", e);
        }
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
            smallFire.setVisibility(View.INVISIBLE);
            mediumFire.setVisibility(View.INVISIBLE);
            largeFire.setVisibility(View.INVISIBLE);
        } else if ("Small fire".equals(value)) {
            currentFireState = 300;
            smallFire.setVisibility(View.VISIBLE);
            mediumFire.setVisibility(View.INVISIBLE);
            largeFire.setVisibility(View.INVISIBLE);
        } else if ("Medium fire".equals(value)) {
            currentFireState = 600;
            smallFire.setVisibility(View.VISIBLE);
            mediumFire.setVisibility(View.VISIBLE);
            largeFire.setVisibility(View.INVISIBLE);
        } else if ("Large fire".equals(value)) {
            currentFireState = 900;
            smallFire.setVisibility(View.VISIBLE);
            mediumFire.setVisibility(View.VISIBLE);
            largeFire.setVisibility(View.VISIBLE);
        }
        fire.setBackgroundResource(fireImages[getFireStateIndex()]);
    }

    private int getFireStateIndex() {
        if (currentFireState >= 900) {
            return 3;
        } else if (currentFireState >= 600) {
            return 2;
        } else if (currentFireState >= 300) {
            return 1;
        } else {
            return 0;
        }
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_VOLUME_UP) {
            super.onKeyDown(keyCode, event);
            SendMessage.glowItem(webSocketManager, soundHelper, "Bellows");
            return true;
        } else if (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN) {
            super.onKeyDown(keyCode, event);
            SendMessage.glowItem(webSocketManager, soundHelper, "Bag");
            return true;
        }
        return super.onKeyDown(keyCode, event);
    }

}