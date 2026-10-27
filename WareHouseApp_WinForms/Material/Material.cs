using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareHouseApp.Material
{
    public class Material
    {
        public int materialID;
        public string materialName;
        private int materialCount;
        public int MaterialCount
        {
            get
            {
                return materialCount;
            }
            set
            {
                materialCount = value;
            }
        }



    }
}
