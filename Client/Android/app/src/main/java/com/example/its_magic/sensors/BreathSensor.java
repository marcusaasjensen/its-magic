package com.example.its_magic.sensors;

import android.content.Context;
import android.content.pm.PackageManager;
import android.media.AudioFormat;
import android.media.AudioRecord;
import android.media.MediaRecorder;
import android.Manifest;
import android.util.Log;

import androidx.core.app.ActivityCompat;

public class BreathSensor extends BaseSensor {
    private static final String TAG = "BreathSensor";
    private static final int SAMPLE_RATE = 44100;
    private static final int BUFFER_SIZE = AudioRecord.getMinBufferSize(
            SAMPLE_RATE, AudioFormat.CHANNEL_IN_MONO, AudioFormat.ENCODING_PCM_16BIT);

    private AudioRecord audioRecord;
    private boolean isRecording = false;
    private Thread recordingThread;

    public BreathSensor(Context context, SensorCallback callback) {
        super(context, callback);
        initAudioRecord();
    }

    @Override
    protected String getSensorType() {
        return "breath";
    }

    private void initAudioRecord() {
        if (ActivityCompat.checkSelfPermission(context, Manifest.permission.RECORD_AUDIO)
                == PackageManager.PERMISSION_GRANTED) {
            audioRecord = new AudioRecord(MediaRecorder.AudioSource.MIC,
                    SAMPLE_RATE,
                    AudioFormat.CHANNEL_IN_MONO,
                    AudioFormat.ENCODING_PCM_16BIT,
                    BUFFER_SIZE);
        }
    }

    @Override
    public void start() {
        if (audioRecord == null || audioRecord.getState() != AudioRecord.STATE_INITIALIZED) {
            Log.e(TAG, "Microphone not available");
            callback.onError("breath", "Microphone not available");
            return;
        }

        try {
            isRecording = true;
            audioRecord.startRecording();

            recordingThread = new Thread(() -> {
                android.os.Process.setThreadPriority(android.os.Process.THREAD_PRIORITY_AUDIO);
                short[] buffer = new short[BUFFER_SIZE];

                while (isRecording && !Thread.interrupted()) {
                    try {
                        int read = audioRecord.read(buffer, 0, buffer.length);
                        if (read > 0) {
                            float amplitude = calculateAmplitude(buffer, read);
                            if (amplitude > 2000) {
                                onBreathDetected(amplitude);
                            }
                        }
                    } catch (Exception e) {
                        Log.e(TAG, "Error reading audio data", e);
                        break;
                    }
                }
            }, "AudioRecordingThread");

            recordingThread.start();
        } catch (Exception e) {
            Log.e(TAG, "Error starting audio recording", e);
            isRecording = false;
        }
    }

    private void onBreathDetected(float amplitude) {
        String value = String.format("%.1f", amplitude);
        callback.onValueChanged("breath", "Breath detected!");
        sendToServer(value);
    }

    @Override
    public void stop() {
        Log.d(TAG, "Stopping BreathSensor");
        isRecording = false;
        if (recordingThread != null) {
            recordingThread.interrupt();
            recordingThread = null;
        }
        if (audioRecord != null) {
            try {
                audioRecord.stop();
            } catch (Exception e) {
                Log.e(TAG, "Error stopping audio record", e);
            }
        }
    }

    @Override
    public void cleanup() {
        Log.d(TAG, "Cleaning up BreathSensor");
        stop();
        if (audioRecord != null) {
            try {
                audioRecord.release();
                audioRecord = null;
            } catch (Exception e) {
                Log.e(TAG, "Error releasing audio record", e);
            }
        }
    }

    private float calculateAmplitude(short[] buffer, int read) {
        float sum = 0;
        for (int i = 0; i < read; i++) {
            sum += Math.abs(buffer[i]);
        }
        return sum / read;
    }
}