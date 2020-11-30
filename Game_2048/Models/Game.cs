using Game_2048.Exceptions;
using System;
using System.Security.Cryptography;

namespace Game_2048.Models
{
    class Game
    {
        Random rnd = new Random();
        //размер поля
        readonly int fieldSize;
        //имя игрока
        string username;
        //игровое поле
        public Cell[,] Cells { get; private set; }


        public Game(string username, int fieldSize = 4)
        {
            this.username = username;
            Cells = new Cell[fieldSize, fieldSize];
            this.fieldSize = fieldSize;
            Init();
        }

        private void Init()                         // Метод для инициализации поля
        {
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    Cells[i, j] = new Cell();
                }
            }
            SetNumOnField();
            SetNumOnField();
        }

        private (int i, int j) GenerateCellPos()
        {
            if(!CheckEmptyCell())
            {
                if (!CanSumCells())                 // Если нельзя перемещать ячейки ни в одном направлении - проигрыш
                {
                    throw new Exceptions.GameOverException();
                }
                else                                // Иначе пропускаем ход
                {
                    throw new SkipTurnException();
                }
            }
            
            int i, j;
            //пока не будет сгенерирована пустая ячейка
            do
            {
                i = rnd.Next(0, fieldSize);
                j = rnd.Next(0, fieldSize);
            } while (Cells[i, j].Value != 0);
            return (i, j);
        }

        public void CheckViktory()
        {
            for (int i = 0; i < Cells.GetLength(0); i++)             // Перебираем все поле
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    if (Cells[i, j].Value == 2048)                   // Если есть 2048 - игрок выиграл
                    {
                        throw new VictoryException();
                    }
                }
            }
        }

        internal void InitNewTurn()                          
        {
            DiskardAdded();                                        // Сбрасываем добавленные ячейки на текущем круге

            SetNumOnField();                                       // Устанавливаем рандомную ячейку на поле
        }

        private void SetNumOnField()
        {
            if (!CheckEmptyCell() && !CanSumCells())
            {
                throw new Exceptions.GameOverException();
            }
            try
            {
                var pos = GenerateCellPos();

                int n = rnd.Next(1, 101);

                //ставим '2'
                if (n <= 90)
                {
                    Cells[pos.i, pos.j].Value = 2;
                }
                else    //ставим '4'
                {
                    Cells[pos.i, pos.j].Value = 4;
                }
            }
            catch(SkipTurnException)
            {

            }
        }

        // Проверка можно ли еще сложить ячейки на поле в любом направлении
        private bool CanSumCells()
        {
            for(int i = 0; i < Cells.GetLength(0); i++)             // Перебираем все поле
            {
                for(int j = 0; j < Cells.GetLength(1); j++)
                {
                    if( j != Cells.GetLength(1) - 1 && Cells[i, j] == Cells[i, j + 1])      // Если можно сложить ячейки по горизонтали возвращаем истину
                    {
                        return true;
                    }
                    if(i != Cells.GetLength(0) - 1 && Cells[i, j] == Cells[i + 1, j])      // Если можно сложить ячейки по вертикали возвращаем истину
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Поиск пустой ячейки на поле
        private bool CheckEmptyCell()
        {
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    if (Cells[i, j].Value == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ToLeft()
        {
            for (int i = 0; i < Cells.GetLength(0); i++)                        // Перебираем от начала поля все ячейки
            {
                for (int j = Cells.GetLength(1) - 1; j >= 0; j--)               // Двигаемся от правого края поля налево
                {
                    int curCell = j;                                            // Текущая ячейка находится на позиции j
                    while (curCell > 0 && Cells[i, curCell - 1].Value == 0)     // Выполняем, пока не дойдем влево и пока ячейка слева равна нулю (есть куда перемещать ячейки)
                    {
                        Swap(ref Cells[i, curCell], ref Cells[i, curCell - 1]); // Перемещаем текущую ячейку с той, что находится левее
                        curCell--;                                              // Переходим к следующей ячейке
                    }

                    int tmpCell = 0;                                            // Текущая ячейка при сложении
                    //сложение значений
                    while (tmpCell < fieldSize - 1)                             // Пока текущая ячейка не дойдет до края поля
                    {
                        // Если можно складывать (ячейка не была изменена в текущем ходе), следующая ячейка равна предыдущей и она не равна нуля
                        if (!(Cells[i, tmpCell].WasAdded || Cells[i, tmpCell + 1].WasAdded) && 
                            Cells[i, tmpCell] == Cells[i, tmpCell + 1] && Cells[i, tmpCell].Value != 0)
                        {
                            Cells[i, tmpCell]++;                                // Увеличиваем значение текущея ячейки в два раза
                            Cells[i, tmpCell + 1].Value = 0;                    // Обнуляем предыдущую ячейку
                            Cells[i, tmpCell].WasAdded = true;

                            tmpCell++;                                          // Переходим к следующей ячейке

                        }
                        tmpCell++;                                              // Переходим к следующей ячейке
                    }

                }
            }
            if (CheckRowsLeft())                                                // Проверяем есть ли еще не перемещенные ячейки и если есть - перемещаем их еще раз
            {
                ToLeft();
            }
        }

        private void DiskardAdded()                                             // Метод для обнуления свойства ячеек, что они были изменены за текущим перемещением
        {
            for(int i = 0; i < Cells.GetLength(0); i++)
            {
                for(int j = 0; j < Cells.GetLength(1); j++)
                {
                    Cells[i, j].WasAdded = false;
                }
            }
        }

        public void ToRight()
        {
            for (int i = 0; i < Cells.GetLength(0); i++)                        // Перебираем все ячейки
            {
                for (int j = 0; j < Cells.GetLength(1); j++)                    // Двигаемся от левого края поля направо
                {
                    int curCell = j;                                            // Текущая ячейка при сложении
                    while (curCell < Cells.GetLength(1) - 1 && Cells[i, curCell + 1].Value == 0)     // Выполняем, пока не дойдем вправо и пока ячейка справа равна нулю (есть куда перемещать ячейки)
                    {
                        Swap(ref Cells[i, curCell], ref Cells[i, curCell + 1]); // Перемещаем текущую ячейку с той, что находится левее
                        curCell++;                                              // Переходим к следующей ячейке
                    }
                    int tmpCell = fieldSize - 1;                                // Текущая ячейка при сложении
                    //сложение значений
                    while (tmpCell > 0)                                         // Пока текущая ячейка не дойдет до края поля
                    {
                        // Если можно складывать (ячейка не была изменена в текущем ходе), следующая ячейка равна предыдущей и она не равна нуля
                        if (!(Cells[i, tmpCell].WasAdded || Cells[i, tmpCell - 1].WasAdded) && 
                            Cells[i, tmpCell] == Cells[i, tmpCell - 1] && Cells[i, tmpCell].Value != 0)
                        {
                            Cells[i, tmpCell]++;                                 // Увеличиваем значение текущея ячейки в два раза
                            Cells[i, tmpCell - 1].Value = 0;                     // Обнуляем предыдущую ячейку
                            Cells[i, tmpCell].WasAdded = true;

                            tmpCell--;                                           // Переходим к следующей ячейке
                        }
                        tmpCell--;                                               // Переходим к следующей ячейке
                    }
                }
            }
            if(CheckRowsRight())                                                // Проверяем есть ли еще не перемещенные ячейки и если есть - перемещаем их еще раз
            {
                ToRight();
            }
        }

        public void ToDown()
        {
            for (int i = 0; i <= Cells.GetLength(1) - 1; i++)                    // Перебираем все ячейки
            {
                for (int j = 0; j <= Cells.GetLength(0) - 1; j++)               // Двигаемся от верха к низу
                {
                    int curCell = j;                                            // Текущая ячейка при сложении
                    while (curCell < Cells.GetLength(0) - 1 && Cells[curCell + 1, i].Value == 0)     // Выполняем, пока не дойдем вниз и пока ячейка снизу равна нулю (есть куда перемещать ячейки)
                    {
                        Swap(ref Cells[curCell, i], ref Cells[curCell + 1, i]); // Перемещаем текущую ячейку с той, что находится ниже
                        curCell++;                                              // Переходим к следующей ячейке
                    }
                    int tmpCell = fieldSize - 1;                                // Текущая ячейка при сложении
                    //сложение значений
                    while (tmpCell > 0)                                         // Пока текущая ячейка не дойдет до края поля
                    {
                        // Если можно складывать (ячейка не была изменена в текущем ходе), следующая ячейка равна предыдущей и она не равна нуля
                        if (!(Cells[tmpCell, i].WasAdded || Cells[tmpCell - 1, i].WasAdded) 
                            && Cells[tmpCell, i] == Cells[tmpCell - 1, i] && Cells[tmpCell, i].Value != 0)
                        {
                            Cells[tmpCell, i]++;                                 // Увеличиваем значение текущея ячейки в два раза
                            Cells[tmpCell - 1, i].Value = 0;                     // Обнуляем предыдущую ячейку
                            Cells[tmpCell, i].WasAdded = true;

                            tmpCell--;                                           // Переходим к следующей ячейке
                        }
                        tmpCell--;                                               // Переходим к следующей ячейке
                    }
                }
            }
            if(CheckRowsDown())                                                // Проверяем есть ли еще не перемещенные ячейки и если есть - перемещаем их еще раз
            {
                ToDown();
            }
        }

        public void ToUp()
        {
            for (int i = 0; i < Cells.GetLength(1); i++)                        // Перебираем все ячейки
            {
                for (int j = Cells.GetLength(0) - 1; j >= 0; j--)               // Двигаемся от низа к верху
                {
                    int curCell = j;                                            // Текущая ячейка при сложении
                    while (curCell > 0 && Cells[curCell - 1, i].Value == 0)     // Выполняем, пока не дойдем вверх и пока ячейка сверху равна нулю (есть куда перемещать ячейки)
                    {
                        Swap(ref Cells[curCell, i], ref Cells[curCell - 1, i]); // Перемещаем текущую ячейку с той, что находится выше
                        curCell--;                                              // Переходим к следующей ячейке
                    }
                    int tmpCell = 0;                                            // Текущая ячейка при сложении
                    //сложение значений
                    while (tmpCell < fieldSize - 1)                             // Пока текущая ячейка не дойдет до края поля
                    {
                        // Если можно складывать (ячейка не была изменена в текущем ходе), следующая ячейка равна предыдущей и она не равна нуля
                        if (!(Cells[tmpCell, i].WasAdded || Cells[tmpCell + 1, i].WasAdded) &&
                            Cells[tmpCell, i] == Cells[tmpCell + 1, i] && Cells[tmpCell, i].Value != 0)
                        {
                            Cells[tmpCell, i]++;                                 // Увеличиваем значение текущея ячейки в два раза
                            Cells[tmpCell + 1, i].Value = 0;                     // Обнуляем предыдущую ячейку
                            Cells[tmpCell, i].WasAdded = true;

                            tmpCell++;                                           // Переходим к следующей ячейке
                        }
                        tmpCell++;                                               // Переходим к следующей ячейке
                    }
                }
            }
            if(CheckRowsUp())                                                    // Проверяем есть ли еще не перемещенные ячейки и если есть - перемещаем их еще раз
            {
                ToUp();
            }
        }

        private bool CheckRowsLeft()                                             // Метод для проверки можно ли еще перемещать ячейки влево  
        {
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1) - 1; j++)
                {
                    if (Cells[i, j].Value == 0 && Cells[i, j + 1].Value != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckRowsRight()                                             // Метод для проверки можно ли еще перемещать ячейки вправо
        {
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = Cells.GetLength(1) - 1; j >= 1; j--)
                {
                    if (Cells[i, j].Value == 0 && Cells[i, j - 1].Value != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckRowsUp()                                             // Метод для проверки можно ли еще перемещать ячейки вверх
        {
            for (int i = 0; i < Cells.GetLength(1); i++)
            {
                for (int j = 0; j < Cells.GetLength(0) - 1; j++)
                {
                    if (Cells[j, i].Value == 0 && Cells[j + 1, i].Value != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool CheckRowsDown()                                             // Метод для проверки можно ли еще перемещать ячейки вниз
        {
            for (int i = 0; i < Cells.GetLength(1); i++)
            {
                for (int j = Cells.GetLength(0) - 1; j >= 1; j--)
                {
                    if (Cells[j, i].Value == 0 && Cells[j - 1, i].Value != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
