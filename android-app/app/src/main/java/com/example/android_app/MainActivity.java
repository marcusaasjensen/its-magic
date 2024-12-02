package com.example.android_app;

import android.content.ContentResolver;
import android.content.Context;
import android.database.ContentObserver;
import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.provider.Settings;
import android.util.Log;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.TextView;
import androidx.appcompat.app.AppCompatActivity;

import java.net.URI;
import java.net.URISyntaxException;

import org.java_websocket.client.WebSocketClient;
import org.java_websocket.handshake.ServerHandshake;

public class MainActivity extends AppCompatActivity {

    private static final String TAG = "BrightnessApp";
    private TextView brightnessStatusTextView;
    private LinearLayout brightnessGauge;
    private Handler handler;
    private ContentResolver contentResolver;
    private ContentObserver brightnessObserver;
    private WebSocketClient webSocketClient;

    private static final String WEBSOCKET_URI = "ws://192.168.130.31:8080"; // Change to your server's IP and port

    // Device-specific gamma for more accurate brightness perception
    private double deviceSpecificGamma = 1.8;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        brightnessStatusTextView = findViewById(R.id.brightnessStatusTextView);
        brightnessGauge = findViewById(R.id.brightnessGauge);
        contentResolver = getContentResolver();
        handler = new Handler(Looper.getMainLooper());

        // Initialize WebSocket connection
        initWebSocket();

        // Start observing brightness
        observeBrightness();
    }

    private void initWebSocket() {
        try {
            URI serverUri = new URI(WEBSOCKET_URI);

            webSocketClient = new WebSocketClient(serverUri) {
                @Override
                public void onOpen(ServerHandshake handshakedata) {
                    Log.d(TAG, "WebSocket connection opened");
                }

                @Override
                public void onMessage(String message) {
                    Log.d(TAG, "Received message from server: " + message);
                }

                @Override
                public void onClose(int code, String reason, boolean remote) {
                    Log.d(TAG, "WebSocket connection closed: " + reason);
                }

                @Override
                public void onError(Exception ex) {
                    Log.e(TAG, "WebSocket Error", ex);
                }
            };

            webSocketClient.connect();
        } catch (URISyntaxException e) {
            Log.e(TAG, "Invalid WebSocket URI", e);
        }
    }

    private void observeBrightness() {
        brightnessObserver = new ContentObserver(handler) {
            @Override
            public void onChange(boolean selfChange) {
                super.onChange(selfChange);
                updateBrightnessStatus();
            }
        };

        contentResolver.registerContentObserver(Settings.System.getUriFor(Settings.System.SCREEN_BRIGHTNESS), false, brightnessObserver);
        updateBrightnessStatus();
    }

    private int convertBrightnessToPercentage(int systemBrightness) {
        double normalizedBrightness = (systemBrightness - 1.0) / (255.0 - 1.0);
        double gammaCorrectedBrightness = Math.pow(normalizedBrightness, 1.0 / deviceSpecificGamma);
        int percentage = (int) Math.round(gammaCorrectedBrightness * 100.0);
        return Math.max(0, Math.min(100, percentage));
    }

    private void sendBrightnessAlert(int brightness) {
        if (webSocketClient != null && webSocketClient.isOpen()) {
            try {
                String message = "Brightness is at " + brightness + "%";
                webSocketClient.send(message);
                Log.d(TAG, "Brightness alert sent: " + message);
            } catch (Exception e) {
                Log.e(TAG, "Error sending alert", e);
            }
        } else {
            Log.w(TAG, "WebSocket is not open. Cannot send message.");
        }
    }

    private void updateBrightnessStatus() {
        try {
            int brightness = Settings.System.getInt(contentResolver, Settings.System.SCREEN_BRIGHTNESS);
            int brightnessPercentage = convertBrightnessToPercentage(brightness);

            if (brightnessPercentage == 0 || brightnessPercentage == 100) {
                Log.d(TAG, "Extreme brightness detected: " + brightnessPercentage + "%");
                sendBrightnessAlert(brightnessPercentage);
            }

            brightnessStatusTextView.setText("Current Brightness: " + brightnessPercentage + "%");
            updateBrightnessGauge(brightnessPercentage);
        } catch (Settings.SettingNotFoundException e) {
            Log.e(TAG, "Error fetching brightness", e);
        }
    }

    private void updateBrightnessGauge(int brightnessPercentage) {
        brightnessGauge.removeAllViews();
        int numRectangles = 10;
        int numFilledRectangles = (int) (brightnessPercentage / 10.0);

        for (int i = numRectangles - 1; i >= 0; i--) {
            View rectangle = new View(this);
            LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(
                    LinearLayout.LayoutParams.MATCH_PARENT, 0, 1f);
            rectangle.setLayoutParams(params);

            if (i < numFilledRectangles) {
                int color;
                if (i < numRectangles / 3) {
                    color = interpolateColor(Color.RED, Color.YELLOW, (float) i / (numRectangles / 3));
                } else {
                    color = interpolateColor(Color.YELLOW, Color.GREEN,
                            (float) (i - numRectangles / 3) / (2 * numRectangles / 3));
                }
                rectangle.setBackgroundColor(color);
            } else {
                rectangle.setBackgroundColor(Color.GRAY);
            }

            brightnessGauge.addView(rectangle);
        }
    }

    private int interpolateColor(int color1, int color2, float ratio) {
        float inverseRatio = 1f - ratio;
        int r = (int) (Color.red(color1) * inverseRatio + Color.red(color2) * ratio);
        int g = (int) (Color.green(color1) * inverseRatio + Color.green(color2) * ratio);
        int b = (int) (Color.blue(color1) * inverseRatio + Color.blue(color2) * ratio);
        return Color.rgb(r, g, b);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if (brightnessObserver != null) {
            contentResolver.unregisterContentObserver(brightnessObserver);
        }

        if (webSocketClient != null) {
            webSocketClient.close();
        }
    }
}
