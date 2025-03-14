using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static Constants;
using System.Text;
using Unity.VisualScripting;

public class MinMaxAI
{
    private int[,] _board; // AI: 1, None: 0, Player: -1
    private int _maxDepth;
    
    private int _boardSize = 14;

    private StringBuilder sb;
    
    public void Initialize(int maxDepth)
    {
        _board = new int[_boardSize,_boardSize];
        _maxDepth = maxDepth;
        
        sb = new StringBuilder();
    }

    public async UniTask<(int, int)?> FindBestMove(int row, int col)
    {
        _board[row, col] = -1;
        float bestValue = float.MinValue;
        (int row, int col)? bestMove = null;

        for (int x = 0; x < _boardSize; x++)
        {
            for (int y = 0; y < _boardSize; y++)
            {
                if (_board[x, y] == 0)
                {
                    _board[x, y] = 1;
                    float value = MinMax(x, y, 1, true, float.MinValue, float.MaxValue);
                    sb.Append($"x:{x} y:{y} value:{value}\n");
                    _board[x, y] = 0;

                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestMove = (x, y);
                    }
                }
            }
        }
        // Debug.Log(sb.ToString());
        _board[bestMove.Value.Item1, bestMove.Value.Item2] = 1;
        

        
        return await UniTask.FromResult(bestMove);
    }
    
    private float Evaluate(int row, int col, int depth, bool isMaximizing)
    {
        var totalScore = ScoringMove(row, col) / (float)depth;
        return isMaximizing ? totalScore : -totalScore;
    }

    private float MinMax(int row, int col, int depth, bool isMaximizing, float alpha, float beta)
    {
        int r = 0, c = 0;
        if (depth == 1)
        {
            r = row;
            c = col;
        }
        
        if (depth >= _maxDepth)
        {
            float score = Evaluate(row, col, depth, isMaximizing);
            // Debug.Log($"Depth: {depth}, IsMaximizing: {isMaximizing}, Score: {score}");
            return score;
        }

        if (IsAllCellPlaced(_board)) return 0;
        
        isMaximizing = !isMaximizing;

        if (isMaximizing) // ai turn
        {
            float maxValue = float.MinValue;
            for (int x = 0; x < _boardSize; x++)
            {
                for (int y = 0; y < _boardSize; y++)
                {
                    if (_board[x, y] == 0)
                    {
                        _board[x, y] = 1; // ai 착수
                        float value = MinMax(x, y, depth + 1, isMaximizing, alpha, beta); // 착수한 결과에 대한 기댓값 계산
                        _board[x, y] = 0; // 원상복구
                        maxValue = Math.Max(maxValue, value);
                        alpha = Math.Max(alpha, value);

                        if (beta <= alpha) return maxValue;
                    }
                }
            }
            return maxValue;
        }

        else // player turn
        {
            // int a = -1, b = -1; 
            float minValue = float.MaxValue;
            for (int x =0; x < _boardSize; x++)
            {
                for (int y = 0; y < _boardSize; y++)
                {
                    if (_board[x, y] == 0)
                    {
                        _board[x, y] = -1; // player 착수
                        float value = MinMax(x, y, depth + 1, isMaximizing, alpha, beta); // 착수한 결과에 대한 기댓값 계산
                        _board[x, y] = 0; // 원상복구

                        if (value < minValue)
                        {
                            minValue = value;
                            // a = x;
                            // b = y;
                        }
                        
                        minValue = Math.Min(minValue, value);
                        beta = Math.Min(beta, value);

                        if (beta <= alpha) return minValue;
                    }
                }
            }

            // if (depth == 1)
            // {
            //     Debug.Log($"AI turn: ({r}),({c}) \n player will ({a}),({b})");
            // }
            
            return minValue;
        }
    }

    private bool IsAllCellPlaced(int[,] board)
    {
        for (int x = 0; x < _boardSize; x++)
        {
            for (int y = 0; y < _boardSize; y++)
            {
                if (board[x, y] == 0) return false;
            }
        }

        return true;
    }

    private float ScoringMove(int row, int col)
    {
        int cellType = _board[row, col];

        List<(int, int)> dira = new List<(int, int)>{ (0, 1), (0, -1) };
        List<(int, int)> dirb = new List<(int, int)>{ (1, 0), (-1, 0) };
        List<(int, int)> dirc = new List<(int, int)>{ (1, 1), (-1, -1) };
        List<(int, int)> dird = new List<(int, int)>{ (1, -1), (-1, 1) };

        List<List<(int, int)>> directions = new List<List<(int, int)>>
        {
            dira,
            dirb,
            dirc,
            dird
        };

        foreach (var dirs in directions)
        {
            int count = 0;
            int openEnd = 0;

            foreach (var dir in dirs)
            {
                for (int i = 1; i < 5; i++)
                {
                    int x = row + dir.Item1 * i;
                    int y = col + dir.Item2 * i;

                    if (x < 0 || x >= _boardSize || y < 0 || y >= _boardSize)
                    {
                        break;
                    }

                    if (_board[x, y] == cellType) // 이어질경우
                    {
                        count++;
                    }
                    else if (_board[x, y] == 0) // 빈칸인경우
                    {
                        openEnd++;
                        break;
                    }
                }

                // count와 openend에 따른 점수 계산
                switch (count)
                {
                    case 4:
                        if (openEnd == 2) return 100000;
                        else if (openEnd == 1) return 5000;
                        break;
                    case 3:
                        if (openEnd == 2) return 1000;
                        else if (openEnd == 1) return 500;
                        break;
                    case 2:
                        if (openEnd == 2) return 100;
                        else if (openEnd == 1) return 50;
                        break;
                    case 1:
                        if (openEnd == 2) return 10;
                        else if (openEnd == 1) return 5;
                        break;
                }
            }
        }

        return 0;
    }
}