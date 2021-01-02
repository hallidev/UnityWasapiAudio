# UnityWasapiAudio
An example of WASAPI Loopback Audio in Unity using CSCore

Check out CSCore: https://github.com/filoe/cscore

This is an updated version of my little Unity audio visualization library including a VFX Graph example.

Instructions (Unity 2020.x, HDRP):

1: Clone / download

2: Open in Unity

3: IMPORTANT! If you are going to be making a build using this, you need to go to Edit -> Project Settings -> Player -> Other Settings -> Api Compatibility Level and set it to ".NET 4.x". Otherwise you will get a kernel exception upon trying to run Unity Player.

4: Play some music on your computer

5: Hit "Play"

That's it!

If you don't have VFX Graph installed and would like to use this, just copy the WasapiAudio folder into your own assets folder and delete the Vfx.* files that won't build. It should work going back to Unity 2017.x at least.
