/*
 * 

 ToDo:
    Disable Plane Visibility
    Reference Points
    (Make brush scale with object size)
    Reduce Raycast max Lenght

 Done:
    Fix static models
    Brighten up the Scene
    Plane Brush performance (alpha)
    Brush Lighting (unlit/emission)
    Plane Brush performance (Quad)
    Static People, oh so static :(
    UI toggle
    Refactor RenderDrawings (seperate Script for brush placement in scene)
    Fix Brush Scaling
    Make Brushes more visible
    Block Raycasts when the UI was touched
    preload delete on update fix
    delegate logic placed in InputModule fix
    Newly created Gameobjects don't care about the Heatmap Setting


Ideas:
    Performance+: Make brushes children of the plane they're sticking to, so you can loop through the children instead of getting all
                  GameObjects tagged "Drawing" every time you place and re-color a brush.
 * 
 * */