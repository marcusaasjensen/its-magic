package com.example.its_magic.utils;

import android.view.View;
import android.view.animation.AnimationUtils;

public class AnimationHelper {

    public static void startAnimation(View view, int animation) {
        android.view.animation.Animation jellyAnimation = AnimationUtils.loadAnimation(view.getContext(), animation);
        view.startAnimation(jellyAnimation);
    }
}
