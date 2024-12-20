package com.example.its_magic.activities;

import android.content.Context;
import android.content.Intent;

public class ActivitySwitcher {

    public static void switchActivity(Context context, Class<?> activityClass) {
        Intent intent = new Intent(context, activityClass);

        if (!(context instanceof android.app.Activity)) {
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        }

        context.startActivity(intent);
    }

    public static void switchActivityWithExtras(Context context, Class<?> targetActivity, Intent extras) {
        Intent intent = new Intent(context, targetActivity);
        if (extras != null) {
            intent.putExtras(extras);
        }
        context.startActivity(intent);
    }
}
