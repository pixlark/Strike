using System;

namespace Strike {
    public struct Color {
        public byte r, g, b, a;
    }

    public struct Vector {
        public float x, y;
        public Vector(float x, float y) {
            this.x = x;
            this.y = y;
        }
        public override string ToString() {
            return string.Format("{{ {0}, {1} }}", x, y);
        }
        public VectorInt ToVectorInt() {
            return new VectorInt((int) x, (int) y);
        }
        public float Magnitude() {
            return (float) Math.Sqrt(x * x + y * y);
        }
        public Vector Normalize() {
            return this / Magnitude();
        }
        
        public static Vector Zero  = new Vector( 0.0f,  0.0f);
        public static Vector Left  = new Vector(-1.0f,  0.0f);
        public static Vector Right = new Vector( 1.0f,  0.0f);
        public static Vector Up    = new Vector( 0.0f,  1.0f);
        public static Vector Down  = new Vector( 0.0f, -1.0f);

        public static Vector operator +(Vector a, Vector b) =>
            new Vector(a.x + b.x, a.y + b.y);
        public static Vector operator -(Vector a, Vector b) =>
            new Vector(a.x - b.x, a.y - b.y);

        public static Vector operator *(Vector a, float f) =>
            new Vector(a.x * f, a.y * f);
        public static Vector operator /(Vector a, float f) =>
            new Vector(a.x / f, a.y / f);
    }

    public struct Rect {
        public float x, y, w, h;
        public override string ToString() {
            return string.Format("{{ {0}, {1}, {2}, {3} }}", x, y, w, h);
        }
    }
    
    public struct VectorInt {
        public int x, y;
        public VectorInt(int x, int y) {
            this.x = x;
            this.y = y;
        }
        public override string ToString() {
            return string.Format("{{ {0}, {1} }}", x, y);
        }
        public Vector ToVector() {
            return new Vector((float) x, (float) y);
        }
        
        public static VectorInt Zero  = new VectorInt( 0,  0);
        public static VectorInt Left  = new VectorInt(-1,  0);
        public static VectorInt Right = new VectorInt( 1,  0);
        public static VectorInt Up    = new VectorInt( 0,  1);
        public static VectorInt Down  = new VectorInt( 0, -1);
        
        public static VectorInt operator +(VectorInt a, VectorInt b) =>
            new VectorInt(a.x + b.x, a.y + b.y);
        public static VectorInt operator -(VectorInt a, VectorInt b) =>
            new VectorInt(a.x - b.x, a.y - b.y);
    }
    
    public struct RectInt {
        public int x, y, w, h;
        public override string ToString() {
            return string.Format("{{ {0}, {1}, {2}, {3} }}", x, y, w, h);
        }
    }
}
