using System;
using System.Collections.Generic;
using System.Linq;
using MathModule.Structs;

namespace MathModule.Models
{
    [Serializable]
    public class HexMap<T>
    {
        public readonly Dictionary<Int2, T> Cells;

        public HexMap()
        {
            Cells = new Dictionary<Int2, T>();
        }

        public HexMap(int capacity)
        {
            Cells = new Dictionary<Int2, T>(capacity);
        }

        public HexMap(Dictionary<Int2, T> cells)
        {
            Cells = cells;
        }

        public HexMap(HexMap<T> hexMap)
        {
            Cells = new Dictionary<Int2, T>(hexMap.Cells);
        }

        public void Set(Int2 position, T obj)
        {
            Cells[position] = obj;
        }

        public void Set(Int2 position, T obj, int radius)
        {
            var form = HexForm.Create(radius);
            Set(position, obj, form);
        }

        /// <param name="position"></param>
        /// <param name="obj"></param>
        /// <param name="form">it's local points that will be filled of the obj.</param>
        public void Set(Int2 position, T obj, IEnumerable<Int2> form)
        {
            foreach (var localPosition in form)
            {
                Set(position + localPosition, obj);
            }
        }

        public T Get(Int2 position)
        {
            Cells.TryGetValue(position, out var obj);
            return obj;
        }

        public List<T> Get(Int2 position, int radius)
        {
            var form = HexForm.Create(radius);
            return Get(position, form);
        }

        /// <param name="form">it's local points from where will be got objects.</param>
        public List<T> Get(Int2 position, IEnumerable<Int2> form)
        {
            return form.Select(localPosition => Get(position + localPosition)).ToList();
        }

        public bool Remove(Int2 position)
        {
            return Cells.Remove(position);
        }
        
        public bool Remove(Int2 position, out T obj)
        {
            return Cells.Remove(position, out obj);
        }

        public void Replace(Int2 oldPosition, Int2 newPosition)
        {
            Remove(oldPosition, out var obj);
            Set(newPosition, obj);
        }

        public void Clear()
        {
            Cells.Clear();
        }

        public HexMap<TElement> Select<TElement>(Func<KeyValuePair<Int2, T>, TElement> elementSelector)
        {
            var cells = Cells.ToDictionary(pair => pair.Key, elementSelector);
            return new HexMap<TElement>(cells);
        }

        public HexMap<TElement> Select<TElement>(Func<KeyValuePair<Int2, T>, TElement> elementSelector, Func<KeyValuePair<Int2, T>, bool> elementRemover)
        {
            var cells = Cells.Where(elementRemover).ToDictionary(pair => pair.Key, elementSelector);
            return new HexMap<TElement>(cells);
        }
    }

    public static class HexForm
    {
        private static readonly Dictionary<string, IEnumerable<Int2>> HexFormPool = new(5);

        public static IEnumerable<Int2> Create(int radius)
        {
            var formId = "radial:" + nameof(radius) + "=" + radius;

            if (HexFormPool.TryGetValue(formId, out var form))
            {
                return form;
            }

            form = CreateInternal(radius);
            HexFormPool.Add(formId, form);

            return form;
        }

        public static IEnumerable<Int2> Create(int radius, int innerRadius)
        {
            var formId = "radial:" + nameof(radius) + "=" + radius + "," + nameof(innerRadius) + "=" + innerRadius;

            if (HexFormPool.TryGetValue(formId, out var form))
            {
                return form;
            }

            var radialForm = CreateInternal(radius);
            var innerRadialForm = CreateInternal(innerRadius);

            foreach (var localPosition in innerRadialForm)
            {
                radialForm.Remove(localPosition);
            }

            HexFormPool.Add(formId, radialForm);

            return radialForm;
        }

        private static List<Int2> CreateInternal(int radius)
        {
            var localPositions = new List<Int2>();

            for (var deltaQ = -radius; deltaQ <= radius; deltaQ++) 
            {
                var minR = Math.Max(-radius, -deltaQ - radius);
                var maxR = Math.Min(radius, -deltaQ + radius);

                for (var deltaR = minR; deltaR <= maxR; deltaR++) 
                {
                    localPositions.Add(new Int2(deltaQ, deltaR));
                }
            }

            return localPositions;
        }
    }
}