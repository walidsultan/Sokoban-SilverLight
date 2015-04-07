using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sokoban.DataTypes
{
    public class User
    {
        [KeyAttribute]
        public int Id { get; set; }
        public string  Username { get; set; }
        public string  Password { get; set; }
        public string IpAddress { get; set; }
        public double Score { get; set; }
    }
}
