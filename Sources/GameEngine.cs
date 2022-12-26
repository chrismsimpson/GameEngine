
namespace Triangle;

public sealed class GameEngine: IDisposable {

    public int ScreenWidth { get; init; }

    public int ScreenHeight { get; init; }

    ///

    public float Scale { get; init; }

    public float RenderScale { get; init; }

    ///

    private IntPtr SDLWindowPtr { get; init; }

    private IntPtr SDLRendererPtr { get; init; }

    ///

    private bool Active { get; set; }

    ///

    public GameEngine(
        int screenWidth,
        int screenHeight) {

        this.ScreenWidth = screenWidth;
        this.ScreenHeight = screenHeight;

        this.Scale = 2.0f;

        this.RenderScale = 1.0f;

        ///

        if (SDL_Init(SDL_INIT_VIDEO) < 0) {

            throw new Exception();
        }

        ///

        var windowFlags = SDL_WindowFlags.SDL_WINDOW_SHOWN
            | SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI;
            // | SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;             
        
        this.SDLWindowPtr = SDL_CreateWindow(
            title: "game",
            x: SDL_WINDOWPOS_CENTERED_DISPLAY(0),
            y: SDL_WINDOWPOS_CENTERED_DISPLAY(0),
            this.ScreenWidth,
            this.ScreenHeight,
            windowFlags);

        // this.SDLRendererPtr = SDL_CreateRenderer(this.SDLWindowPtr, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
        this.SDLRendererPtr = SDL_CreateRenderer(this.SDLWindowPtr, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        SDL_RenderSetScale(this.SDLRendererPtr, this.RenderScale, this.RenderScale);

        SDL_RaiseWindow(this.SDLWindowPtr);

        ///

        // this.MeshCube = new Mesh(
        //     new Triangle[] {
                
        //         // SOUTH
        //         new Triangle(0.0f, 0.0f, 0.0f,    0.0f, 1.0f, 0.0f,    1.0f, 1.0f, 0.0f),
        //         new Triangle(0.0f, 0.0f, 0.0f,    1.0f, 1.0f, 0.0f,    1.0f, 0.0f, 0.0f),

        //         // EAST                                                      
        //         new Triangle(1.0f, 0.0f, 0.0f,    1.0f, 1.0f, 0.0f,    1.0f, 1.0f, 1.0f),
        //         new Triangle(1.0f, 0.0f, 0.0f,    1.0f, 1.0f, 1.0f,    1.0f, 0.0f, 1.0f),

        //         // NORTH                                                     
        //         new Triangle(1.0f, 0.0f, 1.0f,    1.0f, 1.0f, 1.0f,    0.0f, 1.0f, 1.0f),
        //         new Triangle(1.0f, 0.0f, 1.0f,    0.0f, 1.0f, 1.0f,    0.0f, 0.0f, 1.0f),

        //         // WEST                                                      
        //         new Triangle(0.0f, 0.0f, 1.0f,    0.0f, 1.0f, 1.0f,    0.0f, 1.0f, 0.0f),
        //         new Triangle(0.0f, 0.0f, 1.0f,    0.0f, 1.0f, 0.0f,    0.0f, 0.0f, 0.0f),

        //         // TOP                                                       
        //         new Triangle(0.0f, 1.0f, 0.0f,    0.0f, 1.0f, 1.0f,    1.0f, 1.0f, 1.0f),
        //         new Triangle(0.0f, 1.0f, 0.0f,    1.0f, 1.0f, 1.0f,    1.0f, 1.0f, 0.0f),

        //         // BOTTOM                                                    
        //         new Triangle(1.0f, 0.0f, 1.0f,    0.0f, 0.0f, 1.0f,    0.0f, 0.0f, 0.0f),
        //         new Triangle(1.0f, 0.0f, 1.0f,    0.0f, 0.0f, 0.0f,    1.0f, 0.0f, 0.0f),
        //     });

        this.MeshCube = new Mesh(filename: "VideoShip.obj");

        
        this.HeightF = this.ScreenHeight * this.Scale;
        this.WidthF = this.ScreenHeight * this.Scale;

        this.fNear = 0.1f;
        this.fFar = 1000.0f;
		this.fFov = 90.0f;
		this.fAspectRatio = this.HeightF / this.WidthF;
		this.fFovRad = 1.0f / MathF.Tan(this.fFov * 0.5f / 180.0f * 3.14159f);

        this.MatProj = new Mat4x4();
        
		MatProj.M[0][0] = this.fAspectRatio * this.fFovRad;
		MatProj.M[1][1] = this.fFovRad;
		MatProj.M[2][2] = this.fFar / (this.fFar - this.fNear);
		MatProj.M[3][2] = (-this.fFar * this.fNear) / (this.fFar - this.fNear);
		MatProj.M[2][3] = 1.0f;
		MatProj.M[3][3] = 0.0f;

        this.fTheta = 0.0f;
        // this.fTheta = 3.6f;

        this.vCamera = new Vec3D(0, 0, 0);

        ///

        this.OnUpdate(0);
    }

    private Mesh MeshCube { get; init; }

    private float HeightF { get; init; }

    private float WidthF { get; init; }

    private float fNear { get; init; }

    private float fFar { get; init; }

    private float fFov { get; init; }

    private float fAspectRatio { get; init; }

    private float fFovRad { get; init; }

    private Mat4x4 MatProj { get; init; }

    private Vec3D vCamera { get; init; }

    private float fTheta { get; set; }

    ///

    public void Run() {

        this.Active = true;

        ///

        if (!this.OnCreate()) {

            this.Active = false;
        }

        ///

        var tp1 = SDL_GetTicks();
        
        var tp2 = tp1;

        ///

        while (this.Active) {

            // SDL_Delay(1000 / 24);

            ///

            tp2 = SDL_GetTicks();
            
            var elapsedTime = Convert.ToSingle(tp2 - tp1) / 1000.0f;

            tp1 = tp2;

            ///

            SDL_Event sdlEvent;

            while (SDL_PollEvent(out sdlEvent) != 0) {

                switch (sdlEvent.type) {

                    case SDL_EventType.SDL_QUIT:

                        this.Active = false;

                        break;

                    default:

                        break;
                }
            }

            ///

            this.OnUpdate(elapsedTime);
        }
    }


    public bool OnCreate() {

        return true;
    }

    public void OnUpdate(
        float elapsedTime) {

        SDL_SetRenderDrawColor(this.SDLRendererPtr, 0x00, 0x00, 0x00, 0xff);

        SDL_RenderClear(this.SDLRendererPtr);

        /// On loop

        var matRotZ = new Mat4x4();

        var matRotX = new Mat4x4();

        this.fTheta += 1.0f * elapsedTime;
        // this.fTheta += 0.1f * elapsedTime;

        // Rotation Z
		matRotZ.M[0][0] = MathF.Cos(this.fTheta);
		matRotZ.M[0][1] = MathF.Sin(this.fTheta);
		matRotZ.M[1][0] = -MathF.Sin(this.fTheta);
		matRotZ.M[1][1] = MathF.Cos(this.fTheta);
		matRotZ.M[2][2] = 1;
		matRotZ.M[3][3] = 1;

		// Rotation X
		matRotX.M[0][0] = 1;
		matRotX.M[1][1] = MathF.Cos(this.fTheta * 0.5f);
		matRotX.M[1][2] = MathF.Sin(this.fTheta * 0.5f);
		matRotX.M[2][1] = -MathF.Sin(this.fTheta * 0.5f);
		matRotX.M[2][2] = MathF.Cos(this.fTheta * 0.5f);
		matRotX.M[3][3] = 1;

        var cubeVertCount = this.MeshCube.Triangles.Length * 3;

        var cubeVerts = new SDL_Vertex[cubeVertCount];

        var cubeVertIdx = 0;

        var cubeLineCount = this.MeshCube.Triangles.Length * 6;

        var cubeLines = new SDL_FPoint[cubeLineCount];

        var cubeLineIdx = 0;

        var ffFloat = Convert.ToSingle(0xff);

        var zeroTextCoord = new SDL_FPoint { };

        var vecTrianglesToRender = new List<Triangle>();

        foreach (var tri in this.MeshCube.Triangles) {

            var triProjected = new Triangle();
            var triTranslated = new Triangle();
            var triRotatedZ = new Triangle();
            var triRotatedZX = new Triangle();

            // Rotate in Z-Axis
			triRotatedZ.P[0] = this.MultiplyMatrixVector(tri.P[0], matRotZ);
            triRotatedZ.P[1] = this.MultiplyMatrixVector(tri.P[1], matRotZ);
			triRotatedZ.P[2] = this.MultiplyMatrixVector(tri.P[2], matRotZ);
            
			// Rotate in X-Axis
			triRotatedZX.P[0] = this.MultiplyMatrixVector(triRotatedZ.P[0], matRotX);
			triRotatedZX.P[1] = this.MultiplyMatrixVector(triRotatedZ.P[1], matRotX);
			triRotatedZX.P[2] = this.MultiplyMatrixVector(triRotatedZ.P[2], matRotX);
            
			// Offset into the screen
			triTranslated = triRotatedZX;
			triTranslated.P[0].Z = triRotatedZX.P[0].Z + 8.0f;
			triTranslated.P[1].Z = triRotatedZX.P[1].Z + 8.0f;
			triTranslated.P[2].Z = triRotatedZX.P[2].Z + 8.0f;

            // calc normal
            var normal = new Vec3D();
            var line1 = new Vec3D();
            var line2 = new Vec3D();

            line1.X = triTranslated.P[1].X - triTranslated.P[0].X;
            line1.Y = triTranslated.P[1].Y - triTranslated.P[0].Y;
            line1.Z = triTranslated.P[1].Z - triTranslated.P[0].Z;

            line2.X = triTranslated.P[2].X - triTranslated.P[0].X;
            line2.Y = triTranslated.P[2].Y - triTranslated.P[0].Y;
            line2.Z = triTranslated.P[2].Z - triTranslated.P[0].Z;

            normal.X = line1.Y * line2.Z - line1.Z * line2.Y;
            normal.Y = line1.Z * line2.X - line1.X * line2.Z;
            normal.Z = line1.X * line2.Y - line1.Y * line2.X;

            var l = MathF.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);

            // normal.X /= l;
            // normal.Y /= l;
            // normal.Z /= l;

            normal.X = l == 0
                ? normal.X > 0
                    ? float.MaxValue
                    : float.MinValue
                : normal.X / l;

            normal.Y = l == 0
                ? normal.Y > 0
                    ? float.MaxValue
                    : float.MinValue
                : normal.Y / l;

            normal.Z = l == 0
                ? normal.Z > 0
                    ? float.MaxValue
                    : float.MinValue
                : normal.Z / l;


            

            if (normal.X * (triTranslated.P[0].X - this.vCamera.X) +
                normal.Y * (triTranslated.P[0].Y - this.vCamera.Y) +
                normal.Z * (triTranslated.P[0].Z - this.vCamera.Z) >= 0) {

                continue;
            }

            // Illumination

            var lightDirection = new Vec3D(0, 0, -1.0f);

            var ll = MathF.Sqrt(lightDirection.X * lightDirection.X + lightDirection.Y * lightDirection.Y + lightDirection.Z * lightDirection.Z);

            // lightDirection.X /= ll;
            // lightDirection.Y /= ll;
            // lightDirection.Z /= ll;

            lightDirection.X = l == 0
                ? lightDirection.X > 0
                    ? float.MaxValue
                    : float.MinValue
                : lightDirection.X / l;

            lightDirection.Y = l == 0
                ? lightDirection.Y > 0
                    ? float.MaxValue
                    : float.MinValue
                : lightDirection.Y / l;

            lightDirection.Z = l == 0
                ? lightDirection.Z > 0
                    ? float.MaxValue
                    : float.MinValue
                : lightDirection.Z / l;


            var dp = normal.X * lightDirection.X + normal.Y * lightDirection.Y + normal.Z * lightDirection.Z;

            
            
			// Project triangles from 3D --> 2D
			triProjected.P[0] = this.MultiplyMatrixVector(triTranslated.P[0], this.MatProj);
			triProjected.P[1] = this.MultiplyMatrixVector(triTranslated.P[1], this.MatProj);
			triProjected.P[2] = this.MultiplyMatrixVector(triTranslated.P[2], this.MatProj);


			// Scale into view
			triProjected.P[0].X += 1.0f; 
            triProjected.P[0].Y += 1.0f;
			triProjected.P[1].X += 1.0f; 
            triProjected.P[1].Y += 1.0f;
			triProjected.P[2].X += 1.0f; 
            triProjected.P[2].Y += 1.0f;
			triProjected.P[0].X *= 0.5f * this.WidthF;
			triProjected.P[0].Y *= 0.5f * this.HeightF;
			triProjected.P[1].X *= 0.5f * this.WidthF;
			triProjected.P[1].Y *= 0.5f * this.HeightF;
			triProjected.P[2].X *= 0.5f * this.WidthF;
			triProjected.P[2].Y *= 0.5f * this.HeightF;

            // cubeVerts[cubeVertIdx++] = new SDL_Vertex { position = new SDL_FPoint { x = triProjected.P[0].X, y = triProjected.P[0].Y }, color = new SDL_Color { r = 0xff, g = 0x00, b = 0x00, a = 0x00 }, tex_coord = new SDL_FPoint { } };
            // cubeVerts[cubeVertIdx++] = new SDL_Vertex { position = new SDL_FPoint { x = triProjected.P[1].X, y = triProjected.P[1].Y }, color = new SDL_Color { r = 0x00, g = 0xff, b = 0x00, a = 0x00 }, tex_coord = new SDL_FPoint { } };
            // cubeVerts[cubeVertIdx++] = new SDL_Vertex { position = new SDL_FPoint { x = triProjected.P[2].X, y = triProjected.P[2].Y }, color = new SDL_Color { r = 0x00, g = 0x00, b = 0xff, a = 0x00 }, tex_coord = new SDL_FPoint { } };

            new Triangle(triProjected.P[0], triProjected.P[1], triProjected.P[2]);

            var p1 = new SDL_FPoint { x = triProjected.P[0].X, y = triProjected.P[0].Y };
            var p2 = new SDL_FPoint { x = triProjected.P[1].X, y = triProjected.P[1].Y };
            var p3 = new SDL_FPoint { x = triProjected.P[2].X, y = triProjected.P[2].Y };

            var shade = Convert.ToByte(ffFloat * Math.Max(0.0, MathF.Min(1.0f, dp)));

            var color = new SDL_Color { r = shade, g = shade, b = shade, a = 0xff };

            cubeVerts[cubeVertIdx++] = new SDL_Vertex { position = p1, color = color, tex_coord = zeroTextCoord };
            cubeVerts[cubeVertIdx++] = new SDL_Vertex { position = p2, color = color, tex_coord = zeroTextCoord };
            cubeVerts[cubeVertIdx++] = new SDL_Vertex { position = p3, color = color, tex_coord = zeroTextCoord };

            cubeLines[cubeLineIdx++] = p1;
            cubeLines[cubeLineIdx++] = p2;

            cubeLines[cubeLineIdx++] = p2;
            cubeLines[cubeLineIdx++] = p3;

            cubeLines[cubeLineIdx++] = p3;
            cubeLines[cubeLineIdx++] = p1;
        }



        

        SDL_RenderGeometry(this.SDLRendererPtr, IntPtr.Zero, cubeVerts, cubeVertIdx, null, 0);

        ///

        SDL_SetRenderDrawColor(this.SDLRendererPtr, 0xff, 0x00, 0x00, 0xff);

        for (var l = 0; l < cubeLineIdx;) {

            // switch (l % 3) {

            //     case 2:

            //         SDL_SetRenderDrawColor(this.SDLRendererPtr, 0xff, 0x00, 0x00, 0xff);

            //         break;

            //     case 1:
                
            //         SDL_SetRenderDrawColor(this.SDLRendererPtr, 0x00, 0xff, 0x00, 0xff);

            //         break;

            //     default:

            //         SDL_SetRenderDrawColor(this.SDLRendererPtr, 0x00, 0x00, 0xff, 0xff);

            //         break;
            // }


            var s = cubeLines[l++];

            var e = cubeLines[l++];

            SDL_RenderDrawLineF(this.SDLRendererPtr, s.x, s.y, e.x, e.y);
        }

        SDL_RenderPresent(this.SDLRendererPtr);

        ///

        var t = $"Triangles - fps: {MathF.Round(1.0f / elapsedTime, 0).ToString("F1")}, theta: {MathF.Round(this.fTheta, 2).ToString("F2")}";

        // WriteLine(t);

        SDL_SetWindowTitle(this.SDLWindowPtr, t);
    }

    ///


    public Vec3D MultiplyMatrixVector(in Vec3D i, in Mat4x4 m) {

        var x = i.X * m.M[0][0] + i.Y * m.M[1][0] + i.Z * m.M[2][0] + m.M[3][0];

		var y = i.X * m.M[0][1] + i.Y * m.M[1][1] + i.Z * m.M[2][1] + m.M[3][1];

		var z = i.X * m.M[0][2] + i.Y * m.M[1][2] + i.Z * m.M[2][2] + m.M[3][2];
		
        var w = i.X * m.M[0][3] + i.Y * m.M[1][3] + i.Z * m.M[2][3] + m.M[3][3];

		if (w != 0.0f) {

			x /= w; 
            y /= w; 
            z /= w;
		}

        return new Vec3D(x, y, z);
    }

    ///

    ///

    public void Dispose() {

        SDL_Quit();
    }
}