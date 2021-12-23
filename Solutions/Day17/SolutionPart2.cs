using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day17
{
    internal class SolutionPart2 : IntSolution
    {

        public int Solve()
        {
            string[] data = File.ReadAllLines("./Content/Day17/1.txt").First().Split(" ");
            string[] targetAreaX = data[2].Substring(2).Split("..");
            string[] targetAreaY = data[3].Substring(2).Split("..");

            int minx = int.Parse(targetAreaX[0]);
            int maxx = int.Parse(targetAreaX[1]);
            int miny = int.Parse(targetAreaY[0]);
            int maxy = int.Parse(targetAreaY[1]);

            // We can get minimum x speed by sum of an arithmetic progression
            // each step the x-speed will be smaller by 1
            // minx = x + (x - 1) + (x - 2) + ... + 0
            // 2 * minx = x + x * x which has only 1 positive solution...or we can brute force it :D

            int minSpeedX = (int)Math.Ceiling((-1 + Math.Sqrt(1 + 8 * minx)) * 0.5);
            int maxSpeedX = maxx; // Only one step and we overshoot
            int minSpeedY = miny; // we can shoot down as well
            // When we start going down, then at 0 we will have negative speed 
            // on initial y-speed. It shouldn't be higher then maxy then (we overshoot).
            int maxSpeedY = Math.Abs(miny);

            int maxHeight = 0;
            int hitCount = 0;
            for (int x = minSpeedX; x <= maxSpeedX; x++)
            {
                for (int y = minSpeedY; y <= maxSpeedY; y++)
                {
                    int posx = 0;
                    int posy = 0;
                    int speedx = x;
                    int speedy = y;

                    int maxPosY = 0;
                    while (posx <= maxx && posy >= miny)
                    {
                        posx += speedx;
                        posy += speedy;
                        speedx = Math.Max(0, speedx - 1);
                        --speedy;

                        maxPosY = Math.Max(posy, maxPosY);

                        if (posx >= minx && posx <= maxx && posy >= miny && posy <= maxy)
                        {
                            // we hit the target
                            maxHeight = Math.Max(maxPosY, maxHeight);
                            ++hitCount;
                            break;
                        }
                    }
                }
            }

            return hitCount;
        }

    }

}
