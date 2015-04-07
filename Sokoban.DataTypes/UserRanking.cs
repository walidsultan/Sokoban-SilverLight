using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sokoban.DataTypes
{
    public class UserRanking
    {
        [KeyAttribute]
        public string Username { get; set; }
        public int Level { get; set; }
        public double?  Score { get; set; }
    }
}