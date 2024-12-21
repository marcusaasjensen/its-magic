package com.example.its_magic.activities;

import android.os.Bundle;
import android.util.Log;
import android.view.WindowInsets;
import android.view.WindowInsetsController;
import android.view.animation.Animation;
import android.view.animation.AnimationUtils;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.ImageView;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import com.example.its_magic.R;
import com.example.its_magic.WebSocketManager;

public class StartActivity extends AppCompatActivity {
    private static final String TAG = "ForestActivity";
    private WebSocketManager webSocketManager;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.start_scene_layout);
        fullScreen();
        Button magicButton = findViewById(R.id.magic_button);

        Animation jellyAnimation = AnimationUtils.loadAnimation(this, R.anim.jelly_anim);

        magicButton.setOnClickListener(v -> {
            magicButton.startAnimation(jellyAnimation);
            ActivitySwitcher.switchActivity(this, ForestActivity.class);
        });

//        try {
//            this.webSocketManager = WebSocketManager.getInstance(this);
//            initializeViews();
//            initListeners();
//            startAnimation(alarmClockLayout, R.anim.fade_in_up_anim);
//            startAnimation(bagLayout, R.anim.fade_in_up_anim);
//
//        } catch (Exception e) {
//            Log.e(TAG, "Error in onCreate", e);
//            Toast.makeText(this, "Error initializing app", Toast.LENGTH_LONG).show();
//        }
    }

    private void fullScreen() {
        getWindow().setDecorFitsSystemWindows(false);
        WindowInsetsController controller = getWindow().getInsetsController();
        if (controller != null) {
            controller.hide(WindowInsets.Type.statusBars() | WindowInsets.Type.navigationBars());
            controller.setSystemBarsBehavior(WindowInsetsController.BEHAVIOR_SHOW_TRANSIENT_BARS_BY_SWIPE);
        }
    }

//    private void initializeViews() {
//        alarmClockLayout = findViewById(R.id.alarmClockLayout);
//        alarmClock = findViewById(R.id.alarmClock);
//        bagLayout = findViewById(R.id.bagLayout);
//        bag = findViewById(R.id.bag);
//    }
//
//    private void initListeners() {
//        bag.setOnClickListener(v -> {
//            startAnimation(bag, R.anim.jelly_anim);
//            ObjectMessage message = new ObjectMessage(CLIENT_ID, RECIPIENT_ID.get(0), "glow", "bag");
//            webSocketManager.sendDataToServer(message);
//        });
//        alarmClock.setOnClickListener(v -> {
//            startAnimation(alarmClock, R.anim.jelly_anim);
//            ObjectMessage message = new ObjectMessage(CLIENT_ID, RECIPIENT_ID.get(0), "glow", "alarmClock");
//            webSocketManager.sendDataToServer(message);
//        });
//    }
//
//    @Override
//    protected void onResume() {
//        super.onResume();
//        Log.d(TAG, "onResume");
//        startAnimation(alarmClockLayout, R.anim.fade_in_up_anim);
//        startAnimation(bagLayout, R.anim.fade_in_up_anim);
//    }
//
//    @Override
//    protected void onPause() {
//        super.onPause();
//        Log.d(TAG, "onPause");
//    }
//
//    @Override
//    protected void onDestroy() {
//        Log.d(TAG, "onDestroy");
//        super.onDestroy();
//        if (webSocketManager != null) {
//            webSocketManager.cleanup();
//        }
//    }
//
//    private void startAnimation(View view, int animation) {
//        Animation jellyAnimation = AnimationUtils.loadAnimation(this, animation);
//        view.startAnimation(jellyAnimation);
//    }
}
