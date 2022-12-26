
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
        
        this.SDLWindowPtr = SDL_CreateWindow(
            title: "game",
            x: SDL_WINDOWPOS_CENTERED_DISPLAY(0),
            y: SDL_WINDOWPOS_CENTERED_DISPLAY(0),
            this.ScreenWidth,
            this.ScreenHeight,
            windowFlags);

        this.SDLRendererPtr = SDL_CreateRenderer(this.SDLWindowPtr, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

        SDL_RenderSetScale(this.SDLRendererPtr, this.RenderScale, this.RenderScale);

        SDL_RaiseWindow(this.SDLWindowPtr);

        ///

        this.MeshCube = new Mesh(
            new Triangle[] {
                
                // SOUTH
                new Triangle(0.0f, 0.0f, 0.0f,    0.0f, 1.0f, 0.0f,    1.0f, 1.0f, 0.0f),
                new Triangle(0.0f, 0.0f, 0.0f,    1.0f, 1.0f, 0.0f,    1.0f, 0.0f, 0.0f),

                // EAST                                                      
                new Triangle(1.0f, 0.0f, 0.0f,    1.0f, 1.0f, 0.0f,    1.0f, 1.0f, 1.0f),
                new Triangle(1.0f, 0.0f, 0.0f,    1.0f, 1.0f, 1.0f,    1.0f, 0.0f, 1.0f),

                // NORTH                                                     
                new Triangle(1.0f, 0.0f, 1.0f,    1.0f, 1.0f, 1.0f,    0.0f, 1.0f, 1.0f),
                new Triangle(1.0f, 0.0f, 1.0f,    0.0f, 1.0f, 1.0f,    0.0f, 0.0f, 1.0f),

                // WEST                                                      
                new Triangle(0.0f, 0.0f, 1.0f,    0.0f, 1.0f, 1.0f,    0.0f, 1.0f, 0.0f),
                new Triangle(0.0f, 0.0f, 1.0f,    0.0f, 1.0f, 0.0f,    0.0f, 0.0f, 0.0f),

                // TOP                                                       
                new Triangle(0.0f, 1.0f, 0.0f,    0.0f, 1.0f, 1.0f,    1.0f, 1.0f, 1.0f),
                new Triangle(0.0f, 1.0f, 0.0f,    1.0f, 1.0f, 1.0f,    1.0f, 1.0f, 0.0f),

                // BOTTOM                                                    
                new Triangle(1.0f, 0.0f, 1.0f,    0.0f, 0.0f, 1.0f,    0.0f, 0.0f, 0.0f),
                new Triangle(1.0f, 0.0f, 1.0f,    0.0f, 0.0f, 0.0f,    1.0f, 0.0f, 0.0f),
            });

        
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

    private float fTheta { get; set; }

    ///

    public void Run() {

        this.Active = true;

        ///

        if (!this.OnCreate()) {

            this.Active = false;
        }

        ///

        var tp1 = System.Environment.TickCount;
        
        var tp2 = tp1;

        ///

        while (this.Active) {

            // delay?
            SDL_Delay(1000 / 60);

            ///

            tp2 = System.Environment.TickCount;

            // var elapsedTime = tp2 - tp1;

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

        SDL_RenderClear(this.SDLRendererPtr);

        /// On loop

        var matRotZ = new Mat4x4();

        var matRotX = new Mat4x4();

        // this.fTheta += 1.0f * elapsedTime;
        this.fTheta += elapsedTime;

        // this.fTheta += 1.0f;

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

        var cubeVerts = new SDL_Vertex[this.MeshCube.Triangles.Count * 3];

        var cubeVertIdx = 0;

        foreach (var tri in this.MeshCube.Triangles) {

            var triProjected = new Triangle();
            var triTranslated = new Triangle();
            var triRotatedZ = new Triangle();
            var triRotatedZX = new Triangle();

            // Rotate in Z-Axis
			triRotatedZ.P[0] = Program.MultiplyMatrixVector(tri.P[0], matRotZ);
            triRotatedZ.P[1] = Program.MultiplyMatrixVector(tri.P[1], matRotZ);
			triRotatedZ.P[2] = Program.MultiplyMatrixVector(tri.P[2], matRotZ);
            
			// Rotate in X-Axis
			triRotatedZX.P[0] = Program.MultiplyMatrixVector(triRotatedZ.P[0], matRotX);
			triRotatedZX.P[1] = Program.MultiplyMatrixVector(triRotatedZ.P[1], matRotX);
			triRotatedZX.P[2] = Program.MultiplyMatrixVector(triRotatedZ.P[2], matRotX);
            
			// Offset into the screen
			triTranslated = triRotatedZX;
			triTranslated.P[0].Z = triRotatedZX.P[0].Z + 3.0f;
			triTranslated.P[1].Z = triRotatedZX.P[1].Z + 3.0f;
			triTranslated.P[2].Z = triRotatedZX.P[2].Z + 3.0f;
            
			// Project triangles from 3D --> 2D
			triProjected.P[0] = Program.MultiplyMatrixVector(triTranslated.P[0], this.MatProj);
			triProjected.P[1] = Program.MultiplyMatrixVector(triTranslated.P[1], this.MatProj);
			triProjected.P[2] = Program.MultiplyMatrixVector(triTranslated.P[2], this.MatProj);


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

            cubeVerts[cubeVertIdx++] = new SDL_Vertex { position = new SDL_FPoint { x = triProjected.P[0].X, y = triProjected.P[0].Y }, color = new SDL_Color { r = 0xff, g = 0x00, b = 0x00, a = 0x00 }, tex_coord = new SDL_FPoint { } };
            cubeVerts[cubeVertIdx++] = new SDL_Vertex { position = new SDL_FPoint { x = triProjected.P[1].X, y = triProjected.P[1].Y }, color = new SDL_Color { r = 0x00, g = 0xff, b = 0x00, a = 0x00 }, tex_coord = new SDL_FPoint { } };
            cubeVerts[cubeVertIdx++] = new SDL_Vertex { position = new SDL_FPoint { x = triProjected.P[2].X, y = triProjected.P[2].Y }, color = new SDL_Color { r = 0x00, g = 0x00, b = 0xff, a = 0x00 }, tex_coord = new SDL_FPoint { } };
        }

        SDL_RenderGeometry(this.SDLRendererPtr, IntPtr.Zero, cubeVerts, cubeVerts.Length, null, 0);

        SDL_RenderPresent(this.SDLRendererPtr);

        // WriteLine($"Elapsed time: {elapsedTime}");
    }


    ///

    public void Dispose() {

        SDL_Quit();
    }
}