## Scenery Painting : A Town called Paint
A realtime perspective based painting system. Designed for easy of use and implimentation into an ongoing project.
[a collection of houses have have been painted on, with the tex "A Town Called Paint" written on them](images/Unity_aZwcy4QbQy.jpg)
### What does it do and why?
Scenery Paint allows you to import any model, attach a collider and material and then "paint" on it in real time. Models do not need to be prepped outside of specific umity import settings, no textures or data needs to be generated ahead of playing the game. The system is designed to require minimal input from non-technical users and instead.

The goal of this system was to speed up and make collaboration on my game A Town called Paint by enabling artists and other devs to not have to worry about baking special textures or making sure texture scales are correct. By handling all parts at run time I can use any assets, and people I work with do not need to worry or remember to follow any extra steps when testing or building assets as all the uv generation and other systems are handled by the engine or my code.
[a set of colored in houses](images/Unity_QaNjNHdPv4.png)
[the prvious houses from a different angle showing how they are oly correctly coloured when viewed from the right angle](images/Unity_uNMymAwauy.png)
### How to use it
1. Add the Paint Camera prefab to your scene
2. Import a model into unity - in the import settings make sure to select generate lightmap uv, and rescale the object to Unity's expected scale. (If importing from Blender this generally means setting the import scale to 100)
3. Attach a material that uses the Unlit Paint shader
4. Attach a collider
5. Play and Paint!

####Optional
The outline effect can be added by attaching the outline replacement script to any camera

### How it works
Scenery Paint uses a series of custom shaders, and camera to generate a realtime screen space texture mapped to each object being painted that can then be used in a Graphics.Blit function with the drawing shader to draw on each object.

Each time an object is drawn on the steps are as follows:
1. Check if object is rendered
2. Add cacheing script if not already there
3. Render object as uv2 coords with screen space coords as color in RG channels, Depth comparision in B on 2nd Camera
4. Blit screenspace texture with cached texture used by base material
5. Clear screenspace Textureaaaaaaaaaaaaaa

### To Do
-Optimization
  -occlusion of objects by AABB
  -Draw shader optimization (reduce tex2D calls)
-Visuals
  -imporve screen uv edges
  -imporve edgde detection for outline

[![CC BY-NC 4.0][cc-by-nc-shield]][cc-by-nc]

This work is licensed under a
[Creative Commons Attribution-NonCommercial 4.0 International License][cc-by-nc].

[![CC BY-NC 4.0][cc-by-nc-image]][cc-by-nc]

[cc-by-nc]: https://creativecommons.org/licenses/by-nc/4.0/
[cc-by-nc-image]: https://licensebuttons.net/l/by-nc/4.0/88x31.png
[cc-by-nc-shield]: https://img.shields.io/badge/License-CC%20BY--NC%204.0-lightgrey.svg



