# unity-vfx-custom-addons
Custom Add-ons for Unity Visual Effect Graph

Should work with Unity 2020.1 + VFX graph 8.2 and may not work with other versions.

* **Custom Function Block**: Lets you insert custom HLSL in a VFX graph. Based on work by @andybak. I have simplified it to only be a generic custom code node and nothing more.
* **Output Buffer Context**: This can help you to copy the resulting VFX attributes in the Output VertexBuffer to your a global ComputeBuffer / StructuredBuffer. My own work, and a work in progress at that. Requires manual editing of the code generation template to match your vfx graph and data struct. You won't be able to get it to work if you don't know how to modify this manually. No example is included and I am unwilling to provide support.

Please contribute if you are able to help me improve this and to keep this functional before these features officially land in Visual Effect Graph.
