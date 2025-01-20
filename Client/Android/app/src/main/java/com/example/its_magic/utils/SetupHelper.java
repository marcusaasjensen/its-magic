package com.example.its_magic.utils;

import android.app.Activity;
import android.view.WindowInsets;
import android.view.WindowInsetsController;

public class SetupHelper {

    public static void fullScreen(Activity activity) {
        activity.getWindow().setDecorFitsSystemWindows(false);
        WindowInsetsController controller = activity.getWindow().getInsetsController();
        if (controller != null) {
            controller.hide(WindowInsets.Type.statusBars() | WindowInsets.Type.navigationBars());
            controller.setSystemBarsBehavior(WindowInsetsController.BEHAVIOR_SHOW_TRANSIENT_BARS_BY_SWIPE);
        }
    }
}
