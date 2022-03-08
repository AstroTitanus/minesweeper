using System;
using Xunit;
using miny;
using System.Linq;

namespace tests
{
    public class UnitTestMiny
    {
        [Fact]
        public void TestGenerateMines()
        {
            Miny minyHra = new Miny(gamefieldSize: 5, mineCount: 4);
            minyHra.GenerateMines();

            Point[] mines = minyHra.MinesPoints;
            Assert.Equal(4, mines.Length);
            // Overuje ci je kazdy point distinct
            Assert.Equal(mines.Length, mines.Distinct().Count());
        }

        [Fact]
        public void TestInsertMines()
        {
            int mineCount = 4;
            Miny minyHra = new Miny(gamefieldSize: 5, mineCount: mineCount);
            minyHra.MinesPoints = new Point[4];

            minyHra.MinesPoints[0] = new Point(1, 1);
            minyHra.MinesPoints[1] = new Point(1, 2);
            minyHra.MinesPoints[2] = new Point(1, 3);
            minyHra.MinesPoints[3] = new Point(1, 4);

            minyHra.InstertMines();

            int mineCounter = 0;
            foreach (int val in minyHra.Gamefield)
            {
                if (val == 9) 
                {
                    mineCounter++;
                }
            }
            
            Assert.Equal(mineCounter, mineCount);
        }

        [Fact]
        public void TestGetBiggerArrayForProxCalc()
        {
            int mineCount = 3;
            Miny minyHra = new Miny(gamefieldSize: 4, mineCount: mineCount);
            minyHra.Gamefield[0,0] = 9;
            minyHra.Gamefield[0,1] = 9;
            minyHra.Gamefield[0,2] = 9;

            int[,] bigArr = minyHra.GetBiggerArrayForProxCalc();
            foreach (int item in bigArr) {
                Console.WriteLine(item);
            }

            int[,] supposedBigArr = {{-1, -1, -1, -1, -1, -1},
                                     {-1, 9, 9, 9, 0, -1},
                                     {-1, 0, 0, 0, 0, -1},
                                     {-1, 0, 0, 0, 0, -1},
                                     {-1, 0, 0, 0, 0, -1},
                                     {-1, -1, -1, -1, -1, -1}};
            
            Assert.Equal(bigArr, supposedBigArr);
        }

        [Fact]
        public void TestCalcProximities()
        {
            int mineCount = 3;
            Miny minyHra = new Miny(gamefieldSize: 4, mineCount: mineCount);
            minyHra.Gamefield[0,0] = 9;
            minyHra.Gamefield[0,1] = 9;
            minyHra.Gamefield[0,2] = 9;

            minyHra.MinesPoints = new Point[3];
            minyHra.MinesPoints[0] = new Point(0, 0);
            minyHra.MinesPoints[1] = new Point(0, 1);
            minyHra.MinesPoints[2] = new Point(0, 2);

            minyHra.CalcProximities();
    
            int[,] supposedArr = {{9, 9, 9, 1},
                                  {2, 3, 2, 1},
                                  {0, 0, 0, 0},
                                  {0, 0, 0, 0}};

            Assert.Equal(minyHra.Gamefield, supposedArr);
        }
    }
}
