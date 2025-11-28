using UnityEngine;

namespace Core.Interfaces
{
    public interface ILayoutItem
    {
        public Vector3 LayoutPosition { get; set; }
        public Vector3 LayoutRotation { get; set; }
    }
}