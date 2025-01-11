package com.example.its_magic.layouts;

import android.content.Context;
import android.content.res.TypedArray;
import android.graphics.BlurMaskFilter;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.util.AttributeSet;
import android.util.TypedValue;
import android.view.View;

import androidx.annotation.NonNull;

import com.example.its_magic.R;

import java.util.ArrayList;
import java.util.Random;

public class FireflyView extends View {
    private static final int DEFAULT_MIN_SIZE = 5;
    private static final int DEFAULT_MAX_SIZE = 20;
    private static final int DEFAULT_COLOR = Color.GREEN;

    private int fireflyColor = DEFAULT_COLOR;
    private float minSize;
    private float maxSize;
    private float minSpeed = 1f;
    private float maxSpeed = 4f;

    private static final int NUM_FIREFLIES = 20;
    private final ArrayList<Firefly> fireflies = new ArrayList<>();
    private final Random random = new Random();
    private final Paint paint = new Paint();
    private boolean initialized = false;


    public FireflyView(Context context, AttributeSet attrs) {
        super(context, attrs);
        init(attrs);
    }

    private void init(AttributeSet attrs) {
        TypedArray a = getContext().obtainStyledAttributes(attrs, R.styleable.FireflyView);

        fireflyColor = a.getColor(R.styleable.FireflyView_fireflyColor, DEFAULT_COLOR);
        minSize = a.getDimension(R.styleable.FireflyView_fireflyMinSize,
                TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, DEFAULT_MIN_SIZE, getResources().getDisplayMetrics()));
        maxSize = a.getDimension(R.styleable.FireflyView_fireflyMaxSize,
                TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, DEFAULT_MAX_SIZE, getResources().getDisplayMetrics()));
        minSpeed = a.getFloat(R.styleable.FireflyView_fireflyMinSpeed, 1f);
        maxSpeed = a.getFloat(R.styleable.FireflyView_fireflyMaxSpeed, 4f);

        int blurTypeValue = a.getInt(R.styleable.FireflyView_fireflyBlurType, 0);
        BlurMaskFilter.Blur blurType = convertToBlurType(blurTypeValue);

        a.recycle();

        paint.setStyle(Paint.Style.FILL);
        setBlur(blurType);
        setLayerType(LAYER_TYPE_SOFTWARE, null);
    }



    @Override
    protected void onSizeChanged(int width, int height, int oldWidth, int oldHeight) {
        super.onSizeChanged(width, height, oldWidth, oldHeight);

        if (!initialized) {
            for (int i = 0; i < NUM_FIREFLIES; i++) {
                fireflies.add(new Firefly(width, height));
            }
            initialized = true;
        }
    }

    @Override
    protected void onDraw(@NonNull Canvas canvas) {
        super.onDraw(canvas);
        for (Firefly firefly : fireflies) {
            paint.setColor(firefly.color);
            paint.setAlpha(firefly.alpha);
            canvas.drawCircle(firefly.x, firefly.y, firefly.size, paint);
            firefly.update(getWidth(), getHeight());
        }
        invalidate();
    }

    private void setBlur(BlurMaskFilter.Blur blur) {
        paint.setMaskFilter(new BlurMaskFilter(20f, blur));
    }

    private BlurMaskFilter.Blur convertToBlurType(int value) {
        switch (value) {
            case 1: return BlurMaskFilter.Blur.SOLID;
            case 2: return BlurMaskFilter.Blur.OUTER;
            case 3: return BlurMaskFilter.Blur.INNER;
            default: return BlurMaskFilter.Blur.NORMAL;
        }
    }


    private class Firefly {
        float x, y, size;
        float dx, dy;
        int alpha;
        int color;

        Firefly(int width, int height) {
            reset(width, height);
        }

        void reset(int width, int height) {
            if (width <= 0 || height <= 0) return;

            x = random.nextInt(width);
            y = random.nextInt(height);
            size = random.nextFloat() * (maxSize - minSize) + minSize;
            dx = random.nextFloat() * (maxSpeed - minSpeed) + minSpeed * (random.nextBoolean() ? 1 : -1);
            dy = random.nextFloat() * (maxSpeed - minSpeed) + minSpeed * (random.nextBoolean() ? 1 : -1);
            alpha = random.nextInt(156) + 100;
            color = fireflyColor;
        }

        void update(int width, int height) {
            x += dx;
            y += dy;

            if (x < 0 || x > width || y < 0 || y > height) {
                reset(width, height);
            }
        }
    }
}
