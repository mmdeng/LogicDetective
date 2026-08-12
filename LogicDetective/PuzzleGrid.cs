// namespace LogicDetective;

// using System.Text;

// internal static class PuzzleGrid
// {
//     private const int LabelWidth = 12;
//     private const int CellWidth = 8;

//     static string GetGrid(GameSession session)
//     {
//         var grids= session.Puzzle.Categories.EnumerateAllCategoryPairs();
//         var text = new StringBuilder();
//         text.AppendLine("===== 推理グリッド =====");
//         text.AppendLine();

//         foreach (var categoryPair in session.Puzzle.Categories.EnumerateAllCategoryPairs())
//         {
//             text.Append(GetCategoryGrid(session.PlayerState, categoryPair));
//             text.AppendLine();
//         }
//         return text.ToString();
//     }

//     static string GetCategoryGrid(PlayerState playerState, CategoryPair categoryPair)
//     {
//         var text = new StringBuilder();

//         // カテゴリ名
//         text.AppendLine($"{categoryPair.Category1.Name} × {categoryPair.Category2.Name}");

//         // 列タイトル
//         text.Append(string.Empty.PadRight(LabelWidth));
//         foreach (var item2 in categoryPair.Category2.Items)
//         {
//             text.Append(item2.Name.PadRight(CellWidth));
//         }
//         text.AppendLine();

//         foreach (var item1 in categoryPair.Category1.Items)
//         {
//             // 行タイトル
//             text.Append(item1.Name.PadRight(LabelWidth));

//             foreach (var item2 in categoryPair.Category2.Items)
//             {
//                 var mark = GetPlayerMark(playerState, item1, item2);
//                 text.Append(mark.PadRight(CellWidth));
//             }
//             text.AppendLine();
//         }
//         return text.ToString();
//     }

//     static string GetPlayerMark(PlayerState playerState, Item item1, Item item2)
//     {
//         var state = playerState.GetState(item1, item2);
//         return state switch
//         {
//             ReasoningState.Positive => "○",
//             ReasoningState.Negative => "×",
//             ReasoningState.Unknown => "?",
//             _ => throw new InvalidOperationException($"不正な推論状態: {state}"),
//         };
//     }
// }