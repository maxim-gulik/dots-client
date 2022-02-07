# About
The clone of the classis Dots game inspired by https://www.dots.co/dots/  

# Get Started
1. Clone the repo
2. Open project using **UNITY 2020.3.26f1** (proj location ./Dots)
3. Pick "Game" scene to play
4. Enjoy :)

# Proj Structure

![image](https://user-images.githubusercontent.com/98963961/152799043-46c9dd04-45f0-4518-8b77-cac69f7c6cb5.png)

# About base architecture

Game architecture based on a simple controller/actors system. This approach allows writing consequent centralized logic with the ability to cover by unit-test and build flexible systems.

Controller - base unit of a business logic of the game. Can contain any amount of sub-controllers (compositor)
Actor - base unit to provide the ability to controllers control game scene objects.

![image](https://user-images.githubusercontent.com/98963961/152801073-e455b621-26f4-4d34-b690-af6c3c5a2035.png)

# About game arhitecture
More info in the description of classes in the code

![image](https://user-images.githubusercontent.com/98963961/152802879-15145c06-2813-4377-90be-934da41e3e4b.png)

# About game
Endless game mode can be set up here

![image](https://user-images.githubusercontent.com/98963961/152803142-ffa40e8f-c799-4c42-a329-5fed41b0971b.png)

