using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace Infection
{
	public partial class Form1 : Form
	{
		int[][] board;
		Button first_butt;
		bool first, blue_play, is_refresh, is_valid_move;
		Color computer_col;
		int x, y;
		Hashtable archive;

		public Form1()
		{
			InitializeComponent();
			new_game();
		}

		private void new_game()
		{
			board = new int[][]{
				new int[] {1,0,0,0,0,2},
				new int[] {0,0,0,0,0,0},
				new int[] {0,0,0,0,0,0},
				new int[] {0,0,0,0,0,0},
				new int[] {0,0,0,0,0,0},
				new int[] {2,0,0,0,0,1}
			};
			first_butt = null;
			first = true;
			blue_play = true;
			is_refresh = false;
			is_valid_move = false;
			computer_col = Color.Blue;
			x = 0; y = 0;
			archive = new Hashtable();
		}

		/// <summary>
		/// геймплей
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <param name="b"></param>
		private void game(int x, int y, int x2, int y2, Button b)
		{
			int c;
			if (blue_play)
				c = 1;
			else c = 2;
			if (board[y - 1][x - 1] == c)
			{
				int move = can_move(y, x, y2, x2);
				if (move == 1)
				{
					clone(y2, x2, c, b);
					infect(x2, y2);
					change_step();
				}
				else if (move == 2)
				{
					transfer(y, x, y2, x2, b);
					infect(x2, y2);
					change_step();
				}
			}
			count_green.Text = green_count().ToString();
			count_blue.Text = blue_count().ToString();
		}

		/// <summary>
		/// зафиксировать ход
		/// </summary>
		private void change_step()
		{
			if (blue_play)
			{
				blue_play = false;
				butt_curr_color.BackColor = Color.Red;
			}
			else
			{
				blue_play = true;
				butt_curr_color.BackColor = Color.Blue;
			}
		}

		/// <summary>
		/// инфицировать окружающих
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void infect(int x, int y)
		{
			int c1, c2;
			Color curr_col;
			if (blue_play)
			{
				c1 = 2;
				c2 = 1;
				curr_col = Color.Blue;
			}
			else
			{
				c1 = 1;
				c2 = 2;
				curr_col = Color.Red;
			}
			for (int i = x - 2; i <= x; i++)
				for (int j = y - 2; j <= y; j++)
					if (i >= 0 && i <= 5 && j >= 0 && j <= 5 && board[j][i] == c1)
					{
						board[j][i] = c2;
						int k = j * 6 + i + 1;
						this.Controls.Find("but" + k, true)[0].BackColor = curr_col;
					}
		}

		/// <summary>
		/// есть ли ходы для указанной позиции
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private bool have_move(int x, int y)
		{
			for (int i = x - 2; i <= x + 2; i++)
				for (int j = y - 2; j <= y + 2; j++)
					if (i > 0 && i < 7 && j > 0 && j < 7)
						if (can_move(x, y, i, j) != 0)
							return true;
			return false;
		}

		/// <summary>
		/// есть ли ход для указанного цвета
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		private bool have_step(int c) 
		{
			for (int i = 1; i <= 6; i++)
				for (int j = 1; j <= 6; j++)
					if (board[i - 1][j - 1] == c && have_move(i, j))
						return true;
			return false;
		}

		/// <summary>
		/// можно ли ходить из (х,у) в (х2,у2)
		/// 0 - нет хода, 1 - клонировать, 2 - переместить
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <returns></returns>
		private int can_move(int x, int y, int x2, int y2) 
		{
			if (board[x2 - 1][y2 - 1] == 0)
			{
				int xx = x - x2;
				int yy = y - y2;
				if (xx / 2 == 0 && yy / 2 == 0)
					return 1;
				if ((xx == 2 && yy == 0) || (xx == 0 && yy == 2) || (xx == 2 && yy == 2) || (xx == -2 && yy == 0) || (xx == 0 && yy == -2) || (xx == -2 && yy == -2) || (xx == -2 && yy == -2) || (xx == -2 && yy == 2) || (xx == 2 && yy == -2))
					return 2;
			}

			return 0;
		}

		/// <summary>
		/// клонировать инфекцию
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="c"></param>
		/// <param name="b"></param>
		private void clone(int x, int y, int c, Button b) 
		{
			board[x - 1][y - 1] = c;
			if (c == 1)
				b.BackColor = Color.Blue;
			if (c == 2)
				b.BackColor = Color.Red;
		}

		/// <summary>
		/// переместить инфекцию
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <param name="b"></param>
		private void transfer(int x1, int y1, int x2, int y2, Button b)
		{
			int s = 0;
			if (first_butt.BackColor == Color.Red)
				s = 2;
			if (first_butt.BackColor == Color.Blue)
				s = 1;
			board[x1 - 1][y1 - 1] = 0;
			board[x2 - 1][y2 - 1] = s;
			b.BackColor = first_butt.BackColor;
			first_butt.BackColor = Color.White;
		}

		/// <summary>
		/// обработка нажатий
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, EventArgs e) 
		{
			int xb = ((sender as Button).Location.X + 28) / 40;
			int yb = ((sender as Button).Location.Y + 28) / 40;
			if (first)
			{
				if ((sender as Button).BackColor == Color.White)
					return;
				y = yb;
				x = xb;
				first = false;
				(sender as Button).FlatStyle = FlatStyle.System;
				first_butt = (sender as Button);
			}
			else
			{
				if (first_butt == (sender as Button))
				{
					first_butt = null;
					first = true;
					(sender as Button).FlatStyle = FlatStyle.Popup;
					return;
				}
				if ((sender as Button).BackColor == Color.White && can_move(y,x,yb,xb) != 0)
					is_valid_move = true;
				else
					is_valid_move = false;
				game(x, y, xb, yb, (sender as Button));
				is_end();
				if (is_valid_move)
				{
					make_step(!blue_play, 0, -1000, 1000, 3);
					Thread.Sleep(1000);
				}
			}
		}

		/// <summary>
		/// завершена ли игра
		/// </summary>
		private void is_end()
		{
			if (!have_step(1) || !have_step(2))
			{
				MessageBox.Show("Выиграли " + (int.Parse(count_blue.Text) < int.Parse(count_green.Text) ? "зеленые!" : "синие!"), "Вот и всё!");
			}
			first_butt.FlatStyle = FlatStyle.Popup;
			first = is_refresh = true;
		}

		/// <summary>
		/// эвристика
		/// </summary>
		/// <returns></returns>
		private int euristic()
		{
			int green = green_count();
			int blue = blue_count();
			return green - blue;
		}
		
		private int green_count()
		{
			int counter = 0;
			for (int i = 0; i <= 5; i++)
				for (int j = 0; j <= 5; j++)
					if (board[i][j] == 2)
						counter++;
			return counter;
		}

		private int blue_count()
		{
			int counter = 0;
			for (int i = 0; i <= 5; i++)
				for (int j = 0; j <= 5; j++)
					if (board[i][j] == 1)
						counter++;
			return counter;
		}

		/// <summary>
		/// просчет ходов (альфа-бета отсечение)
		/// </summary>
		/// <param name="my_turn"></param>
		/// <param name="depth"></param>
		/// <param name="alpha"></param>
		/// <param name="beta"></param>
		/// <param name="max_depth"></param>
		/// <returns></returns>
		private int make_step(bool my_turn, int depth, int alpha, int beta, int max_depth)
		{
			if (is_refresh)
			{
				Refresh();
				is_refresh = false;
			}

			int eurist = euristic();

			int tmp = -99; // 

			//на листьях дерева возвращаем значение эвристики
			if (depth > max_depth)
				return eurist;

			int[] bestMove = new int[4] { -1, -1, -1, -1 };

			int minimax = my_turn ? int.MinValue : int.MaxValue;
			int move;
			int c = my_turn ? 2 : 1;
			for (int i = 1; i <= 6; i++)
				for (int j = 1; j <= 6; j++)
					if (board[i - 1][j - 1] == c)
						for (int i2 = i - 2; i2 <= i + 2; i2++)
							for (int j2 = j - 2; j2 <= j + 2; j2++)
								if (i2 > 0 && i2 < 7 && j2 > 0 && j2 < 7)
								{
									move = can_move(i, j, i2, j2);
									if (move != 0 && board[i2 - 1][j2 - 1] == 0)
									{
										List<int> L = tmp_move(c, i, j, i2, j2, move);
										tmp = make_step(!my_turn, depth + 1, alpha, beta, max_depth);
										tmp_move_back(L, c, i, j, i2, j2, move);
										if ((tmp > minimax && my_turn) || (tmp <= minimax && !my_turn))
										{
											minimax = tmp;
											bestMove[0] = i;
											bestMove[1] = j;
											bestMove[2] = i2;
											bestMove[3] = j2;
										}
										if (my_turn)
											alpha = Math.Max(alpha, tmp);
										else
											beta = Math.Min(beta, tmp);
										if (beta < alpha)
											break;
									}
								}

			if (bestMove[0] == 99)
				return eurist;

			//выбрали лучший ход - делаем ход
			if (depth == 0 && bestMove[0] != 99)
			{
				int k = (bestMove[0] - 1) * 6 + bestMove[1];
				int k2 = (bestMove[2] - 1) * 6 + bestMove[3];
				first_butt = this.Controls.Find("but" + k, true)[0] as Button;
				game(bestMove[1], bestMove[0], bestMove[3], bestMove[2], this.Controls.Find("but" + k2, true)[0] as Button);
				is_end();
			}
			return minimax;
		}

		private List<int> tmp_move(int c, int i, int j, int i2, int j2, int move)
		{
			List<int> result = new List<int>();
			if (move == 1)
			{
				board[i2 - 1][j2 - 1] = c;
				int c2 = 2;
				if (c == 2)
				{
					c2 = 1;
				}
				for (int ii = j2 - 2; ii <= j2; ii++)
					for (int jj = i2 - 2; jj <= i2; jj++)
						if (ii >= 0 && ii <= 5 && jj >= 0 && jj <= 5 && board[jj][ii] == c2)
						{
							board[jj][ii] = c;
							result.Add(jj);
							result.Add(ii);
						}
			}
			else if (move == 2)
			{
				board[i - 1][j - 1] = 0;
				board[i2 - 1][j2 - 1] = c;
				int c2 = 2;
				if (c == 2)
				{
					c2 = 1;
				}
				for (int ii = j2 - 2; ii <= j2; ii++)
					for (int jj = i2 - 2; jj <= i2; jj++)
						if (ii >= 0 && ii <= 5 && jj >= 0 && jj <= 5 && board[jj][ii] == c2)
						{
							board[jj][ii] = c;
							result.Add(jj);
							result.Add(ii);
						}
			}
			return result;

		}
		private void tmp_move_back(List<int> L, int c, int i, int j, int i2, int j2, int move)
		{
			if (move == 1)
			{
				board[i2 - 1][j2 - 1] = 0;
				int c2 = 2;
				if (c == 2)
				{
					c2 = 1;
				}
				for (int it = 0; it < L.Count; it++)
					board[L[it]][L[++it]] = c2;
			}
			else if (move == 2)
			{
				board[i - 1][j - 1] = c;
				board[i2 - 1][j2 - 1] = 0;
				int c2 = 2;
				if (c == 2)
				{
					c2 = 1;
				}
				for (int it = 0; it < L.Count; it++)
					board[L[it]][L[++it]] = c2;
			}

		}

		private void button2_Click(object sender, EventArgs e)
		{
			make_step(!blue_play, 0, -1000, 1000, 4);
		}
	}
}
