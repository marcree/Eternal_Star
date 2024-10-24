using UnityEditor;
using UnityEngine.UI;

namespace Cubequad
{
    /// <summary>
    /// Editor for SwipyMenuScrollRectю
    /// </summary>
    [CustomEditor(typeof(SwipyMenuScrollRect))]
    [CanEditMultipleObjects]
    public class SwipyMenuScrollRectEditor : Editor
    {
        private enum AvailbleMovementTypes { Elastic, Clamped }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This is just a normal ScrollRect, but restricted to modify only properties below.", MessageType.Info);
            var smsr = target as ScrollRect;
            DrawMovementType();
            smsr.elasticity = EditorGUILayout.FloatField("    Elasticity", smsr.elasticity);
        }

        private void DrawMovementType()
        {
            var smsr = target as ScrollRect;
            AvailbleMovementTypes availbleMovementType = AvailbleMovementTypes.Elastic;
            if (smsr.movementType == ScrollRect.MovementType.Clamped)
                availbleMovementType = AvailbleMovementTypes.Clamped;

            var selectedMovementType = (AvailbleMovementTypes)EditorGUILayout.EnumPopup("Movement Type", availbleMovementType);
            if (selectedMovementType == AvailbleMovementTypes.Elastic && smsr.movementType != ScrollRect.MovementType.Elastic)
                smsr.movementType = ScrollRect.MovementType.Elastic;
            else if (selectedMovementType == AvailbleMovementTypes.Clamped && smsr.movementType != ScrollRect.MovementType.Clamped)
                smsr.movementType = ScrollRect.MovementType.Clamped;
        }
    }
}