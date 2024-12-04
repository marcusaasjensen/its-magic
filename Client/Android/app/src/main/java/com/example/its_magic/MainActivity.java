package com.example.its_magic;

import android.Manifest;
import android.content.Context;
import android.content.pm.PackageManager;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.media.AudioFormat;
import android.media.AudioRecord;
import android.media.MediaRecorder;
import android.os.Bundle;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;

public class MainActivity extends AppCompatActivity implements SensorEventListener {
    // Light Sensor
    private SensorManager sensorManager;
    private Sensor lightSensor;
    private TextView lightTextView;

    // Breath Sensor (Microphone)
    private static final int SAMPLE_RATE = 44100;
    private static final int BUFFER_SIZE = AudioRecord.getMinBufferSize(
            SAMPLE_RATE, AudioFormat.CHANNEL_IN_MONO, AudioFormat.ENCODING_PCM_16BIT);
    private AudioRecord audioRecord;
    private boolean isRecording = false;
    private TextView breathTextView;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main_layout);

        // Initialize UI elements
        lightTextView = findViewById(R.id.lightTextView);
        breathTextView = findViewById(R.id.breathTextView);

        // Initialize Light Sensor
        sensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
        lightSensor = sensorManager.getDefaultSensor(Sensor.TYPE_LIGHT);

        // Request microphone permissions
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.RECORD_AUDIO) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.RECORD_AUDIO}, 1);
        }
        // Initialize AudioRecord
        initAudioRecord();

        // Start listening for breath
        findViewById(R.id.startButton).setOnClickListener(v -> startBreathListening());
        findViewById(R.id.stopButton).setOnClickListener(v -> stopBreathListening());
    }

    @Override
    protected void onResume() {
        super.onResume();
        if (lightSensor != null) {
            sensorManager.registerListener(this, lightSensor, SensorManager.SENSOR_DELAY_NORMAL);
        }
    }

    @Override
    protected void onPause() {
        super.onPause();
        // Unregister Light Sensor Listener
        if (lightSensor != null) {
            sensorManager.unregisterListener(this);
        }
        stopBreathListening();
    }

    // Light Sensor Callback
    @Override
    public void onSensorChanged(SensorEvent event) {
        if (event.sensor.getType() == Sensor.TYPE_LIGHT) {
            float lightLevel = event.values[0];
            lightTextView.setText("Light Level: " + lightLevel + " lx");
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {
        // Not used
    }

    // Initialize AudioRecord
    private void initAudioRecord() {
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.RECORD_AUDIO) == PackageManager.PERMISSION_GRANTED) {
            audioRecord = new AudioRecord(MediaRecorder.AudioSource.MIC,
                    SAMPLE_RATE,
                    AudioFormat.CHANNEL_IN_MONO,
                    AudioFormat.ENCODING_PCM_16BIT,
                    BUFFER_SIZE);
        } else {
            audioRecord = null;
        }
    }


    // Start listening for breath
    private void startBreathListening() {
        if (audioRecord.getState() != AudioRecord.STATE_INITIALIZED) {
            Toast.makeText(this, "Microphone not available", Toast.LENGTH_SHORT).show();
            return;
        }

        isRecording = true;
        audioRecord.startRecording();
        breathTextView.setText("Listening for breath...");

        new Thread(() -> {
            short[] buffer = new short[BUFFER_SIZE];
            while (isRecording) {
                int read = audioRecord.read(buffer, 0, buffer.length);
                if (read > 0) {
                    float amplitude = calculateAmplitude(buffer, read);
                    runOnUiThread(() -> {
                        if (amplitude > 2000) { // Adjust this threshold for breath sensitivity
                            breathTextView.setText("Breath detected!");
                        } else {
                            breathTextView.setText("Listening for breath...");
                        }
                    });
                }
            }
        }).start();
    }

    // Stop listening for breath
    private void stopBreathListening() {
        isRecording = false;
        if (audioRecord != null) {
            audioRecord.stop();
        }
        breathTextView.setText("Breath detection stopped");
    }

    // Calculate average amplitude
    private float calculateAmplitude(short[] buffer, int read) {
        float sum = 0;
        for (int i = 0; i < read; i++) {
            sum += Math.abs(buffer[i]);
        }
        return sum / read;
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if (audioRecord != null) {
            audioRecord.release();
        }
    }
}
