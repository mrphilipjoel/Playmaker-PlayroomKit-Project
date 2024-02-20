This is an Unofficial Example Project showing the integration of Playmaker and PlayroomKit.
I will probably do some tutorials on my youtube channel www.youtube.com/philipherlitz

This is a complete Unity Project that should allow you to use Playmaker and PlayroomKit together, and quickly export to WebGL.

After opening Project, you'll need to install your copy of PlayMaker from Unity Package Manager.
Before Importing Playmaker, make sure Assets/PlayMaker/Resources/PlayMakerGlobals.asset is unchecked so it doesn't get imported.
If you do accidentally import it, just replace that file with the one in this repo.

You may need to import the 'Vector Graphics' package via the Package Manager "by name":
com.unity.vectorgraphics

You will also want to import the latest PlayroomKit Unity SDK from here:
https://github.com/asadm/playroom-unity/releases

Tips When Working with Playroom and Playmaker:
1. Don't try get/set states before the InsertCoin event.
2. Don't have more than one InsertCoin global event transition.
3. There is a limit (I think of around 11, maybe more, maybe less) of how many 'wait for state' actions you can have running at once.
4. Two seperate FSMs waiting for different Playroom GameStates at the same time, can cause the wrong callback to happen (so far I've seen this only when a global event transitions to the seperate FSMs at the same time, and the Playroom wait is in the first FSM state that is transitioned to from the global event).
5. The same FSM State can't have multiple PlayroomWaitForState actions in it.
