namespace PGM
{
    public struct MapTriangle 
    {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        private int[] Vertices;

        public MapTriangle(int a, int b, int c)
        {
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;

            Vertices = new int[3];
            Vertices[0] = a;
            Vertices[1] = b;
            Vertices[2] = c;
        }

        public int this[int i]
        {
            get	{ return Vertices[i]; }
        }

        public bool Contains(int VertexIndex)
        {
            return VertexIndex == vertexIndexA || VertexIndex == vertexIndexB || VertexIndex == vertexIndexC;
        }
    }
}