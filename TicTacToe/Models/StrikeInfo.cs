using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Enums;

namespace TicTacToe.Models
{
    public class StrikeInfo
    {
        public StrikeOutRule StrikeType { get; set; }
        public int Index { get; set; }
    }
}
