using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sokoban.DataTypes
{
    public class Progress
    {
        [KeyAttribute]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PackageId { get; set; }
        public int LevelIndex { get; set; }
        public int Moves { get; set; }
        public int Pushes { get; set; }
        public int Time { get; set; }
    }
}
