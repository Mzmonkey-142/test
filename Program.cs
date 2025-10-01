using System;
using System.Threading;

namespace Final_Project
{
    class Rock_dodger
    {
        static void Main(string[] args)
        {
            // ตั้งค่าขนาดเกม
            int height = 10;
            int width = 10;
            int rockLimit = width;

            int[,] rockData = CreateFilledArray(rockLimit, 2, -1);
            int[,] gameArea = new int[height, width];

            // ตำแหน่งผู้เล่น
            int playerX = width / 2;
            int playerY = height - 1;

            int loopCounter = 0;
            bool gameRunning = true; 
            int difficulty = 3;

            Console.WriteLine("Rock Dodger Game!");
            Console.Write("Input Difficulty 1-Normal 2-Hard : ");
            char dif = char.Parse(Console.ReadLine());
            if (dif == '1')
                difficulty = 3;
            else
                difficulty = 2;
            Console.WriteLine("Use A and D to move left and right. Avoid the red rocks!");
            
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();

            while (gameRunning)
            {
                // รับ input จากผู้เล่น
                HandlePlayerInput(ref playerX, width);

                if (loopCounter % difficulty == 0)
                {
                    // เลื่อนหินลง
                    MoveRocks(rockData, height);

                    // เพิ่มหินใหม่
                    if (ShouldAddRock(ref loopCounter))
                    {
                        AddRock(rockData, width);
                    }
                }
                else
                {
                    loopCounter++; 
                }

                // ตรวจสอบการชน
                if (CheckCollision(playerX, playerY, rockData))
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("GAME OVER!");
                    Console.WriteLine("You hit a rock!");
                    Console.WriteLine("Your Score is {0}", loopCounter);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    // Console.Clear();
                    gameRunning = false;
                }
                else
                {
                    // อัพเดทและวาดเกม
                    UpdateGameData(rockData, ref gameArea, playerX, playerY);
                    DrawGame(gameArea,loopCounter);

                    Thread.Sleep(50); 
                }
            }
        }

        // วาดหน้าจอเกม
        static void DrawGame(int[,] gameArea, int Score)
        {
            Console.Clear();
            for (int i = 0; i < gameArea.GetLength(0); i++)
            {
                for (int j = 0; j < gameArea.GetLength(1); j++)
                {
                    if (gameArea[i, j] == 0)
                        Console.Write("- ");
                    else if (gameArea[i, j] == 1) // หิน
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("# ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (gameArea[i, j] == 2) // ผู้เล่น
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("o ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                Console.WriteLine("");
            }
            Console.WriteLine("Use A and D to move left and right!");
            Console.WriteLine("Score : {0}",Score);
        }

        // อัพเดทข้อมูลเกม
        static void UpdateGameData(int[,] rockData, ref int[,] gameArea, int playerX, int playerY)
        {
            gameArea = new int[gameArea.GetLength(0), gameArea.GetLength(1)];

            for (int i = 0; i < rockData.GetLength(0); i++)         
            {
                int y = rockData[i, 1];
                int x = rockData[i, 0];
                if (y != -1)
                    gameArea[y, x] = 1;
            }
            if (playerY >= 0 && playerY < gameArea.GetLength(0) && playerX >= 0 && playerX < gameArea.GetLength(1))
                gameArea[playerY, playerX] = 2; 
        }

        // สร้าง array ที่เติมค่าเดียวกัน
        static int[,] CreateFilledArray(int rows, int cols, int value)
        {
            int[,] arr = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    arr[i, j] = value;
                }
            }
            return arr;
        }

        // เพิ่มหินใหม่
        static void AddRock(int[,] rockData, int limit)
        {
            Random rng = new Random();

            for (int i = 0; i < rockData.GetLength(0); i++)
            {
                if (rockData[i, 0] == -1)
                {
                    int randomX = rng.Next(limit);

                    rockData[i, 0] = randomX; //สุ่มค่า x แล้วให้หินอยู่บนสุด
                    rockData[i, 1] = 0;

                    break;
                }
            }
        }

        // เลื่อนหินลง
        static void MoveRocks(int[,] rockData, int gameHeight)
        {
            for (int i = 0; i < rockData.GetLength(0); i++)
            {

                if (rockData[i, 0] != -1)// เช็ดว่าในแถวนั้น มีหินไหม
                {
                    rockData[i, 1]++; //ถ้ามีบวกค่า Y ทำให้หินหล่น

                    if (rockData[i, 1] >= gameHeight) // ถึงพื้น(ค่าyสูงสุด) ให้ลยหินทิ้ง
                    {
                        rockData[i, 0] = -1;
                        rockData[i, 1] = -1;
                    }
                }
            }
        }

        // ตรวจสอบว่าต้องเพิ่มหินไหม
        static bool ShouldAddRock(ref int loopCounter)
        {
            if (loopCounter % 2 == 0) //ให้หินเส้นช่องกัน ปรับระยะการเว้นได้
            {
                loopCounter++;
                return true;
            }
            else
            {
                loopCounter++;
                return false;
            }
        }

        // รับ input จากผู้เล่น
        static void HandlePlayerInput(ref int playerX, int gameWidth)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);// อ่านinput

                if (key.Key == ConsoleKey.A && playerX > 0)
                {
                    playerX--; // ไปซ้าย
                }
                else if (key.Key == ConsoleKey.D && playerX < gameWidth - 1)
                {
                    playerX++; //ไปขวา
                }
            }
        }

        // ตรวจสอบการชน
        static bool CheckCollision(int playerX, int playerY, int[,] rockData)
        {
            for (int i = 0; i < rockData.GetLength(0); i++)
            {
                if (rockData[i, 0] != -1) //ไล่เช็ดหินทุกอัน
                {
                    int rockX = rockData[i, 0];
                    int rockY = rockData[i, 1];

                    if (playerX == rockX && playerY == rockY) //ดูว่าตำแหน่งทัยกับผู้เล่นไหม
                    {
                        return true; 
                    }
                }
            }
            return false; 
        }
    }
}