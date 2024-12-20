package com.example.its_magic.fireflies;

import android.content.Context;
import android.graphics.BlurMaskFilter;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.util.AttributeSet;
import android.view.View;

import androidx.annotation.NonNull;

import java.util.ArrayList;
import java.util.Random;

public class FireflyView extends View {

    private static final int NUM_FIREFLIES = 20;
    private final ArrayList<Firefly> fireflies = new ArrayList<>();
    private final Random random = new Random();
    private final Paint paint = new Paint();
    private boolean initialized = false;

    public FireflyView(Context context, AttributeSet attrs) {
        super(context, attrs);
        init();
    }

    private void init() {
        paint.setColor(Color.GREEN);
        paint.setStyle(Paint.Style.FILL);
        paint.setMaskFilter(new BlurMaskFilter(20, BlurMaskFilter.Blur.NORMAL));

        setLayerType(LAYER_TYPE_SOFTWARE, null);
    }

    @Override
    protected void onSizeChanged(int w, int h, int oldw, int oldh) {
        super.onSizeChanged(w, h, oldw, oldh);

        if (!initialized) {
            for (int i = 0; i < NUM_FIREFLIES; i++) {
                fireflies.add(new Firefly(w, h));
            }
            initialized = true;
        }
    }

    @Override
    protected void onDraw(@NonNull Canvas canvas) {
        super.onDraw(canvas);
        for (Firefly firefly : fireflies) {
            paint.setAlpha(firefly.alpha);
            canvas.drawCircle(firefly.x, firefly.y, firefly.size, paint);
            firefly.update(getWidth(), getHeight());
        }
        invalidate();
    }

    private class Firefly {
        float x, y, size;
        float dx, dy;
        int alpha;

        Firefly(int width, int height) {
            reset(width, height);
        }

        void reset(int width, int height) {
            if (width <= 0 || height <= 0) return;

            x = random.nextInt(width);
            y = random.nextInt(height);
            size = random.nextFloat() * 10 + 10;
            dx = random.nextFloat() * 4 - 2;
            dy = random.nextFloat() * 4 - 2;
            alpha = random.nextInt(156) + 100;
        }

        void update(int width, int height) {
            x += dx;
            y += dy;

            // Si sort de l'écran, réinitialiser
            if (x < 0 || x > width || y < 0 || y > height) {
                reset(width, height);
            }
        }
    }
}
