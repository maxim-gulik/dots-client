# About
The clone of the classis Dots game inspired by https://www.dots.co/dots/

The game is a sample that demonstrates my view on the architecture of a framework that allows fast delivery of hyper-casual/casual games with minimal friction and a solid level of production quality.

# Get Started
1. Clone the repo
2. Open project using **UNITY 2020.3.26f1** (proj location ./Dots)
3. Pick "Game" scene to play
4. Enjoy :)

# The core philosophy of the framework

_Let's say that a hyper-casual game is a game with the next characteristics: one simple mechanic and uncomplicated gameplay; fast work is critical; without level selection/preloading/plot inserts; uses simple monetization (ads, IAP); simple UI; usually, do not require a dedicated server (means game logic)_

Unity with a component-based system and a ton of 3-rd-party packages are kindly fit to make sipmle games from the box, but there are a couple reasons why it isn't a solid solution:
- ugly API based on static classes, reflection, etc (bad maintainability and perfomanse)
- component-based system is not intended to write consequent game logic and easy inject 3rd-party dependencies
- hardness to change services without touching the business logic
- hardness to write unit tests

Need to mention, that the difference border between HyperCasual and Casual games is weak. My experience has identified, that the business side sometimes does not feel this difference and requires more than just making games with the simple mechanic. Regarding this, better to design the architecture that is ready for it.

**The philosophy of the framework is to provide a pretty simple, but scalable solution to solve all issues mentioned above and allow developers to concentrate only on game-specific logic**

# About base architecture

Game architecture based on a simple controller/actors system using Task-based async programming and di-container to inject game services.

![image](https://user-images.githubusercontent.com/98963961/154853330-00230da1-15a2-4617-9059-f0eaead3f252.png)

- Controller - the base unit of business logic of the game. Сan inject services and other infra instances. Can contain any amount of sub-controllers (compositor).
- Actor - the base unit to provide control of game scene objects
- Service and Tools - independent infrastructure systems that provides some additional functionality to a game
- Extras - infrastructure instances contain common simple semantically related logic
- Command - some business logic is required to be shared between game controllers (and other commands also)

# Services and components to make production level games

**Base solution must contain**:
- Controller/Actor framework
- DI-Container
- Tools for data transfer and events delivery between game controllers (data bucket, message bus)
- UI system
- Player data storage
- Error handling API
- Remote configs
- Log system
- Input system
- Audio system
- Localisation system
- Composite analytics service
- Monetisation services (iaps system with verification on the server-side or ads - depends on concrete needs of the company)
- Environments control (with the ability to hot-swap)
- Build scripts
- Game console
- Flexible HTTP request convier (for infrastructure tools and other busines requirements)
- Different extras that make the life of developers easier (extensions, utils, base classes, elementary components etc)
- Convinient Unit-tests framework

Others ones that are also reusable and helpful, but can be implemented later in depends on requirements for concrete projects:
- User login service
- Remote asset system
- Social networks (Facebook, WeChat, etc)
- Push notification
- Achievements
- Leaderboards

**Avoid** to include:
- any game mechanic specific logic and components (here better to write separate cores, engines etc)
- any specific graphic assets, 3d-models, sprites, UI-elements etc

# How distribute the solution

I would like to pack all independent systems (as controller/actor framework, services, extras etc) into UPM packages with relevant .asmdefs.

Pros:
- allow to make custom setup in accordance with concriete project requirements
- restrict of injection a game logic to the infra solutions
- setup correct dependencies between an infra and a game

Cons:
- required to spent a time to support all subsystems

# Downsides of the approach
Like any new approach, this solution has an entry threshold. It requires a certain level of technical erudition from a developer (can take some additional time to understand the philosophy of the system and its components)

# What to do with legacy solution
Regarding the legacy tools (submodules, .unitypackages, upm packages, etc), the best strategy here is not to refactor anything without any strong business needs.
But, in case of necessity to use a legacy tool in a new game, it must be at least analized and discussed with business side posibility to refactor it in accordance with standards of the new approach.

# Project structure
![image](https://user-images.githubusercontent.com/98963961/154385556-9da7b44f-7fcd-41be-806e-dfd3d4b0aad4.png)

# About the dots game arhitecture

**Game architecture**

More info in the description of classes in the code
![image](https://user-images.githubusercontent.com/98963961/152802879-15145c06-2813-4377-90be-934da41e3e4b.png)

**Settings**

![image](https://user-images.githubusercontent.com/98963961/152803142-ffa40e8f-c799-4c42-a329-5fed41b0971b.png)

