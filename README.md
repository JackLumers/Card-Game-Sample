# Card-Game-Sample
A card game sample that was made as a test task project. Android platform supported.

# Project installation
1. Enable Touch Screen Simulation in Input System Debugger.
2. Enjoy playmode

# Configuration
In the [Configurable] folder you can configure:
- Card data values;
- Max cards amount in hand;
- Turn duration;
- Cards in test random deck;

# Build
After setting the Android flatform, before making game build, ensure you first build Addressables assets in [Window] -> [Asset Managment] -> [Addressables]. 

Game works on IL2CPP, .Net 4.x, ArmV7, Android API level from 19 to 30 included.
Not tested on other settings.
Not tested on standalone.

# P.S.
There is quite a mess in the codebase. I didn't handle asynchronous state changes in the way that it wood seem nice and easy to read and to work with.
At the time I have some problems with separating logic and visualization, but I really want to work in this direction and learn architecture best-practices.
Tryied MVP pattern for the card objects and it's seems to work, but I'm sure there was a better approach and I eventualy broke the pattern 'cause there is too many dependencies on Views.

Maybe I'll work on it later when I got some ideas. I actually know how to do some parts better but I already spend enough time on it so it will remain as it is for a while.

Thank you for reading this far! I will be glad to know how to separate logic and visuals properly and better approaches for the acync processes.
You can write me on Telegram if you want, link in the profile.

# Used Assets:
- TextMeshPro
- Addressable Assets
- DoTween
- UniTask
- Object Pooling by https://github.com/IntoTheDev/Object-Pooling-for-Unity
- My own pixel-art for sprites https://lumers-gallery.firebaseapp.com/gallery.html

