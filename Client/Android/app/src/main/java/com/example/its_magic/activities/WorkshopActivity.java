package com.example.its_magic.activities;

import static com.example.its_magic.WebSocketManager.CLIENT_ID;
import static com.example.its_magic.WebSocketManager.RECIPIENT_ID;

import android.os.Bundle;
import android.util.Log;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import com.example.its_magic.R;
import com.example.its_magic.WebSocketManager;
import com.example.its_magic.messages.ObjectMessage;
import com.example.its_magic.utils.AnimationHelper;
import com.example.its_magic.utils.SetupHelper;
import com.example.its_magic.utils.SoundHelper;

public class WorkshopActivity extends AppCompatActivity {
    private static final String TAG = "WorkshopActivity";
    private WebSocketManager webSocketManager;
    private SoundHelper soundHelper;

    private final String soundTap = "tapObject";

    private FrameLayout bellowsLayout;
    private ImageView bellows;
    private FrameLayout bagLayout;
    private ImageView bag;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.workshop_scene_layout);
        try {
            SetupHelper.fullScreen(this);
            soundHelper = new SoundHelper();
            initializeViews();
            initListeners();
            initSound();
            this.webSocketManager = WebSocketManager.getInstance(this);
            AnimationHelper.startAnimation(bellowsLayout, R.anim.fade_in_up_anim);
            AnimationHelper.startAnimation(bagLayout, R.anim.fade_in_up_anim);

        } catch (Exception e) {
            Log.e(TAG, "Error in onCreate", e);
            Toast.makeText(this, "Error initializing app", Toast.LENGTH_LONG).show();
        }
    }

    private void initializeViews() {
        bellowsLayout = findViewById(R.id.bellowsLayout);
        bellows = findViewById(R.id.bellows);
        bagLayout = findViewById(R.id.bagLayout);
        bag = findViewById(R.id.bag);
    }

    private void initListeners() {
        bag.setOnClickListener(v -> {
            AnimationHelper.startAnimation(bag, R.anim.jelly_anim);
            soundHelper.playSFX(soundTap, 0.25f);
            ObjectMessage message = new ObjectMessage(CLIENT_ID, RECIPIENT_ID.get(0), "glow", "bag");
            webSocketManager.sendDataToServer(message);
        });
        bellows.setOnClickListener(v -> {
            AnimationHelper.startAnimation(bellows, R.anim.jelly_anim);
            soundHelper.playSFX(soundTap, 0.25f);
            ObjectMessage message = new ObjectMessage(CLIENT_ID, RECIPIENT_ID.get(0), "glow", "bellows");
            webSocketManager.sendDataToServer(message);
//            ActivitySwitcher.switchActivity(this, BreathSensorActivity.class);
        });
    }

    private void initSound() {
        soundHelper.loadSound(this, soundTap, R.raw.sfx_tap);
        String soundGame = "bellowsGame";
        soundHelper.loadSound(this, soundGame, R.raw.sound_forest);
    }

    @Override
    protected void onStart() {
        super.onStart();
        soundHelper.playAmbiance(this, R.raw.sound_forest);
    }

    @Override
    protected void onResume() {
        super.onResume();
        Log.d(TAG, "onResume");
        AnimationHelper.startAnimation(bellowsLayout, R.anim.fade_in_up_anim);
        AnimationHelper.startAnimation(bagLayout, R.anim.fade_in_up_anim);
        soundHelper.playAmbiance(this, R.raw.sound_forest);
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
}
