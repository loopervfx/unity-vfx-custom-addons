# unity-vfx-custom-addons
Custom Add-ons for Unity Visual Effect Graph

Should work with Unity 2020.1 + VFX graph 8.2 and may not work with other versions.

* **Custom Function Block**: Lets you insert custom HLSL in a VFX graph Context. Based on work by @peeweek, updated to work in current versions of VFX graph and some UI modifications like a larger text field for editing. Thanks to @andybak for info on the origin of this code.
* **Output Buffer Context**: This can help you to copy the resulting VFX attributes in the Output VertexBuffer to a Global ComputeBuffer / StructuredBuffer. My own work, and a work in progress at that. Requires manual editing of the code generation template to match your vfx graph and data struct. You won't be able to get it to work if you don't know how to modify this manually. No example is included currently, and I am unwilling to provide support.
* **Smooth Zoom**: A really hacky proof of concept to force Unity's GraphView API to support smooth scrolling / zooming in VFX graph editor windows when used in combination with software like https://www.smoothscroll.net/

Please contribute if you are able to help me improve this and to keep this functional before these features officially land in Visual Effect Graph.
