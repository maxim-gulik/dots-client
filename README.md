# About
The clone of the classis Dots game inspired by https://www.dots.co/dots/

# Get Started
1. Clone the repo
2. Open project using **UNITY 2020.3.26f1** (proj location ./Dots)
3. Pick "Game" scene to play
4. Enjoy :)

# About the project

The main idea of the project is design and emplement convinient framework to have the ability deliver at least one hypercasual game at month with minimal friction and a solid level of production quality.

# The core philosophy of the framework

Let's say that a hyper-casual game is a game with next characterystics:
- one simple mechanic and uncomplicated gameplay
- fast work is critical
- without level selection/preloading/plot inserts
- uses simple monetization (ads, IAP)
- simple UI and
- usually, do not require a dedicated server (means game logic)

Unity component-based system are kindly fit to make this type of games from the box, but there are some reasons why it isn't a solid solution for companies who want to make this process periodically:
- heavy API based on static classes, reflection, etc (bad maintainability)
- lack of architecture solution, best practices, and standards that help write scalable, flexible consequent logic
- hardness to change services and other infra tools without touching the business logic
- hardness to write unit tests

Need to mention, that the difference border between HyperCasual and Casual games is weak. Sometimes a simple hyper-casual game can grow in a big F2P casual content-based one (for example, in case of getting popularity among the players) and architecture must be ready for it.

The goal of this framework is to solve all issues mentioned above. It is pretty simple, maintainable, but scalable.

# About base architecture

Game architecture based on a simple controller/actors system using Task-based async programming and di-container.

- Controller - base unit of a business logic of the game. Can contain any amount of sub-controllers (compositor)
- Actor - base unit to provide the ability to controllers control game scene objects.
- Service - an independent infrastructure system that provides some additional functionality to a game. Services do not contain game logic at all.
- Command - some logic with concrete responsibility is required to be shared between game controllers and other commands (can inject other instances)
- Tools - infrastructure instances contain common simple semantically related logic that helps to share it between different game systems

![image](https://user-images.githubusercontent.com/98963961/154372977-bdbae7f4-9fd2-44dd-a18a-20346f128ead.png)

# Services and components to make production level hyper-casual games

**Base minimum** has to be implemented:
- Controller/Actor framework (base classes, commands, base project structure, scene template)
- UI system (simple popup stack with layers support and commands to control UI assets will be enough)
- Player data storage (a simple system using saves in JSON format)
- Build scripts
- Error handling system
- Remote configs delivery (firebase or internal solutions based on google docs or dedicated service on our server, with the support of AB testing)
- Log system (for flexible works with internal and remote logs)
- Multy analytics service (for marketing, to track game events)
- Monetisation services (iaps/ads depends on concrete needs for the first game)
- Environments control (with the ability to hot-swap without rebuilding the solution)
- Input system
- Audio system
- Localisation system (as usual games are launched all over the world)
- Game console
- Unit test framework (to test internal services, extend the unity-test framework by auto mock will be enough)
- DI-container
- Data serialization system (to work with JSON)
- Flexible HTTP request convier (for infrastructure tools and other busines requirements)
- Different extras that make the life of developers easier (extensions, utils, base classes, common components, common shaders etc)

Others ones that are also reusable and helpfull, but can be implemented later in depends on requirements for concrite projects:
- Base tools for data transfer and events delivery (data bucket, message bus)
- User login service
- Remote asset system
- Social networks (Facebook, WeChat, etc)
- Push notification
- Achievements
- Leaderboards

**Avoid** to include:
- any game mecanic specific logic and components (here better to write separate cores, engines etc)
- any visual elvements (popups, screens etc)
- any graphic assets, prefabs, models etc

# How distribute the solution

I would like to strive to package all independent systems (as controller/actor framework, internal services, etc) into UPM packages with separate .asmdefs implementation.
Pros:
- make custom setup in accordance with project requirements
- restrict of injection a game logic to the infra solutions
- setup correct dependencies between an infra and a game

Cons:
- required to spent a time to support all subsystems

# Downsides of the approach
Like any new approach, this solution has an entry threshold. It requires a certain level of technical erudition from a developer (can take some additional time to understand the philosophy of the system and its components)

# What to do with legacy solution
Regarding the legacy games, the best strategy here does not refactor anything without any strong business needs.
But, legacy tools (submodules, .unitypackages, upm packages, etc) can be very helpful to save time implementing new games. In this case, the helpful legacy tools must be analyzed and standardized in accordance with the new approach before the integration.

# About the dots game arhitecture

**Project structure**
![image](https://user-images.githubusercontent.com/98963961/152799043-46c9dd04-45f0-4518-8b77-cac69f7c6cb5.png)

**Game architecture**
More info in the description of classes in the code
![image](https://user-images.githubusercontent.com/98963961/152802879-15145c06-2813-4377-90be-934da41e3e4b.png)

**Settings**
![image](https://user-images.githubusercontent.com/98963961/152803142-ffa40e8f-c799-4c42-a329-5fed41b0971b.png)

