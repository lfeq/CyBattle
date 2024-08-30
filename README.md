# Multiplayer Third-Person Shooter

I made this game following a tutorial to learn how to make multiplayer games in Unity.
The project is a basic third-person shooter.

## Changes I Made

The tutorial contained several significant programming mistakes, which I attempted to refactor and improve.

### Camera

The camera controller from the tutorial was one of the worst I have seen for a third-person perspective.
When you move the mouse, the crosshair moves, but the character stays still,
the only way to look behind you is by moving.
Overall, it felt unresponsive, and unlike any third-person game I've played.
I changed it to a more traditional third-person camera controller where the character is offset to the side,
and you can move it all the time.

#### Original Camera Controller

![Original Camera Controller](Readme%20Extras/OriginalCameraController.gif)

#### New Camera Controller

![New Camera Controller](Readme%20Extras/NewCameraMovement.gif)

The new camera controller was made using
the [starter asset third-person controller](https://assetstore.unity.com/packages/essentials/starter-assets-thirdperson-updates-in-new-charactercontroller-pa-196526)
from unity, and I added some new animations from [mixamo](https://www.mixamo.com/#/).

### Weapons and Shooting

The tutorial made a class called `WeaponChanger.cs` that originally was used to change the weapon of the player
but as the tutorial carried on, the class started to do more and more things, eventually becoming a monolith script.
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

I decided to limit the game to a single weapon to simplify the code and avoid handling weapon changes.
This reduced the complexity of my code.
If I wanted to add different weapons, I would create a ScriptableObject class that handled weapon
data and create another script to handle weapon switching, the class `PlayerWeaponShooter.cs` I made would just
have a reference to the current weapon to play the sounds, particle effects, etc. of that weapon.
Here is a brief overview of my `PlayerWeaponShooter` class:

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

#### Main Differences

My implementation in `PlayerWeaponShooter.cs` uses helper methods for improved readability and functions
both online and offline for easier testing.

```csharp
private void Update() {
        Shoot();
    }

private void Shoot() {
    if (!m_inputs.Fire) {
        return;
    }
    ShootRaycast();
    if (PhotonNetwork.IsConnected && m_photonView != null) {
        // If connected to Photon, call the RPC
        m_photonView.RPC(nameof(PlayEffects), RpcTarget.All);
    } else {
        // If not connected, call the method locally
        PlayEffects();
    }
    m_inputs.Fire = false;
}

private void ShootRaycast() {
    Vector2 screenCenterPoint = new Vector2(width / 2, height / 2); // Get the center of the screen
    Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
    const float rayDistance = 500f;
    if (Physics.Raycast(ray, out RaycastHit hit, rayDistance)) {
        if (!hit.transform.CompareTag("Player")) {
            return;
        }
        hit.transform.GetComponent<PlayerManager>().TakeDamage(10f);
    }
}

[PunRPC]
private void PlayEffects() {
    muzzleFlash.Play(); // Play the particle system
    AudioSource.PlayClipAtPoint(shootSoundClip, transform.position); // Play audio clip
}
```

While the original implementation in `WeaponChanger.cs` has everything in the update loop.
It also abuses the `GetComponent<>()` function as it is not really recommended to use it every frame.

```csharp
if (Input.GetMouseButtonDown(0)) {
    ammoAmounts[m_weaponNumber]--;
    m_ammoText.text = ammoAmounts[m_weaponNumber].ToString();
    GetComponent<DisplayColor>().PlayGunShot(GetComponent<PhotonView>().Owner.NickName, m_weaponNumber);
    GetComponent<PhotonView>().RPC("gunMuzzleFlash", RpcTarget.All);
    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    if (Physics.Raycast(ray, out hit, 500)) {
        if (hit.transform.gameObject.GetComponent<PhotonView>() != null) {
            gotShotName = hit.transform.gameObject.GetComponent<PhotonView>().Owner.NickName;
        }
        if (hit.transform.gameObject.GetComponent<DisplayColor>() != null) {
            hit.transform.gameObject.GetComponent<DisplayColor>().DeliverDamage(
                GetComponent<PhotonView>().Owner.NickName,
                hit.transform.gameObject.GetComponent<PhotonView>().Owner.NickName,
                damageAmounts[m_weaponNumber]);
        }
        shooterName = GetComponent<PhotonView>().Owner.NickName;
    }
    gameObject.layer = LayerMask.NameToLayer("Default");
}
```

### Health

**WARNING: IF YOU EVER MADE A GAME WITH HEALTH AND HEALTH-BARS, THIS MIGHT BURN YOUR EYES.**

If you are reading this, you have been warned; this is your last chance to skip ahead.

When you press left click using `WeaponChanger.cs`, which is the original implementation, if you hit an enemy you
call this function `hit.transform.gameObject.GetComponent<DisplayColor>().DeliverDamage(
                        GetComponent<PhotonView>().Owner.NickName,
                        hit.transform.gameObject.GetComponent<PhotonView>().Owner.NickName,
                        damageAmounts[m_weaponNumber]);`
which is already huge, it is already split into three lines, and it gets worse from there.

```csharp
public void DeliverDamage(string shooterName, string name, float damageAmount) {
    GetComponent<PhotonView>().RPC("GunDamage", RpcTarget.AllBuffered, shooterName, name, damageAmount);
}
```

Still acceptable, you have to have a function that calls the RPC function, I can understand that; the problem is
when you go to the RPC function.

**LAST WARNING**

```csharp
[PunRPC]
private void GunDamage(string shooterName, string name, float damageAmount) {
    for (int i = 0; i < m_namesObject.GetComponent<NicknamesScript>().names.Length; i++) {
        if (name == m_namesObject.GetComponent<NicknamesScript>().names[i].text) {
            if (m_namesObject.GetComponent<NicknamesScript>().healthBars[i].gameObject.GetComponent<Image>()
                    .fillAmount > 0.1f) {
                GetComponent<Animator>().SetBool("Hit", true);
                m_namesObject.GetComponent<NicknamesScript>().healthBars[i].gameObject.GetComponent<Image>()
                    .fillAmount -= damageAmount;
            }
            else {
                m_namesObject.GetComponent<NicknamesScript>().healthBars[i].gameObject.GetComponent<Image>()
                    .fillAmount = 0;
                GetComponent<Animator>().SetBool("Dead", true);
                GetComponent<PlayerMovement>().isDead = true;
                GetComponent<WeaponChanger>().isDead = true;
                GetComponentInChildren<AimLookAtRef>().isDead = true;
                m_namesObject.GetComponent<NicknamesScript>().runMessage(shooterName, name);
                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }
    }
}
```

I told you to skip this part!
[Here](https://www.reddit.com/r/Eyebleach/comments/1f37o6u/red_panda/) is a cute red panda so you can wash your eyes.

I don't know where to begin, so I'll start saying this code block is too ugly to read.
Getting that off of my chest, I can explain the code.
FOR SOME REASON, the guy in the tutorial thought it was a great idea to have UI health-bars represent the health
of the players.
Not just like the visual representation, but as in THE value inside that UI image represents the health of the player.
This has two glaring issues, you tightly couple the UI to the health of a player which is a big nono.
Second, you have to check the whole list of players UI health-bars to find the correct one and modify it,
this has a complexity of $O(n)$ for something that you could do easily in constant time
(it's literally changing one value!).

#### My Implementation

The first thing to fix is the way health is handled in the game.
I just created two variables in the `PlayerManager` class:
`maxHealth` (the maximum health a player can have) and `currentHealth` (the player's current health).
A `ResetHealth()` function resets the player's health to the maximum value,
and it is called upon spawning and respawning.

Whenever the player receives damage, this function gets called:

```csharp
public void TakeDamage(float t_damage) {
    if (PhotonNetwork.IsConnected && m_photonView is not null) {
        m_photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All); // Play damage animation
        m_photonView.RPC(nameof(ReduceHealthRPC), RpcTarget.AllBuffered, t_damage); // Reduce health
    }
    else {
        TakeDamageRPC();
        ReduceHealthRPC(t_damage);                  
    }
}
```

`TakeDamage()` has two helper methods to make it more readable, the first `PlayDamageAnimationRPC()` that plays the hit
animation
and `ReduceHealthRPC()` that reduces the current health by the damage amount.

```csharp
[PunRPC]
private void PlayDamageAnimationRPC() {
    m_animator.SetBool("Hit", true);
    StartCoroutine(StopHurtAnimation());
}

[PunRPC]
private void ReduceHealthRPC(float t_damage) {
    m_currentHealth -= t_damage;
    if (!(m_currentHealth <= 0)) {
        return;
    }
    if (PhotonNetwork.IsConnected && m_photonView is not null) {
        m_photonView.RPC(nameof(KillPlayerRPC), RpcTarget.All); // Kill player
    }
    else {
        KillPlayerRPC();             
    }
}
```

The second step was to improve the UI health bars. 
To achieve this, I first renamed the script from `NicknamesScript` to `HealthBarsManager`. 
The next change involved assigning a unique health bar index to each player. 
This was done in the `PlayerManager` class within the `Initialize` function:
`m_healthBarIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;`
#### Main Differences

My implementation uses helper methods to improve readability and works online and offline

```csharp
public void TakeDamage(float t_damage) {
    if (PhotonNetwork.IsConnected && m_photonView is not null) {
        m_photonView.RPC(nameof(PlayDamageAnimationRPC), RpcTarget.All); // Play damage animation
        m_photonView.RPC(nameof(ReduceHealthRPC), RpcTarget.AllBuffered, t_damage); // Reduce health
    }
    else {
        PlayDamageAnimationRPC();
        ReduceHealthRPC(t_damage);
    }
}

[PunRPC]
private void PlayDamageAnimationRPC() {
    m_animator.SetBool("Hit", true);
    StartCoroutine(StopHurtAnimation());
}

[PunRPC]
private void ReduceHealthRPC(float t_damage) {
    m_currentHealth -= t_damage;
    m_healthBarsManager.UpdateHealthBar(m_healthBarIndex, m_currentHealth);
    if (!(m_currentHealth <= 0)) {
        return;
    }
    if (PhotonNetwork.IsConnected && m_photonView is not null) {
        m_photonView.RPC(nameof(KillPlayerRPC), RpcTarget.All); // Kill player
    }
    else {
        KillPlayerRPC();
    }
}
```

In contrast, the original implementation is an unwieldy block of text:

```csharp
[PunRPC]
private void GunDamage(string shooterName, string name, float damageAmount) {
    for (int i = 0; i < m_namesObject.GetComponent<HealthBarsManager>().names.Length; i++) {
        if (name == m_namesObject.GetComponent<HealthBarsManager>().names[i].text) {
            if (m_namesObject.GetComponent<HealthBarsManager>().healthBars[i].gameObject.GetComponent<Image>()
                    .fillAmount > 0.1f) {
                GetComponent<Animator>().SetBool("Hit", true);
                m_namesObject.GetComponent<HealthBarsManager>().healthBars[i].gameObject.GetComponent<Image>()
                    .fillAmount -= damageAmount;
            }
            else {
                m_namesObject.GetComponent<HealthBarsManager>().healthBars[i].gameObject.GetComponent<Image>()
                    .fillAmount = 0;
                GetComponent<Animator>().SetBool("Dead", true);
                GetComponent<PlayerMovement>().isDead = true;
                GetComponent<WeaponChanger>().isDead = true;
                GetComponentInChildren<AimLookAtRef>().isDead = true;
                m_namesObject.GetComponent<HealthBarsManager>().RunMessage(shooterName, name);
                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }
    }
}
```