
namespace Triangle;

public struct KeyboardState { 

    public byte[] Keys { get; init; }

    public KeyboardState() {

        throw new Exception();
    }

    public KeyboardState(
        byte[] keys) {

        this.Keys = keys;
    }

    public bool ContainsKey(
        SDL_Keycode sdlKeyCode) {

        return this.Keys[(byte) SDL_GetScancodeFromKey(sdlKeyCode)] == 1;
    }
}

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

        this.HeightF = this.ScreenHeight * this.Scale;
        this.WidthF = this.ScreenHeight * this.Scale;

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

        this.SDLRendererPtr = SDL_CreateRenderer(this.SDLWindowPtr, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        SDL_RenderSetScale(this.SDLRendererPtr, this.RenderScale, this.RenderScale);

        SDL_RaiseWindow(this.SDLWindowPtr);

        ///

        this.MatProj = new Mat4x4();

        this.vCamera = new Vec3D(0, 0, 0);

        ///

        // this.OnUpdate(0);
    }

    private Mesh? Mesh { get; set; }

    private float HeightF { get; set; } = 0;

    private float WidthF { get; set; } = 0;

    // private float fNear { get; set; } = 0;

    // private float fFar { get; set; } = 0;

    // private float fFov { get; set; } = 0;

    // private float fAspectRatio { get; set; } = 0;

    // private float fFovRad { get; set; } = 0;

    private Mat4x4 MatProj { get; set; }

    private Vec3D vCamera { get; set; }

    private Vec3D vLookDir { get; set; }

    private float fYaw { get; set; }

    private float fTheta { get; set; } = 0.0f;

    ///

    private bool RenderWireframes { get; set; } = true;
    // private bool RenderWireframes { get; set; } = false;

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

        // this.Mesh = new Mesh(filename: "VideoShip.obj");
        // this.Mesh = new Mesh(filename: "teapot.obj");
        this.Mesh = new Mesh(filename: "axis.obj");
        
        this.MatProj = this.MatrixMakeProjection(
            90.0f, 
            this.HeightF / this.WidthF,
            0.1f, 
            1000.0f);


        // this.fTheta = 3.6f;

        

        return true;
    }

    public KeyboardState GetKeyboardState() {

        int arraySize;
        
        IntPtr origArray = SDL_GetKeyboardState(out arraySize);
        
        byte[] keys = new byte[arraySize];
        
        Marshal.Copy(origArray, keys, 0, arraySize);

        return new KeyboardState(keys);
    }

    public void OnUpdate(
        float elapsedTime) {

        var keyboardState = this.GetKeyboardState();

        if (keyboardState.ContainsKey(SDL_Keycode.SDLK_UP)) {

            this.vCamera = new Vec3D(this.vCamera.X, this.vCamera.Y + (8.0f * elapsedTime), this.vCamera.Z, this.vCamera.W);
        }

        if (keyboardState.ContainsKey(SDL_Keycode.SDLK_DOWN)) {
        
            this.vCamera = new Vec3D(this.vCamera.X, this.vCamera.Y - (8.0f * elapsedTime), this.vCamera.Z, this.vCamera.W);
        }

        if (keyboardState.ContainsKey(SDL_Keycode.SDLK_LEFT)) {

            this.vCamera = new Vec3D(this.vCamera.X + (8.0f * elapsedTime), this.vCamera.Y, this.vCamera.Z, this.vCamera.W);
        }

        if (keyboardState.ContainsKey(SDL_Keycode.SDLK_RIGHT)) {
        
            this.vCamera = new Vec3D(this.vCamera.X - (8.0f * elapsedTime), this.vCamera.Y, this.vCamera.Z, this.vCamera.W);
        }

        var vForward = this.VectorMul(this.vLookDir, 8.0f * elapsedTime);

        if (keyboardState.ContainsKey(SDL_Keycode.SDLK_w)) {
        
            this.vCamera = VectorAdd(this.vCamera, vForward);
        }

        if (keyboardState.ContainsKey(SDL_Keycode.SDLK_s)) {
        
            this.vCamera = VectorSub(this.vCamera, vForward);
        }

        if (keyboardState.ContainsKey(SDL_Keycode.SDLK_a)) {
        
            this.fYaw -= 2.0f * elapsedTime;
        }
        
        if (keyboardState.ContainsKey(SDL_Keycode.SDLK_d)) {
        
            this.fYaw += 2.0f * elapsedTime;
        }
        

        ///

        SDL_SetRenderDrawColor(this.SDLRendererPtr, 0x00, 0x00, 0x00, 0xff);

        SDL_RenderClear(this.SDLRendererPtr);

        // Uncomment to spin me right round baby right round

        // this.fTheta += 1.0f * elapsedTime;

        ///

        // Set up "World Tranmsform" though not updating theta 
        // makes this a bit redundant

        var matRotZ = MatrixMakeRotationZ(this.fTheta * 0.5f);

        var matRotX = MatrixMakeRotationX(this.fTheta);

        ///

        var matTrans = MatrixMakeTranslation(0.0f, 0.0f, 5.0f);

        var matWorld = MatrixMakeIdentity();
        matWorld = MatrixMultiplyMatrix(matRotZ, matRotX);
        matWorld = MatrixMultiplyMatrix(matWorld, matTrans);

        var vUp = new Vec3D(0.0f, 1.0f, 0.0f);

        var vTarget = new Vec3D(0.0f, 0.0f, 1.0f);

        var matCameraRot = this.MatrixMakeRotationY(this.fYaw);

        this.vLookDir = MatrixMultiplyVector(matCameraRot, vTarget);

        vTarget = VectorAdd(this.vCamera, this.vLookDir);

        var matCamera = this.MatrixPointAt(this.vCamera, vTarget, vUp);

        var matView = this.MatrixQuickInverse(matCamera);

        var ffFloat = Convert.ToSingle(0xff);

        // Store triangles for rasterizing later
        var vecTrianglesToRender = new List<Triangle>();       

        if (this.Mesh is Mesh mesh) {

            for (var i = 0; i < mesh.Triangles.Length; ++i)  {

                var tri = mesh.Triangles[i];

                ///

                var triProjected = new Triangle();
                
                var triTransformed = new Triangle();

                var triViewed = new Triangle();

                // World Matrix Transform
                triTransformed.P[0] = this.MatrixMultiplyVector(matWorld, tri.P[0]);
                triTransformed.P[1] = this.MatrixMultiplyVector(matWorld, tri.P[1]);
                triTransformed.P[2] = this.MatrixMultiplyVector(matWorld, tri.P[2]);

                // Calculate triangle Normal
                // Get lines either side of triangle
                var line1 = VectorSub(triTransformed.P[1], triTransformed.P[0]);
                var line2 = VectorSub(triTransformed.P[2], triTransformed.P[0]);

                // Take cross product of lines to get normal to triangle surface
                var normal = VectorCrossProduct(line1, line2);

                // You normally need to normalise a normal!
                normal = VectorNormalise(normal);

                // Get Ray from triangle to camera

                var vCameraRay = this.VectorSub(triTransformed.P[0], this.vCamera);

                // If ray is aligned with normal, then triangle is visible
                if (this.VectorDotProduct(normal, vCameraRay) < 0.0f) {

                    // Illumination

                    var lightDirection = new Vec3D(0.0f, 1.0f, -1.0f);
                    lightDirection = VectorNormalise(lightDirection);

                    // How "aligned" are light direction and triangle surface normal?
                    var dp = MathF.Max(0.1f, this.VectorDotProduct(lightDirection, normal));

                    byte shade = Convert.ToByte(255.0f * dp);

                    triTransformed.Color = new SDL_Color { r = shade, g = shade, b = shade, a = 0xff };

                    // Convert World Space --> View space 
                    triViewed.P[0] = this.MatrixMultiplyVector(matView, triTransformed.P[0]);
                    triViewed.P[1] = this.MatrixMultiplyVector(matView, triTransformed.P[1]);
                    triViewed.P[2] = this.MatrixMultiplyVector(matView, triTransformed.P[2]);
                    triViewed.Color = triTransformed.Color;

                    // Clip Viewed Triangle against near plane, this could form two additional
                    // additional triangles.
                    
                    var clippedTriangles = 0;

                    // var clipped = new Triangle[2];

                    var clipped = new [] {
                        new Triangle(),
                        new Triangle()
                    }; // this is to create and zero out P arrays so as to not create a null ref exception

                    clippedTriangles = TriangleClipAgainstPlane(new Vec3D(0.0f, 0.0f, 0.1f), new Vec3D(0.0f, 0.0f, 1.0f), triViewed, ref clipped[0], ref clipped[1]);

                    for (var n = 0; n < clippedTriangles; n++) {

                        // Project triangles from 3D --> 2D
                        triProjected.P[0] = this.MatrixMultiplyVector(this.MatProj, clipped[n].P[0]);
                        triProjected.P[1] = this.MatrixMultiplyVector(this.MatProj, clipped[n].P[1]);
                        triProjected.P[2] = this.MatrixMultiplyVector(this.MatProj, clipped[n].P[2]);
                        triProjected.Color = clipped[n].Color;

                        // Scale into view, we moved the normalising into cartesian space
                        // out of the matrix.vector function from the previous videos, so
                        // do this manually
                        triProjected.P[0] = VectorDiv(triProjected.P[0], triProjected.P[0].W);
                        triProjected.P[1] = VectorDiv(triProjected.P[1], triProjected.P[1].W);
                        triProjected.P[2] = VectorDiv(triProjected.P[2], triProjected.P[2].W);

                        // X/Y are inverted so put them back
                        triProjected.P[0].X *= -1.0f;
                        triProjected.P[1].X *= -1.0f;
                        triProjected.P[2].X *= -1.0f;
                        triProjected.P[0].Y *= -1.0f;
                        triProjected.P[1].Y *= -1.0f;
                        triProjected.P[2].Y *= -1.0f;

                        // Offset verts into visible normalised space
                        var vOffsetView = new Vec3D(1.0f, 1.0f, 0.0f);

                        triProjected.P[0] = VectorAdd(triProjected.P[0], vOffsetView);
                        triProjected.P[1] = VectorAdd(triProjected.P[1], vOffsetView);
                        triProjected.P[2] = VectorAdd(triProjected.P[2], vOffsetView);

                        triProjected.P[0].X *= 0.5f * this.WidthF;
                        triProjected.P[0].Y *= 0.5f * this.HeightF;
                        triProjected.P[1].X *= 0.5f * this.WidthF;
                        triProjected.P[1].Y *= 0.5f * this.HeightF;
                        triProjected.P[2].X *= 0.5f * this.WidthF;
                        triProjected.P[2].Y *= 0.5f * this.HeightF;

                        // Store triangle for sorting
                        // vecTrianglesToRender.Add(new Triangle(triProjected.P[0], triProjected.P[1], triProjected.P[2], shade));
                        vecTrianglesToRender.Add(triProjected);
                    }
                }

            }
        }

        ///

        vecTrianglesToRender.Sort((t1, t2) => t2.MidZ.CompareTo(t1.MidZ));

        // vecTrianglesToRender.Sort((t1, t2) => t1.MidZ > t2.MidZ ? -1 : 1);

        ///

        var zeroTextCoord = new SDL_FPoint { };

        SDL_SetRenderDrawColor(this.SDLRendererPtr, 0xff, 0x00, 0x00, 0xff); // line color

        ///

        // foreach (var triProjected in vecTrianglesToRender) {

        //     var p1 = new SDL_FPoint { x = triProjected.P[0].X, y = triProjected.P[0].Y };
        //     var p2 = new SDL_FPoint { x = triProjected.P[1].X, y = triProjected.P[1].Y };
        //     var p3 = new SDL_FPoint { x = triProjected.P[2].X, y = triProjected.P[2].Y };

        //     // var color = new SDL_Color { r = triProjected.Shade, g = triProjected.Shade, b = triProjected.Shade, a = 0xff };

        //     var verts = new [] {
        //         new SDL_Vertex { position = p1, color = triProjected.Color, tex_coord = zeroTextCoord },
        //         new SDL_Vertex { position = p2, color = triProjected.Color, tex_coord = zeroTextCoord },
        //         new SDL_Vertex { position = p3, color = triProjected.Color, tex_coord = zeroTextCoord }
        //     };

        //     SDL_RenderGeometry(this.SDLRendererPtr, IntPtr.Zero, verts, 3, null, 0);

        //     if (this.RenderWireframes) {

        //         SDL_RenderDrawLineF(this.SDLRendererPtr, p1.x, p1.y, p2.x, p2.y);
        //         SDL_RenderDrawLineF(this.SDLRendererPtr, p2.x, p2.y, p3.x, p3.y);
        //         SDL_RenderDrawLineF(this.SDLRendererPtr, p3.x, p3.y, p1.x, p1.y);
        //     }
        // }

        foreach (var triToRaster in vecTrianglesToRender) {

            var clipped = new [] {
                new Triangle(),
                new Triangle()
            };

            var listTriangles = new List<Triangle>();

            listTriangles.Add(triToRaster);

            var newTriangels = 1;

            for (var p = 0; p < 4; p++) {

                var trisToAdd = 0;

                while (newTriangels > 0) {

                    var test = listTriangles.First();
                    listTriangles.RemoveAt(0);
                    newTriangels--;

                    switch (p) {

                        case 0:	{

                            trisToAdd = TriangleClipAgainstPlane(new Vec3D(0.0f, 0.0f, 0.0f), new Vec3D(0.0f, 1.0f, 0.0f), test, ref clipped[0], ref clipped[1]); 
                            
                            break;
                        }

                        case 1:	{

                            trisToAdd = TriangleClipAgainstPlane(new Vec3D(0.0f, this.HeightF - 1, 0.0f), new Vec3D(0.0f, -1.0f, 0.0f), test, ref clipped[0], ref clipped[1]); 
                            
                            break;
                        }

                        case 2:	{

                            trisToAdd = TriangleClipAgainstPlane(new Vec3D(0.0f, 0.0f, 0.0f), new Vec3D(1.0f, 0.0f, 0.0f), test, ref clipped[0], ref clipped[1]); 
                            
                            break;
                        }

                        case 3:	{

                            trisToAdd = TriangleClipAgainstPlane(new Vec3D(this.WidthF - 1, 0.0f, 0.0f), new Vec3D(-1.0f, 0.0f, 0.0f), test, ref clipped[0], ref clipped[1]); 
                            
                            break;
                        }
                    }

                    for (int w = 0; w < trisToAdd; w++) {

                        listTriangles.Add(clipped[w]);
                    }
                }

                newTriangels = listTriangles.Count;
            }

            foreach (var ft in listTriangles) {

                var p1 = new SDL_FPoint { x = ft.P[0].X, y = ft.P[0].Y };
                var p2 = new SDL_FPoint { x = ft.P[1].X, y = ft.P[1].Y };
                var p3 = new SDL_FPoint { x = ft.P[2].X, y = ft.P[2].Y };

                var verts = new [] {
                    new SDL_Vertex { position = p1, color = ft.Color, tex_coord = zeroTextCoord },
                    new SDL_Vertex { position = p2, color = ft.Color, tex_coord = zeroTextCoord },
                    new SDL_Vertex { position = p3, color = ft.Color, tex_coord = zeroTextCoord }
                };

                SDL_RenderGeometry(this.SDLRendererPtr, IntPtr.Zero, verts, 3, null, 0);

                if (this.RenderWireframes) {

                    SDL_RenderDrawLineF(this.SDLRendererPtr, p1.x, p1.y, p2.x, p2.y);
                    SDL_RenderDrawLineF(this.SDLRendererPtr, p2.x, p2.y, p3.x, p3.y);
                    SDL_RenderDrawLineF(this.SDLRendererPtr, p3.x, p3.y, p1.x, p1.y);
                }
            }
             

        }

        SDL_RenderPresent(this.SDLRendererPtr);

        ///

        var t = $"Triangles - fps: {MathF.Round(1.0f / elapsedTime, 0).ToString("F1")}, theta: {MathF.Round(this.fTheta, 2).ToString("F2")}";

        // WriteLine(t);

        SDL_SetWindowTitle(this.SDLWindowPtr, t);
    }

    ///


    // public Vec3D MultiplyMatrixVector(in Vec3D i, in Mat4x4 m) {

    //     var x = i.X * m.M[0][0] + i.Y * m.M[1][0] + i.Z * m.M[2][0] + m.M[3][0];

    //     var y = i.X * m.M[0][1] + i.Y * m.M[1][1] + i.Z * m.M[2][1] + m.M[3][1];

    //     var z = i.X * m.M[0][2] + i.Y * m.M[1][2] + i.Z * m.M[2][2] + m.M[3][2];
        
    //     var w = i.X * m.M[0][3] + i.Y * m.M[1][3] + i.Z * m.M[2][3] + m.M[3][3];

    //     if (w != 0.0f) {

    //         x /= w; 
    //         y /= w; 
    //         z /= w;
    //     }

    //     return new Vec3D(x, y, z);
    // }

    public Vec3D MatrixMultiplyVector(in Mat4x4 m, in Vec3D i) {

        return new Vec3D(
            x: i.X * m.M[0][0] + i.Y * m.M[1][0] + i.Z * m.M[2][0] + m.M[3][0],
            y: i.X * m.M[0][1] + i.Y * m.M[1][1] + i.Z * m.M[2][1] + m.M[3][1],
            z: i.X * m.M[0][2] + i.Y * m.M[1][2] + i.Z * m.M[2][2] + m.M[3][2],
            w: i.X * m.M[0][3] + i.Y * m.M[1][3] + i.Z * m.M[2][3] + m.M[3][3]);
    }

    public Mat4x4 MatrixMakeIdentity() {

        var matrix = new Mat4x4();
        matrix.M[0][0] = 1.0f;
        matrix.M[1][1] = 1.0f;
        matrix.M[2][2] = 1.0f;
        matrix.M[3][3] = 1.0f;
        return matrix;
    }

    public Mat4x4 MatrixMakeRotationX(float fAngleRad) {

        var matrix = new Mat4x4();
        matrix.M[0][0] = 1.0f;
        matrix.M[1][1] = MathF.Cos(fAngleRad);
        matrix.M[1][2] = MathF.Sin(fAngleRad);
        matrix.M[2][1] = -MathF.Sin(fAngleRad);
        matrix.M[2][2] = MathF.Cos(fAngleRad);
        matrix.M[3][3] = 1.0f;
        return matrix;
    }

    public Mat4x4 MatrixMakeRotationY(float fAngleRad) {

        var matrix = new Mat4x4();
        matrix.M[0][0] = MathF.Cos(fAngleRad);
        matrix.M[0][2] = MathF.Sin(fAngleRad);
        matrix.M[2][0] = -MathF.Sin(fAngleRad);
        matrix.M[1][1] = 1.0f;
        matrix.M[2][2] = MathF.Cos(fAngleRad);
        matrix.M[3][3] = 1.0f;
        return matrix;
    }

    public Mat4x4 MatrixMakeRotationZ(float fAngleRad) {

        var matrix = new Mat4x4();
        matrix.M[0][0] = MathF.Cos(fAngleRad);
        matrix.M[0][1] = MathF.Sin(fAngleRad);
        matrix.M[1][0] = -MathF.Sin(fAngleRad);
        matrix.M[1][1] = MathF.Cos(fAngleRad);
        matrix.M[2][2] = 1.0f;
        matrix.M[3][3] = 1.0f;
        return matrix;
    }

    public Mat4x4 MatrixMakeTranslation(float x, float y, float z) {

        var matrix = new Mat4x4();
        matrix.M[0][0] = 1.0f;
        matrix.M[1][1] = 1.0f;
        matrix.M[2][2] = 1.0f;
        matrix.M[3][3] = 1.0f;
        matrix.M[3][0] = x;
        matrix.M[3][1] = y;
        matrix.M[3][2] = z;
        return matrix;
    }

    public Mat4x4 MatrixMakeProjection(float fFovDegrees, float fAspectRatio, float fNear, float fFar) {

        float fFovRad = 1.0f / MathF.Tan(fFovDegrees * 0.5f / 180.0f * 3.14159f);
        
        var matrix = new Mat4x4();
        matrix.M[0][0] = fAspectRatio * fFovRad;
        matrix.M[1][1] = fFovRad;
        matrix.M[2][2] = fFar / (fFar - fNear);
        matrix.M[3][2] = (-fFar * fNear) / (fFar - fNear);
        matrix.M[2][3] = 1.0f;
        matrix.M[3][3] = 0.0f;
        return matrix;
    }

    public Mat4x4 MatrixMultiplyMatrix(in Mat4x4 m1, in Mat4x4 m2) {

        var matrix = new Mat4x4();

        for (int c = 0; c < 4; c++) {

            for (int r = 0; r < 4; r++) {

                matrix.M[r][c] = m1.M[r][0] * m2.M[0][c] + m1.M[r][1] * m2.M[1][c] + m1.M[r][2] * m2.M[2][c] + m1.M[r][3] * m2.M[3][c];
            }
        }

        return matrix;
    }

    public Mat4x4 MatrixPointAt(in Vec3D pos, in Vec3D target, in Vec3D up) {

        // Calculate new forward direction
        var newForward = this.VectorSub(target, pos);
        newForward = this.VectorNormalise(newForward);

        // Calculate new Up direction
        var a = this.VectorMul(newForward, this.VectorDotProduct(up, newForward));
        var newUp = this.VectorSub(up, a);
        newUp = VectorNormalise(newUp);

        // New Right direction is easy, its just cross product
        var newRight = VectorCrossProduct(newUp, newForward);

        // Construct Dimensioning and Translation Matrix    
        var matrix = new Mat4x4();
        matrix.M[0][0] = newRight.X;    matrix.M[0][1] = newRight.Y;    matrix.M[0][2] = newRight.Z;    matrix.M[0][3] = 0.0f;
        matrix.M[1][0] = newUp.X;        matrix.M[1][1] = newUp.Y;        matrix.M[1][2] = newUp.Z;        matrix.M[1][3] = 0.0f;
        matrix.M[2][0] = newForward.X;    matrix.M[2][1] = newForward.Y;    matrix.M[2][2] = newForward.Z;    matrix.M[2][3] = 0.0f;
        matrix.M[3][0] = pos.X;            matrix.M[3][1] = pos.Y;            matrix.M[3][2] = pos.Z;            matrix.M[3][3] = 1.0f;
        return matrix;
    }

    public Mat4x4 MatrixQuickInverse(in Mat4x4 m) { // Only for Rotation/Translation Matrices
    
        var matrix = new Mat4x4();
        matrix.M[0][0] = m.M[0][0]; matrix.M[0][1] = m.M[1][0]; matrix.M[0][2] = m.M[2][0]; matrix.M[0][3] = 0.0f;
        matrix.M[1][0] = m.M[0][1]; matrix.M[1][1] = m.M[1][1]; matrix.M[1][2] = m.M[2][1]; matrix.M[1][3] = 0.0f;
        matrix.M[2][0] = m.M[0][2]; matrix.M[2][1] = m.M[1][2]; matrix.M[2][2] = m.M[2][2]; matrix.M[2][3] = 0.0f;
        matrix.M[3][0] = -(m.M[3][0] * matrix.M[0][0] + m.M[3][1] * matrix.M[1][0] + m.M[3][2] * matrix.M[2][0]);
        matrix.M[3][1] = -(m.M[3][0] * matrix.M[0][1] + m.M[3][1] * matrix.M[1][1] + m.M[3][2] * matrix.M[2][1]);
        matrix.M[3][2] = -(m.M[3][0] * matrix.M[0][2] + m.M[3][1] * matrix.M[1][2] + m.M[3][2] * matrix.M[2][2]);
        matrix.M[3][3] = 1.0f;
        return matrix;
    }

    ///

    public Vec3D VectorAdd(in Vec3D v1, in Vec3D v2) {

        return new Vec3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    }

    public Vec3D VectorSub(in Vec3D v1, in Vec3D v2) {

        return new Vec3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    }

    public Vec3D VectorMul(in Vec3D v1, float k) {

        return new Vec3D(v1.X * k, v1.Y * k, v1.Z * k);
    }

    public Vec3D VectorDiv(in Vec3D v1, float k) {

        return new Vec3D(v1.X / k, v1.Y / k, v1.Z / k);
    }

    public float VectorDotProduct(in Vec3D v1, in Vec3D v2) {

        return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
    }

    public float VectorLength(in Vec3D v) {

        return MathF.Sqrt(this.VectorDotProduct(v, v));
    }

    public Vec3D VectorNormalise(in Vec3D v) {

        float l = VectorLength(v);

        return new Vec3D(v.X / l, v.Y / l, v.Z / l);
    }

    public Vec3D VectorCrossProduct(in Vec3D v1, in Vec3D v2) {

        return new Vec3D(
            x: v1.Y * v2.Z - v1.Z * v2.Y,
            y: v1.Z * v2.X - v1.X * v2.Z,
            z: v1.X * v2.Y - v1.Y * v2.X);
    }

    
    public Vec3D VectorIntersectPlane(in Vec3D plane_p, ref Vec3D plane_n, in Vec3D lineStart, in Vec3D lineEnd) {

        plane_n = VectorNormalise(plane_n);
        float plane_d = -VectorDotProduct(plane_n, plane_p);
        float ad = VectorDotProduct(lineStart, plane_n);
        float bd = VectorDotProduct(lineEnd, plane_n);
        float t = (-plane_d - ad) / (bd - ad);
        var lineStartToEnd = VectorSub(lineEnd, lineStart);
        var lineToIntersect = VectorMul(lineStartToEnd, t);
        return VectorAdd(lineStart, lineToIntersect);
    }

    // public int TriangleClipAgainstPlane(in Vec3D plane_p, ref Vec3D plane_n, in Triangle in_tri, ref Triangle out_tri1, ref Triangle out_tri2) {
    public int TriangleClipAgainstPlane(Vec3D plane_p, Vec3D plane_n, in Triangle in_tri, ref Triangle out_tri1, ref Triangle out_tri2) {

        // Make sure plane normal is indeed normal
        plane_n = VectorNormalise(plane_n);

        // Return signed shortest distance from point to plane, plane normal must be normalised
        Func<Vec3D, float> dist = (Vec3D p) => {

            // var n = VectorNormalise(p);

            // return (plane_n.X * p.X + plane_n.Y * p.Y + plane_n.Z * p.Z - VectorDotProduct(plane_n, plane_p));

            var n = VectorNormalise(p);

            return (plane_n.X * n.X + plane_n.Y * n.Y + plane_n.Z * n.Z - VectorDotProduct(plane_n, plane_p));
        };

        // Create two temporary storage arrays to classify points either side of plane
        // If distance sign is positive, point lies on "inside" of plane

        var insidePoints = new Vec3D[3];
        var insidePointCount = 0;

        var outsidePoints = new Vec3D[3];
        var outsidePointCount = 0;

        // Get signed distance of each point in triangle to plane
        var d0 = dist(in_tri.P[0]);
        var d1 = dist(in_tri.P[1]);
        var d2 = dist(in_tri.P[2]);

        if (d0 >= 0) {

            insidePoints[insidePointCount++] = in_tri.P[0];
        }
        else {

            outsidePoints[outsidePointCount++] = in_tri.P[0];
        }

        if (d1 >= 0) {

            insidePoints[insidePointCount++] = in_tri.P[1];
        }
        else {

            outsidePoints[outsidePointCount++] = in_tri.P[1];
        }

        if (d2 >= 0) {

            insidePoints[insidePointCount++] = in_tri.P[2];
        }
        else {

            outsidePoints[outsidePointCount++] = in_tri.P[2];
        }

        // Now classify triangle points, and break the input triangle into 
        // smaller output triangles if required. There are four possible
        // outcomes...

        if (insidePointCount == 0) {

            // All points lie on the outside of plane, so clip whole triangle
            // It ceases to exist

            return 0; // No returned triangles are valid
        }

        if (insidePointCount == 3) {

            // All points lie on the inside of plane, so do nothing
            // and allow the triangle to simply pass through
            out_tri1 = in_tri;

            return 1; // Just the one returned original triangle is valid
        }

        if (insidePointCount == 1 && outsidePointCount == 2) {

            // Triangle should be clipped. As two points lie outside
            // the plane, the triangle simply becomes a smaller triangle

            // Copy appearance info to new triangle
            // out_tri1.Color = in_tri.Color;
            out_tri1.Color = new SDL_Color { r = 0x00, g = 0x00, b = 0xff, a = 0xff };

            // The inside point is valid, so keep that...
            out_tri1.P[0] = insidePoints[0];

            // but the two new points are at the locations where the 
            // original sides of the triangle (lines) intersect with the plane
            out_tri1.P[1] = VectorIntersectPlane(plane_p, ref plane_n, insidePoints[0], outsidePoints[0]);
            out_tri1.P[2] = VectorIntersectPlane(plane_p, ref plane_n, insidePoints[0], outsidePoints[1]);

            return 1; // Return the newly formed single triangle
        }

        if (insidePointCount == 2 && outsidePointCount == 1) {

            // Triangle should be clipped. As two points lie inside the plane,
            // the clipped triangle becomes a "quad". Fortunately, we can
            // represent a quad with two new triangles

            // Copy appearance info to new triangles
            // out_tri1.Color = in_tri.Color;
            out_tri1.Color = new SDL_Color { r = 0x00, g = 0xff, b = 0x00, a = 0xff }; 

            // out_tri2.Color = in_tri.Color;
            out_tri2.Color = new SDL_Color { r = 0xff, g = 0x00, b = 0x00, a = 0xff };

            // The first triangle consists of the two inside points and a new
            // point determined by the location where one side of the triangle
            // intersects with the plane
            out_tri1.P[0] = insidePoints[0];
            out_tri1.P[1] = insidePoints[1];
            out_tri1.P[2] = VectorIntersectPlane(plane_p, ref plane_n, insidePoints[0], outsidePoints[0]);

            // The second triangle is composed of one of he inside points, a
            // new point determined by the intersection of the other side of the 
            // triangle and the plane, and the newly created point above
            out_tri2.P[0] = insidePoints[1];
            out_tri2.P[1] = out_tri1.P[2];
            out_tri2.P[2] = VectorIntersectPlane(plane_p, ref plane_n, insidePoints[1], outsidePoints[0]);

            return 2; // Return two newly formed triangles which form a quad
        }

        return 0;
    }

    ///

    public void Dispose() {

        SDL_Quit();
    }
}