using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Abalone
{
    public partial class Game : Form
    {
        Menu menu;

        int S, W, H;
        int offsetX;
        int offsetY;

        Player curPlayer;

        List<Player> players = new List<Player>();
        List<List<Cell>> map = new List<List<Cell>>();

        List<Cell> choosen = new List<Cell>();
        List<myPoint> newChoosen = new List<myPoint>();

        public Game(Menu myMenu)
        {
            InitializeComponent();

            menu = myMenu;
        }

        private void Game_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.Sizable;

            H = 9;
            W = 9;

            S = ClientSize.Height / H;
            offsetX = (ClientSize.Width - S * W) / 2;
            offsetY = (ClientSize.Height % S) / 2;

            int num = 4;
            int koef = 1;
            for (int i = 0; i < H; i++)
            {
                num += koef;
                if (num == 9)
                {
                    koef = -1;
                }
                List<Cell> line = new List<Cell>();
                for (int j = 0; j < num; j++)
                {
                    line.Add(new Cell(0, j, i, ChooseType(i)));
                }
                map.Add(line);
            }

            players.Add(new Player(1, Brushes.DarkOrange, Pens.White));
            players.Add(new Player(2, Brushes.DarkRed, Pens.Black));

            curPlayer = players[0];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    if (i != 2)
                    {
                        map[i][j].Value = players[1].Ind;
                        map[H - 1 - i][j].Value = players[0].Ind;
                    }
                    else if (j >= 2 && j < map[i].Count - 2)
                    {
                        map[i][j].Value = players[1].Ind;
                        map[H - 1 - i][j].Value = players[0].Ind;
                    }
                }
            }

            //map[1][3].Value = players[1].Ind;
            //map[2][3].Value = players[1].Ind;

            //map[7][3].Value = players[0].Ind;
            //map[6][3].Value = players[0].Ind;
            //map[5][3].Value = players[0].Ind;
        }
        private void Game_FormClosed(object sender, FormClosedEventArgs e)
        {
            menu.Show();
        }

        private void Game_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Brushes.Black, 3);
            float offline;
            for (int i = 0; i < map.Count; i++)
            {
                offline = (W * S - map[i].Count * S) / 2;
                for (int j = 0; j < map[i].Count; j++)
                {
                    if (map[i][j].Value > 0)
                    {
                        e.Graphics.FillEllipse(players[map[i][j].Value - 1].Brush, j * S + offsetX + offline,
                                                                             i * S + offsetY, S, S);
                    }
                    else if (map[i][j].Value == 0)
                    {
                        e.Graphics.FillEllipse(Brushes.DarkGray, j * S + offsetX + offline + S / 6,
                                                              i * S + offsetY + S / 6, S / (float)1.5, S / (float)1.5);
                    }
                }
            }
            e.Graphics.DrawRectangle(pen, offsetX, offsetY, S * W, S * H);

            e.Graphics.DrawString($"Очки Белого: {players[0].Score}", new Font("Times New Roman", 24, FontStyle.Bold),
                      Brushes.Black, new PointF(W * S + S / 2 + offsetX, S / 2 + S * 6 + offsetY));
            e.Graphics.DrawString($"Очки Черного: {players[1].Score}", new Font("Times New Roman", 24, FontStyle.Bold),
                     Brushes.Black, new PointF(W * S + S / 2 + offsetX, S / 2 + S * 2 + offsetY));
            if (curPlayer.Ind == 1)
            {
                e.Graphics.DrawString($"Ход Белых", new Font("Times New Roman", 36, FontStyle.Bold),
                      Brushes.Black, new PointF(W * S + S / 2 + offsetX, S / 2 + S * 4 + offsetY));
            }
            else
            {
                e.Graphics.DrawString($"Ход Черных", new Font("Times New Roman", 36, FontStyle.Bold),
                   Brushes.Black, new PointF(W * S + S / 2 + offsetX, S / 2 + S * 4 + offsetY));
            }

            pen = new Pen(Brushes.DarkBlue, 5);
            for (int i = 0; i < choosen.Count; i++)
            {
                offline = (W * S - map[choosen[i].Y].Count * S) / 2;
                e.Graphics.DrawEllipse(pen, choosen[i].X * S + offsetX + offline,
                                            choosen[i].Y * S + offsetY, S, S);
            }
        }

        private void Game_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (choosen.Count >= 3) { return; }
                int x = (e.X - offsetX);
                int y = (e.Y - offsetY);

                if (x < 0 || x >= W * S || y < 0 || y >= H * S) { return; }
                y /= S;

                float offline = (W * S - map[y].Count * S) / 2;
                x -= (int)offline;
                if (x < 0 || x >= map[y].Count * S) { return; }
                x /= S;

                if (!CheckLine(x, y)) { choosen.Clear(); Invalidate(); return; }

                if (map[y][x].Value == curPlayer.Ind && !CheckExistInChoosen(x, y))
                {
                    choosen.Add(new Cell(map[y][x].Value, x, y, ChooseType(y)));
                    Invalidate();
                }
            }
            else
            {
                if (choosen.Count <= 0) { return; }

                int x = (e.X - offsetX);
                int y = (e.Y - offsetY);

                if (x < 0 || x >= W * S || y < 0 || y >= H * S) { return; }
                y /= S;

                float offline = (W * S - map[y].Count * S) / 2;
                x -= (int)offline;
                if (x < 0 || x >= map[y].Count * S) { return; }
                x /= S;

                if (CheckExistInChoosen(x, y)) { choosen.Clear(); Invalidate(); return; }

                int nextCell = choosen[0].ChoiceOption(x, y);
                if (nextCell < 0) { choosen.Clear(); Invalidate(); return; }

                if (MoveChoosen(nextCell))
                {
                    ChangeTurn();
                }
                else if (PushChoosen(nextCell))
                {
                    ChangeTurn();
                }
                choosen.Clear();
                Invalidate();

                if (CheckWin())
                {
                    Close();
                }
            }
        }

        private bool CheckLine(int x, int y)
        {
            if (choosen.Count == 1)
            {
                int ind = choosen[0].ChoiceOption(x, y);
                if (ind < 0) { return false; }
            }
            else if (choosen.Count == 2)
            {
                int ind = choosen[0].ChoiceOption(choosen[1].X, choosen[1].Y);
                Point next = choosen[1].GetNexCell(ind);
                if (next.X != x || next.Y != y) { return false; }
            }
            return true;
        }

        private int InvertedNext(int next)
        {
            if (next == 0) { return 3; }
            if (next == 1) { return 4; }
            if (next == 2) { return 5; }

            if (next == 3) { return 0; }
            if (next == 4) { return 1; }
            return 2;
        }
        private bool CheckPushChoosen(int next)
        {
            Point nextCell;
            if (choosen.Count <= 1)
            {
                nextCell = choosen[0].GetNexCell(next);
                if (nextCell.Y < 0 || nextCell.Y >= H ||
                    nextCell.X < 0 || nextCell.X >= map[nextCell.Y].Count ||
                    map[nextCell.Y][nextCell.X].Value != 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            nextCell = choosen[0].GetNexCell(InvertedNext(next));
            if (!CheckExistInChoosen(nextCell.X, nextCell.Y)) { return false; }

            nextCell = choosen[0].GetNexCell(next);
            int power = choosen.Count;
            int powerEnemy = 0;

            if (map[nextCell.Y][nextCell.X].Value != 0 &&
                map[nextCell.Y][nextCell.X].Value != curPlayer.Ind)
            {
                while (true)
                {
                    choosen.Add(new Cell(map[nextCell.Y][nextCell.X].Value, nextCell.X,
                                         nextCell.Y, ChooseType(nextCell.Y)));
                    powerEnemy++;
                    nextCell = map[nextCell.Y][nextCell.X].GetNexCell(next);

                    if (nextCell.Y < 0 || nextCell.Y >= H ||
                        nextCell.X < 0 || nextCell.X >= map[nextCell.Y].Count ||
                        map[nextCell.Y][nextCell.X].Value == 0)
                    {
                        if (power > powerEnemy)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (map[nextCell.Y][nextCell.X].Value == curPlayer.Ind)
                        {
                            return false;
                        }
                    }
                }
            }
            else if (map[nextCell.Y][nextCell.X].Value == 0)
            {
                return true;
            }

            return false;
        }
        private bool PushChoosen(int next)
        {
            if (!CheckPushChoosen(next)) { return false; }

            int curX, curY, value;
            Point nextCell;
            for (int i = 0; i < choosen.Count; i++)
            {
                curX = choosen[i].X;
                curY = choosen[i].Y;
                map[curY][curX].Value = 0;
            }

            for (int i = 0; i < choosen.Count; i++)
            {
                curX = choosen[i].X;
                curY = choosen[i].Y;
                value = choosen[i].Value;

                nextCell = choosen[i].GetNexCell(next);
                if (nextCell.Y < 0 || nextCell.Y >= H ||
                    nextCell.X < 0 || nextCell.X >= map[nextCell.Y].Count)
                {
                    curPlayer.Score++;
                }
                else
                {
                    map[nextCell.Y][nextCell.X] = new Cell(value, nextCell.X,
                                                           nextCell.Y, ChooseType(nextCell.Y));
                }
            }

            return true;
        }

        private bool CheckMoveChoosen(int next)
        {
            Point nextCell;
            for (int i = 0; i < choosen.Count; i++)
            {
                nextCell = choosen[i].GetNexCell(next);

                if (nextCell.Y < 0 || nextCell.Y >= H ||
                    nextCell.X < 0 || nextCell.X >= map[nextCell.Y].Count)
                {
                    return false;
                }
                if (map[nextCell.Y][nextCell.X].Value != 0)
                {
                    return false;
                }
            }
            return true;
        }
        private bool MoveChoosen(int next)
        {
            if (!CheckMoveChoosen(next)) { return false; }

            int curX, curY, value;
            Point nextCell;
            for (int i = 0; i < choosen.Count; i++)
            {
                curX = choosen[i].X;
                curY = choosen[i].Y;
                value = choosen[i].Value;

                nextCell = choosen[i].GetNexCell(next);

                map[curY][curX].Value = 0;
                map[nextCell.Y][nextCell.X] = new Cell(value, nextCell.X,
                                                       nextCell.Y, ChooseType(nextCell.Y));
            }
            return true;
        }

        private int ChooseType(int y)
        {
            if (y < 4) { return 1; }
            if (y == 4) { return 2; }
            return 3;
        }
        private bool CheckExistInChoosen(int x, int y)
        {
            for (int i = 0; i < choosen.Count; i++)
            {
                if (choosen[i].X == x && choosen[i].Y == y)
                {
                    return true;
                }
            }
            return false;
        }

        private void ChangeTurn()
        {
            if (curPlayer.Ind == players[0].Ind)
            {
                curPlayer = players[1];
            }
            else { curPlayer = players[0]; }
        }

        private bool CheckWin()
        {
            if (players[0].Score >= 6)
            {
                MessageBox.Show("White Win");
                return true;
            }
            else if (players[1].Score >= 6)
            {
                MessageBox.Show("Black Win");
                return true;
            }
            return false;
        }
    }
}
