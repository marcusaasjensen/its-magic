package com.example.its_magic.activities;

import android.graphics.Color;
import android.graphics.PorterDuff;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.widget.ImageView;

import androidx.appcompat.app.AppCompatActivity;

public class BaseActivity extends AppCompatActivity {

    private ImageView volumeUp;
    private ImageView volumeDown;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_VOLUME_UP) {
            Log.d("PhysicalButtons", "Physical Volume Up pressed");
            volumeUp.setColorFilter(Color.GREEN, PorterDuff.Mode.SRC_IN);
            return true;
        } else if (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN) {
            Log.d("PhysicalButtons", "Physical Volume Down pressed");
            volumeDown.setColorFilter(Color.RED, PorterDuff.Mode.SRC_IN);
            return true;
        }
        return super.onKeyDown(keyCode, event);
    }

    protected void initVolumeIcons(ImageView volumeUp, ImageView volumeDown) {
        this.volumeUp = volumeUp;
        this.volumeDown = volumeDown;
    }
}
