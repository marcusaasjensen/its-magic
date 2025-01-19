package com.example.its_magic.messages;

import static com.example.its_magic.WebSocketManager.CLIENT_ID;
import static com.example.its_magic.WebSocketManager.RECIPIENT_ID;

import android.view.View;

import com.example.its_magic.R;
import com.example.its_magic.WebSocketManager;
import com.example.its_magic.utils.AnimationHelper;
import com.example.its_magic.utils.SoundHelper;

import java.util.List;

public class SendMessage {

    private static final List<String> sceneNames = List.of("HouseTopScene", "HouseSideScene");

    public static void starGame(WebSocketManager webSocketManager, SoundHelper soundHelper, View view) {
        AnimationHelper.startAnimation(view, R.anim.jelly_anim);
        soundHelper.playSFX(soundHelper.getStartGameSound(), 0.25f);
        for (int i = 0; i < 2; i++) {
            SceneMessage viewMessage = new SceneMessage(CLIENT_ID, RECIPIENT_ID.get(i), "StartGame", sceneNames.get(i));
            webSocketManager.sendDataToServer(viewMessage);
        }
    }

    public static void glowItemAnimate(WebSocketManager webSocketManager, SoundHelper soundHelper, View view, String targetObject) {
        AnimationHelper.startAnimation(view, R.anim.jelly_anim);
        soundHelper.playSFX(soundHelper.getTapSound(), 0.25f);
        ObjectMessage message = new ObjectMessage(CLIENT_ID, RECIPIENT_ID.get(0), "Glow", targetObject);
        webSocketManager.sendDataToServer(message);
    }

    public static void glowItem(WebSocketManager webSocketManager, SoundHelper soundHelper, String targetObject) {
        soundHelper.playSFX(soundHelper.getTapSound(), 0.25f);
        ObjectMessage message = new ObjectMessage(CLIENT_ID, RECIPIENT_ID.get(0), "Glow", targetObject);
        webSocketManager.sendDataToServer(message);
    }

    public static void breath(WebSocketManager webSocketManager, float breathIntensity) {
        BreathMessage message = new BreathMessage(CLIENT_ID, RECIPIENT_ID.get(0), "Wind", breathIntensity);
        webSocketManager.sendDataToServer(message);
    }

    public static void fire(WebSocketManager webSocketManager, float fireIntensity) {
        FireMessage message = new FireMessage(CLIENT_ID, RECIPIENT_ID.get(0), "Fire", fireIntensity);
        webSocketManager.sendDataToServer(message);
    }


}
