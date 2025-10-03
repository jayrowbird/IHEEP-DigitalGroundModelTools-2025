using System.Globalization;

namespace HeepWare.Contouring
{
    public class ContourID 
    {
        public string strElev;
        public double elev;
        private Guid id = Guid.NewGuid();

        public ContourID(string key )
        {
            strElev = key;
           if( Double.TryParse( strElev, out elev ) == false)
            {
                throw new ArgumentException("ContourID could not convert string to double elevation {0}", key);
            }
        }

        public Guid GetId()
        {
            return id;
        }

        public ContourID(double value)
        {
            strElev = String.Format(CultureInfo.InvariantCulture, "{0:00.00}", value);  
            elev = value;
        }
    }
}
