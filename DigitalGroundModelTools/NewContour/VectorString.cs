using TriangleNet.Data;

namespace HeepWare.Contouring
{
    public class LineString
    {
        public List<Vertex> data = null;

        public LineString()
        {
            data = new List<Vertex>();
        }

        public void Add(Vertex v)
        {
            data.Add(v);
        }

        public int Count
        {
            get
            {
                return data.Count;
            }
            set
            {
            }
        }

        public Vertex this[int key]
        {
            get
            {
                if (key >= data.Count)
                {
                    return new Vertex();
                }
                return data[key];
            }
            set
            {
                data[key] = value;
            }
        }
    }
}
