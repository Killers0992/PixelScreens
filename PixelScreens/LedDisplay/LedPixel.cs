namespace LedDisplay
{
    using AdminToys;
    using UnityEngine;

    public class LedPixel : MonoBehaviour
    {
        public Vector2Int Position { get; set; }
        public int Radius => (int)transform.localScale.x;
        public Color Color
        {
            get
            {
                return ObjectToy.NetworkMaterialColor;
            }
            set
            {
                ObjectToy.NetworkMaterialColor = value;
            }
        }

        public PrimitiveObjectToy ObjectToy;
    }
}