# Basic steps for Artificial Intelligence

Update version: 0

In this project you will see how to create a simple AI in 3D.

Agents are capsules, red for the enemy and blue for the player.

**Player Agent**

Its movement is controlled by mouse clicks, its AI is only to go where the player clicked looking always forward.
Using the NavMeshAgent component, it will automatically choose the shortest and quickest path to go avoiding obstacles and walls.

**Enemy Agent**

The enemy is the starring character in this project. As far as we know, the player avoid walls in order to get to the clicked point. But the enemy has different states.

In the enemy section we are creating a Finite State Machine (FSM) with three states:
- Wander: this state has implemented the Reynolds about steering attitude. it will randomdly choose a direction to follow that is updated every frame.
- Chase: once the player run into the field of view, the enemy will start to chase the player for a determined distance.
- Ward Off: when the chase distance is completed, the enemy will flee away from the player for a shorter distance than chase's, the time it completes the ward off distance, it will go back again to wander state.

Even though there are no animations, the FSM is easier to read with the Animator component, each state is an animation but actually the animation has no movements.

To do it a little more intelligent, when the enemy chases the player, if the player goes behind a wall, the enemy is still alerted and keeps following until the distance is completed. It would be a low-level intelligence if the enemy is almost catching up the player and suddenly stops chasing it becuase it went behind a wall.


Any comment, suggestion or question, please contact me at johhaanndev@gmail.com
