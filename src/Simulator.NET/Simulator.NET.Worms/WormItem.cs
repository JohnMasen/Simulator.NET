using ComputeSharp;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.NET.Worms
{
    public struct WormItem
    {
        private static readonly int[] directions = { 0, 1, 2, 3, 4, 6, 7, 8 };
        public int2 HeadPosition;
        public int2 BodyCell1Position;
        public int2 BodyCell2Position;
        public int2 BodyCell3Position;
        public static Memory<WormItem> CreateRandomWorms(int count, Size drawingSize)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0, nameof(count));
            WormItem[] data = new WormItem[count];
            CreateRandomWorms(data, drawingSize);

            return data.AsMemory();
        }

        public static void CreateRandomWorms(Span<WormItem> data, Size drawingSize)
        {
            for (int i = 0; i < data.Length; i++)
            {
                using var bodyCell = MemoryPool<Point>.Shared.Rent(3);
                var head = getRandomPosition(drawingSize);
                WormItem item = new WormItem();
                item.HeadPosition = new Int2(head.X, head.Y);
                var current = head;
                for (int j = 0; j < 3; j++)
                {
                    var v = getBodyCell(current, drawingSize);
                    current = Point.Add(current, new(v.X, v.Y));
                    bodyCell.Memory.Span[j] = v;
                }
                item.BodyCell1Position = new Int2(bodyCell.Memory.Span[0].X, bodyCell.Memory.Span[0].Y);
                item.BodyCell2Position = new Int2(bodyCell.Memory.Span[1].X, bodyCell.Memory.Span[1].Y);
                item.BodyCell3Position = new Int2(bodyCell.Memory.Span[2].X, bodyCell.Memory.Span[2].Y);
                data[i] = item;
            }
        }
        private static Point getRandomPosition(Size drawingSize)
        {
            int x = Random.Shared.Next(drawingSize.Width);
            int y = Random.Shared.Next(drawingSize.Height);
            return new(x, y);
        }

        private static Point getBodyCell(Point parentCell, Size drawingSize)
        {
            return Random.Shared.GetItems(getBodyCellCandidates(parentCell, drawingSize).ToArray(), 1)[0];
        }

        private static IEnumerable<Point> getBodyCellCandidates(Point parentCell, Size drawingSize)
        {
            if (parentCell.X > 0)
            {
                yield return new(-1, 0);
            }
            if (parentCell.X < drawingSize.Width - 1)
            {
                yield return new(1, 0);
            }
            if (parentCell.Y > 0)
            {
                yield return new(0, -1);
            }
            if (parentCell.Y < drawingSize.Height - 1)
            {
                yield return new(0, 1);
            }
        }
    }
}
