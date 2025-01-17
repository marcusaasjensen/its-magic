package com.example.its_magic.activities;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.its_magic.R;
import com.example.its_magic.WebSocketManager;
import com.example.its_magic.layouts.ItemBag;
import com.example.its_magic.layouts.PhysicsLayout;
import com.example.its_magic.utils.SetupHelper;

public class BagActivity extends AppCompatActivity {
    private static final String TAG = "BagActivity";
    private WebSocketManager webSocketManager;

    private PhysicsLayout physicsLayout;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.bag_scene_layout);

        try {
            SetupHelper.fullScreen(this);
            initializeViews();
            this.webSocketManager = WebSocketManager.getInstance(BagActivity.this);
            physicsLayout.restoreObjects();
            Intent intent = getIntent();
            if (intent != null && intent.hasExtra("objectId")) {
                int objectId = intent.getIntExtra("objectId", -1);
                if (objectId != -1) {
                    addItemInBag(objectId);
                }
            }

        } catch (Exception e) {
            Log.e(TAG, "Error in onCreate", e);
            Toast.makeText(this, "Error initializing app", Toast.LENGTH_LONG).show();
        }
    }

    private void initializeViews() {
        physicsLayout = findViewById(R.id.physicsLayout);
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
}

