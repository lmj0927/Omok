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
using Unity.VisualScripting;
using System.Text;

public class MCTS
{
    public Action<TurnData> onSearchComplete;
    private Node rootNode;

    private int[,] board;

    private int iterations;
    private const int _boardSize = 15;
    
    public MCTS()
    {
        board = new int[_boardSize, _boardSize];
    }

    public void SetIterations(int iter)
    {
        iterations = iter;
    }

    async public UniTask RunSearch(int row, int col)
    {
        UpdateRootNode(row, col, -1);
        
        for (int i = 0; i < iterations; i++)
        {
            
            if (rootNode.isWinningBoard) // 필승 전략을 찾았을 때
            {
                Node a = rootNode;
                while (a.children.Count > 0)
                {
                    a = a.children[0];
                }
                
                foreach (var child in rootNode.children)
                {
                    if (child.isWinningBoard)
                    {
                        UpdateRootNode(child.move.Item1, child.move.Item2, 1);
                        UnityThread.executeInUpdate(() => {
                            onSearchComplete?.Invoke(new TurnData{row = child.move.Item1, col = child.move.Item2});
                        });
                        return;
                    }
                }
            }
            
            if (i % 100 == 0) await UniTask.Yield();
            double result;
            
            (Node node, bool winnerDecided) = Select(rootNode); // uct에 따라 leaf node 선택

            if (winnerDecided) // 탐색 중 승부가 결정된 node에 도착했을 때
            {
                result = node.isWinningBoard ? 1d : 0;
                Backpropagate(node, result);
                continue;
            }
            
            result = Simulate(node);
            Backpropagate(node, result);
        }
        
        // 착수 지점 결정
        Node bestchild = BestChild(rootNode);
        
        UpdateRootNode(bestchild.move.Item1, bestchild.move.Item2, 1);
        
        //이건.. UniTask말고 다른걸 사용해서 테스트할때 사용한 코드 - 착수
        UnityThread.executeInUpdate(() => {
            onSearchComplete?.Invoke(new TurnData{row = bestchild.move.Item1, col = bestchild.move.Item2});
        });
    }

    private void UpdateRootNode(int row, int col, int player)
    {
        board[row, col] = player;
        if (rootNode != null)
        {
            // 기존 rootNode의 자식 중 해당 위치의 노드를 찾아 rootNode로 설정
            Node existingChild = rootNode.children.FirstOrDefault(child => child.move == (row, col));
    
            if (existingChild != null)
            {
                rootNode = existingChild;
                rootNode.parent = null; // 기존 부모 정보 제거
            }
            else
            {
                // 기존 rootNode에서 찾을 수 없으면 새로운 노드 생성
                rootNode = new Node(null, board, player, (row, col));
            }
        }
        else
        {
            rootNode = new Node(null, board, player, (row, col));
        }
    }

    private (Node,bool) Select(Node node)
    {
        int depth = 0;
        bool winnerDecided = false;
        while (!node.IsTerminal() && node.IsFullyExpanded() && !winnerDecided)
        {
            (node, winnerDecided) = node.BestUCTChild(); // leaf에 도달할때까지 UCT에 따라 다음 수를 둔다
            depth++;
        }
        
        if (winnerDecided)
        {
            return (node, true);
        }

        // leaf까지 내려간 bestUCTChild에서 승부가 나지 않았을 경우
        if (node.IsTerminal()) return (node, false);

        (Node a, bool b) = node.Expand();
        return (a, b);
    }

    private double Simulate(Node node)
    {
        return node.SimulateRandomPlay();
    }

    private void Backpropagate(Node node, double result)
    {
        while (node != null)
        {
            node.Update(result);
            node = node.parent;
        }
    }

    private Node BestChild(Node node)
    {
        Node bestNode = null;
        int maxVisits = int.MinValue;

        foreach (var child in node.children)
        {
            if (child.visits > maxVisits)
            {
                maxVisits = child.visits;
                bestNode = child;
            }
        }

        return bestNode;
    }
}

public class Node
{
    private const double ExplorationParameter = 1.414;
    private const int boardSize = 15;
    private static Random random = new Random();
    public Node parent { get; set; }
    public List<Node> children { get; private set; }
    public double wins { get; set; }
    public int visits { get; set; }
    public bool fullyExpanded { get; private set; }
    public int[,] board { get; private set; }
    public (int, int) move{get; set;}
    public List<(int, int)> effectiveMoves { get; private set; } // leaf node를 뽑을 때는 놓여있는 돌을 기준으로 위치를 제한한다
    public int currentPlayer { get; private set; } // 현재 board 상태를 만든 player
    public bool isWinningBoard { get; private set; } // 필승이 가능한 상태일 때 true가 된다(현재 player와 관계없이 ai 기준)
    public bool isLosingBoard { get; private set; } //  무조건 지는 상태일 때 true가 된다(현재 player 관계없이 ai 기준

    public Node(Node parent, int[,] board, int player, (int, int) move)
    {
        this.parent = parent;
        this.board = (int[,])board.Clone();
        currentPlayer = player;
        children = new List<Node>();
        wins = 0d;
        visits = 0;
        fullyExpanded = false;
        this.move = move;
        isWinningBoard = false;
        isLosingBoard = false;
    }

    public bool IsTerminal()
    {
        return CheckWin(board, move) || !HasEmptyCells(board);
    }

    public bool IsFullyExpanded()
    {
        return fullyExpanded;
    }

    public (Node, bool) Expand()
    {
        effectiveMoves = GetEffectiveMoves(board);
        
        int losingBoardCount = 0;
        Node winningNode = null;
        
        foreach (var move in effectiveMoves)
        {
            int[,] newBoard = (int[,])board.Clone();
            newBoard[move.Item1, move.Item2] = -currentPlayer;

            bool winnerDecided = CheckWin(newBoard, move);

            if (winnerDecided && currentPlayer == 1) // 다음에 player가 두는 수 중, 지는 경우가 있을 경우
            {
                Node child = new Node(this, newBoard, -currentPlayer, move);
                child.isLosingBoard = true;
                children.Add(child);
                losingBoardCount++;
            }
            else if (winnerDecided && currentPlayer == -1) // 다음 ai가 두는 수 중, 이기는 경우가 있을 경우 -> 현재 노드 = 필승 노드
            {
                Node child = new Node(this, newBoard, -currentPlayer, move);
                child.isWinningBoard = true;
                children.Clear();
                children.Add(child);
                winningNode = child;
                isWinningBoard = true;
                return (winningNode, true);
            }
            else // 당장 다음 노드에서 승부가 나지 않는 경우
            {
                Node child = new Node(this, newBoard, -currentPlayer, (move.Item1, move.Item2));
                children.Add(child);
            }
        }

        fullyExpanded = true;

        if (losingBoardCount == children.Count)
        {
            isLosingBoard = true;
            return (children[random.Next(children.Count)], true);
        }
        
        return (children[random.Next(children.Count)], false);
    }

    public double SimulateRandomPlay()
    {
        int[,] tempBoard = new int[boardSize, boardSize];
        Array.Copy(board, tempBoard, board.Length);

        int player = currentPlayer;
        List<(int, int)> possibleMoves = GetPossibleMoves(tempBoard);
        (int, int) move = this.move;
        
        bool winnerDecided = CheckWin(tempBoard, move);
        
        while (!winnerDecided && possibleMoves.Count > 0)
        {
            player = -player;
            move = possibleMoves[random.Next(possibleMoves.Count)];
            tempBoard[move.Item1, move.Item2] = player;
            possibleMoves.Remove(move);
            
            winnerDecided = CheckWin(tempBoard, move);
        }
        return winnerDecided ? (player == 1 ? 1d : 0) : 0;
    }
    
    public void Update(double result)
    {
        visits++;
        wins += result;
    }
    
    public (Node, bool) BestUCTChild() // bool: winnerDecided
    {
        if (currentPlayer == -1) // 마지막으로 player가 뒀을 때
        {
            double logParentVisits = Math.Log(visits + 1);
            Node bestNode = null;
            double bestValue = double.NegativeInfinity;
            int losingBoardCount = 0;

            foreach (var child in children) // 다음 노드
            {
                if (child.isWinningBoard) // 다음 노드 중 하나라도 iswinning이면 현재 node도 iswinning
                {
                    isWinningBoard = true;
                    return (child, true);
                }
                
                if (child.isLosingBoard) // child 모두가 isLosing이면 현재 노드도 isLosing
                {
                    losingBoardCount++;
                    continue;
                }

                double winRate = (double)child.wins / (child.visits + 1e-6);
                double exploration = ExplorationParameter * Math.Sqrt(2 * logParentVisits / (child.visits + 1e-6));
                double uctValue = winRate + exploration;

                if (uctValue > bestValue)
                {
                    bestValue = uctValue;
                    bestNode = child;
                }
            }

            if (losingBoardCount == children.Count)
            {
                isLosingBoard = true;
                return (children[0], true);
            }
            
            return (bestNode ?? children[random.Next(children.Count)], false);
        }

        else // 마지막으로 ai가 뒀을 때
        {
            double logParentVisits = Math.Log(visits + 1);
            Node bestNode = null;
            double bestValue = double.PositiveInfinity;
            int winningMoveCount = 0;
            
            foreach (var child in children)
            {
                if (child.isLosingBoard) // 하나라도 islosing이면 현재노드도 isLosing
                {
                    isLosingBoard = true;
                    return (child, true);
                }

                if (child.isWinningBoard) // child 전체가 isWinning이면 현재노드도 isWinning
                {
                    winningMoveCount++;
                    continue;
                }
                
                double winRate = 1 - ((double)child.wins / (child.visits + 1e-6));
                double exploration = ExplorationParameter * Math.Sqrt(2 * logParentVisits / (child.visits + 1e-6));
                double uctValue = winRate + exploration;

                if (uctValue > bestValue)
                {
                    bestValue = uctValue;
                    bestNode = child;
                }
            }

            if (winningMoveCount == children.Count)
            {
                isWinningBoard = true;
                return (children[random.Next(children.Count)], true);
            }
            return (bestNode ?? children[random.Next(children.Count)], false);
        } 
    }
    
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

        while (x >= 0 && x < boardSize && y >= 0 && y < boardSize && board[x, y] == player)
        {
            count++;
            x += dx;
            y += dy;
        }

        return count;
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

        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (board[i, j] != 0)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            int xPos = i + x;
                            int yPos = j + y;
                            if (xPos >= 0 && xPos < boardSize && yPos >= 0 && yPos < boardSize && board[xPos, yPos] == 0 && !moves.Contains((xPos, yPos)))
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
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                if(board[i, j] == 0)
                    moves.Add((i, j));
            }
        }

        return moves;
    }
}
