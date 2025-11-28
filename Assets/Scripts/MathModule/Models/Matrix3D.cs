using System;
using System.Collections.Generic;
using System.Linq;
using MathModule.Structs;

namespace MathModule.Models
{
    [Serializable]
    public class Matrix3D<T>
    {
        public readonly Dictionary<Int3, T> Cells;

        public Matrix3D()
        {
            Cells = new Dictionary<Int3, T>();
        }

        public Matrix3D(int capacity)
        {
            Cells = new Dictionary<Int3, T>(capacity);
        }

        public void Set(T obj, Int3 position)
        {
            Cells[position] = obj;
        }

        /// <param name="form">it's local points that will be filled of the obj.</param>
        public void Set(T obj, Int3 position, IEnumerable<Int3> form)
        {
            foreach (var localPosition in form)
            {
                Set(obj, position + localPosition);
            }
        }

        public T Get(Int3 position)
        {
            Cells.TryGetValue(position, out var obj);
            return obj;
        }

        /// <param name="form">it's local points from where will be got objects.</param>
        public List<T> Get(Int3 position, IEnumerable<Int3> form)
        {
            return form.Select(localPosition => Get(position + localPosition)).ToList();
        }
    }
}