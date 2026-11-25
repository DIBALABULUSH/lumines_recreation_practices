using System;
using System.Collections;
using System.Collections.Generic;

namespace Lumines
{
    /// <summary>
    /// one dimension array but use as two dimension grid
    /// </summary>
    public class ArrayGrid<T> : IEnumerable<T>
    {
        private readonly int _width;
        private readonly int _height;
        
        private readonly T[] _array;

        public int Width => _width;
        public int Height => _height;
        public int Length => _width * _height;

        public ArrayGrid(int width, int height)
        {
            _width = width;
            _height = height;
            
            _array = new T[width * height];
        }

        public T[] GetTargetColumnItems(int columnIndex)
        {
            if ((uint)columnIndex >= (uint)_width)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            
            var result =  new T[_height];
            
            for (int y = 0; y < _height; y++)
            { 
                result[y] = _array[y * _width + columnIndex];
            }
            
            return result;
        }
        
        public T[] GetTargetRowItems(int rowIndex)
        {
            if ((uint)rowIndex >= (uint)_height)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            
            var result =  new T[_width];
            
            Array.Copy(_array, rowIndex * _width, result, 0, _width);
            
            return result;
        }

        public T[] GetSquareFrameItems(int startX, int startY, int wid, int hgt)
        {
            // check valid width and height
            if (wid < 0)
                throw new ArgumentOutOfRangeException(nameof(wid));
            if (hgt < 0)
                throw new ArgumentOutOfRangeException(nameof(hgt));

            // double-check the index range
            if ((uint)startX >= (uint)_width ||
                (uint)startY >= (uint)_height || 
                wid > _width - startX ||
                hgt > _height - startY)
            {
                throw new IndexOutOfRangeException();
            }
            
            // if we get the 1x1 size square frame items
            if (wid == 1 && hgt == 1)
            {
                return new []{_array[GetIndex(startX, startY)]};
            }

            // wid == 1 directly return target column 
            if (wid == 1)
            {
                // todo:
            }
            
            // wid == 1 directly return target row 
            if (hgt == 1)
            {
                // todo : 
            }
            
            // set the result array
            var size = wid == 1 || hgt == 1 ?
                wid * hgt :
                2 * wid + 2 * (hgt - 2);
            var result = new T[size];
            var count = 0;
            
            var endX = startX + wid - 1;
            var endY = startY + hgt - 1;
            
            // get top row items
            for (int x = startX; x < startX + wid; x++)
            {
                result[count] = _array[startY * _width + x];
                count++;
            }
            
            // get center items (need to check when width and height is 1, 2)
            if (hgt > 2)
            {
                // if hgt > 2 then center items will collect from left and right
                var centerStartY = startY + 1;
                var centerEndY = startY + hgt - 1;
                for (int y = centerStartY; y < centerEndY; y++)
                {
                    var leftItem = _array[y * _width + startX];
                    result[count] = leftItem;
                    count++;
                    if (wid > 1)
                    {
                        var rightItem = _array[y * _width + endX];
                        result[count] = rightItem;
                        count++;
                    }
                }
            }
            
            // get bottom row items
            if (hgt > 1)
            {
                for (int x = startX; x < startX + wid; x++)
                {
                    result[count] = _array[endY * _width + x];
                    count++;
                }
            }

            // calculate fail
            if (count != size)
            {
                throw new ArgumentException();
            }

            return result;
        }

        private int GetIndex(int x, int y)
        {
            if ((uint)x >= (uint)Width ||
                (uint)y >= (uint)Height)
            {
                throw new IndexOutOfRangeException();
            }
            
            return y * _width + x;
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}