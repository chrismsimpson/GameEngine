
namespace olc;

public struct Vec2D {

    public float U { get; set; }
    
    public float V { get; set; }

    public float W { get; set; }

    ///

    public Vec2D()
        : this(0, 0, 0) { }

    public Vec2D(
        float u,
        float v,
        float w) {

        this.U = u;
        this.V = v;
        this.W = w;
    }
}

public struct Vec3D {
    
    public float X { get; set; }
    
    public float Y { get; set; }
    
    public float Z { get; set; }

    public float W { get; set; }

    ///
    
    public Vec3D()
        : this(0.0f, 0.0f, 0.0f, 1.0f) { }

    public Vec3D(
        float x,
        float y,
        float z)
        : this(x, y, z, 1.0f) { }

    public Vec3D(
        float x,
        float y,
        float z,
        float w) {

        this.X = x;
        this.Y = y;
        this.Z = z;
        this.W = w;
    }
}

public struct Triangle {

    public Vec3D[] P { get; init; }

    public Vec2D[] T { get; init; }

    public SDL_Color Color { get; set; }

    ///

    public float MidZ { get => (this.P[0].Z + this.P[1].Z + this.P[2].Z) / 3.0f; }

    public String Identity { get => $"{this.P[0].X},{this.P[0].Y} {this.P[1].X},{this.P[1].Y} {this.P[2].X},{this.P[2].Y}"; }

    ///

    public Triangle()
        : this(0, 0, 0, 0, 0, 0, 0, 0, 0, new SDL_Color { r = 0xff, g = 0xff, b = 0xff, a = 0x7f }) { }

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
        : this(x1, y1, z1, x2, y2, z2, x3, y3, z3, new SDL_Color { r = 0xff, g = 0xff, b = 0xff, a = 0xff }) { }


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
        
        SDL_Color color) {

        this.P = new [] {

            new Vec3D(x1, y1, z1),
            new Vec3D(x2, y2, z2),
            new Vec3D(x3, y3, z3)
        };

        this.Color = color;

        this.T = new Vec2D[] {

            new Vec2D(),
            new Vec2D(),
            new Vec2D()
        };
    }    

    ///



    public Triangle(
        float x1,
        float y1,
        float z1,
        float w1,
        
        float x2,
        float y2,
        float z2,
        float w2,
        
        float x3,
        float y3,
        float z3,
        float w3)
        : this(x1, y1, z1, w1, x2, y2, z2, w2, x3, y3, z3, w3, new SDL_Color { r = 0xff, g = 0xff, b = 0xff, a = 0xff }) { }

    public Triangle(
        float x1,
        float y1,
        float z1,
        float w1,
        
        float x2,
        float y2,
        float z2,
        float w2,
        
        float x3,
        float y3,
        float z3,
        float w3,
        
        SDL_Color color) {

        this.P = new [] {

            new Vec3D(x1, y1, z1, w1),
            new Vec3D(x2, y2, z2, w2),
            new Vec3D(x3, y3, z3, w3)
        };

        this.T = new Vec2D[] {
            
            new Vec2D(),
            new Vec2D(),
            new Vec2D()
        };

        this.Color = color;
    }

    ///



    public Triangle(
        float x1,
        float y1,
        float z1,
        float w1,
        
        float x2,
        float y2,
        float z2,
        float w2,
        
        float x3,
        float y3,
        float z3,
        float w3,
        
        float u1,
        float v1,
        float tw1,

        float u2,
        float v2,
        float tw2,

        float u3,
        float v3,
        float tw3)
        : this(x1, y1, z1, w1, x2, y2, z2, w2, x3, y3, z3, w3, u1, v1, tw1, u2, v2, tw2, u3, v3, tw3, new SDL_Color { r = 0xff, g = 0xff, b = 0xff, a = 0xff }) { }

    public Triangle(
        float x1,
        float y1,
        float z1,
        float w1,
        
        float x2,
        float y2,
        float z2,
        float w2,
        
        float x3,
        float y3,
        float z3,
        float w3,

        float u1,
        float v1,
        float tw1,

        float u2,
        float v2,
        float tw2,

        float u3,
        float v3,
        float tw3,
        
        SDL_Color color) {

        this.P = new [] {

            new Vec3D(x1, y1, z1, w1),
            new Vec3D(x2, y2, z2, w2),
            new Vec3D(x3, y3, z3, w3)
        };

        this.T = new Vec2D[] {

            new Vec2D(u1, v1, tw1),
            new Vec2D(u2, v2, tw2),
            new Vec2D(u3, v3, tw3)
        };

        this.Color = color;
    }



    ///
    
    public Triangle(
        Vec3D p1,
        Vec3D p2,
        Vec3D p3)
        : this(p1, p2, p3, new SDL_Color { r = 0xff, g = 0xff, b = 0xff, a = 0xff }) { }
    
    public Triangle(
        Vec3D p1,
        Vec3D p2,
        Vec3D p3,
        SDL_Color color) {

        this.P = new [] { p1, p2, p3 };
        this.T = new Vec2D[] {
            
            new Vec2D(),
            new Vec2D(),
            new Vec2D()
        };
        this.Color = color;
    }

    ///

    
    public Triangle(
        Vec3D p1,
        Vec3D p2,
        Vec3D p3,
        Vec2D t1,
        Vec2D t2,
        Vec2D t3)
        : this(p1, p2, p3, t1, t2, t3, new SDL_Color { r = 0xff, g = 0xff, b = 0xff, a = 0xff }) { }
    
    public Triangle(
        Vec3D p1,
        Vec3D p2,
        Vec3D p3,
        Vec2D t1,
        Vec2D t2,
        Vec2D t3,
        SDL_Color color) {

        this.P = new [] { p1, p2, p3 };
        this.T = new [] { t1, t2, t3 };
        this.Color = color;
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

public static partial class Program {

    public static int Main(
        String[] args) {

        // using var app = new GameEngine(800, 640);
        // using var app = new GameEngine(1024, 960);
        // using var app = new GameEngine(384, 216);
        // using var app = new GameEngine(1536, 864);

        using var app = new GameEngine(1280, 832);
        // using var app = new GameEngine(640, 416);

        app.Run();

        return 0;
    }
}