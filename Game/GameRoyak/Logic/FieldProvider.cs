using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GameRoyak.Enums;
using GameRoyak.Models;
using Newtonsoft.Json;
using FilePath = System.IO.Path;

namespace GameRoyak.Logic
{
    public static class FieldProvider
    {
        public static List<List<CellField>> Field { get; set; }
        private static readonly Random Random = new Random();

        public const int NumRow = 6;
        public const int NumColumn = 6;
        public static bool IsBossBattle { get; set; } = false;
        
        private static readonly string Directory = FilePath.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        
        public static void GenerateField()
        {
            Field = new List<List<CellField>>();
            for (var i = 0; i < NumColumn; i++)
            {
                Field.Add(new List<CellField>());
                for (var j = 0; j < NumRow; j++)
                {
                    Field[i].Add(new CellField
                    {
                        IsVisited = false,
                        IsWorking = false,
                        WDirection = false,
                        ADirection = false,
                        SDirection = false, 
                        DDirection = false,
                        IsGenerated = false,
                        OpacityCell = 0
                    });
                    if (i == 0 && j == 0)
                        Field[i][j].IsVisited = true;
                }
            }

            for (var i = 0; i < NumColumn; i++)
            {
                for (var j = 0; j < NumRow; j++)
                {
                    if (i == 0 && j == 0)
                        GenerateMainLine(5, 5);
                    else if (!Field[i][j].IsGenerated)
                    {
                        var stateCurrentCell = CheckCellDirections(i, j);
                        GenerateCell(stateCurrentCell, i, j);
                        Field[i][j].IsGenerated = true;
                    }

                }
            }
        }
        private static void GenerateMainLine(int xEnd, int yEnd)
        {
            var currentDirection = Directions.Right;
            var x = 0;
            var y = 0;
            Field[x][y].SDirection = true;
            Field[x][y].DDirection = true;
            Field[x][y].IsGenerated = true;
            while (x < xEnd || y < yEnd)
            {
                switch (currentDirection)
                {
                    case Directions.Right:
                        x++;
                        var stateCellRight = CheckCellDirections(x, y);
                        Field[x][y].SDirection = true;
                        Field[x][y].ADirection = true;
                        stateCellRight[2] = StateCell.Open;
                        stateCellRight[1] = StateCell.Open;
                        GenerateCell(stateCellRight, x, y);
                        currentDirection = Directions.Down;
                        break;
                    case Directions.Down:
                        y++;
                        var stateCellDown = CheckCellDirections(x, y);
                        Field[x][y].DDirection = true;
                        Field[x][y].WDirection = true;
                        stateCellDown[3] = StateCell.Open;
                        stateCellDown[0] = StateCell.Open;
                        GenerateCell(stateCellDown, x, y);
                        currentDirection = Directions.Right;
                        break;
                }

                Field[x][y].CellNum = GenerateCellNum();
                Field[x][y].IsGenerated = true;
            }
            
            if (Field[xEnd - 1][yEnd].DDirection)
                Field[xEnd][yEnd].ADirection = true;
            if (Field[xEnd][yEnd - 1].SDirection)
                Field[xEnd][yEnd].WDirection = true;
            Field[xEnd][yEnd].DDirection = false;
            Field[xEnd][yEnd].SDirection = false;
            Field[xEnd][yEnd].CellNum = 10;

        }
        private static void GenerateCell(StateCell[] directions, int x, int y)
        {
            Field[x][y].CellNum = GenerateCellNum();
            switch (directions[0])
            {
                case StateCell.Open:
                    Field[x][y].WDirection = true;
                    break;
                case StateCell.Close:
                    Field[x][y].WDirection = false;
                    break;
                case StateCell.Random:
                    Field[x][y].WDirection = Random.Next(0, 3) == 1;
                    break;
            }

            switch (directions[1])
            {
                case StateCell.Open:
                    Field[x][y].ADirection = true;
                    break;
                case StateCell.Close:
                    Field[x][y].ADirection = false;
                    break;
                case StateCell.Random:
                    Field[x][y].ADirection = Random.Next(0, 3) == 1;
                    break;
            }

            switch (directions[2])
            {
                case StateCell.Open:
                    Field[x][y].SDirection = true;
                    break;
                case StateCell.Close:
                    Field[x][y].SDirection = false;
                    break;
                case StateCell.Random:
                    Field[x][y].SDirection = Random.Next(0, 3) == 1;
                    break;
            }

            switch (directions[3])
            {
                case StateCell.Open:
                    Field[x][y].DDirection = true;
                    break;
                case StateCell.Close:
                    Field[x][y].DDirection = false;
                    break;
                case StateCell.Random:
                {
                    Field[x][y].DDirection = Random.Next(0, 3) == 1;
                    break;
                }
            }
        }
        private static StateCell[] CheckCellDirections(int x, int y)
        {
            var result = new StateCell[4];

            if (y > 0)
                if (Field[x][y - 1].SDirection)
                    result[0] = StateCell.Open;
                else
                    result[0] = Field[x][y - 1].IsGenerated ? StateCell.Close : StateCell.Random;
            else
                result[0] = StateCell.Close;

            if (x > 0)
                if (Field[x - 1][y].DDirection)
                    result[1] = StateCell.Open;
                else
                    result[1] = Field[x - 1][y].IsGenerated ? StateCell.Close : StateCell.Random;
            else
                result[1] = StateCell.Close;

            if (y < NumRow - 1)
                if (Field[x][y + 1].WDirection)
                    result[2] = StateCell.Open;
                else
                    result[2] = Field[x][y + 1].IsGenerated ? StateCell.Close : StateCell.Random;
            else
                result[2] = StateCell.Close;

            if (x < NumColumn - 1)
                if (Field[x + 1][y].ADirection)
                    result[3] = StateCell.Open;
                else
                    result[3] = Field[x + 1][y].IsGenerated ? StateCell.Close : StateCell.Random;
            else
                result[3] = StateCell.Close;

            return result;
        }
        private static int GenerateCellNum()
        {
            //TODO: Избавться от магических чисел
            var chanceCell =
                JsonConvert.DeserializeObject<ChanceCell>(
                    File.ReadAllText(FilePath.Combine(Directory, "ChanceCell.json")));
            var randomNum = Random.Next(0, 100);
            if (randomNum < chanceCell?.ChanceCell1)
                return 1;
            if (randomNum < chanceCell?.ChanceCell2)
                return 2;
            if (randomNum < chanceCell?.ChanceCell3)
                return 3;
            if (randomNum < chanceCell?.ChanceCell4)
                return 4;
            if (randomNum < chanceCell?.ChanceCell5)
                return 5;
            if (randomNum < chanceCell?.ChanceCell6)
                return 6;
            return -1;
        }
    }
}