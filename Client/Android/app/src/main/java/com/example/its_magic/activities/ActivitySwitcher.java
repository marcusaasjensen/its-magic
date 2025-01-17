package com.example.its_magic.activities;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

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

    public static void switchActivityWithExtras(Context context, Class<?> targetActivity, int objectId) {
        Intent intent = new Intent(context, targetActivity);
        intent.putExtra("objectId", objectId);

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
