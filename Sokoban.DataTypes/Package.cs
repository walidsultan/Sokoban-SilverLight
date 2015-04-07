using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sokoban.DataTypes
{
    public class Package
    {
        [KeyAttribute]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
