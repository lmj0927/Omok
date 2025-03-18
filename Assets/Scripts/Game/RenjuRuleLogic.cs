using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public static class RenjuRuleLogic
{   
    public enum StoneState
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
    public static Dictionary<string, List<TurnData>> testWhitePatterns = new Dictionary<string, List<TurnData>>();
    
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
       testWhitePatterns.Clear();
       // 1. 삼삼(쌍삼) 패턴들

       List<TurnData> noneWhite = new List<TurnData>()
       {

       };

   //테스트 삼삼
        List<TurnData> doubleTreePattern_overlinePattern = new List<TurnData>
        {
            //new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 7, col = 8},  
            new TurnData{row = 7, col = 6},
            //new TurnData{row = 7, col = 12},
            new TurnData{row = 8, col = 7},  // 세로 방향
            new TurnData{row = 9, col = 7},
            // 착점 위치: (7, 6) - 이 위치에 두면 삼삼과 장목이 동시에 발생
        };

        List<TurnData> doubleTreePattern_overlinePattern_White = new List<TurnData>
        {
            new TurnData { row = 7, col = 5 },
        };
        
        List<TurnData> doubleTreePattern_overlinePattern1 = new List<TurnData>
        {
            //new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 7, col = 8},  
            new TurnData{row = 7, col = 6},
            //new TurnData{row = 7, col = 12},
            new TurnData{row = 8, col = 7},  // 세로 방향
            new TurnData{row = 9, col = 7},
            // 착점 위치: (7, 6) - 이 위치에 두면 삼삼과 장목이 동시에 발생
        };

        List<TurnData> doubleTreePattern_overlinePattern_White1 = new List<TurnData>
        {
            new TurnData { row = 7, col = 4 },
        };
        
        // 삼삼 패턴 1: 가로-세로 방향 삼삼
        List<TurnData> doubleThreePattern1 = new List<TurnData>
        {
            //new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 7, col = 10},  // 가로 방향
            new TurnData{row = 7, col = 11},
            new TurnData{row = 10, col = 7},  // 세로 방향
            new TurnData{row = 11, col = 7},
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

        //가짜 삼삼
        List<TurnData> doubleThreeFake = new List<TurnData>
        {
            new TurnData { row = 7, col = 7},
            new TurnData { row = 7, col = 8},
            new TurnData { row = 8, col = 9},
            new TurnData { row = 9, col = 9},
            new TurnData { row = 7, col = 4},

        };

        List<TurnData> doubleThreeFake_White = new List<TurnData>
        {
            new TurnData { row = 7, col = 11 },
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
        
        List<TurnData> doubleFourPattern_White = new List<TurnData>
        {
            new TurnData { row = 7, col = 5 },
            new TurnData { row = 7, col = 11 },
        };
        
        // 사사 패턴 2: 대각선 방향 사사
        List<TurnData> doubleFourPattern2 = new List<TurnData>
        {
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
            new TurnData{row = 7, col = 1},
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
            new TurnData{row = 7, col = 6},  // 중앙
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
        
        // 좌우 삼삼
        List<TurnData> DoubleThreePattern5 = new List<TurnData>
        {
            new TurnData { row = 7, col = 8 },
            new TurnData { row = 7, col = 10 },
            new TurnData { row = 7, col = 6 },
            new TurnData { row = 7, col = 4 },

        };
        
        List<TurnData> DoubleFourPattern5 = new List<TurnData>
        {
            new TurnData { row = 7, col = 8 },
            new TurnData { row = 7, col = 10 },
            new TurnData { row = 7, col = 11 },
            new TurnData { row = 7, col = 5 },
            new TurnData { row = 7, col = 4 },

        };
        
        List<TurnData> DoubleFourPattern6 = new List<TurnData>
        {
            new TurnData { row = 7, col = 8 },
            new TurnData { row = 7, col = 9 },
            new TurnData { row = 7, col = 11 },
            new TurnData { row = 7, col = 5 },
        };
        
        List<TurnData> doubleTreePattern_fake = new List<TurnData>
        {
            //new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 7, col = 7},
            new TurnData{row = 7, col = 8},
            new TurnData{row = 9, col = 8},
            new TurnData{row = 10, col = 5},  // 세로 방향
            new TurnData{row = 10, col = 9},
            // 착점 위치: (7, 6) - 이 위치에 두면 삼삼과 장목이 동시에 발생
        };

        List<TurnData> doubleTreePattern_fake1 = new List<TurnData>
        {
            //new TurnData{row = 7, col = 7},  // 중앙
            new TurnData{row = 7, col = 7},
            new TurnData{row = 7, col = 8},
            new TurnData{row = 9, col = 8},
            new TurnData{row = 10, col = 5},  // 세로 방향
            new TurnData{row = 10, col = 9},
            new TurnData{row = 10, col = 7},
            // 착점 위치: (7, 6) - 이 위치에 두면 삼삼과 장목이 동시에 발생
        };


        List<TurnData> FakeDoubleThreePattern = new List<TurnData>
        {
            new TurnData { row = 7, col = 7 },
            new TurnData { row = 8, col = 7 },
            new TurnData { row = 9, col = 7 },
            new TurnData { row = 10, col = 6 },
            new TurnData { row = 6, col = 8 },
            new TurnData { row = 7, col = 9 },
            new TurnData { row = 8, col = 9 },
        };

        List<TurnData> FakeDoubleThreePattern_White = new List<TurnData>
        {
            new TurnData { row = 5, col = 6 },
            new TurnData { row = 6, col = 6 },
            new TurnData { row = 7, col = 6 },
            new TurnData { row = 9, col = 6 },
            
            new TurnData { row = 6, col = 7 },
            new TurnData { row = 7, col = 8 },
            new TurnData { row = 6, col = 9 },
            
        };

        // 테스트 패턴 추가
        testPatterns.Add("DoubleTreePattern_overlinePattern", doubleTreePattern_overlinePattern);
        testWhitePatterns.Add("DoubleTreePattern_overlinePattern_White", doubleTreePattern_overlinePattern_White);

        testPatterns.Add("doubleTreePattern_fake", doubleTreePattern_fake);
        testWhitePatterns.Add("None-1", noneWhite);

        testPatterns.Add("doubleTreePattern_fake1", doubleTreePattern_fake1);
        testWhitePatterns.Add("None-2", noneWhite);

        testPatterns.Add("FakeDoubleThreePattern", FakeDoubleThreePattern);
        testWhitePatterns.Add("FakeDoubleThreePattern_White", FakeDoubleThreePattern_White);
        
        testPatterns.Add("DoubleTreePattern_overlinePattern1", doubleTreePattern_overlinePattern1);
        testWhitePatterns.Add("DoubleTreePattern_overlinePattern_White1", doubleTreePattern_overlinePattern_White1);

        testPatterns.Add("DoubleThreeFake", doubleThreeFake);
        testWhitePatterns.Add("DoubleThreeFake_White", doubleThreeFake_White);
        
        testPatterns.Add("DoubleThreePattern1", doubleThreePattern1);
        testWhitePatterns.Add("None1", noneWhite);
        
        testPatterns.Add("DoubleThreePattern2", doubleThreePattern2);
        testWhitePatterns.Add("None2", noneWhite);
        
        testPatterns.Add("DoubleThreePattern3", doubleThreePattern3);
        testWhitePatterns.Add("None3", noneWhite);
        
        testPatterns.Add("DoubleThreePattern4", doubleThreePattern4);
        testWhitePatterns.Add("None4", noneWhite);
        
        testPatterns.Add("DoubleFourPattern1", doubleFourPattern1);
        testWhitePatterns.Add("doubleFourPattern_White", doubleFourPattern_White);
        
        testPatterns.Add("DoubleFourPattern2", doubleFourPattern2);
        testWhitePatterns.Add("None6", noneWhite);
        
        testPatterns.Add("DoubleFourPattern3", doubleFourPattern3);
        testWhitePatterns.Add("None7", noneWhite);
        
        testPatterns.Add("OverlinePattern1", overlinePattern1);
        testWhitePatterns.Add("None8", noneWhite);
        
        testPatterns.Add("OverlinePattern2", overlinePattern2);
        testWhitePatterns.Add("None9", noneWhite);
        
        testPatterns.Add("OverlinePattern3", overlinePattern3);
        testWhitePatterns.Add("None10", noneWhite);
        
        testPatterns.Add("ComplexPattern1", complexPattern1);
        testWhitePatterns.Add("None11", noneWhite);
        
        
        testPatterns.Add("ComplexPattern2", complexPattern2);
        testWhitePatterns.Add("None12", noneWhite);
        
        testPatterns.Add("DoubleThreePattern5", DoubleThreePattern5);
        testWhitePatterns.Add("None13", noneWhite);
        
        testPatterns.Add("DoubleFourPattern5", DoubleFourPattern5);
        testWhitePatterns.Add("None14", noneWhite);
        
        testPatterns.Add("DoubleFourPattern6", DoubleFourPattern6);
        testWhitePatterns.Add("None15", noneWhite);
        
   }

    public static (bool, int, int) IsRenjuRuleViolation(int x, int y)
    {
        //CountThreeFour
        (int threeCount, int fourCount) = CountThreeFour(x, y);
        
        if(threeCount >= 2 || fourCount >= 2){
            Debug.Log("TreeCount: " + threeCount + " FourCount: " + fourCount);
            return (true, threeCount, fourCount);
        }
        
        // 장목(6목 이상) 체크
        if (CheckOverline(x, y))
        {
            return (true, 0, 0);
        }
        
        return (false, 0, 0);
    }

    public static bool IsRenjuRuleFakeViolation(int x, int y){
        //CountThreeFour
        (int threeCount, int fourCount) = CountThreeFour(x, y);
        
        if(threeCount >= 2 || fourCount >= 2){
            Debug.Log("TreeCount: " + threeCount + " FourCount: " + fourCount);
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

    public static void SetBoard(int x, int y, StoneState state){
        board[x, y] = state;
    }


    static (int, int) CountThreeFour(int x, int y, StoneState originState = StoneState.Empty)
    {
        int threeCount = 0;
        int fourCount = 0;
        
        board[x, y] = StoneState.Black;
        
        foreach (var (dx, dy) in directions)
        {
            (int three, int four)= CheckThreeFour(x, y, dx, dy);

            threeCount += three;
            fourCount += four;
        }
        
        board[x, y] = originState;
        
        return (threeCount, fourCount);
    }

    //Debug Print Board
    static void PrintBoard(){
        string line = "";
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                line += GetDebugStr(board[i, j]);
            }
            line += "\n";
        }

        Debug.Log(line);
    }

    static string GetDebugStr(StoneState state){
        if(state == StoneState.Black){
            return "O";
        }else if(state == StoneState.White){
            return "X";
        }else{
            return "Z";
        }
    }
 
    
    static (int, int) CheckThreeFour(int x, int y, int dx, int dy)
    {
        int count = 1;
        int emptyBefore = 0;
        int emptyAfter = 0;

        bool prevIsEmpty = false;

        List<(int, int)> black_list = new List<(int, int)>();
        List<(int, int)> empty_list = new List<(int, int)>();

        black_list.Add((x, y));
        
        int i = 1;
        while(true)
        {
            int nx = x + dx * i;
            int ny = y + dy * i;
            
            if (IsValidPosition(nx, ny))
            {
                if (board[nx, ny] == StoneState.Black)
                {
                    count++;
                    prevIsEmpty = false;
                    black_list.Add((nx, ny));
                }
                else if (board[nx, ny] == StoneState.Empty)
                {
                    emptyAfter++;
                    
                    if(prevIsEmpty){
                        break;
                    }
                    empty_list.Add((nx, ny));
                    prevIsEmpty = true;
                }
                else
                {
                    break;
                }
            }
            else{
                break;
            }

            i++;
        }
        
        prevIsEmpty = false;

        i = 1;
        while(true)
        {
            int nx = x - dx * i;
            int ny = y - dy * i;
            
            if (IsValidPosition(nx, ny))
            {
                if (board[nx, ny] == StoneState.Black)
                {
                    count++;
                    prevIsEmpty = false;
                    black_list.Add((nx, ny));
                }
                else if (board[nx, ny] == StoneState.Empty)
                {
                    emptyBefore++;
                    
                    if(prevIsEmpty){
                        break;
                    }
                    empty_list.Add((nx, ny));
                    prevIsEmpty = true;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }

            i++;
        }
      
        
        int threeCount = 0;
        if(count == 3 && emptyBefore > 0 && emptyAfter > 0)
        {
            //PrintBoard();
            foreach(var (nx, ny) in empty_list){
                //IsRenjuRuleFakeViolation(nx, ny);

                if(IsOpenFourChecker(nx, ny, dx, dy)){
                    threeCount++;
                    break;
                }
            }
        }

        int fourCount = 0;
        List<List<(int, int)>> five_list = new List<List<(int, int)>>();

        if(count >= 4 && (emptyBefore > 0 || emptyAfter > 0))
        {
            foreach(var (nx, ny) in empty_list){
                //PrintBoard();
                List<(int, int)> five = IsFiveChecker(nx, ny, dx, dy);
                
                bool isExist = false;
                foreach(var item in five_list){
                    if(item.SequenceEqual(five.OrderBy(t => t.Item1).ThenBy(t => t.Item2))){
                        Debug.Log("Exist");
                        isExist = true;
                        break;
                    }
                }

                if(five.Count == 4 && !isExist){
                    five_list.Add(five.OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToList());
                }
            }
        }

        fourCount = five_list.Count;

        return (threeCount, fourCount);
    }

    static bool IsOpenFourChecker(int x, int y, int dx, int dy){
        bool ret = false;
        
        // 임시로 돌 놓기
        board[x, y] = StoneState.Black;

        if(IsOpenFour(x, y, dx, dy)){
            ret = true;
        }

        board[x, y] = StoneState.Empty;

        return ret;
    }

    static List<(int, int)> IsFiveChecker(int x, int y, int dx, int dy){
        board[x, y] = StoneState.Black;
        List<(int, int)> black_list = CheckFive(x, y, dx, dy);
        board[x, y] = StoneState.Empty;

        return black_list;
    }    
    

    static bool IsOpenFour(int x, int y, int dx, int dy)
    {
        int count = 1;
        bool isOpen = false;
        bool prevIsEmpty = false;
        
        for (int i = 1; i <= 4; i++)
        {
            int nx = x + dx * i;
            int ny = y + dy * i;
            
            if (IsValidPosition(nx, ny))
            {
                if (board[nx, ny] == StoneState.Black)
                {
                    if(prevIsEmpty){
                        isOpen = false; //장목
                        break;
                    }
                    count++;
                }
                else if (board[nx, ny] == StoneState.Empty)
                {
                    if(prevIsEmpty){
                        break;
                    }
                    prevIsEmpty = true;
                    isOpen = true;
                }
                else
                {
                    if(prevIsEmpty){
                        break;
                    }
                    isOpen = false;
                    break;
                }
            }
        }
        
        if(!isOpen){
            return false;
        }

        prevIsEmpty = false;

        
        for (int i = 1; i <= 4; i++)
        {
            int nx = x - dx * i;
            int ny = y - dy * i;
            
            if (IsValidPosition(nx, ny))
            {
                if (board[nx, ny] == StoneState.Black)
                {
                    if(prevIsEmpty){
                        isOpen = false; // 장목
                        break;
                    }
                    count++;
                }
                else if (board[nx, ny] == StoneState.Empty)
                {
                    if(prevIsEmpty){
                        break;
                    }
                    prevIsEmpty = true;
                    isOpen = true;
                }
                else
                {
                    if(prevIsEmpty){
                        break;
                    }
                    isOpen = false;
                    break;
                }
            }
        }
        
        return count == 4 && isOpen;
    }

    static List<(int, int)> CheckFive(int x, int y, int dx, int dy)
    {
        List<(int, int)> black_list = new List<(int, int)>();
        
        for (int i = 1; i <= 5; i++)
        {
            int nx = x + dx * i;
            int ny = y + dy * i;
            
            if (IsValidPosition(nx, ny))
            {
                if (board[nx, ny] == StoneState.Black)
                {
                    black_list.Add((nx, ny));
                }
                else if (board[nx, ny] == StoneState.Empty)
                {
                    break;
                }
                else
                {
                    break;
                }
            }
        }
        
        
        for (int i = 1; i <= 5; i++)
        {
            int nx = x - dx * i;
            int ny = y - dy * i;
            
            if (IsValidPosition(nx, ny))
            {
                if (board[nx, ny] == StoneState.Black)
                {
                    black_list.Add((nx, ny));
                }
                else if (board[nx, ny] == StoneState.Empty)
                {
                    break;
                }
                else
                {
                    break;
                }
            }
        }

        if(black_list.Count != 4){
            black_list.Clear();
        }
        
        return black_list;
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