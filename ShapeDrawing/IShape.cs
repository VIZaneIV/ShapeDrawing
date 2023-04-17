using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeDrawing
{
    interface IShape
    {
        char name { get; set; }

        public bool Check(int x, int y);

        public string Save();
    }
}
