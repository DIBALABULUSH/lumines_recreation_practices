using System;
using System.Collections;
using System.Collections.Generic;

namespace Lumines
{
    /// <summary>
    /// one dimension array but use as two dimension grid.
    /// grid order is column dense: index = x * height + y.
    /// </summary>
    public class ArrayGridColumn<T> : IEnumerable<T>
    {
        private readonly int _width;
        private readonly int _height;
        
        private readonly T[] _array;

        public int Width => _width;
        public int Height => _height;
        public int Length => _width * _height;

        public ArrayGridColumn(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("width and height must be positive");
            }
            
            _width = width;
            _height = height;
            
            _array = new T[width * height];
        }

        public T[] GetColumnItems(int columnIndex)
        {
            if ((uint)columnIndex >= (uint)_width)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            
            var result =  new T[_height];
            
            Array.Copy(_array, columnIndex * _height, result, 0, _height);
            
            return result;
        }
        
        public T[] GetRowItems(int rowIndex)
        {
            if ((uint)rowIndex >= (uint)_height)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            
            var result =  new T[_width];
            
            for (int x = 0; x < _width; x++)
            {
                result[x] = _array[x * _height + rowIndex];
            }
            
            return result;
        }

        public T[] GetRectFrameItems(int startX, int startY, int wid, int hgt)
        {
            // check valid width and height
            if (wid < 0)
                throw new ArgumentOutOfRangeException(nameof(wid));
            if (hgt < 0)
                throw new ArgumentOutOfRangeException(nameof(hgt));

            if(wid == 0 || hgt == 0)
                return Array.Empty<T>();
            
            // double-check the index range
            if ((uint)startX >= (uint)_width ||
                (uint)startY >= (uint)_height || 
                wid > _width - startX ||
                hgt > _height - startY)
            {
                throw new IndexOutOfRangeException();
            }
            
            // set the result array
            var size = wid == 1 || hgt == 1 ?
                wid * hgt :
                2 * wid + 2 * (hgt - 2);
            var result = new T[size];
            var count = 0;
            
            var endIndexX = startX + wid - 1;
            var endIndexY = startY + hgt - 1;
            
            // get top row items
            for (int x = startX; x < startX + wid; x++)
            {
                result[count] = _array[x * _height + startY];
                count++;
            }
            
            // get center items from left and right
            // if width == 0 then we don't care the right item
            var centerStartY = startY + 1;
            var centerEndY = startY + hgt - 1;
            for (int y = centerStartY; y < centerEndY; y++)
            {
                // left item
                result[count] = _array[startX * _height + y];
                count++;
                
                if (wid <= 1)
                {
                    continue;
                }

                // right item
                result[count] = _array[endIndexX * _height + y];
                count++;
            }
            
            // get bottom row items
            // if height == 0 then we don't care the bottom item
            if (hgt > 1)
            {
                for (int x = startX; x < startX + wid; x++)
                {
                    result[count] = _array[x * _height + endIndexY];
                    count++;
                }
            }

            // collect issue
            if (count != size)
            {
                throw new ArgumentException(
                    $"Trying to collect {size} items, but collected {count} items.");
            }

            return result;
        }

        public T[] GetFullRectItems(int startX, int startY, int wid, int hgt)
        {
            // check valid width and height
            if (wid < 0)
                throw new ArgumentOutOfRangeException(nameof(wid));
            if (hgt < 0)
                throw new ArgumentOutOfRangeException(nameof(hgt));

            if(wid == 0 || hgt == 0)
                return Array.Empty<T>();
            
            // double-check the index range
            if ((uint)startX >= (uint)_width ||
                (uint)startY >= (uint)_height || 
                wid > _width - startX ||
                hgt > _height - startY)
            {
                throw new IndexOutOfRangeException();
            }
            
            // set the result array
            var size = wid * hgt;
            var result = new T[size];
            
            // Preserve row order in the returned rectangle.
            for (int y = 0; y < hgt; y++)
            {
                for (int x = 0; x < wid; x++)
                {
                    result[y * wid + x] = _array[(startX + x) * _height + startY + y];
                }
            }
            return result;
        }
        
        public void AlignRowLeft(int row)
        {
            if ((uint)row >= (uint)_height)
                throw new ArgumentOutOfRangeException(nameof(row));

            CompactItems(row, 
                _height, 
                _width);
        }

        public void AlignRowRight(int row)
        {
            if ((uint)row >= (uint)_height)
                throw new ArgumentOutOfRangeException(nameof(row));

            CompactItems(
                (_width - 1) * _height + row, 
                -_height, 
                _width);
        }

        public void AlignColumnUp(int column)
        {
            if ((uint)column >= (uint)_width)
                throw new ArgumentOutOfRangeException(nameof(column));

            CompactItems(
                column * _height, 
                1, 
                _height);
        }

        public void AlignColumnDown(int column)
        {
            if ((uint)column >= (uint)_width)
                throw new ArgumentOutOfRangeException(nameof(column));

            CompactItems(
                column * _height + _height - 1,
                -1,
                _height);
        }
        
        private void CompactItems(int startIndex, int step, int length)
        {
            int write = startIndex;
            int read = startIndex;

            for (int i = 0; i < length; i++, read += step)
            {
                var item = _array[read];

                if (item is null)
                    continue;

                if (read != write)
                {
                    _array[write] = item;
                    _array[read] = default;
                }

                write += step;
            }
        }
        
        public void SetItemAt(int x, int y, T item)
        {
            var index = GetIndex(x, y);
            
            _array[index] = item;
        }
        
        private int GetIndex(int x, int y)
        {
            if ((uint)x >= (uint)Width ||
                (uint)y >= (uint)Height)
            {
                throw new IndexOutOfRangeException();
            }
            
            return x * _height + y;
        }

        public (int x, int y) GetGridCoord(int index)
        {
            if ((uint)index >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            
            var x = index / _height;
            var y = index % _height;
            
            return (x, y);
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// reset Array grid, set all items into default
        /// </summary>
        public void Clear()
        {
            for (var i = 0; i < Length; i++)
            {
                _array[i] = default;
            }
        }
    }
}