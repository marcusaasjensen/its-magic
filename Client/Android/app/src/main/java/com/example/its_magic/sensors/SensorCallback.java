package com.example.its_magic.sensors;

public interface SensorCallback {
    void onValueChanged(String sensorName, String value);
    void onError(String sensorName, String error);
}