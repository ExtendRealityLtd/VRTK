Quick Outline
=============

Developed by Chris Nolet (c) 2018


About
-----

Quick Outline is a world-space outline tool, that adds a solid outline to any object.

It’s ideally suited for VR.

Many outline shaders work in screen space, which makes them slow – and they don’t support MSAA. If they do work in world space, they have ‘gaps’ on hard corners. Quick Outline addresses these issues.

Quick Outline was originally designed for VR, so it supports Instanced Stereo rendering and MSAA. It looks great in any HMD, and it won’t impact the frame rate.

- Designed for VR (including single pass)
- Supports MSAA
- Compatible with post-processing stack
- Multiple outline modes
- Lightweight and performant


Instructions
------------

To add an outline to an object, drag-and-drop the Outline.cs script onto the object. The outline materials will be loaded at runtime.

You can also add outlines programmatically with:

    var outline = gameObject.AddComponent<Outline>();

    outline.OutlineMode = Outline.Mode.OutlineAll;
    outline.OutlineColor = Color.yellow;
    outline.OutlineWidth = 5f;

The outline script does a small amount of work in Awake(). For best results, use outline.enabled to toggle the outline. Avoid removing and re-adding the component if possible.

For large meshes, you may also like to enable 'Precompute Outline' in the editor. This will reduce the amount of work performed in Awake().
