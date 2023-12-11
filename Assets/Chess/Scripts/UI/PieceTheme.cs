using UnityEngine;
[CreateAssetMenu (menuName = "Theme/Pieces")]
public class PieceTheme : ScriptableObject {

	public PieceSprites whitePieces;
	public PieceSprites blackPieces;

	[System.Serializable]
	public class PieceSprites {
		public Sprite pawn, rook, knight, bishop, queen, king;

		public Sprite this [int i] {
			get {
				return new Sprite[] { pawn, rook, knight, bishop, queen, king }[i];
			}
		}
	}
}
