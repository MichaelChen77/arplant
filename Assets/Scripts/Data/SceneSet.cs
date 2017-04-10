using System.Collections.Generic;

namespace IMAV
{
    public class SceneSet
    {
        List<SceneData> m_data = new List<SceneData>();
        public List<SceneData> data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        public int status { get; set; }
    }
}