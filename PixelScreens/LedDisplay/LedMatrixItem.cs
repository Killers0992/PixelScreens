namespace LedDisplay
{
    using LedDisplay.Enums;
    using UnityEngine;

    public class LedMatrixItem
    {
        public int Id { get; }
        bool[,] m_bMasterLedOn;
        public Vector2Int CurrentLocation;
        public ItemDirection Direction { get; set; }
        public ItemSpeed Speed { get; set; }
        public Color Color { get; set; }

        public LedMatrixItem()
        {
            Id = -1;
            CurrentLocation = new Vector2Int(0, 0);
        }

        public LedMatrixItem(int id,
                             bool[,] ledOn,
                             Vector2Int location,
                             Color color,
                             ItemDirection direction,
                             ItemSpeed speed)
        {
            Id = id;
            m_bMasterLedOn = ledOn;
            Color = color;
            Direction = direction;
            Speed = speed;
            CurrentLocation = location;
        }

        public int GetLineOffset()
        {
            return CurrentLocation.y;
        }

        public int GetRowOffset()
        {
            return CurrentLocation.x;
        }

        public bool[,] GetMasterLedOn()
        {
            return m_bMasterLedOn;
        }

        public void SetLedOnArray(bool[,] ledOn)
        {
            m_bMasterLedOn = ledOn;
        }

        public void MoveItem(uint tick, int line, int row)
        {
            int iOffset = 0;

            switch (Speed)
            {
                case ItemSpeed.Slow:
                    {
                        if ((tick % 2) == 0)
                        {
                            iOffset = 1;
                        }
                        break;
                    }
                case ItemSpeed.Idle:
                    {
                        iOffset = 1;
                        break;
                    }
                case ItemSpeed.Fast:
                    {
                        iOffset = 2;
                        break;
                    }

                default:
                    break;
            }

            if (iOffset == 0)
                return;

            switch (Direction)
            {
                case ItemDirection.Up:
                    CurrentLocation.y -= iOffset;
                    break;
                case ItemDirection.Down:
                    CurrentLocation.y += iOffset;
                    break;
                case ItemDirection.Left:
                    CurrentLocation.x -= iOffset;
                    break;
                case ItemDirection.Right:
                    CurrentLocation.x += iOffset;
                    break;
                default:
                    break;
            }


            if (CurrentLocation.x >= row)
            {
                CurrentLocation.x = -m_bMasterLedOn.GetLength(1);
            }
            else if (CurrentLocation.x < -m_bMasterLedOn.GetLength(1))
            {
                CurrentLocation.x = row - 1;
            }

            if (CurrentLocation.y >= line)
            {
                CurrentLocation.y = -m_bMasterLedOn.GetLength(0);
            }
            else if (CurrentLocation.y < -m_bMasterLedOn.GetLength(0))
            {
                CurrentLocation.y = line - 1;
            }
        }
    }
}