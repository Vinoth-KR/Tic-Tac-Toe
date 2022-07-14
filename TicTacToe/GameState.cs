using System;
using System.Collections.Generic;
using System.Text;
using TicTacToe.Enums;
using TicTacToe.Models;

namespace TicTacToe
{
    public class GameState
    {
        public Player[,] GameGrid { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public int StatesPassed { get; private set; }
        public bool GameOver { get; private set; }

        public event Action<int, int> MoveMade;
        public event Action<GameResult> GameEnded;
        public event Action GameRestarted;

        public GameState()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            StatesPassed = 0;
            GameOver = false;
        }

        private bool CanMakeMove(int row, int col)
        {
            return !GameOver && GameGrid[row, col] == Player.None;
        }

        private bool IsGridFull()
        {
            return StatesPassed == 9;
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player.X? Player.O: Player.X;
        }

        private bool AreSquaresMarked((int, int)[] squares, Player player)
        {
            foreach((int row,int col) in squares)
            {
                if (GameGrid[row, col] != player) return false;                
            }

            return true;
        }

        private bool DidMoveWin(int r, int c, out StrikeInfo strikeInfo)
        {
            (int, int)[] row = new[] { (r, 0), (r, 1), (r, 2) };
            (int, int)[] col = new[] { (0, c), (1, c), (2, c) };
            (int, int)[] leftDiag = new[] { (0, 0), (1, 1), (2, 2) };
            (int, int)[] rightDiag = new[] { (0, 2), (1, 1), (2, 0) };

            if(AreSquaresMarked(row, CurrentPlayer))
            {
                strikeInfo = new StrikeInfo { StrikeType = StrikeOutRule.Row, Index = r };
                return true;
            }

            if(AreSquaresMarked(col, CurrentPlayer))
            {
                strikeInfo = new StrikeInfo { StrikeType = StrikeOutRule.Column, Index = c };
                return true;
            }

            if(AreSquaresMarked(leftDiag, CurrentPlayer))
            {
                strikeInfo = new StrikeInfo { StrikeType = StrikeOutRule.LeftDiagonal };
                return true;
            }

            if(AreSquaresMarked(rightDiag, CurrentPlayer))
            {
                strikeInfo = new StrikeInfo { StrikeType = StrikeOutRule.RightDiagonal };
                return true;
            }

            strikeInfo = null;
            return false;
        }

        private bool DidMoveWinGame(int r, int c, out GameResult gameResult)
        {
            if(DidMoveWin(r,c,out StrikeInfo strikeInfo))
            {
                gameResult = new GameResult { Winner = CurrentPlayer, StrikeInfo = strikeInfo };
                return true;
            }

            if(IsGridFull())
            {
                gameResult = new GameResult { Winner = Player.None };
                return true;
            }

            gameResult = null;
            return false;
        }

        public void MakeMove(int r, int c)
        {
            if (!CanMakeMove(r, c)) return;

            GameGrid[r, c] = CurrentPlayer;
            StatesPassed++;

            if(DidMoveWinGame(r, c, out GameResult gameResult))
            {
                GameOver = true;
                MoveMade?.Invoke(r, c);
                GameEnded?.Invoke(gameResult);
            }
            else
            {
                SwitchPlayer();
                MoveMade?.Invoke(r, c);
            }
        }

        public void Restart()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            StatesPassed = 0;
            GameOver = false;
            GameRestarted?.Invoke();
        }

    }
}
