package com.example.its_magic.activities;//package com.example.its_magic.activities;
//
//import android.animation.ValueAnimator;
//import android.content.Context;
//import android.graphics.Canvas;
//import android.graphics.Paint;
//import android.graphics.Rect;
//import android.os.Bundle;
//import android.util.Log;
//import android.view.View;
//import android.view.animation.AccelerateInterpolator;
//import android.widget.Button;
//import android.widget.ImageView;
//import android.widget.LinearLayout;
//import android.widget.Toast;
//
//import androidx.annotation.NonNull;
//import androidx.annotation.Nullable;
//import androidx.appcompat.app.AppCompatActivity;
//
//import com.example.its_magic.R;
//
//import java.util.ArrayList;
//
//public class BagActivity extends AppCompatActivity {
//    ImageView disk, ground;
//    //    CollisionVisualizer collisionVisualizer;
//    Button button;
//    ArrayList<ImageView> newViews = new ArrayList<>();
//    int i = 0;
//    LinearLayout container;
//
//
//    @Override
//    protected void onCreate(@Nullable Bundle savedInstanceState) {
//        super.onCreate(savedInstanceState);
//        setContentView(R.layout.bag_scene_layout);
//
//        ground = findViewById(R.id.ground_view);
//        disk = findViewById(R.id.disk_view);
//        button = findViewById(R.id.buttonButton);
//        container = findViewById(R.id.container);
//
//        button.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View view) {
//                // Créer une nouvelle ImageView
//                ImageView imageView = new ImageView(BagActivity.this);
//
//                // Configurer les LayoutParams pour définir la taille
//                LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(200, 200);
//                imageView.setLayoutParams(params);
//
//                // Définir la position en utilisant des marges (si nécessaire)
//                params.setMargins(0, (int) (300F * i), 0, 0);
//
//                // Définir une image comme arrière-plan
//                imageView.setBackgroundResource(R.color.cyan); // Utilisez un drawable ici
//
//                // Ajouter l'ImageView à la liste et au conteneur
//                newViews.add(imageView);
//                container.addView(imageView);
//
//                // Incrémenter l'index
//                i++;
//            }
//        });
//
//
////        // Ajoutez la vue de visualisation des collisions
////        collisionVisualizer = new CollisionVisualizer(this);
//    }
//
////    @Override
////    protected void onStart() {
////        super.onStart();
////        startFallingAnimation();
////    }
//
////    private void startFallingAnimation() {
////        float startY = disk.getY();
////        float endY = ground.getY() - disk.getHeight();
////
////        Log.d("BagActivity", "start  : " + startY);
////
////        Log.d("BagActivity", "Y : " + endY);
////
////        ValueAnimator animator = ValueAnimator.ofFloat(startY, endY);
////        animator.setInterpolator(new AccelerateInterpolator());
////        animator.setDuration(2000);
////
////        animator.addUpdateListener(animation -> {
////            float currentY = (float) animation.getAnimatedValue();
////            disk.setY(currentY);
////
////            // Met à jour les rectangles pour la visualisation
////            collisionVisualizer.updateRects(disk, ground);
////
////            if (checkCollision(disk, ground)) {
////                animator.cancel();
////                Toast.makeText(BagActivity.this, "Collision detected!", Toast.LENGTH_SHORT).show();
////            }
////        });
////
////        animator.start();
////    }
////
////    private boolean checkCollision(View v1, View v2) {
////        Rect rect1 = new Rect();
////        v1.getHitRect(rect1);
////
////        Rect rect2 = new Rect();
////        v2.getHitRect(rect2);
////
////        return Rect.intersects(rect1, rect2);
////    }
////
////    // Classe pour dessiner les rectangles
////    private static class CollisionVisualizer extends View {
////        private final Paint paintDisk;
////        private final Paint paintGround;
////        private final Rect rectDisk = new Rect();
////        private final Rect rectGround = new Rect();
////
////        public CollisionVisualizer(Context context) {
////            super(context);
////            paintDisk = new Paint();
////            paintDisk.setColor(0x55FF0000); // Rouge transparent
////            paintDisk.setStyle(Paint.Style.FILL);
////
////            paintGround = new Paint();
////            paintGround.setColor(0x5500FF00); // Vert transparent
////            paintGround.setStyle(Paint.Style.FILL);
////        }
////
////        public void updateRects(View disk, View ground) {
////            disk.getHitRect(rectDisk);
////            ground.getHitRect(rectGround);
////            invalidate(); // Redessine la vue
////        }
////
////        @Override
////        protected void onDraw(@NonNull Canvas canvas) {
////            super.onDraw(canvas);
////
////            // Dessinez les rectangles
////            canvas.drawRect(rectDisk, paintDisk);
////            canvas.drawRect(rectGround, paintGround);
////        }
////    }
//}

import android.os.Bundle;
import android.util.Log;
import android.widget.RelativeLayout;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.its_magic.layouts.ItemBag;
import com.example.its_magic.layouts.PhysicsLayout;
import com.example.its_magic.R;
import com.example.its_magic.WebSocketManager;
import com.example.its_magic.utils.SetupHelper;

public class BagActivity extends AppCompatActivity {
    private static final String TAG = "BagActivity";
    private WebSocketManager webSocketManager;

    private PhysicsLayout physicsLayout;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.bag_scene_layout);

        try {
            SetupHelper.fullScreen(this);
            initializeViews();
            this.webSocketManager = WebSocketManager.getInstance(BagActivity.this);
            physicsLayout.restoreObjects();

        } catch (Exception e) {
            Log.e(TAG, "Error in onCreate", e);
            Toast.makeText(this, "Error initializing app", Toast.LENGTH_LONG).show();
        }
    }

    private void initializeViews() {
        physicsLayout = findViewById(R.id.physicsLayout);
    }

    public void addItemInBag(int objectId) {
        int x = (int) (Math.random() * 800);
        int y = 200;
        physicsLayout.addItemBag(new ItemBag(x, y, setImageBackground(objectId)));
    }

    private int setImageBackground(int id) {
        switch (id) {
            case 1:
                return R.drawable.mushroom;
            case 2:
                return R.drawable.acorn;
            default:
                return R.drawable.berry;
        }
    }
}

