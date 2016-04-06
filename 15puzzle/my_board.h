#ifndef BOARD_H
#define BOARD_H

#include <iostream>
using namespace std;

class my_board
{
  int data[4][4];
  int z_x = 0;
  int z_y = 0;

public:
  my_board(int m[4][4])
  {
    for (int i = 0; i < 4; i++)
    {
      for (int j = 0; j < 4; j++)
      {
        data[i][j] = m[i][j];
        if (m[i][j] == 0)
        {
          z_x = i;
          z_y = j;
        }
      }
    }
  }

  void make_right()
  {
    int i = z_x;
    int j = z_y;
    data[i][j] = data[i][j + 1];
    data[i][j + 1] = 0;
    z_y++;
  }
   
  void make_left()
  {
    int i = z_x;
    int j = z_y;
    data[i][j] = data[i][j - 1];
    data[i][j - 1] = 0;
    z_y--;
  }

  void make_up()
  {
    int i = z_x;
    int j = z_y;
    data[i][j] = data[i - 1][j];
    data[i - 1][j] = 0;
    z_x--;
  }
    
  void make_down()
  {
    int i = z_x;
    int j = z_y;
    data[i][j] = data[i + 1][j];
    data[i + 1][j] = 0;
    z_x++;
  }

  friend ostream& operator<<(ostream& os, const my_board& m);
};

inline ostream& operator<<(ostream& os, const my_board& m)
{
    for (int i = 0; i < 4; i++)
    {
      for (int j = 0; j < 4; j++)
      {
        os.width(3);
        if (m.data[i][j] == 0)
          os << left << "_";
        else
          os << left << m.data[i][j];
      }
      os << endl;
    }
    return os;
}

#endif