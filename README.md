# Card-Game-Sample
Card game sample.

Project installation:
1. Enable Touch Screen Simulation in Input System Debugger.
2. Build Addressables in [Window] -> [Asset Managment] -> [Addressables]

Configuration:
In the [Configurable] folder you can configure:
- Card data values;
- Max card amount in hand;
- Turn duration;
- Cards in random deck;

P.S.
There is quite a mess in the codebase. I didn't handle asynchronous state changes in the way that it wood seems nice and easy to read and work with.
At the time I have some problems with separating logic and visualization, but I really want to work in this direction and learn architecture best-practices.
Tryied MVP pattern for the card objects and it's seems to work, but I'm sure there was a better approach and I eventualy broke the pattern 'cause there is too many dependencies on Views.

Thank you for reading this far! I will be glad to know how to separate logic and visuals properly and better approach for the acync processes.

Used Assets:
- TextMeshPro
- Addressable Assets
- DoTween
- UniTask
- Object Pooling by https://github.com/IntoTheDev/Object-Pooling-for-Unity
- My own pixel-art for sprites https://lumers-gallery.firebaseapp.com/gallery.html

