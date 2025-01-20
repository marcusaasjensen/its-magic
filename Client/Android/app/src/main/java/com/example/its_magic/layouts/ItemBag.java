package com.example.its_magic.layouts;

import android.widget.ImageView;

public class ItemBag {
    private int id;
    private int x;
    private int y;
    private int imageRes;
    private ImageView imageView;

    private static int idCounter = 0;

    public ItemBag(int x, int y, int imageRes) {
        this.id = generateId();
        this.x = x;
        this.y = y;
        this.imageRes = imageRes;
    }

    public ItemBag(int id, int x, int y, int imageRes) {
        this.id = id;
        this.x = x;
        this.y = y;
        this.imageRes = imageRes;
    }

    public ItemBag(int id, int x, int y, int imageRes, ImageView imageView) {
        this.id = id;
        this.x = x;
        this.y = y;
        this.imageRes = imageRes;
        this.imageView = imageView;
    }

    private static int generateId() {
        return idCounter++;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getX() {
        return x;
    }

    public void setX(int x) {
        this.x = x;
    }

    public int getY() {
        return y;
    }

    public void setY(int y) {
        this.y = y;
    }

    public int getImageRes() {
        return imageRes;
    }

    public void setImageRes(int imageRes) {
        this.imageRes = imageRes;
    }

    public ImageView getImageView() {
        return imageView;
    }

    public void setImageView(ImageView imageView) {
        this.imageView = imageView;
    }
}
