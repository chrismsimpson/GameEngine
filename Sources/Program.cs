
namespace Triangle;

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

    public byte Shade { get; set; }

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

public static partial class Program {

    public static int Main(
        String[] args) {

        // using var app = new GameEngine(800, 640);
        using var app = new GameEngine(1024, 960);

        app.Run();

        return 0;
    }
}