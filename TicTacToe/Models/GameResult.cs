using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Enums;

namespace TicTacToe.Models
{
    public class GameResult
    {
        public Player Winner { get; set; }
        public StrikeInfo StrikeInfo { get; set; }
    }
}
