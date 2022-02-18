# About
The clone of the classis Dots game inspired by https://www.dots.co/dots/

The game is a sample demonstrates the view to a convenient solid framework for fast development of hypercasual/casual games.

# Get Started
1. Clone the repo
2. Open project using **UNITY 2020.3.26f1** (proj location ./Dots)
3. Pick "Game" scene to play
4. Enjoy :)

# The core philosophy of the framework

Let's say that a hyper-casual game is a game with next characterystics:
- one simple mechanic and uncomplicated gameplay
- fast work is critical
- without level selection/preloading/plot inserts
- uses simple monetization (ads, IAP)
- simple UI and
- usually, do not require a dedicated server (means game logic)

Unity with a component-based system and a ton of 3-rd-party packages are kindly fit to make sipmle games from the box, but there are a couple reasons why it isn't a solid solution:
- ugly API based on static classes, reflection, etc (bad maintainability and perfomanse)
- component-based system is not intended to write consequent game logic and inject 3rd-party dependencies
- hardness to change services without touching the business logic
- hardness to write unit tests

Need to mention, that the difference border between HyperCasual and Casual games is weak. Regarding this, game architecture must allow more than just make games with the simple mechanic to be posible resolve any business needs.

**The goal of the framework is provide pretty simple, but scalable solution to solve all issues mentioned above and get developers the ability to concentrate only on game specific logic**

# About base architecture

Game architecture based on a simple controller/actors system using Task-based async programming and di-container to inject game services.

- Controller - the base unit of business logic of the game. Regarding this, can inject services and other infra instances. Can contain any amount of sub-controllers (compositor).
- Actor - the base unit to provide the ability to control game scene objects
- Service and Tools - independent infrastructure systems that provides some additional functionality to a game
- Extras - infrastructure instances contain common simple semantically related logic that helps to share it between different game systems
- Command - some logic with concrete responsibility is required to be shared between game controllers and other commands (can inject other instances)

![image](https://user-images.githubusercontent.com/98963961/154686797-e6e6a6be-9d0f-4a21-9ff5-5672e861cd5a.png)

# Services and components to make production level games

**Base solution must contain**:
- Controller/Actor framework
- DI-Container
- Tools for data transfer and events delivery between game instances (data bucket, message bus)
- UI system
- Player data storage
- Error handling API
- Remote configs
- Log system
- Input system
- Audio system
- Localisation system
- Composit analytics service
- Monetisation services (iaps system with verification on the server-side or ads - depends on concrete needs of the company)
- Environments control (with the ability to hot-swap without rebuilding the solution)
- Build scripts
- Game console
- Flexible HTTP request convier (for infrastructure tools and other busines requirements)
- Different extras that make the life of developers easier (extensions, utils, base classes, elementary components etc)
- Convinient Unit-tests framework

Others ones that are also reusable and helpfull, but can be implemented later in depends on requirements for concrite projects:
- User login service
- Remote asset system
- Social networks (Facebook, WeChat, etc)
- Push notification
- Achievements
- Leaderboards

**Avoid** to include:
- any game mechanic specific logic and components (here better to write separate cores, engines etc)
- any visual elements (popups, screens etc)
- any graphic assets, models etc

# How distribute the solution

I would like to strive to package all independent systems (as controller/actor framework, internal services, etc) into UPM packages with separate .asmdefs implementation.

Pros:
- allow to make custom setup in accordance with concriete project requirements
- restrict of injection a game logic to the infra solutions
- setup correct dependencies between an infra and a game

Cons:
- required to spent a time to support all subsystems

# Downsides of the approach
Like any new approach, this solution has an entry threshold. It requires a certain level of technical erudition from a developer (can take some additional time to understand the philosophy of the system and its components)

# What to do with legacy solution
Regarding the legacy games, the best strategy here is not refactor anything without any strong business needs.
But, legacy tools (submodules, .unitypackages, upm packages, etc) can be very helpful and save a lot of development time. In case of necessity to use a legacy tool in a new game, it must be analyzed and refactored in accordance with standards of the new approach.

# Project structure
![image](https://user-images.githubusercontent.com/98963961/154385556-9da7b44f-7fcd-41be-806e-dfd3d4b0aad4.png)

# About the dots game arhitecture

**Game architecture**

More info in the description of classes in the code
![image](https://user-images.githubusercontent.com/98963961/152802879-15145c06-2813-4377-90be-934da41e3e4b.png)

**Settings**

![image](https://user-images.githubusercontent.com/98963961/152803142-ffa40e8f-c799-4c42-a329-5fed41b0971b.png)

