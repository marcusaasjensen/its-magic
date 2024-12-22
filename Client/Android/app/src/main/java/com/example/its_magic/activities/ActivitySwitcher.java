package com.example.its_magic.activities;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;

import com.example.its_magic.R;

public class ActivitySwitcher {

    public static void switchActivity(Context context, Class<?> activityClass) {
        Intent intent = new Intent(context, activityClass);
        context.startActivity(intent);

        if (context instanceof Activity) {
            ((Activity) context).overridePendingTransition(
                    R.anim.fade_in_anim,
                    R.anim.fade_out_anim
            );
        }
    }

    public static void switchActivityWithExtras(Context context, Class<?> targetActivity, Intent extras) {
        Intent intent = new Intent(context, targetActivity);
        if (extras != null) {
            intent.putExtras(extras);
        }
        context.startActivity(intent);
    }
}
