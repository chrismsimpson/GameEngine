
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

    private float fTheta { get; set; } = 0.0f;

    ///

    // private bool RenderWireframes { get; set; } = true;
    private bool RenderWireframes { get; set; } = false;

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
        this.Mesh = new Mesh(filename: "teapot.obj");
        

        // this.fNear = 0.1f;
        // this.fFar = 1000.0f;
        // this.fFov = 90.0f;
        // this.fAspectRatio = this.HeightF / this.WidthF;
        // this.fFovRad = 1.0f / MathF.Tan(this.fFov * 0.5f / 180.0f * 3.14159f);

        // this.MatProj.M[0][0] = this.fAspectRatio * this.fFovRad;
        // this.MatProj.M[1][1] = this.fFovRad;
        // this.MatProj.M[2][2] = this.fFar / (this.fFar - this.fNear);
        // this.MatProj.M[3][2] = (-this.fFar * this.fNear) / (this.fFar - this.fNear);
        // this.MatProj.M[2][3] = 1.0f;
        // this.MatProj.M[3][3] = 0.0f;

        this.MatProj = this.MatrixMakeProjection(
            90.0f, 
            this.HeightF / this.WidthF,
            0.1f, 
            1000.0f);


        // this.fTheta = 3.6f;

        

        return true;
    }

    public void OnUpdate(
        float elapsedTime) {

        SDL_SetRenderDrawColor(this.SDLRendererPtr, 0x00, 0x00, 0x00, 0xff);

        SDL_RenderClear(this.SDLRendererPtr);

        /// On loop

        this.fTheta += 1.0f * elapsedTime;

        ///

        var matRotZ = MatrixMakeRotationZ(this.fTheta * 0.5f);

        var matRotX = MatrixMakeRotationX(this.fTheta);

        // var matRotZ = new Mat4x4();

        // var matRotX = new Mat4x4();

        // Rotation Z
        // matRotZ.M[0][0] = MathF.Cos(this.fTheta);
        // matRotZ.M[0][1] = MathF.Sin(this.fTheta);
        // matRotZ.M[1][0] = -MathF.Sin(this.fTheta);
        // matRotZ.M[1][1] = MathF.Cos(this.fTheta);
        // matRotZ.M[2][2] = 1;
        // matRotZ.M[3][3] = 1;

        // Rotation X
        // matRotX.M[0][0] = 1;
        // matRotX.M[1][1] = MathF.Cos(this.fTheta * 0.5f);
        // matRotX.M[1][2] = MathF.Sin(this.fTheta * 0.5f);
        // matRotX.M[2][1] = -MathF.Sin(this.fTheta * 0.5f);
        // matRotX.M[2][2] = MathF.Cos(this.fTheta * 0.5f);
        // matRotX.M[3][3] = 1;

        ///

        var matTrans = MatrixMakeTranslation(0.0f, 0.0f, 8.0f);

        var matWorld = MatrixMakeIdentity();
        matWorld = MatrixMultiplyMatrix(matRotZ, matRotX);
        matWorld = MatrixMultiplyMatrix(matWorld, matTrans);

        var ffFloat = Convert.ToSingle(0xff);

        var vecTrianglesToRender = new List<Triangle>();       

        if (this.Mesh is Mesh mesh) {

            for (var i = 0; i < mesh.Triangles.Length; ++i)  {

                var tri = mesh.Triangles[i];

                var triProjected = new Triangle();
                // var triTranslated = new Triangle();
                // var triRotatedZ = new Triangle();
                // var triRotatedZX = new Triangle();

                var triTransformed = new Triangle();

                // // Rotate in Z-Axis
                // triRotatedZ.P[0] = this.MultiplyMatrixVector(tri.P[0], matRotZ);
                // triRotatedZ.P[1] = this.MultiplyMatrixVector(tri.P[1], matRotZ);
                // triRotatedZ.P[2] = this.MultiplyMatrixVector(tri.P[2], matRotZ);
                
                // // Rotate in X-Axis
                // triRotatedZX.P[0] = this.MultiplyMatrixVector(triRotatedZ.P[0], matRotX);
                // triRotatedZX.P[1] = this.MultiplyMatrixVector(triRotatedZ.P[1], matRotX);
                // triRotatedZX.P[2] = this.MultiplyMatrixVector(triRotatedZ.P[2], matRotX);
                
                // // Offset into the screen
                // triTranslated = triRotatedZX;
                // triTranslated.P[0].Z = triRotatedZX.P[0].Z + 8.0f;
                // triTranslated.P[1].Z = triRotatedZX.P[1].Z + 8.0f;
                // triTranslated.P[2].Z = triRotatedZX.P[2].Z + 8.0f;

                triTransformed.P[0] = this.MatrixMultiplyVector(matWorld, tri.P[0]);
                triTransformed.P[1] = this.MatrixMultiplyVector(matWorld, tri.P[1]);
                triTransformed.P[2] = this.MatrixMultiplyVector(matWorld, tri.P[2]);

                // Use Cross-Product to get surface normal
                // var normal = new Vec3D();
                // var line1 = new Vec3D();
                // var line2 = new Vec3D();

                // line1.X = triTranslated.P[1].X - triTranslated.P[0].X;
                // line1.Y = triTranslated.P[1].Y - triTranslated.P[0].Y;
                // line1.Z = triTranslated.P[1].Z - triTranslated.P[0].Z;

                var line1 = VectorSub(triTransformed.P[1], triTransformed.P[0]);

                // line2.X = triTranslated.P[2].X - triTranslated.P[0].X;
                // line2.Y = triTranslated.P[2].Y - triTranslated.P[0].Y;
                // line2.Z = triTranslated.P[2].Z - triTranslated.P[0].Z;

                var line2 = VectorSub(triTransformed.P[2], triTransformed.P[0]);

                // normal.X = line1.Y * line2.Z - line1.Z * line2.Y;
                // normal.Y = line1.Z * line2.X - line1.X * line2.Z;
                // normal.Z = line1.X * line2.Y - line1.Y * line2.X;

                var normal = VectorCrossProduct(line1, line2);

                normal = VectorNormalise(normal);

                // var l = MathF.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);

                // normal.X /= l;
                // normal.Y /= l;
                // normal.Z /= l;

                // if (normal.X * (triTranslated.P[0].X - this.vCamera.X) +
                //     normal.Y * (triTranslated.P[0].Y - this.vCamera.Y) +
                //     normal.Z * (triTranslated.P[0].Z - this.vCamera.Z) >= 0) {

                //     continue;
                // }

                var vCameraRay = this.VectorSub(triTransformed.P[0], this.vCamera);

                if (this.VectorDotProduct(normal, vCameraRay) >= 0.0f) {

                    continue;
                }

                // Illumination

                var lightDirection = new Vec3D(0, 0, -1.0f);

                // var ll = MathF.Sqrt(lightDirection.X * lightDirection.X + lightDirection.Y * lightDirection.Y + lightDirection.Z * lightDirection.Z);

                // lightDirection.X /= ll;
                // lightDirection.Y /= ll;
                // lightDirection.Z /= ll;

                // var dp = normal.X * lightDirection.X + normal.Y * lightDirection.Y + normal.Z * lightDirection.Z;

                lightDirection = VectorNormalise(lightDirection);

                var dp = MathF.Max(0.1f, this.VectorDotProduct(lightDirection, normal));

                if (dp is float.NaN
                    || dp < 0) {

                    continue;
                }

                byte shade = Convert.ToByte(255.0f * dp);

                triTransformed.Shade = shade;

                // Project triangles from 3D --> 2D

                // triProjected.P[0] = this.MultiplyMatrixVector(triTranslated.P[0], this.MatProj);
                // triProjected.P[1] = this.MultiplyMatrixVector(triTranslated.P[1], this.MatProj);
                // triProjected.P[2] = this.MultiplyMatrixVector(triTranslated.P[2], this.MatProj);

                triProjected.P[0] = this.MatrixMultiplyVector(this.MatProj, triTransformed.P[0]);
                triProjected.P[1] = this.MatrixMultiplyVector(this.MatProj, triTransformed.P[1]);
                triProjected.P[2] = this.MatrixMultiplyVector(this.MatProj, triTransformed.P[2]);
                triProjected.Shade = triTransformed.Shade;

                triProjected.P[0] = VectorDiv(triProjected.P[0], triProjected.P[0].W);
                triProjected.P[1] = VectorDiv(triProjected.P[1], triProjected.P[1].W);
                triProjected.P[2] = VectorDiv(triProjected.P[2], triProjected.P[2].W);

                // Scale into view

                // triProjected.P[0].X += 1.0f; 
                // triProjected.P[0].Y += 1.0f;
                // triProjected.P[1].X += 1.0f; 
                // triProjected.P[1].Y += 1.0f;
                // triProjected.P[2].X += 1.0f; 
                // triProjected.P[2].Y += 1.0f;

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

                

                vecTrianglesToRender.Add(new Triangle(triProjected.P[0], triProjected.P[1], triProjected.P[2], shade));
            }

        }

        ///

        vecTrianglesToRender.Sort((t1, t2) => t2.MidZ.CompareTo(t1.MidZ));

        ///

        var zeroTextCoord = new SDL_FPoint { };

        SDL_SetRenderDrawColor(this.SDLRendererPtr, 0xff, 0x00, 0x00, 0xff); // line color

        ///

        foreach (var triProjected in vecTrianglesToRender) {

            var p1 = new SDL_FPoint { x = triProjected.P[0].X, y = triProjected.P[0].Y };
            var p2 = new SDL_FPoint { x = triProjected.P[1].X, y = triProjected.P[1].Y };
            var p3 = new SDL_FPoint { x = triProjected.P[2].X, y = triProjected.P[2].Y };

            var color = new SDL_Color { r = triProjected.Shade, g = triProjected.Shade, b = triProjected.Shade, a = 0xff };

            var verts = new [] {
                new SDL_Vertex { position = p1, color = color, tex_coord = zeroTextCoord },
                new SDL_Vertex { position = p2, color = color, tex_coord = zeroTextCoord },
                new SDL_Vertex { position = p3, color = color, tex_coord = zeroTextCoord }
            };

            SDL_RenderGeometry(this.SDLRendererPtr, IntPtr.Zero, verts, 3, null, 0);

            if (this.RenderWireframes) {

                SDL_RenderDrawLineF(this.SDLRendererPtr, p1.x, p1.y, p2.x, p2.y);
                SDL_RenderDrawLineF(this.SDLRendererPtr, p2.x, p2.y, p3.x, p3.y);
                SDL_RenderDrawLineF(this.SDLRendererPtr, p3.x, p3.y, p1.x, p1.y);
            }
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
		matrix.M[0][0] = newRight.X;	matrix.M[0][1] = newRight.Y;	matrix.M[0][2] = newRight.Z;	matrix.M[0][3] = 0.0f;
		matrix.M[1][0] = newUp.X;		matrix.M[1][1] = newUp.Y;		matrix.M[1][2] = newUp.Z;		matrix.M[1][3] = 0.0f;
		matrix.M[2][0] = newForward.X;	matrix.M[2][1] = newForward.Y;	matrix.M[2][2] = newForward.Z;	matrix.M[2][3] = 0.0f;
		matrix.M[3][0] = pos.X;			matrix.M[3][1] = pos.Y;			matrix.M[3][2] = pos.Z;			matrix.M[3][3] = 1.0f;
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

    ///

    public void Dispose() {

        SDL_Quit();
    }
}