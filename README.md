# CodinGameFall2022

## Tools

```powershell
dotnet tool install -g dotnet-combine
```

Usage:

```powershell
dotnet-combine single-file . -o Combined.cs
```

Output:
```powershell
Output file: .\Combined.cs
```

## Surround Strategy

A strategy that surrounds the enemy's units and puts recyclers all around them if possible.

### Early Game

SPAWN: Until turn 10, build new units every turn as close to the enemy as possible.

BUILD: No building before turn 10.

MOVE: Move units toward enemy but try to avoid stacking (spread out on line).
  Also send far left and far right to go "around" enemy territory once within 3 of enemy territory.

### Defining Around Territory

My southernmost unit will try to go 1 space further south than the enemy's territory.
My northernmost unit will try to go 1 space further north than the enemy's terrirory.

Then, turn toward the enemy's territory and continue until 1 space east/west past enemy territory.

### Mid Game

Starting turn 10 (or on smaller maps maybe sooner - like the turn after enemies are adjacent to my units).

SPAWN: No more spawning until enemy is surrounded by recyclers.

BUILD: Build recyclers in any of my spawnable spaces that are adjacent to enemy units, ordered by unit count descending.

MOVE: If adjacent to enemy, move toward them. Otherwise, continue attempting to surround territory.

### End Game

Triggered by having 10 recyclers on the board? Or turn 15.

SPAWN: Shift to spawning units in spawnable squares closest to enemy squares but not adjacent to my recyclers.

BUILD: Build only if an enemy unit is going to break blockade. Not sure how to detect this, yet.

MOVE: Move to nearest enemy squares if there's a path to them. Otherwise move to nearest neutral square (spread out).
