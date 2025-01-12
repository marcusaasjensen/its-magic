package com.example.its_magic.activities;

import static com.example.its_magic.WebSocketManager.CLIENT_ID;
import static com.example.its_magic.WebSocketManager.RECIPIENT_ID;
import static com.example.its_magic.sensors.BreathSensor.REQUEST_RECORD_AUDIO_PERMISSION;

import android.Manifest;
import android.content.Context;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.util.Log;
import android.widget.Button;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.example.its_magic.R;
import com.example.its_magic.WebSocketManager;
import com.example.its_magic.messages.Message;
import com.example.its_magic.messages.SceneMessage;
import com.example.its_magic.utils.AnimationHelper;
import com.example.its_magic.utils.SetupHelper;
import com.example.its_magic.utils.SoundHelper;

public class StartActivity extends AppCompatActivity {
    private static final String TAG = "StartActivity";
    private WebSocketManager webSocketManager;
    private SoundHelper soundHelper;
    private Button playButton;
    private final String soundPlayGame = "playGame";

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.start_scene_layout);
        try {
            if (ContextCompat.checkSelfPermission(this, Manifest.permission.RECORD_AUDIO)
                    != PackageManager.PERMISSION_GRANTED) {
                ActivityCompat.requestPermissions(this,
                        new String[]{Manifest.permission.RECORD_AUDIO}, REQUEST_RECORD_AUDIO_PERMISSION);
            }
            SetupHelper.fullScreen(this);
            soundHelper = new SoundHelper();
            initializeViews();
            initListeners();
            initSound();
            this.webSocketManager = WebSocketManager.getInstance(this);
            clearSharedPreferences();
        } catch (Exception e) {
            Log.e(TAG, "Error in onCreate", e);
            Toast.makeText(this, "Error initializing app", Toast.LENGTH_LONG).show();
        }
    }

    private void initializeViews() {
        playButton = findViewById(R.id.magic_button);
    }

    private void initListeners() {
        playButton.setOnClickListener(v -> {
            AnimationHelper.startAnimation(playButton, R.anim.jelly_anim);
            SceneMessage topViewMessage = new SceneMessage(CLIENT_ID, RECIPIENT_ID.get(0), "StartGame", "ForestTopSceneMarie");
            webSocketManager.sendDataToServer(topViewMessage);
            SceneMessage sideViewMessage = new SceneMessage(CLIENT_ID, RECIPIENT_ID.get(1), "StartGame", "ForestSideSceneMarie");
            webSocketManager.sendDataToServer(sideViewMessage);

            soundHelper.playSFX(soundPlayGame, 0.25f);
//            ActivitySwitcher.switchActivity(this, WorkshopActivity.class);
        });
    }

    private void initSound() {
        soundHelper.loadSound(this, soundPlayGame, R.raw.sfx_maaagical);
        String soundGame = "startGame";
        soundHelper.loadSound(this, soundGame, R.raw.sound_start);
    }

    @Override
    protected void onStart() {
        super.onStart();
        soundHelper.playAmbiance(this, R.raw.sound_start);
    }

    @Override
    protected void onResume() {
        super.onResume();
        Log.d(TAG, "onResume");
        soundHelper.playAmbiance(this, R.raw.sound_start);
    }

    @Override
    protected void onPause() {
        super.onPause();
        Log.d(TAG, "onPause");
        soundHelper.stopAmbiance();

    }

    @Override
    protected void onDestroy() {
        Log.d(TAG, "onDestroy");
        super.onDestroy();
        if (webSocketManager != null) {
            webSocketManager.cleanup();
        }
        soundHelper.stopAmbiance();
        soundHelper.release();
    }

    private void clearSharedPreferences() {
        SharedPreferences preferences = getSharedPreferences("PhysicsLayoutPrefs", Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = preferences.edit();
        editor.clear(); // Effacer toutes les données
        editor.apply(); // Appliquer les changements
        Log.d("Physics", "Les SharedPreferences ont été vidées au démarrage.");
    }
}
