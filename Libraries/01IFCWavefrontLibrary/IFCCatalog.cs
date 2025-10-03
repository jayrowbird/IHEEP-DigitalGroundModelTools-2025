namespace HeepWare.IFC.Catalog
{
    public class IFCCatalog
    {
        private static int currentID = 1;
        private Dictionary<int, string> IDKey = new Dictionary<int, string>();
        private Dictionary<string, int> GuidKey = new Dictionary<string, int>();

        IFCCatalog() { }
        private static readonly object lock1 = new object();
        private static IFCCatalog? instance = null;
        public static IFCCatalog Instance
        {
            get
            {
                lock (lock1)
                {
                    if (instance == null)
                    {
                        instance = new IFCCatalog();
                    }
                    return instance;
                }
            }
        }

        public void Clear()
        {
            IDKey.Clear();
            GuidKey.Clear();
            currentID = 1;
        }



        public int Add(string guid)
        { 
            int id = currentID ;
            guid = guid.Trim();
            
            Console.WriteLine("IFCCatalog:: Add {0} id == {1}", guid, id );
            
            //Check if guid is already stored
            if (IDKey.ContainsValue(guid) == false)
            {
                IDKey.Add(id, guid);
                currentID++;
            }
            
            if (GuidKey.TryAdd(guid, id) == false)
            {
                guid += "-";
                while (GuidKey.TryAdd(guid + "_", id) == false)
                {
                    guid += "-";
                }
            }
            return id;
        }

        public int GetID(string guid)
        {
            guid = guid.Trim();
            int id = -1;
            GuidKey.TryGetValue(guid, out id);
            if (id < 1)
            {
                id = Add(guid);
            }
            return id;
        }

        public string? GetGuid(int id)
        {
            string? guid = null;
            IDKey.TryGetValue(id, out guid);
            return guid;
        }
    }
}
