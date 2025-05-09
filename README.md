# 🌟 Welcome to the Global Azure 2025 Orleans Demos Repository! 🌟

## 🚀 Introduction

Welcome to the **Global Azure 2025 Orleans Demos** repository! This is your one-stop shop for exploring the power of the **.NET Orleans Framework**. Orleans is a groundbreaking framework for building **cloud-native, distributed, and scalable applications**. Whether you're a seasoned developer or just starting your cloud-native journey, Orleans makes it easy to build robust and high-performance systems. 💻☁️

Why Orleans? 🤔
- **Actor Model**: Simplifies concurrency and state management.
- **Cloud-Native**: Designed for modern distributed systems.
- **Scalability**: Handles millions of actors with ease.
- **Developer-Friendly**: Focus on your business logic, not the plumbing.

Orleans is the secret sauce behind many large-scale applications, and now it's your turn to harness its power! 💪

---

## 🛠️ Requirements

To run these demos, you'll need:

1. **.NET SDK** (6.0 or later) - [Download here](https://dotnet.microsoft.com/download)
2. **Azure CLI** - [Install here](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)
3. **Kubernetes CLI (kubectl)** - [Install here](https://kubernetes.io/docs/tasks/tools/)
4. **Docker** (optional for containerized demos) - [Get Docker](https://www.docker.com/)
5. An Azure account - [Sign up for free](https://azure.microsoft.com/free/)
6. **Visual Studio Code** - [Download here](https://code.visualstudio.com/)

---

## 📂 Repository Structure & Usage

This repository is organized into several demo projects, each showcasing a unique aspect of Orleans:



### **Demo001 - Orleans Basics**
- **What it is**: A "Hello World" introduction to Orleans.
- **How to run**:
  1. Open the `Demo001 - Orleans Basics` folder and explore the solution using **Visual Studio Code**.
  2. Open the terminal in **Visual Studio Code** and navigate to the `Silos` folder.
  3. Start the **Silos** project by running `dotnet run`.
  4. Open another terminal and navigate to the `Client` folder.
  5. Run the **Client** project by executing `dotnet run`.
- **What you'll learn**: The basics of grains, silos, and the actor model.

### **Demo002 - Microsoft Adventure Game**
![Adventure Game Box Art](Demos/Demo002%20-%20Microsoft%20Adventure%20Game/assets/BoxArt.jpg)

- **What it is**: A fun, text-based adventure game built with Orleans.
- **How to run**:
  1. Navigate to the `Demo002 - Microsoft Adventure Game` folder in **Visual Studio Code**.
  2. Use the integrated terminal to start the **AdventureServer** project by running `dotnet run` in the `AdventureServer` folder.
  3. Run the **AdventureClient** project by navigating to the `AdventureClient` folder and executing `dotnet run`.
  4. Follow the prompts in the console to play the game.
- **What you'll learn**: How to model complex systems with grains.
- **Example**: The game allows you to explore a virtual world, interact with objects, and complete quests, here is a sample interaction:
  ```
    ______      __                         __
    /\  _  \    /\ \                       /\ \__
    \ \ \L\ \   \_\ \  __  __     __    ___\ \ ,_\  __  __  _ __    __
     \ \  __ \  /'_` \/\ \/\ \  /'__`\/' _ `\ \ \/ /\ \/\ \/\`'__\/'__`\
      \ \ \/\ \/\ \L\ \ \ \_/ |/\  __//\ \/\ \ \ \_\ \ \_\ \ \ \//\  __/
       \ \_\ \_\ \___,_\ \___/ \ \____\ \_\ \_\ \__\\ \____/\ \_\\ \____\
        \/_/\/_/\/__,_ /\/__/   \/____/\/_/\/_/\/__/ \/___/  \/_/ \/____/

    What's your name?
    Mike the Mage
    You are standing in an open field west of a white house with a boarded front door.
    There is a forest to the north.
    Beware! These guys are in the room with you:
      An Angry Customer

    go north
    This is a dimly lit forest, with large trees all around.
    There is a clearing to the north, and a house to the south.

    go north
    You are in a clearing, with a forest surrounding you on the west and south.
    There is a forest to the south, and a canyon to the west.
    The following things are present:
      brass key

    take brass key
    Okay.
    go west
    You are at the top of the great canyon on its south wall.
    There is a beach to the west, and a clearing to the east.
    The following things are present:
      knife
    You are holding the following items:
    brass key

    take knife
    Okay.
    go west
    You are on a small beach on the Frigid River, past the falls.
    There is a faint smell of sulfur coming from the ground.
    There is a clearing to the north, and a canyon to the east.
    The following things are present:
      sandwich
      glass of water
    Beware! These guys are in the room with you:
      The Wicked Witch of the East
    You are holding the following items:
    brass key
    knife

    take sandwich
    Okay.
    take glass of water
    Okay.
   
    look
    You are on a small beach on the Frigid River, past the falls.
    There is a faint smell of sulfur coming from the ground.
    There is a clearing to the north, and a canyon to the east.
    Beware! These guys are in the room with you:
      The Wicked Witch of the East
  ```

### **Demo003 - Orleans Voting App**
- **What it is**: A cloud-native voting app using Orleans.
- **How to run**:
  1. Use the `provision.ps1` script to set up Azure resources (AKS, ACR, etc.).
  2. Deploy the app using the provided `deployment.yaml` file.
  3. Access the app via the provided URL.
- **What you'll learn**: Building scalable, cloud-native apps with Orleans.

### **Demo004 - Orleans Batch Processing on Aspire**
- **What it is**: A demo showcasing batch processing with Orleans and custom grain placement.
- **How to run**:
  1. Navigate to the `Demo004 - Orleans Batch Processing on Aspire` folder in **Visual Studio Code**.
  2. Follow the README in the `Demo004` folder for detailed steps.
- **What you'll learn**: Using Orleans for batch processing and task orchestration.

---

## 🤝 Contributing

We ❤️ contributions! Here's how you can help:

1. Fork this repository.
2. Create a new branch for your feature or bug fix.
3. Submit a pull request with a clear description of your changes.

Feel free to open issues for bugs, feature requests, or questions. Let's make this repository even better together! 🎉

---

## 📚 External Links

- [Orleans Documentation](https://learn.microsoft.com/en-us/dotnet/orleans/)
- [Orleans GitHub Repository](https://github.com/dotnet/orleans)
- [Azure Free Account](https://azure.microsoft.com/free/)

---

Happy coding! 🚀✨