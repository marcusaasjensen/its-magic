package com.example.its_magic.sensors;


import android.Manifest;
import android.app.Activity;
import android.content.Context;
import android.content.pm.PackageManager;
import android.media.AudioFormat;
import android.media.AudioRecord;
import android.media.MediaRecorder;
import android.os.Handler;
import android.util.Log;

import androidx.core.app.ActivityCompat;

public class BreathSensor extends BaseSensor {
    private static final String TAG = "BreathSensor";
    public static final int REQUEST_RECORD_AUDIO_PERMISSION = 200;

    private static final int SAMPLE_RATE = 44100;
    private static final int BUFFER_SIZE = AudioRecord.getMinBufferSize(SAMPLE_RATE, AudioFormat.CHANNEL_IN_MONO, AudioFormat.ENCODING_PCM_16BIT);
    private AudioRecord audioRecord;
    private boolean isRecording = false;
    private final short[] audioBuffer = new short[BUFFER_SIZE];
    private int currentFireState = 0;
    private long lastBreathTime;
    private static final long BREATH_TIMEOUT = 5000;
    private final Activity activity;
    private boolean isBreath = false;

    public BreathSensor(Context context, SensorCallback callback, Activity activity) {
        super(context, callback);
        this.activity = activity;
    }

    @Override
    protected String getSensorType() {
        return "Breath";
    }

    @Override
    public void start() {
        if (ActivityCompat.checkSelfPermission(context, Manifest.permission.RECORD_AUDIO) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(activity,
                    new String[]{Manifest.permission.RECORD_AUDIO}, REQUEST_RECORD_AUDIO_PERMISSION);

            return;
        }
        audioRecord = new AudioRecord(
                MediaRecorder.AudioSource.MIC,
                SAMPLE_RATE,
                AudioFormat.CHANNEL_IN_MONO,
                AudioFormat.ENCODING_PCM_16BIT,
                BUFFER_SIZE
        );

        audioRecord.startRecording();
        isRecording = true;

        Handler handler = new Handler();
        handler.postDelayed(new Runnable() {
            @Override
            public void run() {
                if (isRecording) {
                    detectBlow();
                    handler.postDelayed(this, 100);
                }
            }
        }, 100);
    }

    private void updateFireAnimation() {
        if (currentFireState >= 300) {
            callback.onValueChanged("fire", "Large fire");
        } else if (currentFireState >= 200) {
            callback.onValueChanged("fire", "Medium fire");
        } else if (currentFireState >= 100) {
            callback.onValueChanged("fire", "Small fire");
        } else {
            callback.onValueChanged("fire", "Fire off");
        }
    }

    @Override
    public void stop() {
        Log.d(TAG, "Stopping BreathSensor");
        if (audioRecord != null && isRecording) {
            audioRecord.stop();
            audioRecord.release();
            isRecording = false;
        }
    }

    @Override
    public void cleanup() {
        Log.d(TAG, "Cleaning up BreathSensor");
        stop();
    }

    private void detectBlow() {
        int numberOfShortsRead = audioRecord.read(audioBuffer, 0, audioBuffer.length);

        if (numberOfShortsRead > 0) {
            double amplitude = 0;

            for (int i = 0; i < numberOfShortsRead; i++) {
                amplitude += Math.abs(audioBuffer[i]);
            }

            amplitude /= numberOfShortsRead;

            if (amplitude > 2000) {
                Log.d(TAG, "Breath detected");
                isBreath = true;
                String value = String.format("%.1f", amplitude);
                callback.onValueChanged("breath", "Breath detected!");
                sendToServer(value);

                if (currentFireState < 300) {
                    currentFireState += 100;
                }

                updateFireAnimation();

                lastBreathTime = System.currentTimeMillis();
            } else {
                isBreath = false;
            }
        }
    }

    public void reduceFireIfIdle() {
        long currentTime = System.currentTimeMillis();
        if (currentTime - lastBreathTime > BREATH_TIMEOUT) {
            if (currentFireState > 0) {
                currentFireState -= 50;
            }
            updateFireAnimation();
        }
    }

    public boolean isBreath() {
        return isBreath;
    }
}
