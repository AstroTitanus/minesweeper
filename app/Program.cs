using System;
using System.Collections.Generic;

namespace miny
{

    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Miny
    {
        int GamefieldSize { get; set; }
        int MineCount { get; set; }
        int RevealedFields { get; set; }
        public Point[] MinesPoints { get; set; }
        public int[,] Gamefield { get; set; }
        string[,] GameFieldStr { get; set; }

        public Miny(int gamefieldSize = 5, int mineCount = 5) 
        {
            GamefieldSize = gamefieldSize;
            MineCount = mineCount;
            RevealedFields = 0;
            Gamefield = new int[GamefieldSize, GamefieldSize];
            GameFieldStr = new string[GamefieldSize, GamefieldSize];

            // GenerateMines();
            // InstertMines();
            // CalcProximities();
            // FillGameFieldStr();
        }

        public void PrintField(string field = "out", bool gridNums = false)
        {
            // Grid numbers
            if ( gridNums == true ) {

                // Left offset
                Console.Write("    ");

                for (int i = 1; i < GamefieldSize+1; i++)
                {
                    Console.Write($"{i}");
                    if ( i != GamefieldSize ) {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();

                // Left offset
                Console.Write("    ");
                
                for (int i = 0; i < (GamefieldSize*2)-1; i++)
                {
                    Console.Write($"-");
                }
                Console.WriteLine();
            }

            for (int i = 0; i < GamefieldSize; i++)
            {
                if (gridNums == true) {
                    Console.Write($"{i+1} | ");
                }
                for (int j = 0; j < GamefieldSize; j++)
                {
                    if ( field == "out" ) { Console.Write($"{GameFieldStr[i, j]}"); }
                    else if ( field == "in" ) { Console.Write($"{Gamefield[i, j]}"); }
                    
                    if (j != GamefieldSize-1) {
                        Console.Write(" ");
                    }

                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        public void FillGameFieldStr()
        {
            for (int i = 0; i < GamefieldSize; i++)
            {
                for (int j = 0; j < GamefieldSize; j++)
                {
                    GameFieldStr[i, j] = "X";
                }
            }
        }

        public void GenerateMines()
        {
            Point[] minesPoints = new Point[MineCount];

            for (int i = 0; i < MineCount; i++)
            {
                //Generovanie suradnic
                Random rand = new Random();
                int num1 = rand.Next(0,GamefieldSize-1);
                int num2 = rand.Next(0,GamefieldSize-1);

                // Vytvorenie Pointu zo suradnic
                Point px = new Point(num1, num2);
                
                // Kontrola
                int equlasCount = 0;
                foreach ( Point j in minesPoints)
                {
                    if (j.X == px.X && j.Y == px.Y) {
                        equlasCount++;
                    }
                }

                if ( equlasCount == 0 ) {
                    minesPoints[i] = px;
                    // Console.WriteLine($"X: {px.X}; Y: {px.Y}");
                }
                else {
                    i--;
                }
            }
            
            MinesPoints = minesPoints;
        }

        public void InstertMines()
        {
            foreach (Point px in MinesPoints)
            {
                Gamefield[px.X, px.Y] = 9;
            }
        }

        public int[,] GetBiggerArrayForProxCalc()
        {
            int bigArrSize = GamefieldSize+2;
            int[,] bigArr = new int[bigArrSize, bigArrSize];
            
            for (int i = 0; i < bigArrSize; i++)
            {
                for (int j = 0; j < bigArrSize; j++)
                {
                    bigArr[i, j] = -1;
                }
            }

            for (int i = 1; i < bigArrSize-1; i++)
            {
                for (int j = 1; j < bigArrSize-1; j++)
                {
                    bigArr[i, j] = Gamefield[i-1, j-1];
                }
            }

            return bigArr;
        }

        public void WriteCalcDataToGamefield(int[,] bigArr)
        {
            int bigArrSize = GamefieldSize+2;

            for (int i = 1; i < bigArrSize-1; i++)
            {
                for (int j = 1; j < bigArrSize-1; j++)
                {
                    Gamefield[i-1, j-1] = bigArr[i, j];
                }
            }
        }

        public void CalcProximities()
        {
            // Get bigger array
            int bigArrSize = GamefieldSize+2;
            int[,] bigArr = GetBiggerArrayForProxCalc();

            // all neighbors coords
            int[] xarr = {-1,-1,-1,0,0,+1,+1,+1};
            int[] yarr = {-1,0,+1,-1,+1,-1,0,+1};

            // Calc prox
            foreach (Point p in MinesPoints)
            {
                int x = p.X+1;
                int y = p.Y+1;
                
                for (int i = 0 ; i < 8 ; i++)
                {
                    if (bigArr[x-xarr[i], y-yarr[i]] != 9) {
                        bigArr[x-xarr[i], y-yarr[i]]++;
                    }
                }
            }

            WriteCalcDataToGamefield(bigArr);
        }

        public void RevealNeighBorZeros(int px, int py)
        {
            int[,] bigArr = GetBiggerArrayForProxCalc();

            List<Point> zeroPoints = new List<Point>();
            List<Point> zeroPointsChecked = new List<Point>();
            // Point first = new Point(x, y);
            zeroPoints.Add(new Point(px, py));

            // all neighbors coords
            int[] xarr = {-1,-1,-1,0,0,+1,+1,+1};
            int[] yarr = {-1,0,+1,-1,+1,-1,0,+1};

            // Calc prox
            while (zeroPoints.Count != 0)
            {
                Point p = zeroPoints[0];
                int x = p.X+1;
                int y = p.Y+1;

                for (int i = 0 ; i < 8 ; i++)
                {
                    if (bigArr[x-xarr[i], y-yarr[i]] != -1 && GameFieldStr[x-xarr[i]-1, y-yarr[i]-1] == "X") {
                        GameFieldStr[x-xarr[i]-1, y-yarr[i]-1] = bigArr[x-xarr[i], y-yarr[i]].ToString();
                        RevealedFields++;
                    }

                    if (bigArr[x-xarr[i], y-yarr[i]] == 0) {
                        Point newPoint = new Point(x-xarr[i]-1, y-yarr[i]-1);
                        if ( zeroPointsChecked.Contains(newPoint) || zeroPoints.Contains(newPoint) ) {
                            continue;
                        }

                        zeroPoints.Add(new Point(x-xarr[i]-1, y-yarr[i]-1));
                    }
                }

                zeroPointsChecked.Add(zeroPoints[0]);
                zeroPoints.RemoveAt(0);
            }
        }

        public void RevealMines()
        {
            foreach (Point p in MinesPoints)
            {
                // int x = p.X;
                // int y = p.Y;
                
                GameFieldStr[p.X, p.Y] = "M";
            }
        }

        public void Play()
        {
            // Inits
            GenerateMines();
            InstertMines();
            CalcProximities();
            FillGameFieldStr();

            // Game loop
            bool loop = true;
            string errorMessage = "";
            while (loop)
            {
                Console.Clear();
                PrintField(gridNums: true);

                if ( errorMessage != "" ) {
                    Console.WriteLine(errorMessage);
                    errorMessage = "";
                }

                Console.WriteLine("Zadaj suradnice v tvare '1, 2':");

                //input
                string input = Console.ReadLine().Trim();

                // user can exit anytime
                if ( input == "end" || input == "stop" || input ==  "quit" || input == "exit") {
                    Console.Clear();
                    loop = false;
                    continue;
                }

                // parsing user input
                int commaIndex = input.IndexOf(",");

                if ( commaIndex == -1 ) {
                    errorMessage = "Jednotlive koordinacie musia byt rozdelene jednym znakom ','.";
                    continue;
                }

                string[] nums = input.Split(",");

                if ( nums.Length > 2 ) {
                    errorMessage = "Jednotlive koordinacie musia byt rozdelene jednym znakom ','.";
                    continue;
                }

                int x;
                int y;
                bool num1 = int.TryParse(nums[0], out x);
                bool num2 = int.TryParse(nums[1], out y);
                x--; y--;

                // Console.WriteLine(x);
                // Console.WriteLine(y);

                if ( !num1 || !num2) {
                    errorMessage = "Jednotlive koordinacie musia byt cele cisla.";
                    continue;
                }
    
                // check if coords are in bounds
                if ( x < 0 || x >= GamefieldSize || y < 0 || y >= GamefieldSize ) {
                    errorMessage = "Zadaj suradnice vramci hracieho pola.";
                    continue;
                }

                int val = Gamefield[x, y];

                if ( val == 9 ) {
                    RevealMines();
                    Console.Clear();
                    PrintField(gridNums: true);
                    Console.WriteLine("BOOM! Vybuchol si.");
                    loop = false;
                }

                if ( val == 0 ) {
                    RevealNeighBorZeros(x, y);
                    RevealedFields--;
                }

                int HiddenFields = (GamefieldSize*GamefieldSize) - RevealedFields;
                errorMessage = RevealedFields.ToString();
                if ( HiddenFields == MineCount + 1 ) {
                    GameFieldStr[x, y] = val.ToString();
                    RevealMines();
                    Console.Clear();
                    PrintField(gridNums: true);
                    Console.WriteLine("VYHRAL SI!");
                    loop = false;
                }

                GameFieldStr[x, y] = val.ToString();
                RevealedFields++;
            }

            Console.ReadKey();
            Console.Clear();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // int mineCount = 3;
            // Miny minyHra = new Miny(gamefieldSize: 4, mineCount: mineCount);
            // minyHra.MinesPoints = new Point[mineCount];

            // minyHra.MinesPoints[0] = new Point(0, 0);
            // minyHra.MinesPoints[1] = new Point(0, 1);
            // minyHra.MinesPoints[2] = new Point(0, 2);
            //             minyHra.InstertMines();


            // int[,] bigArr = minyHra.GetBiggerArrayForProxCalc();
            // foreach (int item in bigArr) {
            //     Console.WriteLine(item);
            // }

            Miny minyHra = new Miny(gamefieldSize: 5, mineCount: 3);
            // minyHra.MinesPoints;
            
            // DEBHUG vypíše odhalené pole mín
            // int[,] arr = minyHra.GetBiggerArrayForProxCalc();
            // for (int i = 0; i < arr.GetLength(0); i++)
            // {
            //     for (int j = 0; j < arr.GetLength(1); j++)
            //     {
            //         Console.Write($"{arr[i, j]}");
            //         if (j != arr.Length-1) {
            //             Console.Write(" ");
            //         }
            //     }
            //     Console.WriteLine();
            // }

            minyHra.Play();
        }
    }
}
