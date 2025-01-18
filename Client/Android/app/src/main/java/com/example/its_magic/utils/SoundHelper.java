package com.example.its_magic.utils;

import android.content.Context;
import android.media.AudioAttributes;
import android.media.MediaPlayer;
import android.media.SoundPool;

import java.util.HashMap;

public class SoundHelper {
    private SoundPool soundPool;
    private final HashMap<String, Integer> soundMap;
    private final HashMap<String, Integer> playingLoops;
    private MediaPlayer mediaPlayer;
    private boolean isLoaded = false;


    public SoundHelper() {
        AudioAttributes audioAttributes = new AudioAttributes.Builder()
                .setUsage(AudioAttributes.USAGE_GAME)
                .setContentType(AudioAttributes.CONTENT_TYPE_SONIFICATION)
                .build();

        soundPool = new SoundPool.Builder()
                .setMaxStreams(10)
                .setAudioAttributes(audioAttributes)
                .build();

        soundMap = new HashMap<>();
        playingLoops = new HashMap<>();

        soundPool.setOnLoadCompleteListener((soundPool, sampleId, status) -> {
            if (status == 0) {
                isLoaded = true;
            }
        });
    }

    public String getTapSound() {
        return "tapObject";
    }

    public String getStartGameSound() {
        return "playGame";
    }

    /**
     * Charge un son court (SFX) à partir des ressources brutes (raw).
     */
    public void loadSound(Context context, String soundKey, int resId) {
        int soundId = soundPool.load(context, resId, 1);
        soundMap.put(soundKey, soundId);
    }

    /**
     * Joue un effet sonore ponctuel (SFX).
     */
    public void playSFX(String soundKey, float volume) {
        if (soundPool != null && isLoaded && soundMap.containsKey(soundKey)) {
            int soundId = soundMap.get(soundKey);
            soundPool.play(soundId, volume, volume, 1, 0, 1.0f);
        }
    }

    /**
     * Joue un son d'ambiance en boucle avec MediaPlayer.
     */
    public void playAmbiance(Context context, int resId) {
        stopAmbiance();
        mediaPlayer = MediaPlayer.create(context, resId);
        if (mediaPlayer != null) {
            mediaPlayer.setLooping(true);
            mediaPlayer.setVolume(0.25f, 0.25f);
            mediaPlayer.start();
        }
    }

    /**
     * Arrête la musique d'ambiance.
     */
    public void stopAmbiance() {
        if (mediaPlayer != null) {
            mediaPlayer.stop();
            mediaPlayer.release();
            mediaPlayer = null;
        }
    }

    /**
     * Arrête un effet sonore en boucle.
     */
    public void stopLoopingSFX(String soundKey) {
        if (soundPool != null && playingLoops.containsKey(soundKey)) {
            int streamId = playingLoops.get(soundKey);
            soundPool.stop(streamId);
            playingLoops.remove(soundKey);
        }
    }

    /**
     * Joue un effet sonore en boucle.
     */
    public void playLoopingSFX(String soundKey, float volume) {
        if (soundPool != null && isLoaded && soundMap.containsKey(soundKey)) {
            int soundId = soundMap.get(soundKey);
            int streamId = soundPool.play(soundId, volume, volume, 1, -1, 1.0f);
            playingLoops.put(soundKey, streamId);
        }
    }

    /**
     * Libère toutes les ressources utilisées.
     */
    public void release() {
        if (soundPool != null) {
            soundPool.release();
            soundPool = null;
            soundMap.clear();
            playingLoops.clear();
        }

        if (mediaPlayer != null) {
            mediaPlayer.release();
            mediaPlayer = null;
        }
    }
}
