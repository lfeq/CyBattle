# Multiplayer Third-Person Shooter

Made this game following an Udemy tutorial.

## Changes I Made

The tutorial made some huge programming mistakes, so I tried to refactor and improve the following

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