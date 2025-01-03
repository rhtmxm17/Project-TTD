using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/Auto Expand Grid Layout Group", 152)]
    public class DynamicGrid : LayoutGroup
    {
        public enum Corner { UpperLeft = 0, UpperRight = 1, LowerLeft = 2, LowerRight = 3 }
        public enum Axis { Horizontal = 0, Vertical = 1 }
        public enum Constraint { Flexible = 0, FixedColumnCount = 1, FixedRowCount = 2 }

        public enum ExpandSetting { x, y, both, none };
        public ExpandSetting expandSetting;

        [SerializeField]
        protected Corner m_StartCorner = Corner.UpperLeft;
        public Corner startCorner { get { return m_StartCorner; } set { SetProperty(ref m_StartCorner, value); } }

        [SerializeField]
        protected Axis m_StartAxis = Axis.Horizontal;
        public Axis startAxis { get { return m_StartAxis; } set { SetProperty(ref m_StartAxis, value); } }

        [SerializeField]
        protected Vector2 m_CellSize = new Vector2(100, 100);
        public Vector2 cellSize { get { return m_CellSize; } set { SetProperty(ref m_CellSize, value); } }

        [SerializeField]
        protected Vector2 m_Spacing = Vector2.zero;
        public Vector2 spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }

        [SerializeField]
        protected Constraint m_Constraint = Constraint.Flexible;
        public Constraint constraint { get { return m_Constraint; } set { SetProperty (ref m_Constraint, value); } }

        [SerializeField]
        protected int m_ConstraintCount = 2;
        public int constraintCount { get { return m_ConstraintCount; } set { SetProperty(ref m_ConstraintCount, Mathf.Max(1, value)); } }

        protected DynamicGrid()
        {

        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            constraintCount = constraintCount;
        }
#endif
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            int minColumns = 0;
            int preferredColumns = 0;

            if (m_Constraint == Constraint.FixedColumnCount)
            {
                minColumns = preferredColumns = m_ConstraintCount;
            }
            else if (m_Constraint == Constraint.FixedRowCount)
            {
                minColumns = preferredColumns = Mathf.CeilToInt(rectChildren.Count / (float)m_ConstraintCount - 0.001f); // 소수점 올리기
            }
            else
            {
                minColumns = 1;
                preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(rectChildren.Count)); // 소수점 올리기
            }

            SetLayoutInputForAxis(
                padding.horizontal + (cellSize.x + spacing.x) * minColumns - spacing.x,
                padding.horizontal + (cellSize.x + spacing.x) * preferredColumns - spacing.x,
                -1, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            int minRows = 0;
            if ( m_Constraint == Constraint.FixedColumnCount)
            {
                minRows = Mathf.CeilToInt(rectChildren.Count / (float)m_ConstraintCount - 0.001f); // 소수점 올리기
            }
            else if (m_Constraint == Constraint.FixedRowCount)
            {
                minRows = m_ConstraintCount;
            }
            else
            {
                float width = rectTransform.rect.size.x;
                int cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));
                minRows = Mathf.CeilToInt(rectChildren.Count / (float)(cellCountX));
            }
            float minSpace = padding.vertical + (cellSize.y + spacing.y) * minRows - spacing.y;
            SetLayoutInputForAxis(minSpace, minSpace, -1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            SetCellsAlongAxis(0);
        }
        public override void SetLayoutVertical()
        {
            SetCellsAlongAxis(1);
        }
        private void SetCellsAlongAxis(int axis)
        {
            /* 
            // 보통 LayoutController 가로값이 불렸을때만 가로값이 set되는데
            // 세로값도 똑같이 세로값 콜됐을때만 set
            // 근데 이 경우에선, 가로 새로 둘다  세로축 콜되면 가로 세로 위치 설정함.
            // 가로 위치만 변경하니까 (사이즈x)  자식레이아웃 지장 안가니까
            // 모든 가로 레이아웃이 세로레이아웃보다 먼저 계산되야하는 룰 어기면 안됨 
            // Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
            // and only vertical values when invoked for the vertical axis.
            // However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
            // Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
            // and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.
            */
            if (axis == 0)
            {
                // Only set the sizes when invoked for horizontal axis, not the positions
                for (int i = 0; i < rectChildren.Count; i++)
                {
                    RectTransform rect = rectChildren[i];

                    m_Tracker.Add(this, rect,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.SizeDelta
                        );
                    rect.anchorMin = Vector2.up;
                    rect.anchorMax = Vector2.up;
                    rect.sizeDelta = cellSize;
                }
                return;
            }
            float width = rectTransform.rect.size.x;
            float height = rectTransform.rect.size.y;

            int cellCountX = 1;
            int cellCountY = 1;
            if (m_Constraint == Constraint.FixedColumnCount)
            {
                cellCountX = m_ConstraintCount;
                cellCountY = Mathf.CeilToInt(rectChildren.Count / (float)cellCountX - 0.001f);
            }
            else if ( m_Constraint == Constraint.FixedRowCount)
            {
                cellCountX = m_ConstraintCount;
                cellCountX = Mathf.CeilToInt(rectChildren.Count / (float)cellCountY - 0.001f);
            }
            else
            {
                if (cellSize.x + spacing.x <= 0)
                    cellCountX = int.MaxValue;
                else
                    cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));
            }
            int cornerX = (int)startCorner % 2;
            int cornerY = (int)startCorner / 2;

            int cellsPerMainAxis, actualCellCountX, actualCellCountY;
            if (startAxis == Axis.Horizontal)
            {
                cellsPerMainAxis = cellCountX;
                actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildren.Count); // min ~ max 값리턴 넘어가면 max로
                actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));
            }
            else
            {
                cellsPerMainAxis = cellCountY;
                actualCellCountY = Mathf.Clamp(cellCountY, 1, rectChildren.Count);
                actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));
            }

            Vector2 requiredSpace = new Vector2(
                actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x,
                actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
                );
            Vector2 startOffset = new Vector2(
                GetStartOffset(0, requiredSpace.x),
                GetStartOffset(0, requiredSpace.y)
                );
            for (int i = 0; i < rectChildren.Count; i++)
            {
                int positionX;
                int positionY;
                if (startAxis == Axis.Horizontal)
                {
                    positionX = i % cellsPerMainAxis;
                    positionY = i / cellsPerMainAxis;
                }
                else
                {
                    positionX = i % cellsPerMainAxis;
                    positionY = i / cellsPerMainAxis;
                }

                if (cornerX == 1)
                    positionX = actualCellCountX - 1 - positionX;
                if (cornerY == 1)
                    positionY = actualCellCountY - 1- positionY;

                float realSizeY;
                if (expandSetting == ExpandSetting.both)
                {
                    if (rectChildren.Count != 1)
                    {
                        realSizeY = ((height - (spacing[0] * (actualCellCountY - 1))) / actualCellCountY);
                    }
                    else
                    {
                        realSizeY = ((height / constraintCount - (spacing[0] * (actualCellCountY - 1))) / actualCellCountY);

                    }

                    float realsize = ((width - (spacing[0] * (actualCellCountX - 1))) / actualCellCountX);
                    SetChildAlongAxis(rectChildren[i], 0, startOffset.x + (realsize + spacing[0]) * positionX, realsize);
                    SetChildAlongAxis(rectChildren[i], 1, startOffset.y + (realSizeY + spacing[1]) * positionY, realSizeY);


                }
                if (expandSetting == ExpandSetting.x)
                {


                    float realSize = ((width - (spacing[0] * (actualCellCountX - 1))) / actualCellCountX);
                    SetChildAlongAxis(rectChildren[i], 0, startOffset.x + (realSize + spacing[0]) * positionX, realSize);

                    SetChildAlongAxis(rectChildren[i], 1, startOffset.y + (cellSize[1] + spacing[1]) * positionY, cellSize[1]);

                }
                if (expandSetting == ExpandSetting.y)
                {
                    if (rectChildren.Count != 1)
                    {
                        realSizeY = ((height - (spacing[1] * (actualCellCountY - 1))) / actualCellCountY);
                    }
                    else
                    {
                        realSizeY = ((height / constraintCount - (spacing[1] * (actualCellCountY - 1))) / actualCellCountY);
                    }


                    SetChildAlongAxis(rectChildren[i], 1, startOffset.y + (realSizeY + spacing[1]) * positionY, realSizeY);

                    SetChildAlongAxis(rectChildren[i], 0, startOffset.x + (cellSize[0] + spacing[0]) * positionX, cellSize[0]);


                }
                if (expandSetting == ExpandSetting.none)
                {
                    SetChildAlongAxis(rectChildren[i], 1, startOffset.y + (cellSize[1] + spacing[1]) * positionY, cellSize[1]);
                    SetChildAlongAxis(rectChildren[i], 0, startOffset.x + (cellSize[0] + spacing[0]) * positionX, cellSize[0]);
                }
            }


        }
    }


}

