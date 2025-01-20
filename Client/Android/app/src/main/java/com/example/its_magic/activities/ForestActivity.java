package com.example.its_magic.activities;

import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.Toast;

import androidx.annotation.Nullable;

import com.example.its_magic.R;
import com.example.its_magic.WebSocketManager;
import com.example.its_magic.messages.SendMessage;
import com.example.its_magic.utils.AnimationHelper;
import com.example.its_magic.utils.SetupHelper;
import com.example.its_magic.utils.SoundHelper;

public class ForestActivity extends BaseActivity {
    private static final String TAG = "ForestActivity";
    private WebSocketManager webSocketManager;
    private SoundHelper soundHelper;

    private FrameLayout alarmClockLayout;
    private ImageView alarmClock;
    private FrameLayout bagLayout;
    private ImageView bag;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.forest_scene_layout);
        try {
            SetupHelper.fullScreen(this);
            soundHelper = new SoundHelper();
            initializeViews();
            initListeners();
            initSound();
            this.webSocketManager = WebSocketManager.getInstance(this);
            AnimationHelper.startAnimation(alarmClockLayout, R.anim.fade_in_up_anim);
            AnimationHelper.startAnimation(bagLayout, R.anim.fade_in_up_anim);

        } catch (Exception e) {
            Log.e(TAG, "Error in onCreate", e);
            Toast.makeText(this, "Error initializing app", Toast.LENGTH_LONG).show();
        }
    }

    private void initializeViews() {
        alarmClockLayout = findViewById(R.id.alarmClockLayout);
        alarmClock = findViewById(R.id.alarmClock);
        bagLayout = findViewById(R.id.bagLayout);
        bag = findViewById(R.id.bag);
        ImageView volumeUp = findViewById(R.id.volume_up);
        ImageView volumeDown = findViewById(R.id.volume_down);
        initVolumeIcons(volumeUp, volumeDown);
    }

    private void initListeners() {
        bag.setOnClickListener(v -> SendMessage.glowItemAnimate(webSocketManager, soundHelper, bag, "Bag"));
        alarmClock.setOnClickListener(v -> SendMessage.glowItemAnimate(webSocketManager, soundHelper, alarmClock, "AlarmClock"));
    }

    private void initSound() {
        soundHelper.loadSound(this, soundHelper.getTapSound(), R.raw.sfx_tap);
    }

    @Override
    protected void onStart() {
        super.onStart();
    }

    @Override
    protected void onResume() {
        super.onResume();
        Log.d(TAG, "onResume");
        AnimationHelper.startAnimation(alarmClockLayout, R.anim.fade_in_up_anim);
        AnimationHelper.startAnimation(bagLayout, R.anim.fade_in_up_anim);
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


    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_VOLUME_UP) {
            super.onKeyDown(keyCode, event);
            SendMessage.glowItemAnimate(webSocketManager, soundHelper, alarmClock, "AlarmClock");
            return true;
        } else if (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN) {
            super.onKeyDown(keyCode, event);
            SendMessage.glowItemAnimate(webSocketManager, soundHelper, bag, "Bag");
            return true;
        }
        return super.onKeyDown(keyCode, event);
    }
}
