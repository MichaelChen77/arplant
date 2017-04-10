using System.Collections.Generic;

namespace IMAV
{
    public class FurnitureSet
    {
        List<FurnitureData> data = new List<FurnitureData>();
        public List<FurnitureData> Data
        {
            get { return data; }
            set { data = value; }
        }

        public int status { get; set; }
    }
}
