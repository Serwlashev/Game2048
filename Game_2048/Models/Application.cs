using Game_2048.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_2048.Models
{
    class Application
    {
        Game game;                          // Экземпляр игры
        string username;                    // Имя пользователя
        public void Start()
        {
            username = InitUser();          // Вводим имя пользователя
            int size = EnterSize();         // Вводим размер поля
            game = new Game(username, size);// Запускаем игру
            try
            {
                while (true)
                {
                    Console.Clear();
                    DrawField();            

                    var key = Console.ReadKey().Key;

                    switch (key)
                    {
                        case ConsoleKey.LeftArrow:
                            game.ToLeft();
                            break;
                        case ConsoleKey.RightArrow:
                            game.ToRight();
                            break;
                        case ConsoleKey.DownArrow:
                            game.ToDown();
                            break;
                        case ConsoleKey.UpArrow:
                            game.ToUp();
                            break;
                        default:
                            continue;
                    }

                    game.CheckViktory();    // Проверяем есть ли 2048 на поле
                    game.InitNewTurn();     // Инициализируем новый ход - выставляем новую цифру и сбрасываем свойство ячейки, что она была перемещена
                }
            }
            catch (GameOverException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch(VictoryException ex)
            {
                DrawField();
                Console.WriteLine(ex.Message);
            }
            ContinueGame();

        }

        private void ContinueGame()         
        {
            Console.WriteLine("Вы хотите сыграть еще раз? (1 - да, 0 - нет)");
            bool choice = EnterBool();
            if (choice)
            {
                Cell.Score = 0;
                Start();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("До свидания!");
            }
        }

        private bool EnterBool()
        {
            string res = Console.ReadLine();

            if (res.Equals("1"))
            {
                return true;
            }
            return false;
        }

        private int EnterSize()
        {
            int size = 4;
            while(true)
            {
                Console.WriteLine("Введите размер поля от 4 до 10: ");

                try
                {
                    size = int.Parse(Console.ReadLine());
                    if(size >= 4 && size <= 10)
                    {
                        break;
                    }
                }
                catch(Exception)
                {
                    Console.WriteLine("Введите еще раз!");
                }
            }
            return size;
        }

        private string InitUser()
        {
            if(this.username != null)
            {
                return this.username;
            }

            Console.Write("Введите имя пользователя: ");
            string username = Console.ReadLine();
            return username;
        }
        private void DrawField()
        {
            Console.WriteLine("Игрок: " + username);
            Console.WriteLine($"Счет: {Cell.Score}\n");
            for (int i = 0; i < game.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < game.Cells.GetLength(1); j++)
                {
                    var cell = game.Cells[i, j].Value == 0 ? " " : game.Cells[i, j].Value.ToString();
                    Console.Write($"{cell,3}");
                }
                Console.WriteLine();
            }
        }
    }
}
