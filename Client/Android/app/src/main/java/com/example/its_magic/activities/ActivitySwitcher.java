package com.example.its_magic.activities;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;

import com.example.its_magic.R;

public class ActivitySwitcher {

    public static void switchActivity(Context context, Class<?> activityClass) {
        Intent intent = new Intent(context, activityClass);
        if (!(context instanceof Activity)) {
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        }


        if (context instanceof Activity) {
            ((Activity) context).overridePendingTransition(
                    R.anim.fade_in_anim,
                    R.anim.fade_out_anim
            );
        }
        context.startActivity(intent);

    }

    public static void switchBagActivityWithExtras(Context context, Class<?> targetActivity, int objectId, String scene) {
        Intent intent = new Intent(context, targetActivity);
        intent.putExtra("objectId", objectId);
        intent.putExtra("sceneName", scene);

        if (!(context instanceof Activity)) {
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        }
        if (context instanceof Activity) {
            ((Activity) context).overridePendingTransition(
                    R.anim.fade_in_anim,
                    R.anim.fade_out_anim
            );
        }
        context.startActivity(intent);
    }

    public static void switchBreathActivityWithExtras(Context context, Class<?> targetActivity, boolean canBlow) {
        Intent intent = new Intent(context, targetActivity);
        intent.putExtra("canBlow", canBlow);

        if (!(context instanceof Activity)) {
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        }
        if (context instanceof Activity) {
            ((Activity) context).overridePendingTransition(
                    R.anim.fade_in_anim,
                    R.anim.fade_out_anim
            );
        }
        context.startActivity(intent);
    }
}
