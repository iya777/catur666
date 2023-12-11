﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class BoardUI : MonoBehaviour {

    public Shader squareShader;
    // pawn, rook, knight, bishop, queen, king (HARUS URUT)
    public Sprite[] whitePieceSprites = new Sprite[6];
    public Sprite[] blackPieceSprites = new Sprite[6];

    // UI Board
    public BoardTheme boardTheme;
	public bool showLegalMoves;

	public bool whiteIsBottom = true;

	MeshRenderer[, ] squareRenderers;
    SpriteRenderer[, ] squarePieceRenderers;
	Move lastMadeMove;
	MoveGenerator moveGenerator;

	const float pieceDepth = -0.1f;
	const float pieceDragDepth = -0.2f;

    [SerializeField] private Transform whiteClockTransform;
    [SerializeField] private Transform blackClockTransform;

	void Awake () {
		moveGenerator = new MoveGenerator ();
		CreateBoardUI ();

	}

	public void HighlightLegalMoves (Board board, Coord fromSquare) {
		if (showLegalMoves) {

			var moves = moveGenerator.GenerateMoves (board);

			for (int i = 0; i < moves.Count; i++) {
				Move move = moves[i];
				if (move.StartSquare == BoardRepresentation.IndexFromCoord (fromSquare)) {
					Coord coord = BoardRepresentation.CoordFromIndex (move.TargetSquare);
					SetSquareColour (coord, boardTheme.lightSquares.legal, boardTheme.darkSquares.legal);
				}
			}
		}
	}

	public void DragPiece (Coord pieceCoord, Vector2 mousePos) {
		squarePieceRenderers[pieceCoord.fileIndex, pieceCoord.rankIndex].transform.position = new Vector3 (mousePos.x, mousePos.y, pieceDragDepth);
	}

	public void ResetPiecePosition (Coord pieceCoord) {
		Vector3 pos = PositionFromCoord (pieceCoord.fileIndex, pieceCoord.rankIndex, pieceDepth);
		squarePieceRenderers[pieceCoord.fileIndex, pieceCoord.rankIndex].transform.position = pos;
	}

	public void SelectSquare (Coord coord) {
		SetSquareColour (coord, boardTheme.lightSquares.selected, boardTheme.darkSquares.selected);
	}

	public void DeselectSquare (Coord coord) {
		ResetSquareColours ();
	}

	public bool TryGetSquareUnderMouse (Vector2 mouseWorld, out Coord selectedCoord) {
		int file = (int) (mouseWorld.x + 4);
		int rank = (int) (mouseWorld.y + 4);
		if (!whiteIsBottom) {
			file = 7 - file;
			rank = 7 - rank;
		}
		selectedCoord = new Coord (file, rank);
		return file >= 0 && file < 8 && rank >= 0 && rank < 8;
	}

	public void UpdatePosition (Board board) {
		for (int rank = 0; rank < 8; rank++) {
			for (int file = 0; file < 8; file++) {
				Coord coord = new Coord (file, rank);
				int piece = board.Square[BoardRepresentation.IndexFromCoord (coord.fileIndex, coord.rankIndex)];
				squarePieceRenderers[file, rank].sprite = GetPieceSprite(piece);
				squarePieceRenderers[file, rank].transform.position = PositionFromCoord (file, rank, pieceDepth);
			}
		}

	}

    private Sprite GetPieceSprite(int piece)
    {
        if (Piece.IsColour(piece, Piece.White))
        {
            switch (Piece.PieceType(piece))
            {
                case Piece.Pawn:
                    return whitePieceSprites[0];
                case Piece.Rook:
                    return whitePieceSprites[1];
                case Piece.Knight:
                    return whitePieceSprites[2];
                case Piece.Bishop:
                    return whitePieceSprites[3];
                case Piece.Queen:
                    return whitePieceSprites[4];
                case Piece.King:
                    return whitePieceSprites[5];
                default:
                    if (piece != 0)
                    {
                        Debug.Log(piece);
                    }
                    return null;
            }
        }
        else
        {
            switch (Piece.PieceType(piece))
            {
                case Piece.Pawn:
                    return blackPieceSprites[0];
                case Piece.Rook:
                    return blackPieceSprites[1];
                case Piece.Knight:
                    return blackPieceSprites[2];
                case Piece.Bishop:
                    return blackPieceSprites[3];
                case Piece.Queen:
                    return blackPieceSprites[4];
                case Piece.King:
                    return blackPieceSprites[5];
                default:
                    if (piece != 0)
                    {
                        Debug.Log(piece);
                    }
                    return null;
            }
        }
    }

	public void OnMoveMade (Board board, Move move, bool animate = false) {
		lastMadeMove = move;
		if (animate) {
			StartCoroutine (AnimateMove (move, board));
		} else {
			UpdatePosition (board);
			ResetSquareColours ();
		}
	}

	IEnumerator AnimateMove (Move move, Board board) {
		float t = 0;
		const float moveAnimDuration = 0.15f;
		Coord startCoord = BoardRepresentation.CoordFromIndex (move.StartSquare);
		Coord targetCoord = BoardRepresentation.CoordFromIndex (move.TargetSquare);
		Transform pieceT = squarePieceRenderers[startCoord.fileIndex, startCoord.rankIndex].transform;
		Vector3 startPos = PositionFromCoord (startCoord);
		Vector3 targetPos = PositionFromCoord (targetCoord);
		SetSquareColour (BoardRepresentation.CoordFromIndex (move.StartSquare), boardTheme.lightSquares.moveFromHighlight, boardTheme.darkSquares.moveFromHighlight);

		while (t <= 1) {
			yield return null;
			t += Time.deltaTime * 1 / moveAnimDuration;
			pieceT.position = Vector3.Lerp (startPos, targetPos, t);
		}
		UpdatePosition (board);
		ResetSquareColours ();
		pieceT.position = startPos;
	}

	void HighlightMove (Move move) {
		SetSquareColour (BoardRepresentation.CoordFromIndex (move.StartSquare), boardTheme.lightSquares.moveFromHighlight, boardTheme.darkSquares.moveFromHighlight);
		SetSquareColour (BoardRepresentation.CoordFromIndex (move.TargetSquare), boardTheme.lightSquares.moveToHighlight, boardTheme.darkSquares.moveToHighlight);
	}

	void CreateBoardUI () {

        squareRenderers = new MeshRenderer[8, 8];
        squarePieceRenderers = new SpriteRenderer[8, 8];

		for (int rank = 0; rank < 8; rank++) {
			for (int file = 0; file < 8; file++) {
                // Create square
                Transform square = GameObject.CreatePrimitive (PrimitiveType.Quad).transform;
                square.parent = transform;
				square.name = BoardRepresentation.SquareNameFromCoordinate (file, rank);
				square.position = PositionFromCoord (file, rank, 0);
				Material squareMaterial = new Material (squareShader);

				squareRenderers[file, rank] = square.gameObject.GetComponent<MeshRenderer> ();
                squareRenderers[file, rank].material = squareMaterial;

                // Create piece sprite renderer for current square
                SpriteRenderer pieceRenderer = new GameObject("Piece").AddComponent<SpriteRenderer> ();
				pieceRenderer.transform.parent = square;
				pieceRenderer.transform.position = PositionFromCoord (file, rank, pieceDepth);
				pieceRenderer.transform.localScale = Vector3.one * 100 / (2000 / 6f);
				squarePieceRenderers[file, rank] = pieceRenderer;
			}
		}

		ResetSquareColours ();
	}

	void ResetSquarePositions () {
		for (int rank = 0; rank < 8; rank++) {
			for (int file = 0; file < 8; file++) {
				squareRenderers[file, rank].transform.position = PositionFromCoord (file, rank, 0);
				squarePieceRenderers[file, rank].transform.position = PositionFromCoord (file, rank, pieceDepth);
			}
		}

		if (!lastMadeMove.IsInvalid) {
			HighlightMove (lastMadeMove);
		}
	}

    
	public void SetPerspective (bool whitePOV) {
		whiteIsBottom = whitePOV;
		ResetSquarePositions ();
        if (!whitePOV)
        {
            Vector3 tmp = whiteClockTransform.position;
            whiteClockTransform.position = blackClockTransform.position;
            blackClockTransform.position = tmp;
        }
	}

	public void ResetSquareColours (bool highlight = true) {
		for (int rank = 0; rank < 8; rank++) {
			for (int file = 0; file < 8; file++) {
				SetSquareColour (new Coord (file, rank), boardTheme.lightSquares.normal, boardTheme.darkSquares.normal);
			}
		}
		if (highlight) {
			if (!lastMadeMove.IsInvalid) {
				HighlightMove (lastMadeMove);
			}
		}
	}

	void SetSquareColour (Coord square, Color lightCol, Color darkCol) {
		squareRenderers[square.fileIndex, square.rankIndex].material.color = (square.IsLightSquare ()) ? lightCol : darkCol;
	}

	public Vector3 PositionFromCoord (int file, int rank, float depth = 0) {
		if (whiteIsBottom) {
			return new Vector3 (-3.5f + file, -3.5f + rank, depth);
		}
		return new Vector3 (-3.5f + 7 - file, 7 - rank - 3.5f, depth);

	}

	public Vector3 PositionFromCoord (Coord coord, float depth = 0) {
		return PositionFromCoord (coord.fileIndex, coord.rankIndex, depth);
	}

}
