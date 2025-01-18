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

import com.example.its_magic.messages.SendMessage;

public class BreathSensor extends BaseSensor {
    private static final String TAG = "BreathSensor";
    public static final int REQUEST_RECORD_AUDIO_PERMISSION = 200;
    private static final int SAMPLE_RATE = 44100;
    private static final int BUFFER_SIZE = AudioRecord.getMinBufferSize(SAMPLE_RATE, AudioFormat.CHANNEL_IN_MONO, AudioFormat.ENCODING_PCM_16BIT);
    private static final long BREATH_TIMEOUT = 5000;

    private static final double MAX_BREATH_INTENSITY = 10000.0;
    private AudioRecord audioRecord;
    private boolean isRecording = false;
    private final short[] audioBuffer = new short[BUFFER_SIZE];
    private int currentFireState = 0;
    private long lastBreathTime;
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

            double normalizedBreathIntensity = Math.min(amplitude / MAX_BREATH_INTENSITY, 1.0);
            double normalizedFireIntensity = Math.min((double) currentFireState / 900, 1.0);

            long currentTime = System.currentTimeMillis();

            if (amplitude > 2000 && (currentTime - lastBreathTime > 1000)) {
                Log.d(TAG, "Breath detected with normalized intensity: " + normalizedBreathIntensity);

                if (currentFireState < 900) {
                    currentFireState += 100;
                }
                SendMessage.fireWind(webSocketManager, (float) normalizedFireIntensity, (float) normalizedBreathIntensity);
                updateFireImage(currentFireState);
                lastBreathTime = currentTime;
            }
        }
    }
    public void reduceFireIfIdle() {
        long currentTime = System.currentTimeMillis();
        if (currentTime - lastBreathTime > BREATH_TIMEOUT) {
            if (currentFireState > 0) {
                currentFireState -= 50;
                if (currentFireState < 0) {
                    currentFireState = 0;
                }
            } else if (currentFireState >= 900) {
                currentFireState = 900;
            }
            updateFireImage(currentFireState);
        }
    }

    private void updateFireImage(int fireValue) {
        if (fireValue >= 900) {
            callback.onValueChanged("fire", "Large fire");
        } else if (fireValue >= 600) {
            callback.onValueChanged("fire", "Medium fire");
        } else if (fireValue >= 300) {
            callback.onValueChanged("fire", "Small fire");
        } else {
            callback.onValueChanged("fire", "Fire off");
        }
    }

    public boolean isBreath() {
        return isBreath;
    }
}
