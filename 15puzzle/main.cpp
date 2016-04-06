#include <stdio.h>
#include <vector>
#include <algorithm> 
#include <time.h>  
#include <string>
#include <sstream>
#include "my_board.h"

class node
{
public: int x, y;
        node(int xx, int yy) { x = xx; y = yy; }
        node() { }
};

//int board[4][4] = { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
//int board[4][4] = { { 2, 11, 14, 5 }, { 9, 7, 0, 4 }, { 13, 12, 1, 3 }, { 8, 10, 15, 6 } };
int board[4][4] = { { 0, 11, 14, 5 }, { 9, 7, 2, 4 }, { 13, 12, 1, 3 }, { 8, 10, 15, 6 } };
int exmpl_board[4][4] = { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 }, { 13, 14, 15, 0 } };

vector<node*> can_change;
vector<node*> manh_board;

char move_desc[] = { 'D', 'U', 'R', 'L' };
char oposite_desc[] = { 'U', 'D', 'L', 'R' };

string result = "";

node* serch_zero()
{
  for (int i = 0; i < 4; i++)
    for (int j = 0; j < 4; j++)
      if (board[i][j] == 0)
        return new node(j, i);
  return false;
}

bool is_availible_board()
{
  int shoud_be_even = 0;
  for (int i = 0; i < 4; i++)
  {
    for (int j = 0; j < 4; j++)
    {
      int k = board[i][j];
      if (k != 0)
      {
        for (int i2 = i * 4 + j; i2 < 16; i2++)
        {
          if (k>board[i2 / 4][i2 % 4] && board[i2 / 4][i2 % 4] != 0)
            shoud_be_even += 1;
        }
      }
    }
  }
  int k = (serch_zero()->y + 1 + shoud_be_even);

  if ((serch_zero()->y + 1 + shoud_be_even) % 2 == 0)
    return true;
  else
    return false;
}

void initialization()
{
  can_change.push_back(new node(0, 1));
  can_change.push_back(new node(0, -1));
  can_change.push_back(new node(1, 0));
  can_change.push_back(new node(-1, 0));

  manh_board.push_back(new node(3, 3));
  manh_board.push_back(new node(0, 0));
  manh_board.push_back(new node(1, 0));
  manh_board.push_back(new node(2, 0));
  manh_board.push_back(new node(3, 0));
  manh_board.push_back(new node(0, 1));
  manh_board.push_back(new node(1, 1));
  manh_board.push_back(new node(2, 1));
  manh_board.push_back(new node(3, 1));
  manh_board.push_back(new node(0, 2));
  manh_board.push_back(new node(1, 2));
  manh_board.push_back(new node(2, 2));
  manh_board.push_back(new node(3, 2));
  manh_board.push_back(new node(0, 3));
  manh_board.push_back(new node(1, 3));
  manh_board.push_back(new node(2, 3));
}

int manh_heuristic(int x, int y, int i)
{
  int k = board[x][y];
  if (k != 0)
  {
    int div_i = k / i + 1;
    int mod_i = k % i;
    if (mod_i == 0)
    {
      div_i -= 1;
      mod_i += i;
    }
    return abs(x + 1 - div_i) + abs(y + 1 - mod_i);
  }
  return 0;
}

int heurisic()
{
  int result = 0;
  for (int i = 0; i < 4; i++)
    for (int j = 0; j < 4; j++)
      result += manh_heuristic(i, j, 4);
  return result;
}

bool is_goal()
{
  for (int i = 0; i < 4; i++)
    for (int j = 0; j < 4; j++)
      if (board[i][j] != exmpl_board[i][j])
        return false;
  return true;
}

bool is_availible_coord(node* n, int prev, int curr)
{
  if (prev != -1)
    if (n->x >= 0 && n->x < 4 && n->y >= 0 && n->y < 4 && oposite_desc[prev] != move_desc[curr])
      return true;
    else
      return false;
  else
    if (n->x >= 0 && n->x < 4 && n->y >= 0 && n->y < 4)
      return true;
    else
      return false;
}

void swap(node* n, int x, int y)
{
  int k = board[x][y];
  board[x][y] = board[n->y][n->x];
  board[n->y][n->x] = k;
}

int search(int x, int y, int g, int bound, int prev)
{
  int heu = heurisic();
  int f = g + heu;
  if (f > bound)
    return f;
  if (heu == 0)
    return -1;
  int min = INT_MAX;
  for (int i = 0; i < 4; i++)
  {
    node* succ = new node(y + can_change[i]->x, x + can_change[i]->y);
    if (is_availible_coord(succ, prev, i))
    {
      swap(succ, x, y);
      int tmp = search(succ->y, succ->x, g + 1, bound, i);
      if (tmp == -1)
      {
        result += move_desc[i];
        return -1;
      }
      swap(succ, x, y);
      if (tmp < min)
        min = tmp;
    }
  }
  return min;
}

bool ida_star()
{
  int heu = heurisic();
  while (true)
  {
    node* k = serch_zero();
    int tmp = search(k->y, k->x, 0, heu, -1);
    if (tmp == -1)
      return true;
    if (tmp == INT_MAX)
      return false;
    heu = tmp;
  }
}

my_board* show;
void print_solve()
{
  cout << endl << (*show) << endl;
  for (auto it = result.rbegin(); it != result.rend(); it++)
  {
    switch (*it)
    {
      case 'R':
        show->make_right();
        break;
      case 'L':
        show->make_left();
        break;
      case 'U':
        show->make_up();
        break;
      case 'D':
        show->make_down();
        break;
    }
    cout << endl << (*show) << endl;
  }
}

int main()
{
  setlocale(0, "");
  srand(time(NULL));
  initialization();

  cout << "Задайте доску: " << endl;
  //for (int i = 0; i < 4; i++)
  //  for (int j = 0; j < 4; j++)
  //    cin >> board[i][j];

  show = new my_board(board);

  if (is_availible_board())
  {
    clock_t time = clock();
    ida_star();
    print_solve();
    cout << "Длина: " << result.length() << endl;
    time = clock() - time;
    cout << "Время вычисления: " << (double)time / 1000 << endl;
  }
  else
    cout << "Нет решения!" << endl;
  system("pause");
  return 0;
}