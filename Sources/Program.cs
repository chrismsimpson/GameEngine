
namespace Triangle;

public struct Vec3D {

    public float X { get; set; }
    
    public float Y { get; set; }
    
    public float Z { get; set; }

    ///

    public Vec3D(
        float x,
        float y,
        float z) {

        this.X = x;
        this.Y = y;
        this.Z = z;
    }
}

public struct Triangle {

    public Vec3D[] P { get; init; }

    public byte Shade { get; init; }

    ///

    public float MidZ { get => (this.P[0].Z + this.P[1].Z + this.P[2].Z) / 3.0f; }

    ///

    public Triangle()
        : this(0, 0, 0, 0, 0, 0, 0, 0, 0, 0xff) { }

    public Triangle(
        float x1,
        float y1,
        float z1,
        
        float x2,
        float y2,
        float z2,
        
        float x3,
        float y3,
        float z3)
        : this(x1, y1, z1, x2, y2, z2, x3, y3, z3, 0xff) { }

    public Triangle(
        float x1,
        float y1,
        float z1,
        
        float x2,
        float y2,
        float z2,
        
        float x3,
        float y3,
        float z3,
        
        byte shade) {

        this.P = new [] {

            new Vec3D(x1, y1, z1),
            new Vec3D(x2, y2, z2),
            new Vec3D(x3, y3, z3)
        };

        this.Shade = shade;
    }    
    
    public Triangle(
        Vec3D p1,
        Vec3D p2,
        Vec3D p3)
        : this(p1, p2, p3, 0xff) { }
    
    public Triangle(
        Vec3D p1,
        Vec3D p2,
        Vec3D p3,
        byte shade) {

        this.P = new [] { p1, p2, p3 };
        this.Shade = shade;
    }
}


public struct Mesh {

    public Triangle[] Triangles { get; init; }

    ///

    public Mesh(
        Triangle[] triangles) {

        this.Triangles = triangles;
    }

    public Mesh(
        String filename) {

        using var stream = File.Open(filename, FileMode.Open);

        using var reader = new StreamReader(stream);

        var verts = new List<Vec3D>();

        var tris = new List<Triangle>();

        while (!reader.EndOfStream) {

            var line = reader.ReadLine();

            if (String.IsNullOrWhiteSpace(line)) {

                continue;
            }

            ///

            if (line[0] == 'v') {

                var lineSegments = line.Split(' ');

                ///

                float x = 0;

                if (!float.TryParse(lineSegments[1], out x)) {

                    throw new Exception();
                }

                ///

                float y = 0;

                if (!float.TryParse(lineSegments[2], out y)) {

                    throw new Exception();
                }

                ///
                
                float z = 0;

                if (!float.TryParse(lineSegments[3], out z)) {

                    throw new Exception();
                }

                ///

                verts.Add(new Vec3D(x, y, z));
            }

            if (line[0] == 'f') {

                var lineSegments = line.Split(' ');

                ///

                int f1 = 0;

                if (!int.TryParse(lineSegments[1], out f1)) {

                    throw new Exception();
                }

                ///

                int f2 = 0;

                if (!int.TryParse(lineSegments[2], out f2)) {

                    throw new Exception();
                }

                ///

                int f3 = 0;

                if (!int.TryParse(lineSegments[3], out f3)) {

                    throw new Exception();
                }

                ///

                tris.Add(new Triangle(verts[f1 - 1], verts[f2 - 1], verts[f3 - 1]));
            }
        }

        this.Triangles = tris.ToArray();
    }

}

public struct Mat4x4 {

    public float[][] M { get; init; }

    ///

    public Mat4x4(float[][] m) {

        this.M = m;
    }

    public Mat4x4() {

        this.M = new [] {
            new float[4],
            new float[4],
            new float[4],
            new float[4]
        };
    }
}

///


///

public static partial class Program {

    // public static Vec3D MultiplyMatrixVector(in Vec3D i, in Mat4x4 m) {

    //     var x = i.X * m.M[0][0] + i.Y * m.M[1][0] + i.Z * m.M[2][0] + m.M[3][0];

	// 	var y = i.X * m.M[0][1] + i.Y * m.M[1][1] + i.Z * m.M[2][1] + m.M[3][1];

	// 	var z = i.X * m.M[0][2] + i.Y * m.M[1][2] + i.Z * m.M[2][2] + m.M[3][2];
		
    //     var w = i.X * m.M[0][3] + i.Y * m.M[1][3] + i.Z * m.M[2][3] + m.M[3][3];

	// 	if (w != 0.0f) {

	// 		x /= w; 
    //         y /= w; 
    //         z /= w;
	// 	}

    //     return new Vec3D(x, y, z);
    // }

    public static int Main(
        String[] args) {

        // WriteLine($"hello foo!");

        using var app = new GameEngine(800, 640);

        app.Run();

        return 0;
    }

    

    // public static void Main2(
    //     String[] args) {

    //     var scale = 2.0f;

    //     ///

    //     var width = 800;
    //     var height = 600;

    //     ///

    //     var meshCube = new Mesh(
    //         new Triangle[] {

    //             // SOUTH
    //             new Triangle(0.0f, 0.0f, 0.0f,    0.0f, 1.0f, 0.0f,    1.0f, 1.0f, 0.0f),
    //             new Triangle(0.0f, 0.0f, 0.0f,    1.0f, 1.0f, 0.0f,    1.0f, 0.0f, 0.0f),

    //             // EAST                                                      
    //             new Triangle(1.0f, 0.0f, 0.0f,    1.0f, 1.0f, 0.0f,    1.0f, 1.0f, 1.0f),
    //             new Triangle(1.0f, 0.0f, 0.0f,    1.0f, 1.0f, 1.0f,    1.0f, 0.0f, 1.0f),

    //             // NORTH                                                     
    //             new Triangle(1.0f, 0.0f, 1.0f,    1.0f, 1.0f, 1.0f,    0.0f, 1.0f, 1.0f),
    //             new Triangle(1.0f, 0.0f, 1.0f,    0.0f, 1.0f, 1.0f,    0.0f, 0.0f, 1.0f),

    //             // WEST                                                      
    //             new Triangle(0.0f, 0.0f, 1.0f,    0.0f, 1.0f, 1.0f,    0.0f, 1.0f, 0.0f),
    //             new Triangle(0.0f, 0.0f, 1.0f,    0.0f, 1.0f, 0.0f,    0.0f, 0.0f, 0.0f),

    //             // TOP                                                       
    //             new Triangle(0.0f, 1.0f, 0.0f,    0.0f, 1.0f, 1.0f,    1.0f, 1.0f, 1.0f),
    //             new Triangle(0.0f, 1.0f, 0.0f,    1.0f, 1.0f, 1.0f,    1.0f, 1.0f, 0.0f),

    //             // BOTTOM                                                    
    //             new Triangle(1.0f, 0.0f, 1.0f,    0.0f, 0.0f, 1.0f,    0.0f, 0.0f, 0.0f),
    //             new Triangle(1.0f, 0.0f, 1.0f,    0.0f, 0.0f, 0.0f,    1.0f, 0.0f, 0.0f),
    //         });

    //     var heightF = height * scale;
    //     var widthF = width * scale;

    //     var fNear = 0.1f;
	// 	var fFar = 1000.0f;
	// 	var fFov = 90.0f;
	// 	var fAspectRatio = heightF / widthF;
	// 	var fFovRad = 1.0f / MathF.Tan(fFov * 0.5f / 180.0f * 3.14159f);

    //     var matProj = new Mat4x4();
        
	// 	matProj.M[0][0] = fAspectRatio * fFovRad;
	// 	matProj.M[1][1] = fFovRad;
	// 	matProj.M[2][2] = fFar / (fFar - fNear);
	// 	matProj.M[3][2] = (-fFar * fNear) / (fFar - fNear);
	// 	matProj.M[2][3] = 1.0f;
	// 	matProj.M[3][3] = 0.0f;

    //     float fTheta = 0.0f;

    //     /// On loop

    //     var matRotZ = new Mat4x4();

    //     var matRotX = new Mat4x4();

    //     // fTheta += 1.0f * fElapsedTime;

    //     fTheta += 1.0f;

    //     // Rotation Z
	// 	matRotZ.M[0][0] = MathF.Cos(fTheta);
	// 	matRotZ.M[0][1] = MathF.Sin(fTheta);
	// 	matRotZ.M[1][0] = -MathF.Sin(fTheta);
	// 	matRotZ.M[1][1] = MathF.Cos(fTheta);
	// 	matRotZ.M[2][2] = 1;
	// 	matRotZ.M[3][3] = 1;

	// 	// Rotation X
	// 	matRotX.M[0][0] = 1;
	// 	matRotX.M[1][1] = MathF.Cos(fTheta * 0.5f);
	// 	matRotX.M[1][2] = MathF.Sin(fTheta * 0.5f);
	// 	matRotX.M[2][1] = -MathF.Sin(fTheta * 0.5f);
	// 	matRotX.M[2][2] = MathF.Cos(fTheta * 0.5f);
	// 	matRotX.M[3][3] = 1;

    //     ///
        
    //     if (SDL_Init(SDL_INIT_VIDEO) < 0) {

    //         throw new Exception();
    //     }


    //     ///

       

    //     var renderScale = 1.0f;     // used for SDL_RenderSetScale,
    //                                 // if you want to draw at the most granular
    //                                 // available pixel, set this to 1.0
    //                                 // setting to 2.0 will create correct geometry,
    //                                 // but will be blocky if on high DPI 

    //                                 // hence why a secondary 'scale' is needed,
    //                                 // to scale geometry when drawing at finest granularity

    //     var windowFlags = SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI;       

    //     ///

    //     var sdlWindowPtr = SDL_CreateWindow(
    //         title: "foo",
    //         x: SDL_WINDOWPOS_CENTERED_DISPLAY(0),
    //         y: SDL_WINDOWPOS_CENTERED_DISPLAY(0),
    //         width,
    //         height,
    //         windowFlags);

    //     ///

    //     var sdlWindowId = SDL_GetWindowID(sdlWindowPtr);

    //     ///

    //     var sdlRendererPtr = SDL_CreateRenderer(sdlWindowPtr, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

    //     ///

    //     SDL_RenderSetScale(sdlRendererPtr, renderScale, renderScale);

    //     ///

    //     SDL_RenderClear(sdlRendererPtr);

    //     ///

    //     var cubeVerts = new SDL_Vertex[meshCube.Triangles.Count * 3];

    //     var cubeVertIdx = 0;

    //     foreach (var tri in meshCube.Triangles) {

    //         var triProjected = new Triangle();
    //         var triTranslated = new Triangle();
    //         var triRotatedZ = new Triangle();
    //         var triRotatedZX = new Triangle();

    //         // Rotate in Z-Axis
	// 		triRotatedZ.P[0] = MultiplyMatrixVector(tri.P[0], matRotZ);
    //         triRotatedZ.P[1] = MultiplyMatrixVector(tri.P[1], matRotZ);
	// 		triRotatedZ.P[2] = MultiplyMatrixVector(tri.P[2], matRotZ);
            
	// 		// Rotate in X-Axis
	// 		triRotatedZX.P[0] = MultiplyMatrixVector(triRotatedZ.P[0], matRotX);
	// 		triRotatedZX.P[1] = MultiplyMatrixVector(triRotatedZ.P[1], matRotX);
	// 		triRotatedZX.P[2] = MultiplyMatrixVector(triRotatedZ.P[2], matRotX);
            
	// 		// Offset into the screen
	// 		triTranslated = triRotatedZX;
	// 		triTranslated.P[0].Z = triRotatedZX.P[0].Z + 3.0f;
	// 		triTranslated.P[1].Z = triRotatedZX.P[1].Z + 3.0f;
	// 		triTranslated.P[2].Z = triRotatedZX.P[2].Z + 3.0f;
            
	// 		// Project triangles from 3D --> 2D
	// 		triProjected.P[0] = MultiplyMatrixVector(triTranslated.P[0], matProj);
	// 		triProjected.P[1] = MultiplyMatrixVector(triTranslated.P[1], matProj);
	// 		triProjected.P[2] = MultiplyMatrixVector(triTranslated.P[2], matProj);


	// 		// Scale into view
	// 		triProjected.P[0].X += 1.0f; 
    //         triProjected.P[0].Y += 1.0f;
	// 		triProjected.P[1].X += 1.0f; 
    //         triProjected.P[1].Y += 1.0f;
	// 		triProjected.P[2].X += 1.0f; 
    //         triProjected.P[2].Y += 1.0f;
	// 		triProjected.P[0].X *= 0.5f * widthF;
	// 		triProjected.P[0].Y *= 0.5f * heightF;
	// 		triProjected.P[1].X *= 0.5f * widthF;
	// 		triProjected.P[1].Y *= 0.5f * heightF;
	// 		triProjected.P[2].X *= 0.5f * widthF;
	// 		triProjected.P[2].Y *= 0.5f * heightF;

    //         cubeVerts[cubeVertIdx++] = new SDL_Vertex { position = new SDL_FPoint { x = triProjected.P[0].X, y = triProjected.P[0].Y }, color = new SDL_Color { r = 0xff, g = 0x00, b = 0x00, a = 0x00 }, tex_coord = new SDL_FPoint { } };
    //         cubeVerts[cubeVertIdx++] = new SDL_Vertex { position = new SDL_FPoint { x = triProjected.P[1].X, y = triProjected.P[1].Y }, color = new SDL_Color { r = 0x00, g = 0xff, b = 0x00, a = 0x00 }, tex_coord = new SDL_FPoint { } };
    //         cubeVerts[cubeVertIdx++] = new SDL_Vertex { position = new SDL_FPoint { x = triProjected.P[2].X, y = triProjected.P[2].Y }, color = new SDL_Color { r = 0x00, g = 0x00, b = 0xff, a = 0x00 }, tex_coord = new SDL_FPoint { } };
    //     }

    //     SDL_RenderGeometry(sdlRendererPtr, IntPtr.Zero, cubeVerts, cubeVerts.Length, null, 0);

    //     ///

    //     var verts = new SDL_Vertex[] {

    //         new SDL_Vertex { position = new SDL_FPoint { x = 200 * scale, y = 150 * scale }, color = new SDL_Color { r = 0xff, g = 0x00, b = 0x00, a = 0x00 }, tex_coord = new SDL_FPoint { } },
    //         new SDL_Vertex { position = new SDL_FPoint { x = 200 * scale, y = 450 * scale }, color = new SDL_Color { r = 0x00, g = 0x00, b = 0xff, a = 0x00 }, tex_coord = new SDL_FPoint { } },
    //         new SDL_Vertex { position = new SDL_FPoint { x = 600 * scale, y = 450 * scale }, color = new SDL_Color { r = 0x00, g = 0xff, b = 0x00, a = 0x00 }, tex_coord = new SDL_FPoint { } },

    //         new SDL_Vertex { position = new SDL_FPoint { x = 200 * scale, y = 150 * scale }, color = new SDL_Color { r = 0xff, g = 0x00, b = 0x00, a = 0x00 }, tex_coord = new SDL_FPoint { } },
    //         new SDL_Vertex { position = new SDL_FPoint { x = 600 * scale, y = 450 * scale }, color = new SDL_Color { r = 0x00, g = 0xff, b = 0x00, a = 0x00 }, tex_coord = new SDL_FPoint { } },
    //         new SDL_Vertex { position = new SDL_FPoint { x = 600 * scale, y = 150 * scale }, color = new SDL_Color { r = 0x7f, g = 0x7f, b = 0x7f, a = 0x00 }, tex_coord = new SDL_FPoint { } },

    //         // new SDL_Vertex { position = new SDL_FPoint { x = 400 * scale, y = 450 * scale }, color = new SDL_Color { r = 0xff, g = 0xff, b = 0x00, a = 0x00 }, tex_coord = new SDL_FPoint { } }

            
    //     };

    //     // SDL_RenderGeometry(sdlRendererPtr, IntPtr.Zero, verts, verts.Length, null, 0);

    //     ///

    //     SDL_RenderPresent(sdlRendererPtr);









    //     ///

    //     SDL_RaiseWindow(sdlWindowPtr);


    //     ///

    //     var quit = false;

    //     while (!quit) {

    //         SDL_Delay(1000 / 60);

    //         ///

    //         SDL_Event sdlEvent;

    //         while (SDL_PollEvent(out sdlEvent) != 0) {

    //             switch (sdlEvent.type) {

    //                 case SDL_EventType.SDL_QUIT:

    //                     WriteLine($"Quit event");

    //                     quit = true;

    //                     break;

    //                 default:

    //                     break;
    //             }
    //         }

    //         ///
    //     }

    //     ///

    //     // SDL_DestroyTexture(sdlTexturePtr);

    //     ///

    //     SDL_DestroyRenderer(sdlRendererPtr);

    //     ///

    //     SDL_DestroyWindow(sdlWindowPtr);

    //     ///

    //     SDL_Quit();
    // }
}