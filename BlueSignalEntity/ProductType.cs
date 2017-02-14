using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueSignalEntity
{
    public class ProductType
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string ProductName { get; set; }

        public bool IsActive { get; set; }

    }
}
