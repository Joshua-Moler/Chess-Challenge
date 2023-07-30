using ChessChallenge.API;

public class MyBot : IChessBot
{

    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 1, 3, 3, 5, 9, 10000 };

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        float maxEvaluation = 0;
        System.Random rng = new();
        Move moveToPlay = moves[rng.Next(moves.Length)];
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            float moveEvaluation = this.Evaluate(board);
            if (moveEvaluation > maxEvaluation)
            {
                moveToPlay = move;
                maxEvaluation = moveEvaluation;
            }
            board.UndoMove(move);
        }
        return moveToPlay;
    }

    /// <summary>
    /// Evaluate the current position and return a score.
    /// A positive score indicates an evaluation favoring white,
    /// a negative score indicates an evaluation favoring black.
    /// </summary>
    float Evaluate(Board board, uint depth = 0)
    {

        // If depth is zero, simply add the scores of all pieces (omitting kings)
        if (depth == 0)
        {
            PieceList[] pieceList = board.GetAllPieceLists();
            int whiteScore = pieceValues[1] * pieceList[0].Count +
                                pieceValues[2] * pieceList[1].Count +
                                pieceValues[3] * pieceList[2].Count +
                                pieceValues[4] * pieceList[3].Count +
                                pieceValues[5] * pieceList[4].Count -
                            (
                                pieceValues[1] * pieceList[6].Count +
                                pieceValues[2] * pieceList[7].Count +
                                pieceValues[3] * pieceList[8].Count +
                                pieceValues[4] * pieceList[9].Count +
                                pieceValues[5] * pieceList[10].Count
                            );
            return whiteScore;

        }

        // Add logic to search tree

        return Evaluate(board, depth - 1);
    }
}