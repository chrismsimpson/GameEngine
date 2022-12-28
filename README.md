# Game Engine

This glob of code is very much based on the [olcPixelGameEngine](https://github.com/OneLoneCoder/olcPixelGameEngine), specifically the version represented in this [YouTube tutorial](https://www.youtube.com/watch?v=HXSuNxpCzdM&t=3059s).

Instead of using the original engine, or it's C++ header file, I am using bare bones C# (.NET 6, specifically), paired with SDL2 (specifically [SDL2-CS](https://github.com/flibitijibibo/SDL2-CS)), and therefore have ported all the relevant bits of code to C# from C++. Some creative licence is required in places when undertaking such an effort. My translation is by no means perfect. Also, C++ (with no runtime) and C# (which very much has a runtime) handle cases like NaN and divide by zero very differently, and I have attempted to port the intent as accurately as possible, but have found myself now lost in the weeds.

## Why SDL2? 

SDL2 seems quite promising. The recently added SDL_RenderGeometry API means all triangle drawing can be done on the GPU essentially natively (as is in this repo), without having to call into specificl GPU APIs (Vulkan/DirectX/Metal etc.). There's also a proposed equivalent shader API in the works.


