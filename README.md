# HexGame

## Game Design Document

### Overview
HexGame is a turn-based strategy game built in Unity 6 that combines hex-based grid gameplay with army management and territorial conquest mechanics inspired by classic board games like Risk. Players compete to control territories, manage armies, and defeat opponents through strategic combat.

### Game Objectives
- **Victory Condition**: Control all tiles on the map
- **Defeat Condition**: Lose all controlled tiles

### Core Gameplay Mechanics

#### 1. Map & Tiles
**Status**: ✅ Implemented

- **Hex Grid Generation**: Procedurally generated hex grid map of configurable size
- **Tile System**: Basic hex tile prefab supporting different types (grass, water) using material or color changes
- **Tile Selection**: Players can select tiles by clicking on them, with visual highlighting
- **Adjacent Tile Detection**: System identifies neighboring tiles for attack validation

#### 2. Camera System
**Status**: ✅ Implemented

- **Camera Controls**: 
  - WASD keys for panning
  - Mouse drag for panning
- **Camera Constraints**: Camera movement is constrained to the edges of the hex map
- **Smooth Movement**: Ensures fluid camera transitions

#### 3. User Interface
**Status**: ✅ Implemented

- **Turn Tracker**: Displays whose turn it is (Blue or Green player)
- **Army Counter**: Shows total armies for each player
- **Next Turn Button**: Allows players to end their turn and proceed to the next
- **Tile Information**: Displays information about selected tiles (coordinates, type, ownership)

#### 4. Players & Teams
**Status**: ✅ Implemented

- **Two Player Colors**: Blue and Green
- **Color Coding**: All player-related UI, armies, and tiles are clearly associated with their respective color
- **Turn-Based System**: Players take turns sequentially

#### 5. Army Management
**Status**: ✅ Implemented

- **Army Representation**: Armies are represented as small cylinders (approximately 10% of tile scale)
- **Army Placement**: Multiple armies can be displayed on a single tile without overlap
- **Army Generation**: At the start of each turn, players receive one army per tile they control
- **Automatic Distribution**: Armies are placed automatically in each controlled tile at the beginning of the turn

#### 6. Combat System
**Status**: ✅ Implemented

The combat system uses a Risk-inspired dice-rolling mechanism with all-at-once resolution:

- **Attack Initiation**: Players can attack adjacent tiles controlled by the opponent
- **Battle Resolution**:
  1. Both sides roll one die per army (capped at 10 dice maximum)
  2. Dice are sorted in descending order for both sides
  3. Highest die vs highest die, second highest vs second highest, etc.
  4. For each pair:
     - If attacker's die > defender's die: defender loses an army
     - If attacker's die ≤ defender's die: attacker loses an army
  5. Extra unpaired dice are resolved against default values
  6. Side with most remaining armies wins

- **Victory Handling**: 
  - Winning attacker moves all surviving armies into conquered tile
  - Tile ownership transfers to the attacker
  - Game checks for win/loss conditions after each battle

#### 7. AI System
**Status**: 🔄 In Progress

The AI system is designed to provide challenging computer opponents with varying difficulty levels:

- **Difficulty Levels** (Planned):
  - Easy: 25% chance of making optimal decisions
  - Medium: 50% chance of making optimal decisions  
  - Hard: 75% chance of making optimal decisions

- **Decision Making** (Planned):
  - AI evaluates available moves and actions
  - Random number generator determines whether AI makes optimal choice or mistake
  - Difficulty level affects probability of optimal play

- **Integration** (Planned):
  - AI seamlessly integrated into team turn flow
  - AI teams play automatically alongside human teams
  - AI behavior validated through unit tests and playtesting

### Technical Implementation

#### Platform
- **Engine**: Unity 6
- **Language**: C#
- **Coding Standards**:
  - Always use curly braces for control statements
  - Explicit type declarations (no `var` keyword)
  - Preserve existing comments

#### Key Systems
1. **Hex Grid Generator**: Procedural generation system for creating the game map
2. **Tile Selection System**: Click-based tile interaction with visual feedback
3. **Combat Resolver**: Dice-based combat calculation with probabilistic outcomes
4. **Turn Manager**: Handles turn flow, army distribution, and win condition checking
5. **UI Manager**: Manages all on-screen information and player controls

### Game Flow

1. **Game Start**: Map is generated with tiles distributed between players
2. **Turn Start**: Active player receives armies based on controlled tiles
3. **Player Actions**: 
   - Select owned tiles
   - Attack adjacent enemy tiles
   - Resolve combat through dice rolls
4. **Turn End**: Player clicks "Next Turn" button
5. **Victory Check**: Game checks if any player has won or lost
6. **Next Player**: Turn passes to the next player

### Planned Features

#### AI Opponents
- ✅ Design AI decision-making logic for team moves
- ✅ Implement AI difficulty settings (Easy, Medium, Hard)
- ✅ Implement chance-based decision making
- 🔄 Integrate AI into team turn flow
- 🔄 Test and tune AI behavior and difficulty odds

#### Combat Refinement
- ✅ Implement Risk-inspired combat resolution
- ✅ Handle tile capture and army movement
- ✅ Simple attack resolution: army vs. army comparison

### Development History

The game was developed iteratively with the following major milestones:

1. **Foundation** (Issues #1-3): Camera controller, hex tile prefab, and grid generation
2. **Interaction** (Issues #4-5): Tile selection and basic UI overlay  
3. **Gameplay Core** (Issues #9, #11-12): Army representation, army generation, and attacking
4. **Combat System** (Issues #14-15, #28): Attack resolution, tile capture, and dice-based combat
5. **Win Conditions** (Issue #18): Victory and defeat condition checking
6. **Polish** (Issue #19): Turn tracking, army counting, and turn management
7. **AI Development** (Issues #20-23, #27): AI system design and implementation (ongoing)