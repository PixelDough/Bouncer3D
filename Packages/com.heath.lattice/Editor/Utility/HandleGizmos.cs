using UnityEditor;
using UnityEngine;

namespace Lattice.Editor
{
	/// <summary>
	/// Utility class for drawing gizmos to move, rotate and scale handle selections
	/// </summary>
	public class HandleGizmos
	{
		private readonly Lattice _lattice;
		private readonly SelectedHandles _handles;

		private Quaternion _previousToolRotation = Quaternion.identity;
		private Vector3 _previousToolScale = Vector3.one;

		public HandleGizmos(Lattice lattice, SelectedHandles handles)
		{
			_lattice = lattice;
			_handles = handles;
			Reset();
		}

		public void Reset()
		{
			_previousToolScale = Vector3.one;
			_previousToolRotation = (Tools.pivotRotation == PivotRotation.Global)
				? Quaternion.identity
				: _lattice.transform.rotation;
		}

		public void Draw()
		{
			if (Event.current.type == EventType.MouseUp) Reset();

			Vector3 worldPivot = _handles.GetPivot();
			Vector3 localPivot = _lattice.transform.InverseTransformPoint(worldPivot);

			// Draw currently selected tool
			if (Tools.current == Tool.Move) DrawPositionGizmo(worldPivot);
			else if (Tools.current == Tool.Rotate) DrawRotationGizmo(worldPivot);
			else if (Tools.current == Tool.Scale) DrawScaleGizmo(localPivot, worldPivot);
		}

		private void DrawPositionGizmo(Vector3 worldPivot)
		{
			Quaternion rotation = (Tools.pivotRotation == PivotRotation.Global)
				? Quaternion.identity
				: _lattice.transform.rotation;

			EditorGUI.BeginChangeCheck();

			Vector3 finalPosition = Handles.PositionHandle(worldPivot, rotation);

			if (EditorGUI.EndChangeCheck())
			{
				Vector3 offset = _lattice.transform.InverseTransformVector(finalPosition - worldPivot);
				foreach (Vector3Int handle in _handles.Handles)
				{
					Undo.RecordObject(_lattice.GetHandle(handle), "Moved Lattice Handles");
					Vector3 handlePosition = _lattice.GetHandlePosition(handle);
					_lattice.SetHandlePosition(handle, handlePosition + offset);
				}

				EditorApplication.QueuePlayerLoopUpdate();
			}
		}

		private void DrawRotationGizmo(Vector3 worldPivot)
		{
			EditorGUI.BeginChangeCheck();

			Quaternion finalRotation = Handles.RotationHandle(_previousToolRotation, worldPivot);

			if (EditorGUI.EndChangeCheck())
			{
				Quaternion change = Quaternion.Inverse(_previousToolRotation) * finalRotation;
				_previousToolRotation = finalRotation;

				foreach (Vector3Int handle in _handles.Handles)
				{
					Undo.RecordObject(_lattice.GetHandle(handle), "Rotated Lattice Handles");
					Vector3 handlePosition = _lattice.GetHandleWorldPosition(handle);

					Vector3 relativePosition = handlePosition - worldPivot;
					Vector3 transformedRelativePosition;

					if (Tools.pivotRotation == PivotRotation.Global)
					{
						transformedRelativePosition = change * relativePosition;
					}
					else
					{
						transformedRelativePosition = _lattice.transform.rotation * change * 
							Quaternion.Inverse(_lattice.transform.rotation) * relativePosition;
					}

					Vector3 transformedPosition = transformedRelativePosition + worldPivot;

					_lattice.SetHandleWorldPosition(handle, transformedPosition);
				}

				EditorApplication.QueuePlayerLoopUpdate();
			}
		}

		private void DrawScaleGizmo(Vector3 localPivot, Vector3 worldPivot)
		{
			Quaternion rotation = (Tools.pivotRotation == PivotRotation.Global)
				? Quaternion.identity
				: _lattice.transform.rotation;

			EditorGUI.BeginChangeCheck();

			Vector3 finalScale = Handles.ScaleHandle(_previousToolScale, worldPivot, rotation);

			if (EditorGUI.EndChangeCheck())
			{
				Vector3 change = new(
					finalScale.x / _previousToolScale.x,
					finalScale.y / _previousToolScale.y,
					finalScale.z / _previousToolScale.z
				);
				_previousToolScale = finalScale;

				foreach (Vector3Int handle in _handles.Handles)
				{
					Undo.RecordObject(_lattice.GetHandle(handle), "Scaled Lattice Handles");
					Vector3 handlePosition = _lattice.GetHandlePosition(handle);

					Vector3 transformedHandlePosition;
					if (Tools.pivotRotation == PivotRotation.Global)
					{
						Vector3 worldPosition = _lattice.transform.TransformPoint(handlePosition);
						Vector3 relativePosition = worldPosition - worldPivot;
						relativePosition.Scale(change);
						Vector3 transformedPosition = relativePosition + worldPivot;
						transformedHandlePosition = _lattice.transform.InverseTransformPoint(transformedPosition);
					}
					else
					{
						Vector3 relativePosition = handlePosition - localPivot;
						relativePosition.Scale(change);
						transformedHandlePosition = relativePosition + localPivot;
					}
					_lattice.SetHandlePosition(handle, transformedHandlePosition);
				}

				EditorApplication.QueuePlayerLoopUpdate();
			}
		}
	}
}
