namespace ConnectFour;

public class GameState
{
    // 静态构造函数，在类首次加载时执行，用于计算所有可能的胜利位置组合
    static GameState()
    {
        CalculateWinningPlaces();
    }

    /// <summary>
    /// 指示玩家是否获胜，游戏是否平局，或游戏是否正在进行
    /// </summary>
    public enum WinState
    {
        No_Winner = 0,  // 游戏继续进行，无胜者
        Player1_Wins = 1, // 玩家1获胜
        Player2_Wins = 2, // 玩家2获胜
        Tie = 3  // 平局
    }

    /// <summary>
    /// 计算当前轮到的玩家。默认情况下，玩家1先开始。
    /// 玩家通过计算已下棋子的数量来决定轮次（奇数轮为玩家2，偶数轮为玩家1）。
    /// </summary>
    public int PlayerTurn => TheBoard.Count(x => x != 0) % 2 + 1;

    /// <summary>
    /// 当前游戏中已完成的回合数，即棋盘上已放置的棋子数
    /// </summary>
    public int CurrentTurn => TheBoard.Count(x => x != 0);

    /// <summary>
    /// 存储所有可能的胜利组合（横向、纵向、对角线）
    /// </summary>
    public static readonly List<int[]> WinningPlaces = new();

    /// <summary>
    /// 计算所有可能的胜利位置（四个连续棋子的位置）
    /// </summary>
    public static void CalculateWinningPlaces()
    {
        // 计算水平方向的胜利组合
        for (byte row = 0; row < 6; row++)
        {
            byte rowCol1 = (byte)(row * 7);
            byte rowColEnd = (byte)((row + 1) * 7 - 1);
            byte checkCol = rowCol1;
            while (checkCol <= rowColEnd - 3)
            {
                WinningPlaces.Add(new int[]
                {
                    checkCol,
                    (byte)(checkCol + 1),
                    (byte)(checkCol + 2),
                    (byte)(checkCol + 3)
                });
                checkCol++;
            }
        }

        // 计算垂直方向的胜利组合
        for (byte col = 0; col < 7; col++)
        {
            byte colRow1 = col;
            byte colRowEnd = (byte)(35 + col);
            byte checkRow = colRow1;
            while (checkRow <= 14 + col)
            {
                WinningPlaces.Add(new int[]
                {
                    checkRow,
                    (byte)(checkRow + 7),
                    (byte)(checkRow + 14),
                    (byte)(checkRow + 21)
                });
                checkRow += 7;
            }
        }

        // 计算正斜线（“/”方向）对角线的胜利组合
        for (byte col = 0; col < 4; col++)
        {
            byte colRow1 = (byte)(21 + col);
            byte colRowEnd = (byte)(35 + col);
            byte checkPos = colRow1;
            while (checkPos <= colRowEnd)
            {
                WinningPlaces.Add(new int[]
                {
                    checkPos,
                    (byte)(checkPos - 6),
                    (byte)(checkPos - 12),
                    (byte)(checkPos - 18)
                });
                checkPos += 7;
            }
        }

        // 计算反斜线（“\”方向）对角线的胜利组合
        for (byte col = 0; col < 4; col++)
        {
            byte colRow1 = (byte)(0 + col);
            byte colRowEnd = (byte)(14 + col);
            byte checkPos = colRow1;
            while (checkPos <= colRowEnd)
            {
                WinningPlaces.Add(new int[]
                {
                    checkPos,
                    (byte)(checkPos + 8),
                    (byte)(checkPos + 16),
                    (byte)(checkPos + 24)
                });
                checkPos += 7;
            }
        }
    }

    /// <summary>
    /// 检查棋盘是否存在获胜情况
    /// </summary>
    public WinState CheckForWin()
    {
        if (TheBoard.Count(x => x != 0) < 7) return WinState.No_Winner;

        foreach (var scenario in WinningPlaces)
        {
            if (TheBoard[scenario[0]] == 0) continue;
            if (TheBoard[scenario[0]] == TheBoard[scenario[1]] &&
                TheBoard[scenario[1]] == TheBoard[scenario[2]] &&
                TheBoard[scenario[2]] == TheBoard[scenario[3]])
                return (WinState)TheBoard[scenario[0]];
        }

        return TheBoard.Count(x => x != 0) == 42 ? WinState.Tie : WinState.No_Winner;
    }

    /// <summary>
    /// 在指定列放置棋子
    /// </summary>
    public byte PlayPiece(int column)
    {
        if (CheckForWin() != WinState.No_Winner) throw new ArgumentException("游戏已结束");
        if (TheBoard[column] != 0) throw new ArgumentException("列已满");

        var landingSpot = column;
        for (var i = column; i < 42; i += 7)
        {
            if (TheBoard[landingSpot + 7] != 0) break;
            landingSpot = i;
        }

        TheBoard[landingSpot] = PlayerTurn;
        return ConvertLandingSpotToRow(landingSpot);
    }

    /// <summary>
    /// 棋盘数组（42个格子）
    /// 0 表示未放置棋子，1 代表玩家1的棋子，2 代表玩家2的棋子
    /// </summary>
    public List<int> TheBoard { get; private set; } = new List<int>(new int[42]);

    /// <summary>
    /// 重置棋盘
    /// </summary>
    public void ResetBoard()
    {
        TheBoard = new List<int>(new int[42]);
    }

    /// <summary>
    /// 将棋子放置的索引转换为行号（1-6）
    /// </summary>
    private byte ConvertLandingSpotToRow(int landingSpot)
    {
        return (byte)(Math.Floor(landingSpot / (decimal)7) + 1);
    }
}
