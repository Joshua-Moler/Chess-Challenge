using ChessChallenge.API;

public class MyBot : IChessBot
{

    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 1, 3, 3, 5, 9, 10000 };

    Move bestMove = new Move();
    int moveCount = 0;
    int maxMoveCount = 200;

    public Move Think(Board board, Timer timer)
    {

        // If blacks turn, treat more negative scores as higher
        int multiplier = board.IsWhiteToMove ? 1 : -1;

        Move[] moves = board.GetLegalMoves();
        System.Random rng = new();
        float score = float.NegativeInfinity;
        this.bestMove = moves[0];

        if (board.IsWhiteToMove)
        {
            score = this.WhiteSearch(board: board, depth: 4, setMove: true);
        }
        else
        {
            score = this.BlackSearch(board: board, depth: 4, setMove: true);
        }
        // System.Console.WriteLine("____________\n\n\n");
        return this.bestMove;
    }

    float BlackSearch(
        Board board,
        int depth = 0,
        float alpha = float.NegativeInfinity,
        float beta = float.PositiveInfinity,
        bool setMove = false)
    {
        if (depth == 0)
        {
            if (setMove)
            {
                this.bestMove = new Move();
            }
            return this.ScorePosition(board);
        }

        float score = float.PositiveInfinity;
        Move moveToSet = new Move();
        foreach (Move move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            float newScore = WhiteSearch(board: board, depth: depth - 1, alpha: alpha, beta: beta);
            board.UndoMove(move);

            if (newScore <= alpha)
            {
                // System.Console.Write("Pruning with newScore: ");
                // System.Console.Write(newScore);
                // System.Console.Write(" and alpha: ");
                // System.Console.Write(alpha);
                // System.Console.Write(" and depth: ");
                // System.Console.WriteLine(depth);
                if (setMove)
                {
                    this.bestMove = move;
                }
                return newScore;
            }
            if (newScore <= beta)
            {
                beta = newScore;
            }
            if (newScore < score)
            {
                moveToSet = move;
                score = newScore;
            }
        }
        if (setMove)
        {
            this.bestMove = moveToSet;
        }
        return score;
    }

    float WhiteSearch(
        Board board,
        int depth = 0,
        float alpha = float.NegativeInfinity,
        float beta = float.PositiveInfinity,
        bool setMove = false)
    {
        if (depth == 0)
        {
            if (setMove)
            {
                this.bestMove = new Move();
            }
            return this.ScorePosition(board);
        }

        float score = float.NegativeInfinity;
        Move moveToSet = new Move();

        foreach (Move move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            float newScore = BlackSearch(board: board, depth: depth - 1, alpha: alpha, beta: beta);
            board.UndoMove(move);

            if (newScore >= beta)
            {
                if (setMove)
                {
                    this.bestMove = move;
                }
                return newScore;
            }
            if (newScore >= alpha)
            {
                alpha = newScore;
            }
            if (newScore > score)
            {
                moveToSet = move;
                score = newScore;
            }

        }
        if (setMove)
        {
            this.bestMove = moveToSet;
        }
        return score;
    }

    float ScorePosition(Board board)
    {

        if (board.IsDraw())
        {
            return 0;
        }
        if (board.IsInCheckmate())
        {
            return board.IsWhiteToMove ? float.NegativeInfinity : float.PositiveInfinity;
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

}