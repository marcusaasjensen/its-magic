package com.example.its_magic.activities;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.its_magic.R;
import com.example.its_magic.WebSocketManager;
import com.example.its_magic.layouts.ItemBag;
import com.example.its_magic.layouts.PhysicsLayout;
import com.example.its_magic.messages.SendMessage;
import com.example.its_magic.utils.SetupHelper;
import com.example.its_magic.utils.SoundHelper;

public class BagActivity extends AppCompatActivity {
    private static final String TAG = "BagActivity";
    private WebSocketManager webSocketManager;
    private SoundHelper soundHelper;

    private String scene;


    private PhysicsLayout physicsLayout;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.bag_scene_layout);

        try {
            SetupHelper.fullScreen(this);
            this.webSocketManager = WebSocketManager.getInstance(BagActivity.this);
            soundHelper = new SoundHelper();
            initializeViews();
            initSound();
            physicsLayout.restoreObjects();
            Intent intent = getIntent();
            if (intent != null && intent.hasExtra("objectId")) {
                int objectId = intent.getIntExtra("objectId", -1);
                if (objectId != -1) {
                    addItemInBag(objectId);
                }
            }
            if (intent != null && intent.hasExtra("sceneName")) {
                scene = intent.getStringExtra("sceneName");
            }

        } catch (Exception e) {
            Log.e(TAG, "Error in onCreate", e);
            Toast.makeText(this, "Error initializing app", Toast.LENGTH_LONG).show();
        }
    }

    private void initializeViews() {
        physicsLayout = findViewById(R.id.physicsLayout);
    }

    private void initSound() {
        soundHelper.loadSound(this, soundHelper.getTapSound(), R.raw.sfx_tap);
    }

    public void addItemInBag(int objectId) {
        int x = (int) (Math.random() * 800);
        int y = 200;
        physicsLayout.addItemBag(new ItemBag(x, y, setImageBackground(objectId)));
    }

    private int setImageBackground(int id) {
        switch (id) {
            case 1:
                return R.drawable.mushroom;
            case 2:
                return R.drawable.acorn;
            case 3:
                return R.drawable.berry;
            default:
                return R.drawable.firefly;
        }
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_VOLUME_UP) {
            super.onKeyDown(keyCode, event);
            if (scene.equals("forest")) {
                SendMessage.glowItem(webSocketManager, soundHelper, "AlarmClock");
            } else if (scene.equals("workshop")) {
                SendMessage.glowItem(webSocketManager, soundHelper, "Bellows");
            }
            return true;
        } else if (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN) {
            super.onKeyDown(keyCode, event);
            SendMessage.glowItem(webSocketManager, soundHelper, "Bag");
            return true;
        }
        return super.onKeyDown(keyCode, event);
    }
}

