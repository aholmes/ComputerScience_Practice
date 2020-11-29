#define PRINT_STEP

using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;

namespace NQueen
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.SetCurrentFont("Consolas", 40);
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            const int N = 8;
            const int displayDelay = 0;

            var nQueen = new NQueen(N, displayDelay);

            var sw = new Stopwatch();
            sw.Start();
            var result = nQueen.Go(slow: false);
            sw.Stop();

            nQueen.DisplayBoard(0);
            Console.WriteLine($"N={N} {(result ? "has a solution" : "does not have a solution")}");
            Console.WriteLine($"Time taken: {sw.ElapsedTicks / 10000}ms");
        }

        const string Black = "\u001b[30m\u001b[40m";
        const string White = "\u001b[37m\u001b[47m";
        const string Cyan = "\u001b[36m\u001b[46m";
        const string Red = "\u001b[31;1m\u001b[41m";
        const string Green = "\u001b[32m";
        const string Yellow = "\u001b[33m";
        const string Magenta = "\u001b[35;1m\u001b[45;1m";
        const string Reset = "\u001b[0m";
        private static string Color(string s, string color) => color + s + Reset;


        /// <summary>
        /// Reference https://www.geeksforgeeks.org/n-queen-problem-backtracking-3/
        /// </summary>
        public class NQueen
        {
            private const string B = "■";
            private const string W = "⬜";
            private const string Q = "Q";
            private const string X = "X";
            private string[,] _board;
            private readonly int N;
            private readonly int displayDelay;

            public NQueen(int size, int displayDelay)
            {
                N = size;
                this.displayDelay = displayDelay;
                ld = new bool[N * 2 - 1];
                rd = new bool[N * 2 - 1];
                cl = new bool[N * 2 - 1];
                _fillBoard();
            }

            private void _fillBoard()
            {
                _board = new string[N, N];
                for(var i = 0; i < N * N; i++)
                {
                    var col = i % N;
                    var row = i / N;
                    _board[row, col] = (i + row) % 2 == 0 ? Color(B, Black) : Color(W, White);
                }
            }

            #region Slow approach
            private bool isSafeSlow(int row, int col)
            {
                // check all squares to the left
                for(var c = col - 1; c >= 0; c--)
                {
                    var value = _board[row, c];

                    _board[row, c] = Color(X, Cyan);
#if PRINT_STEP
					DisplayBoard(displayDelay);
#endif

                    _board[row, c] = value;
                    if(value.IndexOf(Q) >= 0) return false;
                }

                // check up-left squares
                for(int r = row - 1, c = col - 1; r >= 0 && c >= 0; r--, c--)
                {
                    var value = _board[r, c];

                    _board[r, c] = Color(X, Cyan);
#if PRINT_STEP
					DisplayBoard(displayDelay);
#endif

                    _board[r, c] = value;
                    if(value.IndexOf(Q) >= 0) return false;
                }

                // check down-left squares
                for(int r = row + 1, c = col - 1; r < N && c >= 0; r++, c--)
                {
                    var value = _board[r, c];

                    _board[r, c] = Color(X, Cyan);
#if PRINT_STEP
					DisplayBoard(displayDelay);
#endif

                    _board[r, c] = value;
                    if(value.IndexOf(Q) >= 0) return false;
                }

                return true;
            }

            private bool solveSlow(int col)
            {
                if(col >= N) return true;

                for(var r = 0; r < N; r++)
                {
                    var oldCheck = _board[r, col];
                    _board[r, col] = Color(oldCheck, Red);

#if PRINT_STEP
					DisplayBoard(displayDelay);
#endif
                    var safe = isSafeSlow(r, col);
                    if(safe)
                    {
                        var old = _board[r, col];
                        _board[r, col] = Color(Q, Green);

                        if(solveSlow(col + 1)) return true;

                        _board[r, col] = old;
                    }

                    _board[r, col] = oldCheck;
                }

                return false;
            }
            #endregion

            #region Faster approach
            private readonly bool[] ld;
            private readonly bool[] rd;
            private readonly bool[] cl;

            private bool solveFast(int col)
            {
                if(col >= N) return true;

                for(var row = 0; row < N; row++)
                {
                    var old = _board[row, col];
                    _board[row, col] = Color(Q, Magenta);
                    displayFromRowCol(row, col, 1);
                    _board[row, col] = old;

                    var ldiag = row - col + N - 1;
                    var rdiag = row + col;
                    if (!ld[ldiag] && !rd[rdiag] && !cl[row])
                    {
                        old = _board[row, col];
                        _board[row, col] = Color(Q, Green);
                        displayFromRowCol(row, col);
                        ld[ldiag] = rd[rdiag] = cl[row] = true;

                        if(solveFast(col + 1)) return true;

                        _board[row, col] = old;
                        ld[ldiag] = rd[rdiag] = cl[row] = false;
                    }
                }

                return false;
            }
            #endregion

            public bool Go(bool slow) => slow ? solveSlow(0) : solveFast(0);

            private void displayFromRowCol(int row, int col, int? wait = null)
            {
                var rowsBefore = row;
                var rowsAfter = N - row - 1;
                var colsBefore = col;
                //var colsAfter = N - col - 1;

                var upLeft = rowsBefore < colsBefore ? rowsBefore : colsBefore;
                //var downRight = rowsAfter < colsAfter ? rowsAfter : colsAfter;
                //var upRight = rowsBefore < colsAfter ? rowsBefore : colsAfter;
                var downLeft = rowsAfter < colsBefore ? rowsAfter : colsBefore;

                var temp = new string[N, N];

                static string getChar(string input)
                {
                    return input.IndexOf(Q) >= 0
                        ? Color(Q, Red)
                        : Color(X, Cyan);
                }

                for(var i = 0; i < col; i++)
                {
                    var r = row;
                    var c = col - i - 1;
                    temp[r, c] = _board[r, c];
                    _board[r, c] = getChar(temp[r, c]);
                }
                for(var i = 0; i < upLeft; i++)
                {
                    var r = row - i - 1;
                    var c = col - i - 1;
                    temp[r, c] = _board[r, c];
                    _board[r, c] = getChar(temp[r, c]);
                }
                for(var i = 0; i < downLeft; i++)
                {
                    var r = row + i + 1;
                    var c = col - i - 1;
                    temp[r, c] = _board[r, c];
                    _board[r, c] = getChar(temp[r, c]);
                }

                DisplayBoard(wait ?? displayDelay);

                for(var i = 0; i < col; i++)
                {
                    var r = row;
                    var c = col - i - 1;
                    _board[r, c] = temp[r, c];
                }
                for(var i = 0; i < upLeft; i++)
                {
                    var r = row - i - 1;
                    var c = col - i - 1;
                    _board[r, c] = temp[r, c];
                }
                for(var i = 0; i < downLeft; i++)
                {
                    var r = row + i + 1;
                    var c = col - i - 1;
                    _board[r, c] = temp[r, c];
                }
            }

            public void DisplayBoard(int wait, bool reset = true)
            {
                var sb = new StringBuilder();
                for(var i = 0; i < N * N; i++)
                {
                    var row = i / N;
                    var col = i % N;

                    if(col == 0)
                    {
                        sb.AppendLine();
                    }
                    sb.Append(_board[row, col]);
                }

                if(reset)
                {
                    Console.SetCursorPosition(0, 0);
                }

                Console.WriteLine(sb.ToString());

                if(wait > 0)
                {
                    System.Threading.Thread.Sleep(wait);
                }
            }
        }

    }
}
