using ChessChallenge.API;

public class MyBot : IChessBot
{

    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 1, 3, 3, 5, 9, 10000 };

    public Move Think(Board board, Timer timer)
    {

        // If blacks turn, treat more negative scores as higher
        int multiplier = board.IsWhiteToMove ? 1 : -1;

        Move[] moves = board.GetLegalMoves();
        float maxEvaluation = float.NegativeInfinity;
        System.Random rng = new();
        Move moveToPlay = moves[rng.Next(moves.Length)];
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            float moveEvaluation = this.Evaluate(board, 2) * multiplier;
            // System.Console.WriteLine(move);
            // System.Console.WriteLine(multiplier);
            // System.Console.WriteLine(moveEvaluation);
            // System.Console.WriteLine('\n');
            if (moveEvaluation > maxEvaluation)
            {

                moveToPlay = move;
                maxEvaluation = moveEvaluation;
            }
            board.UndoMove(move);
        }
        // System.Console.WriteLine("____________\n\n\n");
        return moveToPlay;
    }

    float ScorePosition(Board board)
    {

        if (board.IsDraw())
        {
            return 0;
        }
        if (board.IsInCheckmate())
        {
            return board.IsWhiteToMove ? -100000 : 100000;
        }
        PieceList[] pieceList = board.GetAllPieceLists();
        float[] weights = { 1F, 0.01F };
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

        int activeMoveCount = board.IsWhiteToMove ? board.GetLegalMoves().Length : -board.GetLegalMoves().Length;
        board.ForceSkipTurn();
        int otherMoveCount = board.IsWhiteToMove ? board.GetLegalMoves().Length : -board.GetLegalMoves().Length;
        board.UndoSkipTurn();
        int moveAdvantage = activeMoveCount + otherMoveCount;

        // System.Console.Write("Number of moves: ");
        // System.Console.WriteLine(moveAdvantage);


        return weights[0] * whiteScore + weights[1] * moveAdvantage;
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
            return this.ScorePosition(board);

        }


        // If blacks turn, treat more negative scores as higher
        int multiplier = board.IsWhiteToMove ? 1 : -1;

        float bestScore = board.IsWhiteToMove ? float.NegativeInfinity : float.PositiveInfinity;
        // Iterate through all possible moves
        foreach (Move move in board.GetLegalMoves())
        {


            // Evaluate the position after each move
            board.MakeMove(move);
            float score = Evaluate(board, depth - 1);
            board.UndoMove(move);


            if (score * multiplier > bestScore * multiplier)
            {
                bestScore = score;
            }

        }
        // System.Console.WriteLine(bestScore);
        // System.Console.WriteLine(board.IsWhiteToMove);
        // System.Console.WriteLine("\n\n\n");
        return bestScore;
    }
}