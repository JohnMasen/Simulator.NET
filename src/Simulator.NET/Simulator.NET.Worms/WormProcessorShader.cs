using ComputeSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Worms
{
    [GeneratedComputeShaderDescriptor]
    [ThreadGroupSize(DefaultThreadGroupSizes.X)]
    public readonly partial struct WormProcessorShader(ReadWriteBuffer<WormItem> source, ReadWriteBuffer<WormItem> target, int2 size, float seed, float navigationCapacity,float stability) : IComputeShader
    {
        public void Execute()
        {
            WormItem current = source[ThreadIds.X];

            if (Hlsl.All(current.HeadPosition == current.TargetPosition))//check if current position is target
            {
                float randomX = getRandom(seed*current.RandomSeed.X );
                float randomY = getRandom(seed * current.RandomSeed.Y);
                current.TargetPosition = new Int2((int)(Hlsl.Round(Hlsl.Lerp(0, size.X - 1, randomX))), (int)(Hlsl.Round(Hlsl.Lerp(0, size.Y - 1, randomY))));
            }


            //calculate available next position
            float4 neighbours = 0;
            neighbours[0] = current.HeadPosition.X > 0 ? 1 : 0;//can move left
            neighbours[1] = current.HeadPosition.X < size.X - 1 ? 1 : 0;//can move right
            neighbours[2] = current.HeadPosition.Y > 0 ? 1 : 0; //can move up
            neighbours[3] = current.HeadPosition.Y < size.Y - 1 ? 1 : 0; //can move down

            //check target
            neighbours.YX *= getNavigation(current.HeadPosition.X, current.TargetPosition.X);
            neighbours.WZ *= getNavigation(current.HeadPosition.Y, current.TargetPosition.Y);

            //get next move position
            int nextMove = getRandomPosition(neighbours, seed * ThreadIds.X);

            //move body, move each body to next position
            current.BodyCells[2] = current.BodyCells[1];
            current.BodyCells[1] = current.BodyCells[0];
            current.BodyCells[0] = current.HeadPosition;

            //move head
            if (nextMove == 0) //move left
            {
                current.HeadPosition += new Int2(-1, 0);
            }
            if (nextMove == 1) //move right
            {
                current.HeadPosition += new Int2(1, 0);
            }
            if (nextMove == 2)//move up
            {
                current.HeadPosition += new Int2(0, -1);
            }
            if (nextMove == 3)//move down
            {
                current.HeadPosition += new int2(0, 1);
            }
            target[ThreadIds.X] = current;
        }

        /// <summary>
        /// Decide if should move to target position base on current position and target position (1 dimention)
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="navigationCapacity"></param>
        /// <returns></returns>
        private float2 getNavigation(int current, int target)
        {
            
            if (current==target)
            {
                return 1-stability;
            }
            if (current<target)
            {
                return new float2(navigationCapacity, 1 - navigationCapacity);
            }
            else
            {
                return new float2(1-navigationCapacity, navigationCapacity);
            }
        }

        private int getRandomPosition(float4 weights, float seed)
        {
            float sum = weights[0] + weights[1] + weights[2] + weights[3];
            float4 v = weights / sum;
            float r = getRandom(seed);
            float a = 0;
            int result = 0;
            for (int i = 0; i < 4; i++)
            {
                a += v[i];
                if (r <= a)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        private float getRandom(float2 p)
        {
            return Hlsl.Frac(Hlsl.Cos(Hlsl.Dot(p, new float2(4.898f, 7.23f))) * 23421.631f);
            //p = Hlsl.Frac(p * 0.1031f);
            //p *= p + 33.33f;
            //p *= p + p;
            //return Hlsl.Frac(p);

        }
    }
}
