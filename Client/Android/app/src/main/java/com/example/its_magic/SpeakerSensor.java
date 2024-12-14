package com.example.its_magic;

import android.content.Context;
import android.os.VibrationEffect;
import android.os.Vibrator;

public class SpeakerSensor {
    private Context context;

    public SpeakerSensor(Context context) {
        this.context = context;
    }

    public void vibratePhone() {
        if (context != null) {
            Vibrator vibrator = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);
            if (vibrator != null) {
                    vibrator.vibrate(VibrationEffect.createOneShot(500, VibrationEffect.DEFAULT_AMPLITUDE));
            } else {
                System.out.println("Vibrator service not available");
            }
        }
    }
}