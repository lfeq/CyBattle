# Multiplayer Third-Person Shooter

Made this game following a Udemy tutorial.

## Changes I Made

The tutorial made some huge programming mistakes, so I tried to refactor and improve the following:

### Camera

The camera controller the tutorial made is the worst I have seen for a third-person perspective. When you move the
mouse, the crosshair moves, but the character stays still, the only way to look behind you is by moving. In
conclusion, it feels bad and I don't think I ever played a game with that kind of camera. I changed it to a more 
traditional camera controller where the character is offset to the side, and you can move it all the time.

#### Original Camera Controller
![Original Camera Controller](Readme%20Extras/OriginalCameraController.gif)


#### New Camera Controller
![New Camera Controller](Readme%20Extras/NewCameraMovement.gif)

The new camera controller was made using the [starter asset third-person controller](https://assetstore.unity.com/packages/essentials/starter-assets-thirdperson-updates-in-new-charactercontroller-pa-196526)
from unity, and I added some new animations from [mixamo](https://www.mixamo.com/#/).

### Weapons and Shooting
The tutorial made a class called `WeaponChanger.cs` that originally was used to change the weapon of the player
but as the tutorial carried on the class started to do more and more things, eventually becoming a monolith script.
If you ask ChatGPT what the class does, it gives you this response:

- **Handles weapon switching and shooting mechanics**:
    - Tracks and switches between different weapons using `weapons[]` and `m_weaponNumber`.
    - Updates weapon-specific UI elements like `m_weaponIcon` and `m_ammoText`.
    - Handles ammo count and updates the UI accordingly.

- **Manages IK (Inverse Kinematics) for hands**:
    - Uses `TwoBoneIKConstraint` for both hands to position them correctly based on the selected weapon.
    - Rebuilds the rig with `rig.Build()` after switching weapons.

- **Integrates with Photon for multiplayer functionality**:
    - Synchronizes weapon changes and shooting across the network using `PhotonView` and RPC methods (`change`, 
    `gunMuzzleFlash`, etc.).
    - Disables certain components for remote players (e.g., `PlayerMovement`).

- **Handles shooting mechanics**:
    - Detects shooting input (`Input.GetMouseButtonDown(0)`), reduces ammo, and triggers visual 
    effects like muzzle flash.
    - Performs raycasting to detect hits on other players and applies damage using `DisplayColor`.

- **Initializes camera settings**:
    - Sets up the `CinemachineVirtualCamera` to follow and look at the player.

- **Handles weapon pickup updates**:
    - Updates the ammo display when a new weapon is picked up.

- **Miscellaneous tasks**:
    - Disables Photon-related components if the player is not connected to the network.
    - Handles temporary tasks like checking for existing weapons on the map (`m_testForWeapons`).

If this class only managed weapon behavior, I think it would be acceptable, but as it does some other things like 
initialize the camera, it makes my eyes burn, but it's still not as bad as the `DisplayColor.cs` script. 

#### My Implementation
I decided to only make one weapon available, so I don't have to handle weapon changing, this reduced the complexity
of my code. If I wanted to add different weapons, I would create a ScriptableObject class that handled weapon
data and make another script that handled weapon changing, the class `PlayerWeaponShooter.cs` I made would just
have a reference to the current weapon to play the sounds, particle effects, etc. of that weapon. Here is a small
view of my class:

```csharp
public class PlayerWeaponShooter : MonoBehaviour {
    StarterAssetsInputs inputs;

    private void Start() {
        inputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update() {
        Shoot();
    }

    private void Shoot() {
        if (!inputs.Fire) {
            return;
        }
        Debug.Log("Shooting");
        inputs.Fire = false;
    }
}
```