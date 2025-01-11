package com.example.its_magic.layouts;

import static com.example.its_magic.WebSocketManager.CLIENT_ID;
import static com.example.its_magic.WebSocketManager.RECIPIENT_ID;

import android.content.Context;
import android.content.SharedPreferences;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.Rect;
import android.os.Handler;
import android.util.AttributeSet;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.widget.FrameLayout;
import android.widget.ImageView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.core.content.ContextCompat;

import com.example.its_magic.R;
import com.example.its_magic.WebSocketManager;
import com.example.its_magic.messages.ObjectMessage;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

public class PhysicsLayout extends FrameLayout {
    private List<ItemBag> itemBagList;
    private Rect deleteZone;
    private Rect ground;
    private Paint paint;
    private final int screenWidth;
    private final Handler handler = new Handler();
    private final int gravity = 10;
    private WebSocketManager webSocketManager;


    public PhysicsLayout(Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
        init();
        this.webSocketManager = WebSocketManager.getInstance(context);
        screenWidth = context.getResources().getDisplayMetrics().widthPixels;
    }

    public PhysicsLayout(Context context) {
        super(context);
        init();
        this.webSocketManager = WebSocketManager.getInstance(context);
        screenWidth = context.getResources().getDisplayMetrics().widthPixels;
    }

    private void init() {
        itemBagList = new ArrayList<>();
        deleteZone = new Rect();
        ground = new Rect();
        paint = new Paint();
        paint.setStyle(Paint.Style.FILL);
    }

    public void addItemBag(ItemBag itemBag) {
        ImageView imageView = new ImageView(getContext());
        imageView.setImageResource(itemBag.getImageRes());
        imageView.setLayoutParams(new LayoutParams(200, 200));
        imageView.setX(itemBag.getX());
        imageView.setY(itemBag.getY());
        imageView.setTag(itemBag.getId());

        imageView.setOnTouchListener(new DragTouchListener());

        saveObjectPosition(itemBag, itemBag.getX(), itemBag.getY());
        ItemBag newItemBag = new ItemBag(itemBag.getId(), itemBag.getX(), itemBag.getY(), itemBag.getImageRes(), imageView);

        itemBagList.add(newItemBag);
        addView(imageView);
        startFalling(imageView);
    }

    private void removeItemBag(View view) {
        int itemBagId = (int) view.getTag();
        sendItemToServer(findItemBagById(itemBagId));
        for (ItemBag itemBag : itemBagList) {
            if (itemBag.getId() == itemBagId) {
                itemBagList.remove(itemBag);
                break;
            }
        }
        removeView(view);
    }

    private void startFalling(View view) {
        handler.post(new Runnable() {
            @Override
            public void run() {
                if (view.getY() + view.getHeight() <= ground.top && !isInCollision(view)) {
                    view.setY(view.getY() + gravity);
                    handler.postDelayed(this, 16);
                }
            }
        });
    }

    @Override
    protected void onLayout(boolean changed, int left, int top, int right, int bottom) {
        super.onLayout(changed, left, top, right, bottom);

        deleteZone.set(0, 0, getWidth(), 200);

        int groundTop = getHeight() - 200;
        ground.set(0, groundTop, getWidth(), getHeight());
    }

    private boolean isInCollision(View view) {
        if (view.getY() + view.getHeight() >= ground.top) {
            view.setY(ground.top - view.getHeight());

            Log.d("Physics Collision", "Collision with ground detected for object : " + view.getTag());
            saveObjectPosition(findItemBagById((int) view.getTag()), (int) view.getX(), (int) view.getY());
            return true;
        }

        if (isInDeleteZone(view)) {
            Log.d("Physics Collision", "Object: " + view.getTag() + " is in the delete zone.");
            removeObjectFromStore(findItemBagById((int) view.getTag()));
            removeView(view);
            removeItemBag(view);
            return true;
        }

        for (ItemBag obj : itemBagList) {
            if (obj.getImageView() != view && isColliding(view, obj.getImageView())) {
                Log.d("Physics Collision", "Object collision detected " + view.getTag() + " and Object " + obj.getImageView().getTag());
                saveObjectPosition(findItemBagById((int) view.getTag()), (int) view.getX(), (int) view.getY());
                return true;
            }
        }

        return false;
    }

    private boolean isColliding(View view1, View view2) {
        int[] location1 = new int[2];
        int[] location2 = new int[2];
        view1.getLocationInWindow(location1);
        view2.getLocationInWindow(location2);

        Rect rect1 = new Rect(location1[0], location1[1], location1[0] + view1.getWidth(), location1[1] + view1.getHeight());
        Rect rect2 = new Rect(location2[0], location2[1], location2[0] + view2.getWidth(), location2[1] + view2.getHeight());

        return Rect.intersects(rect1, rect2);
    }

    private boolean isInDeleteZone(View view) {
        int[] location = new int[2];
        view.getLocationOnScreen(location);

        int viewX = location[0];
        int viewY = location[1];

        return deleteZone.contains(
                viewX + view.getWidth() / 2,
                viewY + view.getHeight() / 2
        );
    }

    private void saveObjectPosition(ItemBag itemBag, int newX, int newY) {
        itemBag.setX(newX);
        itemBag.setY(newY);
        SharedPreferences preferences = getContext().getSharedPreferences("PhysicsLayoutPrefs", Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = preferences.edit();

        String objectKey = "object_" + itemBag.getId();

        editor.putInt(objectKey + "_x", itemBag.getX());
        editor.putInt(objectKey + "_y", itemBag.getY());
        editor.putInt(objectKey + "_imageRes", itemBag.getImageRes());
        editor.apply();

        Log.d("PhysicsLayout", "Saving the object : ID=" + itemBag.getId() +
                " X=" + itemBag.getX() +
                " Y=" + itemBag.getY() +
                " ImageRes=" + itemBag.getImageRes());
    }

    private void removeObjectFromStore(ItemBag itemBag) {
        SharedPreferences preferences = getContext().getSharedPreferences("PhysicsLayoutPrefs", Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = preferences.edit();

        String objectKey = "object_" + itemBag.getId();

        editor.remove(objectKey + "_x");
        editor.remove(objectKey + "_y");
        editor.remove(objectKey + "_imageRes");
        editor.apply();

        // Log pour vérifier la suppression
        Log.d("PhysicsLayout", "Object deletion : ID=" + itemBag.getId());
    }

    public void restoreObjects() {
        SharedPreferences preferences = getContext().getSharedPreferences("PhysicsLayoutPrefs", Context.MODE_PRIVATE);
        Map<String, ?> allPrefs = preferences.getAll();

        Log.d("PhysicsLayout", "Start object restoration...");
        for (Map.Entry<String, ?> entry : allPrefs.entrySet()) {
            String key = entry.getKey();

            if (key.startsWith("object_") && key.contains("_x")) {
                String objectId = key.substring(7, key.lastIndexOf("_x"));
                int x = preferences.getInt("object_" + objectId + "_x", -1);
                int y = preferences.getInt("object_" + objectId + "_y", -1);
                int imageRes = preferences.getInt("object_" + objectId + "_imageRes", 0);

                if (x != -1 && y != -1 && imageRes != 0) {
                    Log.d("PhysicsLayout", "Object restoration : ID=" + objectId +
                            " X=" + x +
                            " Y=" + y +
                            " ImageRes=" + imageRes);

                    ItemBag restoredItemBag = new ItemBag(Integer.parseInt(objectId), x, y, imageRes);
                    addItemBag(restoredItemBag);

                    Log.d("PhysicsLayout", "Object added to layout : ID=" + objectId);

                    startFalling(getChildAt(getChildCount() - 1));
                } else {
                    Log.w("PhysicsLayout", "Invalid data for object : ID=" + objectId);
                }
            }
        }
        Log.d("PhysicsLayout", "Restoration complete.");
    }


    private class DragTouchListener implements OnTouchListener {
        private float downX, downY;
        private float dX, dY;

        @Override
        public boolean onTouch(View view, MotionEvent event) {
            switch (event.getAction()) {
                case MotionEvent.ACTION_DOWN:
                    downX = event.getX();
                    downY = event.getY();
                    dX = view.getX() - event.getRawX();
                    dY = view.getY() - event.getRawY();
                    return true;

                case MotionEvent.ACTION_MOVE:
                    float newX = event.getRawX() + dX;
                    float newY = event.getRawY() + dY;

                    newX = Math.max(0, Math.min(newX, screenWidth - view.getWidth()));
                    newY = Math.max(0, Math.min(newY, ground.top - view.getHeight()));

                    view.setX(newX);
                    view.setY(newY);

                    for (ItemBag obj : itemBagList) {
                        if (obj.getImageView() != view && isColliding(view, obj.getImageView())) {
                            applyPush(view, obj.getImageView());
                            startFalling(obj.getImageView());
                        }
                    }
                    saveObjectPosition(findItemBagById((int) view.getTag()), (int) view.getX(), (int) view.getY());
                    return true;

                case MotionEvent.ACTION_UP:
                    if (!isInCollision(view)) {
                        startFalling(view);
                    }
                    return true;
            }
            return false;
        }
    }

    public ItemBag findItemBagById(int id) {
        for (ItemBag itemBag : itemBagList) {
            if (itemBag.getId() == id) {
                return itemBag;
            }
        }
        return null;
    }

    private void applyPush(View movingObj, View collidingObj) {
        float deltaX = movingObj.getX() - collidingObj.getX();
        float deltaY = movingObj.getY() - collidingObj.getY();
        float distance = (float) Math.sqrt(deltaX * deltaX + deltaY * deltaY);

        if (distance > 0) {
            int pushForce = 50;
            float pushX = (deltaX / distance) * pushForce;
            float pushY = (deltaY / distance) * pushForce;

            float newCollidingX = collidingObj.getX() - pushX;
            float newCollidingY = collidingObj.getY() - pushY;

            newCollidingX = Math.max(0, Math.min(newCollidingX, screenWidth - collidingObj.getWidth()));
            newCollidingY = Math.max(0, Math.min(newCollidingY, ground.top - collidingObj.getHeight()));

            collidingObj.setX(newCollidingX);
            collidingObj.setY(newCollidingY);
        }
    }

    @Override
    protected void onDraw(@NonNull Canvas canvas) {
        super.onDraw(canvas);
        paint.setColor(ContextCompat.getColor(getContext(), R.color.outsideBag));
        canvas.drawRect(deleteZone, paint);

        paint.setColor(ContextCompat.getColor(getContext(), R.color.bottomBag));
        canvas.drawRect(ground, paint);
    }

    private void sendItemToServer(ItemBag itemBag) {
        ObjectMessage message = null;
        int imageRes = itemBag.getImageRes();

        if (imageRes == R.drawable.mushroom) {
            message = new ObjectMessage(CLIENT_ID, RECIPIENT_ID.get(0), "showItem", "mushroom");
        } else if (imageRes == R.drawable.acorn) {
            message = new ObjectMessage(CLIENT_ID, RECIPIENT_ID.get(0), "showItem", "acorn");
        } else if (imageRes == R.drawable.berry) {
            message = new ObjectMessage(CLIENT_ID, RECIPIENT_ID.get(0), "showItem", "berry");
        }

        if (message != null) {
            webSocketManager.sendDataToServer(message);
        }
    }

}
