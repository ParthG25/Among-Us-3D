# 🛸 3D Among Us

A fully functional **3D remake** of the popular game **Among Us**, recreated in **Unity** with feature parity to the original 2D game. This version supports multiplayer gameplay, player-hosted lobbies, tasks, voting, and more — all in an immersive 3D environment.

## 🎮 Features

- 🔁 **One-to-One Feature Parity** with the original 2D Among Us game  
- 🌐 **Multiplayer Gameplay** with indirect peer-to-peer networking  
- 🧑‍🤝‍🧑 **Player-Hosted Lobbies** via Client/Server architecture  
- ✅ **Tasks System** identical to the original game  
- 🗳️ **Voting System** allowing players to vote out imposters  
- 🧑‍🎨 **3D Models** created in Blender, with custom textures and animations  
- 🎵 **Audio Effects** edited using Audacity  
- 🎨 **UI Elements** and sprites designed with Photoshop  
- 💻 **Developed in C#** using Unity’s game engine

## 🚀 Getting Started

### Prerequisites

- Unity (recommended version: *2021.3 LTS* or newer)
- Git (to clone the repository)

### Installation

1. Clone the repository.

2. Open the project in Unity Hub or directly via Unity Editor.

3. Build or run in the Unity Editor.

### Multiplayer Hosting

- One player hosts the game (acts as the server).
- Other players join using the host's IP address.
- The networking is set up using an **indirect peer-to-peer model** with server logic handled by the host.

## 🛠️ Built With

- [Unity](https://unity.com/) – Game engine
- [C#](https://learn.microsoft.com/en-us/dotnet/csharp/) – Programming language
- [Blender](https://www.blender.org/) – 3D modeling
- [Audacity](https://www.audacityteam.org/) – Audio editing
- [Photoshop](https://www.adobe.com/products/photoshop.html) – UI and texture design

## 📂 Project Structure

```
Assets/
│
├── Animations/          # Animator controllers, animation clips for characters and objects
├── Audio/               # Edited sound effects and background music
├── Baked Lighting/      # Precomputed lighting data for static scenes
├── Images/              # UI elements, icons, and texture atlases
├── Materials/           # Material assets used for rendering 3D models
├── Models/              # Blender-imported 3D models (FBX/OBJ files)
├── Photon/              # Photon PUN or Fusion networking plugin files
├── Probuilder Data/     # The ProBuilder material palette
├── Resources/           # Important prefabs 
├── Scenes/              # Game scenes (Main Menu, Lobby, Game Map, etc.)
├── Scripts/             # All C# scripts (Networking, Gameplay, UI, Tasks, etc.)
├── Shaders/             # Custom shaders for materials and effects
├── Skyboxes/            # Skybox materials and textures for environmental backgrounds
├── TextMesh Pro/        # TextMesh Pro Unity package and font assets
```

## 📜 License

This project is intended for **educational and personal use** only. It is **not affiliated with or endorsed by Innersloth**, the creators of *Among Us*.  
Please do not use it for commercial purposes.

## 🙌 Acknowledgements

- [Innersloth](https://www.innersloth.com/) for the original *Among Us* game
- Unity Documentation and Community for guidance on multiplayer architecture

## 📸 Screenshots

![Home Screen](https://github.com/ParthG25/Among-Us-3D/blob/0425d98e40126528a6d40deb79b0e173e91a449c/Readme%20Images/Home%20screen.png)

![Settings](https://github.com/ParthG25/Among-Us-3D/blob/0425d98e40126528a6d40deb79b0e173e91a449c/Readme%20Images/Settings.png)

![Lobby Settings](https://github.com/ParthG25/Among-Us-3D/blob/0425d98e40126528a6d40deb79b0e173e91a449c/Readme%20Images/Lobby%20settings.png)

![Skins](https://github.com/ParthG25/Among-Us-3D/blob/0425d98e40126528a6d40deb79b0e173e91a449c/Readme%20Images/Skins.png)

![In-Game Task](https://github.com/ParthG25/Among-Us-3D/blob/0425d98e40126528a6d40deb79b0e173e91a449c/Readme%20Images/Task.png)

![Dead Body Reported](https://github.com/ParthG25/Among-Us-3D/blob/0425d98e40126528a6d40deb79b0e173e91a449c/Readme%20Images/Dead%20body%20reported.png)
