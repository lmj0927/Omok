using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Random = System.Random;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;

public class MCTS
{
    public Action<TurnData> onSearchComplete;
    private Node rootNode;

    private int[,] board;
    
    private int iterations = 10000;

    public MCTS()
    {
        board = new int[14, 14];
    }

    public void SetIterations(int iter)
    {
        iterations = iter;
    }

    async public UniTask RunSearch(int row, int col)
    {
        board[row, col] = -1;
        rootNode = new Node(null, board, -1, (row, col));
        
        for (int i = 0; i < iterations; i++)
        {
            if (i % 100 == 0) await UniTask.Yield();
            
            Node node = Select(rootNode); // uct에 따라 leaf node 선택
            int result = Simulate(node);
            Backpropagate(node, result);
        }
        
        Node bestchild = BestChild(rootNode);
        board[bestchild.move.Item1, bestchild.move.Item2] = 1;
        
        //이건.. UniTask말고 다른걸 사용해서 테스트할때 사용한 코드
        UnityThread.executeInUpdate(() => {
            onSearchComplete?.Invoke(new TurnData{row = bestchild.move.Item1, col = bestchild.move.Item2});
        });
    }

    private Node Select(Node node)
    {
        while (!node.IsTerminal() && node.IsFullyExpanded())
        {
            node = node.BestUCTChild(); // leaf에 도달할때까지 UCT에 따라 다음 수를 둔다
        }
        
        return node.IsTerminal() ? node : node.Expand(); // leaf에 도달하면, 가능한 수 중에서 랜덤하게 선택한다
    }

    private int Simulate(Node node)
    {
        return node.SimulateRandomPlay();
    }

    private void Backpropagate(Node node, int result)
    {
        while (node != null)
        {
            node.Update(result);
            node = node.Parent;
        }
    }

    private Node BestChild(Node node)
    {
        Node bestNode = null;
        int maxVisits = int.MinValue;

        foreach (var child in node.Children)
        {
            if (child.Visits > maxVisits)
            {
                maxVisits = child.Visits;
                bestNode = child;
            }
        }

        return bestNode;
    }
}

public class Node
{
    private const double ExplorationParameter = 1.414;
    private static Random random = new Random();
    public Node Parent { get; set; }
    public List<Node> Children { get; private set; }
    public int Wins { get; set; }
    public int Visits { get; set; }
    public bool FullyExpanded { get; private set; }
    public int[,] Board { get; private set; }
    public (int, int) move{get; set;}
    public List<(int, int)> effectiveMoves { get; private set; } // leaf node를 뽑을 때는 놓여있는 돌을 기준으로 위치를 제한한다
    public int CurrentPlayer { get; private set; }

    public Node(Node parent, int[,] board, int player, (int, int) move)
    {
        Parent = parent;
        Board = (int[,])board.Clone();
        CurrentPlayer = player;
        Children = new List<Node>();
        Wins = 0;
        Visits = 0;
        FullyExpanded = false;
        this.move = move;

    }

    public bool IsTerminal()
    {
        return CheckWin(Board, move) || !HasEmptyCells(Board);
    }

    public bool IsFullyExpanded()
    {
        return FullyExpanded;
    }

    public Node Expand()
    {
        effectiveMoves = GetEffectiveMoves(Board);
        foreach (var move in effectiveMoves)
        {
            int[,] newBoard = (int[,])Board.Clone();
            newBoard[move.Item1, move.Item2] = CurrentPlayer;
            Node child = new Node(this, newBoard, -CurrentPlayer, (move.Item1, move.Item2));
            Children.Add(child);
        }
        FullyExpanded = true;
        return Children[random.Next(Children.Count)];
    }

    public int SimulateRandomPlay()
    {
        int[,] tempBoard = new int[Board.GetLength(0), Board.GetLength(1)];
        Array.Copy(Board, tempBoard, Board.Length);

        int player = CurrentPlayer;
        List<(int, int)> possibleMoves = GetPossibleMoves(tempBoard);
        (int, int) move = this.move;
        while (!CheckWin(tempBoard, move) && possibleMoves.Count > 0)
        {
            move = possibleMoves[random.Next(possibleMoves.Count)];
            tempBoard[move.Item1, move.Item2] = player;
            possibleMoves.Remove(move);
            player = -player;
        }
        return CheckWin(tempBoard, move) ? (player == 1 ? 5 : -5) : 0;
    }


    public void Update(int result)
    {
        Visits++;
        Wins += result;
    }
    public Node BestUCTChild()
    {
        double logParentVisits = Math.Log(Visits + 1);
        Node bestNode = null;
        double bestValue = double.NegativeInfinity;

        foreach (var child in Children)
        {
            double winRate = (double)child.Wins / (child.Visits + 1e-6);
            double exploration = ExplorationParameter * Math.Sqrt(2 * logParentVisits / (child.Visits + 1e-6));
            double uctValue = winRate + exploration;

            if (uctValue > bestValue)
            {
                bestValue = uctValue;
                bestNode = child;
            }
        }

        return bestNode;
    }
    
    // public Node BestUCTChild()
    // {
    //     return Children.OrderByDescending(n => (double)n.Wins / (n.Visits + 1e-6) + ExplorationParameter * Math.Sqrt(2 * Math.Log(Visits + 1) / (n.Visits + 1e-6))).FirstOrDefault();
    // }
    
    private static readonly (int, int)[] directions = 
    {
        (1, 0),
        (0, 1),
        (1, 1),
        (1, -1)
    };

    private static bool CheckWin(int[,] board, (int, int) lastMove)
    {
        int x = lastMove.Item1, y = lastMove.Item2;
        int player = board[x, y];
        if (player == 0) return false;

        foreach (var (dx, dy) in directions)
        {
            if (CountConsecutiveStones(board, x, y, dx, dy, player) + CountConsecutiveStones(board, x, y, -dx, -dy, player) - 1 >= 5)
            {
                return true;
            }
        }
        return false;
    }

    private static int CountConsecutiveStones(int[,] board, int x, int y, int dx, int dy, int player)
    {
        int count = 0;
        int boardCount = board.GetLength(0);

        while (x >= 0 && x < boardCount && y >= 0 && y < boardCount && board[x, y] == player)
        {
            count++;
            x += dx;
            y += dy;
        }

        return count;
    }

    private static bool CheckDirection(int[,] board, int x, int y, int dx, int dy)
    {
        int count = 0;
        int player = board[x, y];
        for (int i = 0; i < 5; i++)
        {
            int nx = x + dx * i;
            int ny = y + dy * i;
            if (nx >= 0 && ny >= 0 && nx < board.GetLength(0) && ny < board.GetLength(1) && board[nx, ny] == player)
                count++;
            else
                break;
        }
        return count == 5;
    }

    private static bool HasEmptyCells(int[,] board)
    {
        foreach (var cell in board)
        {
            if (cell == 0) return true;
        }
        return false;
    }

    private static List<(int, int)> GetEffectiveMoves(int[,] board)
    {
        List<(int, int)> moves = new List<(int, int)>();

        for (int i = 0; i < 14; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                if (board[i, j] != 0)
                {
                    for (int x = -2; x < 3; x++)
                    {
                        for (int y = -2; y < 3; y++)
                        {
                            int xPos = i + x;
                            int yPos = j + y;
                            if (xPos >= 0 && xPos < 14 && yPos >= 0 && yPos < 14 && board[xPos, yPos] == 0 && !moves.Contains((xPos, yPos)))
                            {
                                moves.Add((xPos, yPos));
                            }
                        }
                    }
                }
            }
        }
        return moves;
    }
    
    private static List<(int, int)> GetPossibleMoves(int[,] board)
    {
        List<(int, int)> moves = new List<(int, int)>();
        for (int i = 0; i < 14; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                if(board[i, j] == 0)
                    moves.Add((i, j));
            }
        }

        return moves;
    }
}
