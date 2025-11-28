using System;
using System.Collections.Generic;
using System.Linq;
using MathModule.Structs;

namespace MathModule.Models
{
    [Serializable]
    public class Matrix2D<T>
    {
        public readonly Dictionary<Int2, T> Cells;

        public Matrix2D()
        {
            Cells = new Dictionary<Int2, T>();
        }

        public Matrix2D(int capacity)
        {
            Cells = new Dictionary<Int2, T>(capacity);
        }

        public void Set(T obj, Int2 position)
        {
            Cells[position] = obj;
        }

        /// <param name="form">it's local points that will be filled of the obj.</param>
        public void Set(T obj, Int2 position, IEnumerable<Int2> form)
        {
            foreach (var localPosition in form)
            {
                Set(obj, position + localPosition);
            }
        }

        public T Get(Int2 position)
        {
            Cells.TryGetValue(position, out var obj);
            return obj;
        }

        /// <param name="form">it's local points from where will be got objects.</param>
        public List<T> Get(Int2 position, IEnumerable<Int2> form)
        {
            return form.Select(localPosition => Get(position + localPosition)).ToList();
        }
    }
}