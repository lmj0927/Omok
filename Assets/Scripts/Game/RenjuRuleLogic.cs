using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public static class RenjuRuleLogic
{   
    enum StoneState
    {
        Black = 0,
        White = 1,
        Empty = 2,
    }
 
    static StoneState[,] board; 
    static readonly int BOARD_SIZE = 14;

    static readonly (int, int)[] directions = 
    {
        (1, 0),
        (0, 1),
        (1, 1),
        (1, -1)
    };

    public static Dictionary<string, List<TurnData>> testPatterns = new Dictionary<string, List<TurnData>>();

    
    public static void SetBoard(Cell[,] board){
        RenjuRuleLogic.board = new StoneState[BOARD_SIZE, BOARD_SIZE];

        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                if(board[i, j].GetCellType() == Constants.CELL_TYPE.Black){
                    RenjuRuleLogic.board[i, j] = StoneState.Black;
                }else if(board[i, j].GetCellType() == Constants.CELL_TYPE.White){
                    RenjuRuleLogic.board[i, j] = StoneState.White;
                }else{
                    RenjuRuleLogic.board[i, j] = StoneState.Empty;
                }
            }
        }
    }

   // 테스트용 렌주룰 위반 패턴 초기화
    public static void InitializeTestPatterns()
    {
        testPatterns.Clear();
        // 1. 삼삼(쌍삼) 패턴들
        
        // 삼삼 패턴 1: 가로-세로 방향 삼삼
        List<TurnData> doubleThreePattern1 = new List<TurnData>
        {
            //new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 7, col = 8},  // 가로 방향
            new TurnData{row = 7, col = 9},
            new TurnData{row = 8, col = 7},  // 세로 방향
            new TurnData{row = 9, col = 7},
            // 착점 위치: (7, 6) - 이 위치에 두면 삼삼 발생
        };
        
        // 삼삼 패턴 2: 대각선 방향 삼삼
        List<TurnData> doubleThreePattern2 = new List<TurnData>
        {
            //new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 8, col = 8},  // 대각선 ↗
            new TurnData{row = 9, col = 9},
            new TurnData{row = 8, col = 6},  // 대각선 ↘
            new TurnData{row = 9, col = 5},
            // 착점 위치: (6, 6) - 이 위치에 두면 삼삼 발생
        };
        
        // 삼삼 패턴 3: 가로-대각선 방향 삼삼
        List<TurnData> doubleThreePattern3 = new List<TurnData>
        {
            //new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 7, col = 8},  // 가로 방향
            new TurnData{row = 7, col = 9},
            new TurnData{row = 8, col = 8},  // 대각선 방향
            new TurnData{row = 9, col = 9},
            // 착점 위치: (6, 6) - 이 위치에 두면 삼삼 발생
        };

        // 삼삼 패턴 4: 한칸 떨어진 삼삼
        List<TurnData> doubleThreePattern4 = new List<TurnData>
        {
            new TurnData{row = 7, col = 8},  // 가로 방향
            new TurnData{row = 7, col = 10},
            new TurnData{row = 8, col = 7},  // 세로 방향
            new TurnData{row = 9, col = 7},
        };

        
        // 2. 사사(쌍사) 패턴들
        
        // 사사 패턴 1: 가로-세로 방향 사사
        List<TurnData> doubleFourPattern1 = new List<TurnData>
        {
            //new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 7, col = 8},  // 가로 방향
            new TurnData{row = 7, col = 9},
            new TurnData{row = 7, col = 10},
            new TurnData{row = 8, col = 7},  // 세로 방향
            new TurnData{row = 9, col = 7},
            new TurnData{row = 10, col = 7},
            // 착점 위치: (6, 7) - 이 위치에 두면 사사 발생
        };
        
        // 사사 패턴 2: 대각선 방향 사사
        List<TurnData> doubleFourPattern2 = new List<TurnData>
        {
            new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 8, col = 8},  // 대각선 ↗
            new TurnData{row = 9, col = 9},
            new TurnData{row = 10, col = 10},
            new TurnData{row = 8, col = 6},  // 대각선 ↘
            new TurnData{row = 9, col = 5},
            new TurnData{row = 10, col = 4},
            // 착점 위치: (6, 6) - 이 위치에 두면 사사 발생
        };
        
        // 사사 패턴 3: 가로-대각선 방향 사사 (다른 형태)
        List<TurnData> doubleFourPattern3 = new List<TurnData>
        {
            new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 7, col = 8},  // 가로 방향
            new TurnData{row = 7, col = 9},
            new TurnData{row = 7, col = 10},
            new TurnData{row = 8, col = 8},  // 대각선 방향
            new TurnData{row = 9, col = 9},
            new TurnData{row = 10, col = 10},
            // 착점 위치: (7, 6) - 이 위치에 두면 사사 발생
        };
        
        // 3. 장목(6목 이상) 패턴들
        
        // 장목 패턴 1: 가로 방향 장목
        List<TurnData> overlinePattern1 = new List<TurnData>
        {
            new TurnData{row = 7, col = 2},
            new TurnData{row = 7, col = 3},
            new TurnData{row = 7, col = 4},
            new TurnData{row = 7, col = 5},
            new TurnData{row = 7, col = 7},
            // 착점 위치: (7, 6) - 이 위치에 두면 장목 발생
        };
        
        // 장목 패턴 2: 세로 방향 장목
        List<TurnData> overlinePattern2 = new List<TurnData>
        {
            new TurnData{row = 2, col = 7},
            new TurnData{row = 3, col = 7},
            new TurnData{row = 4, col = 7},
            new TurnData{row = 5, col = 7},
            new TurnData{row = 7, col = 7},
            // 착점 위치: (6, 7) - 이 위치에 두면 장목 발생
        };
        
        // 장목 패턴 3: 대각선 방향 장목
        List<TurnData> overlinePattern3 = new List<TurnData>
        {
            new TurnData{row = 2, col = 2},
            new TurnData{row = 3, col = 3},
            new TurnData{row = 4, col = 4},
            new TurnData{row = 5, col = 5},
            new TurnData{row = 7, col = 7},
            // 착점 위치: (6, 6) - 이 위치에 두면 장목 발생
        };
        
        // 4. 복합 패턴들 (삼삼+장목, 사사+장목 등)
        
        // 복합 패턴 1: 삼삼+장목
        List<TurnData> complexPattern1 = new List<TurnData>
        {
            new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 7, col = 8},  // 가로 방향
            new TurnData{row = 7, col = 9},
            new TurnData{row = 7, col = 10},
            new TurnData{row = 7, col = 11},
            new TurnData{row = 8, col = 7},  // 세로 방향
            new TurnData{row = 9, col = 7},
            // 착점 위치: (7, 6) - 이 위치에 두면 삼삼과 장목이 동시에 발생
        };
        
        // 복합 패턴 2: 사사+삼삼
        List<TurnData> complexPattern2 = new List<TurnData>
        {
            new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 7, col = 8},  // 가로 방향 (사)
            new TurnData{row = 7, col = 9},
            new TurnData{row = 7, col = 10},
            new TurnData{row = 8, col = 7},  // 세로 방향 (삼)
            new TurnData{row = 9, col = 7},
            new TurnData{row = 8, col = 8},  // 대각선 방향 (삼)
            new TurnData{row = 9, col = 9},
            // 착점 위치: (6, 6) - 이 위치에 두면 사사와 삼삼이 동시에 발생
        };

        // 테스트 패턴 추가
        testPatterns.Add("DoubleThreePattern1", doubleThreePattern1);
        testPatterns.Add("DoubleThreePattern2", doubleThreePattern2);
        testPatterns.Add("DoubleThreePattern3", doubleThreePattern3);
        testPatterns.Add("DoubleThreePattern4", doubleThreePattern4);

        testPatterns.Add("DoubleFourPattern1", doubleFourPattern1);
        testPatterns.Add("DoubleFourPattern2", doubleFourPattern2);
        testPatterns.Add("DoubleFourPattern3", doubleFourPattern3);

        testPatterns.Add("OverlinePattern1", overlinePattern1);
        testPatterns.Add("OverlinePattern2", overlinePattern2);
        testPatterns.Add("OverlinePattern3", overlinePattern3);

        testPatterns.Add("ComplexPattern1", complexPattern1);
        testPatterns.Add("ComplexPattern2", complexPattern2);
    }

    public static bool IsRenjuRuleViolation(int x, int y)
    {
        // 삼삼 체크
        if (CountDoubleThrees(x, y) >= 2)
        {
            return true;
        }
        
        // 사사 체크
        if (CountOpenFours(x, y) >= 2)
        {
            return true;
        }
        
        // 장목(6목 이상) 체크
        if (CheckOverline(x, y))
        {
            return true;
        }
        
        return false;
    }

    static bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < BOARD_SIZE && y >= 0 && y < BOARD_SIZE;
    }

    // 삼삼(쌍삼) 개수 세기
    static int CountDoubleThrees(int x, int y)
    {
        int count = 0;
        
        // 임시로 돌 놓기
        board[x, y] = StoneState.Black;
        
        // 각 방향에 대해 세 개 연속 돌(삼) 체크
        foreach (var (dx, dy) in directions)
        {
            if (IsOpenThree(x, y, dx, dy))
            {
                count++;
            }

            if(IsOpenThree(x, y, -dx, -dy)){
                count++;
            }
        }
        
        // 임시 돌 제거
        board[x, y] = StoneState.Empty;
        
        // 같은 방향의 양쪽을 중복 계산했으므로 2로 나눔
        return count /2;
    }
 
    // 열린 삼(한쪽이 뚫린 세 개 연속 돌) 체크
    static bool IsOpenThree(int x, int y, int dx, int dy)
    {
        // 구현: 열린 삼인지 체크하는 로직
        // 완전한 구현을 위해서는 다양한 패턴 체크 필요
        
        // 간단한 구현 예시
        int count = 1; // 현재 위치 포함
        int emptyBefore = 0;
        int emptyAfter = 0;
        
        // 해당 방향으로 연속된 같은 돌 세기
        for (int i = 1; i <= 3; i++)
        {
            int nx = x + dx * i;
            int ny = y + dy * i;
            
            if (IsValidPosition(nx, ny))
            {
                if (board[nx, ny] == StoneState.Black)
                {
                    count++;
                }
                else if (board[nx, ny] == StoneState.Empty)
                {
                    emptyAfter++;
                    break;
                }
                else
                {
                    break;
                }
            }
        }
        
        // 반대 방향으로 연속된 같은 돌 세기
        for (int i = 1; i <= 3; i++)
        {
            int nx = x - dx * i;
            int ny = y - dy * i;
            
            if (IsValidPosition(nx, ny))
            {
                if (board[nx, ny] == StoneState.Black)
                {
                    count++;
                }
                else if (board[nx, ny] == StoneState.Empty)
                {
                    emptyBefore++;
                    break;
                }
                else
                {
                    break;
                }
            }
        }
        
        // 열린 삼: 정확히 3개의 돌이 있고, 양쪽이 모두 비어있어야 함
        return count == 3 && emptyBefore > 0 && emptyAfter > 0;
    }
    
    // 사사(쌍사) 개수 세기
    static int CountOpenFours(int x, int y)
    {
        int count = 0;
        
        // 임시로 돌 놓기
        board[x, y] = StoneState.Black;
        
        // 각 방향에 대해 4개 연속 돌(사) 체크
        foreach (var (dx, dy) in directions)
        {
            if (IsOpenFour(x, y, dx, dy))
            {
                count++;
            }
        }
        
        // 임시 돌 제거
        board[x, y] = StoneState.Empty;
        
        return count;
    }

    
    // 열린 사(한쪽이 뚫린 네 개 연속 돌) 체크
    static bool IsOpenFour(int x, int y, int dx, int dy)
    {
        // 임시 구현: 열린 4인지 체크하는 로직
        
        int count = 1; // 현재 위치 포함
        bool isOpen = false;
        
        // 정방향으로 연속된 같은 돌 세기
        for (int i = 1; i <= 4; i++)
        {
            int nx = x + dx * i;
            int ny = y + dy * i;
            
            if (IsValidPosition(nx, ny))
            {
                if (board[nx, ny] == StoneState.Black)
                {
                    count++;
                }
                else if (board[nx, ny] == StoneState.Empty)
                {
                    isOpen = true;
                    break;
                }
                else
                {
                    break;
                }
            }
        }
        
        // 반대 방향으로도 체크
        for (int i = 1; i <= 4; i++)
        {
            int nx = x - dx * i;
            int ny = y - dy * i;
            
            if (IsValidPosition(nx, ny))
            {
                if (board[nx, ny] == StoneState.Black)
                {
                    count++;
                }
                else if (board[nx, ny] == StoneState.Empty && count == 4)
                {
                    isOpen = true;
                    break;
                }
                else
                {
                    break;
                }
            }
        }
        
        // 열린 사: 정확히 4개의 돌이 있고, 한쪽이 비어있어야 함
        return count == 4 && isOpen;
    }
    
    // 장목(6목 이상) 체크
    static bool CheckOverline(int x, int y)
    {
        // 임시로 돌 놓기
        board[x, y] = StoneState.Black;
        
        // 각 방향에 대해 연속된 돌 개수 세기
        foreach (var (dx, dy) in directions)
        {
            int count = 1; // 현재 위치 포함
            
            // 정방향으로 세기
            for (int i = 1; i < 6; i++)
            {
                int nx = x + dx * i;
                int ny = y + dy * i;
                
                if (IsValidPosition(nx, ny) && board[nx, ny] == StoneState.Black)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            
            // 반대 방향으로 세기
            for (int i = 1; i < 6; i++)
            {
                int nx = x - dx * i;
                int ny = y - dy * i;
                
                if (IsValidPosition(nx, ny) && board[nx, ny] == StoneState.Black)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            
            // 6목 이상이면 장목
            if (count >= 6)
            {
                // 임시 돌 제거
                board[x, y] = StoneState.Empty;
                return true;
            }
        }
        
        // 임시 돌 제거
        board[x, y] = StoneState.Empty;
        return false;
    }
}