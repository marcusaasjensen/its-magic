# 🌲🪄 It's Magic - Distributed Interface Game Project 🪄🌲

Welcome to our immersive game project developed at Polytech! This project was created as one of our final college projects, designed to enhance interaction and immersion using distributed systems across multiple devices.

## 📜 Overview

To ensure optimal interaction and an immersive experience, we designed a distributed system utilizing several devices. Each device plays a specific role, offering complementary perspectives and functionalities.

## 🎯 Game's Goal

The game is designed for three players:
- Two players on the interactive table.
- One player using both the vertical screen and the phone.

**Objective**: As a group of three wizards, your goal is to prepare a potion by following these steps:
1. **🍄 Harvest Ingredients**: Collect all required ingredients in the forest and store them in the bag.
2. **🥣 Prepare the Cauldron**: Throw the ingredients into the cauldron inside the house.
3. **🪵 Heat the Cauldron**: Move the cauldron to the chimney when you hear strong bubbling sounds.
4. **🔥 Light the Fire**: Use the bellows to ignite the fire under the cauldron.
5. **⚗️ Finish the Potion**: Once dark blue bubbles appear, pour the potion into a flask.

**⚠️ Caution**: Be careful not to drop the potion!

### ✨ Features

- **Interactive Table**: Provides a top-down view of game scenes like the forest and workshop. Players can gather ingredients and manage resources for potion-making.
- **Vertical Screen**: Offers detailed side views (front, back, left, right) for enhanced immersion in key environments like the forest and workshop.
- **Mobile Phone**: Acts as the main launch point for the application, enabling interactions like blowing into the microphone to ignite fires and managing brightness to change the time of day.

<img width="755" alt="image" src="https://github.com/user-attachments/assets/c88956ce-76a6-44bd-89c0-506c72a829f9" />

<img width="212" alt="image" src="https://github.com/user-attachments/assets/1fda0afb-8b20-424d-93fe-37a08c753aa7" />

## 👆 Device Interactions

### ⬇️ Interactive Table

- **Multi-touch Selection**: Handle cooperative or individual tasks for harvesting ingredients and manipulating objects.
- **Drag and Drop**: Interact with objects in scenes like the forest and workshop. Use the magic wand to reveal parts of the side view on the vertical screen.

<img width="277" alt="image" src="https://github.com/user-attachments/assets/d76ea95a-ef0e-40fd-84e7-3f354a9114e6" />
<img width="203" alt="image" src="https://github.com/user-attachments/assets/fa88282f-da62-46a8-85ce-361a5688d05f" />

### ➡️ Vertical Screen

- **Slash Interaction**: Cut ingredients in the side forest scene to make them fall onto the interactive table.
- **Tap (Individual)**: Navigate scenes by clicking on doors and interacting with objects.
- **UI Communication**: Use the UI to indicate object locations and get hints for crafting the potion.

<img width="239" alt="image" src="https://github.com/user-attachments/assets/7fb8af25-fdf1-48e5-a896-421157196fef" />
<img width="394" alt="image" src="https://github.com/user-attachments/assets/37941215-6dac-43fd-b6bd-b603a2767876" />
<img width="245" alt="image" src="https://github.com/user-attachments/assets/4f6c4de8-af2b-484a-8e9a-da50e5d9b6d7" />

### 📱 Mobile Phone

- **💨 Blow into Microphone**: Ignite fires in the workshop to prepare potions in the cauldron.

<img width="285" alt="image" src="https://github.com/user-attachments/assets/41eaa7d9-6514-45c9-a9b2-a053f608475a" />
<img width="265" alt="image" src="https://github.com/user-attachments/assets/a020516a-ce8b-4da8-b0d3-1eac9aaaeed0" />
<img width="184" alt="image" src="https://github.com/user-attachments/assets/f68f0b1b-a833-4388-92fe-bda12f5c5b30" />
<img width="491" alt="image" src="https://github.com/user-attachments/assets/ac54f0f5-1ddf-4a03-9f4f-3de95f6a6d03" />

- **👜 Throwable Inventory**: Manage collected ingredients in the inventory to inform other players about the collected items.

<img width="169" alt="image" src="https://github.com/user-attachments/assets/e858d8f6-dc7c-4805-b129-a0d1606e4770" />
<img width="154" alt="image" src="https://github.com/user-attachments/assets/ea80da4b-dce1-41a7-a66b-9a477f10f7d4" />

- **💡 Environment Brightness**: Change the time of day to collect night-only ingredients like fireflies.

<img width="193" alt="image" src="https://github.com/user-attachments/assets/c684f643-aa29-4529-94a8-b688a41dd73c" />
<img width="173" alt="image" src="https://github.com/user-attachments/assets/62f7de9d-b6bd-4ba9-9784-9dafc2c3d986" />
<img width="246" alt="image" src="https://github.com/user-attachments/assets/fb47218d-b9d1-4475-a7f4-91650a656004" />

## ⚙️ Technical Details

The project was built using:
- **Unity 3D 2022.3.20f1** for both top and side views.
- **Android Studio** for the phone app.

All clients communicate through a WebSocket server.

### Setup Instructions

1. **Build the Android App**:
   - Use Android Studio to build the app from the `Client/Android` folder.

2. **Build Unity Clients**:
   - Build separate executables for the top and side view clients in Unity 3D.
   - Update the `config.json` file with the correct IP address and port (use `config.template.json` as a template).

3. **Run the Server**:
   - Navigate to the `Server` folder and run `npm install`, then `npm run start` with the appropriate port.

4. **Run Unity Clients**:
   - Run each Unity instance on separate computers for the table and vertical screen.

5. **Launch the Android App**:
   - Start the game on the phone. If the app crashes, simply restart it.

Enjoy the game! 🎮
