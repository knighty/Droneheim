using UnityEngine;

namespace Droneheim
{
	class PieceBuilder : MonoBehaviour
	{
		public BasicKeyframeList<Vector3> Size;

		public void Update()
		{
			Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(100, 100, 100));
			foreach (Collider collider in colliders)
			{
				Piece piece = collider.GetComponent<Piece>();
				if (piece != null)
				{
					//piece.hideFlags = HideFlags.
					piece.gameObject.SetActive(false);
				}
			}
			//Physics.OverlapBox()
		}
	}
}
