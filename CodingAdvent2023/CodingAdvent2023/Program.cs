using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodingAdvent2023
{
  public class Program
  {
    public const string symbolList = "!@#$%^&*()_-+=~`<>?/'\":;\\|][}{";

    public static void Main()
    {
      var input = GetInput();
      var multiplierSideList = new int[input.Length];

      var sum = 0;
      for (int i = 0; i < input.Length; i++)
      {
        var matches = ReturnNumberOfWinnersPerLine(input[i]);
        sum += multiplierSideList[i] + 1;
        for (int j = 1; j < matches + 1; j++)
        {
          multiplierSideList[i + j] += multiplierSideList[i] + 1;
        }
      }

      //foreach (var line in input)
      //{
      //  sum += SomeMethod(line);
      //}

      Console.WriteLine($"The sum was {sum}");
    }

    public static string[] GetInput()
    {
      var assembly = Assembly.GetExecutingAssembly();
      var resourceName = "CodingAdvent2023.Resources.input4.txt";

      using (Stream stream = assembly.GetManifestResourceStream(resourceName))
      using (StreamReader reader = new StreamReader(stream))
      {
        string result = reader.ReadToEnd();
        return result.Split('\n');
      }

    }

    // Day 4
    private static int ReturnNumberOfWinnersPerLine(string line)
    {
      var splits = line.Split(':');
      var game = splits[0].Trim();
      var winningNumbers = splits[1].Trim().Split('|')[0].Split(' ').Where(a => !string.IsNullOrEmpty(a)).Select(x => x.Trim());
      var myNumbers = splits[1].Trim().Split('|')[1].Split(' ').Where(a => !string.IsNullOrEmpty(a)).Select(x => x.Trim());
      var count = 0;
      foreach (var winNumber in winningNumbers)
      {
        if (myNumbers.Contains(winNumber))
        {
          count++;
        }
      }

      return count;
    }

    private static int ReturnWinningTotalForLine(string line)
    {
      var splits = line.Split(':');
      var game = splits[0].Trim();
      var winningNumbers = splits[1].Trim().Split('|')[0].Split(' ').Where(a => !string.IsNullOrEmpty(a)).Select(x => x.Trim());
      var myNumbers = splits[1].Trim().Split('|')[1].Split(' ').Where(a => !string.IsNullOrEmpty(a)).Select(x => x.Trim());
      var count = 0;
      foreach (var winNumber in winningNumbers)
      {
        if (myNumbers.Contains(winNumber))
        {
          count++;
        }
      }
      var potentialReturn = Math.Pow(2, count - 1);
      return count > 0 ? (int)potentialReturn : 0;
    }

    // Day 3
    private static int ReturnGearPartNumbers(string mainLine, string precedingLine, string nextLine)
    {
      var numberDictionary = new List<KeyValuePair<int, List<int>>>();
      var inlineGearSymbols = new List<int>();
      var totalSum = 0;
      var gearsThatMattered = 0;
      // Store Numbers that are not touching a symbol inline, and add numbers into the sum that are.
      for (int i = 0; i < mainLine.Length; i++)
      {
        if (Char.IsDigit(mainLine[i]))
        {
          var endIndex = i;
          for (int j = i + 1; j < mainLine.Length; j++)
          {
            // We are at the end so we need to handle this one differently.
            if (j == mainLine.Length - 1)
            {
              if (Char.IsDigit(mainLine[j]))
              {
                endIndex = j;
                break;
              }
            }

            if (Char.IsDigit(mainLine[j]))
            {
              continue;
            }
            else
            {
              endIndex = j - 1;
              break;
            }
          }
          var number = int.Parse(mainLine.Substring(i, endIndex - i + 1));

          var indices = new List<int>();
          for (int k = i - 1; k <= endIndex + 1; k++)
          {
            indices.Add(k);
          }
          numberDictionary.Add(new KeyValuePair<int, List<int>>(number, indices));
          i = endIndex;
        }
        if (mainLine[i] == '*')
        {
          inlineGearSymbols.Add(i);
        }
      }

      foreach (var gearSymbol in inlineGearSymbols)
      {
        // For inline gears
        if (gearSymbol > 0 && gearSymbol < mainLine.Length && Char.IsDigit(mainLine[gearSymbol - 1]) && Char.IsDigit(mainLine[gearSymbol + 1]))
        {
          var firstNumber = GetNumberFromLine(mainLine, gearSymbol - 1);
          var secondNumber = GetNumberFromLine(mainLine, gearSymbol + 1);
          totalSum += firstNumber * secondNumber;
          gearsThatMattered++;
          continue;
        }

        // For gears trailing a number
        if (Char.IsDigit(mainLine[gearSymbol - 1]))
        {
          var possiblePrecedingPosition = CheckLineForDigitAtPosition(precedingLine, gearSymbol);
          if (possiblePrecedingPosition > -1)
          {
            totalSum += GetNumberFromLine(precedingLine, possiblePrecedingPosition) * GetNumberFromLine(mainLine, gearSymbol - 1);
            gearsThatMattered++;
            continue;
          }
          var possibleFollowingPosition = CheckLineForDigitAtPosition(nextLine, gearSymbol);
          if (possibleFollowingPosition > -1)
          {
            totalSum += GetNumberFromLine(nextLine, possibleFollowingPosition) * GetNumberFromLine(mainLine, gearSymbol - 1);
            gearsThatMattered++;
            continue;
          }
        }

        // For gears before a number
        if (Char.IsDigit(mainLine[gearSymbol + 1]))
        {
          var possiblePrecedingPosition = CheckLineForDigitAtPosition(precedingLine, gearSymbol);
          if (possiblePrecedingPosition > -1)
          {
            totalSum += GetNumberFromLine(precedingLine, possiblePrecedingPosition) * GetNumberFromLine(mainLine, gearSymbol + 1);
            gearsThatMattered++;
            continue;
          }
          var possibleFollowingPosition = CheckLineForDigitAtPosition(nextLine, gearSymbol);
          if (possibleFollowingPosition > -1)
          {
            totalSum += GetNumberFromLine(nextLine, possibleFollowingPosition) * GetNumberFromLine(mainLine, gearSymbol + 1);
            gearsThatMattered++;
            continue;
          }
        }


        // For gears with numbers on top and bottom
        var possibleTopNumberPosition = CheckLineForDigitAtPosition(precedingLine, gearSymbol); // Check for a number touching above
        var possibleBottomNumberPosition = CheckLineForDigitAtPosition(nextLine, gearSymbol); // Check for a number touching below
        if (possibleTopNumberPosition > -1)
        {
          if (possibleBottomNumberPosition > -1) // Check for a match on the bottom
          {
            totalSum += GetNumberFromLine(precedingLine, possibleTopNumberPosition) * GetNumberFromLine(nextLine, possibleBottomNumberPosition);
            gearsThatMattered++;
            continue;
          }
          if (!Char.IsDigit(precedingLine[gearSymbol])) // If there is a . just above the gear symbol, and we didn't find one below, we need to check for another one above.
          {
            var possibleNewNumberPositionToCheck = possibleTopNumberPosition > gearSymbol ? gearSymbol - 1 : gearSymbol + 1;
            if (Char.IsDigit(precedingLine[possibleNewNumberPositionToCheck]))
            {
              totalSum += GetNumberFromLine(precedingLine, possibleTopNumberPosition) * GetNumberFromLine(precedingLine, possibleNewNumberPositionToCheck);
              gearsThatMattered++;
              continue;
            }
          }

        }
        else if (possibleBottomNumberPosition > -1) // There is a number below, but not above.  We need to check for two numbers below, both diagonal.
        {
          if (!Char.IsDigit(nextLine[gearSymbol])) // If there is a . just above the gear symbol, and we didn't find one below, we need to check for another one above.
          {
            var possibleNewNumberPositionToCheck = possibleBottomNumberPosition > gearSymbol ? gearSymbol - 1 : gearSymbol + 1;
            if (Char.IsDigit(nextLine[possibleNewNumberPositionToCheck]))
            {
              totalSum += GetNumberFromLine(nextLine, possibleBottomNumberPosition) * GetNumberFromLine(nextLine, possibleNewNumberPositionToCheck);
              gearsThatMattered++;
            }
          }
        }
      }

      return totalSum;
    }

    private static int CheckLineForDigitAtPosition(string line, int position)
    {
      return string.IsNullOrEmpty(line) ? -1
        : Char.IsDigit(line[position]) ? position
        : position + 1 < line.Length && Char.IsDigit(line[position + 1]) ? position + 1
        : position - 1 > 0 && Char.IsDigit(line[position - 1]) ? position - 1
        : -1;
    }

    private static int GetNumberFromLine(string line, int position)
    {
      int startPosition = position;
      int endPosition = position;
      for (int i = position + 1; i < line.Length; i++)
      {
        if (Char.IsDigit(line[i]))
        {
          endPosition++;
        }
        else
        {
          break;
        }
      }

      for (int i = position - 1; i >= 0; i--)
      {
        if (Char.IsDigit(line[i]))
        {
          startPosition--;
        }
        else
        {
          break;
        }
      }
      return int.Parse($"{line.Substring(startPosition, endPosition - startPosition + 1)}");
    }

    private static int ReturnMachinePartNumbers(string mainLine, string precedingLine, string nextLine)
    {
      var numberDictionary = new List<KeyValuePair<int, List<int>>>();
      var totalSum = 0;

      // Store Numbers that are not touching a symbol inline, and add numbers into the sum that are.
      for (int i = 0; i < mainLine.Length; i++)
      {
        if (Char.IsDigit(mainLine[i]))
        {
          var endIndex = i;
          for (int j = i + 1; j < mainLine.Length; j++)
          {
            // We are at the end so we need to handle this one differently.
            if (j == mainLine.Length - 1)
            {
              if (Char.IsDigit(mainLine[j]))
              {
                endIndex = j;
                break;
              }
            }

            if (Char.IsDigit(mainLine[j]))
            {
              continue;
            }
            else
            {
              endIndex = j - 1;
              break;
            }
          }
          var number = int.Parse(mainLine.Substring(i, endIndex - i + 1));
          if ((i > 0 && symbolList.Contains(mainLine[i - 1])) || (i < mainLine.Length && symbolList.Contains(mainLine[endIndex + 1])))
          {
            totalSum += number;
            i = endIndex;
            continue;
          }

          var indices = new List<int>();
          for (int k = i - 1; k <= endIndex + 1; k++)
          {
            indices.Add(k);
          }
          numberDictionary.Add(new KeyValuePair<int, List<int>>(number, indices));
          i = endIndex;
        }
      }

      var precedingLineSymbols = GetSymbolIndices(precedingLine);
      var nextLineSymbols = GetSymbolIndices(nextLine);

      foreach (var number in numberDictionary)
      {
        if (number.Value.Intersect(precedingLineSymbols).Any() || number.Value.Intersect(nextLineSymbols).Any())
        {
          totalSum += number.Key;
        }
      }

      return totalSum;
    }

    private static List<int> GetSymbolIndices(string line)
    {
      var list = new List<int>();
      for (int i = 0; i < line.Length; i++)
      {
        if (symbolList.Contains(line[i]))
        {
          list.Add(i);
        }
      }
      return list;
    }

    // Day 2
    private static int GetMinimumGamePowers(string line)
    {
      var stringWithOutPrefix = line.ToLower().Split(':').Select(x => x.Trim()).ToArray();
      var pulls = stringWithOutPrefix[1].Split(';').Select(y => y.Trim());

      int highestRed = 0;
      int highestBlue = 0;
      int highestGreen = 0;

      foreach (var pull in pulls)
      {
        var colors = pull.Split(',').Select(x => x.Trim());
        foreach (var color in colors)
        {
          var breakdown = color.Split(' ');
          switch (breakdown[1])
          {
            case "green":
              if (int.Parse(breakdown[0]) > highestGreen)
              {
                highestGreen = int.Parse(breakdown[0]);
              }
              break;
            case "blue":
              if (int.Parse(breakdown[0]) > highestBlue)
              {
                highestBlue = int.Parse(breakdown[0]);
              }
              break;
            case "red":
              if (int.Parse(breakdown[0]) > highestRed)
              {
                highestRed = int.Parse(breakdown[0]);
              }
              break;
            default:
              Console.WriteLine($"Line contained an unexpected color: {line}");
              break;
          }
        }
      }

      var power = highestRed * highestGreen * highestBlue;
      return power;
    }

    private static int GameLineIfitWasPossible(string line)
    {
      var stringWithOutPrefix = line.ToLower().Split(':').Select(x => x.Trim()).ToArray();
      var pulls = stringWithOutPrefix[1].Split(';').Select(y => y.Trim());

      int highestRed = 0;
      int highestBlue = 0;
      int highestGreen = 0;

      foreach (var pull in pulls)
      {
        var colors = pull.Split(',').Select(x => x.Trim());
        foreach (var color in colors)
        {
          var breakdown = color.Split(' ');
          switch (breakdown[1])
          {
            case "green":
              if (int.Parse(breakdown[0]) > highestGreen)
              {
                highestGreen = int.Parse(breakdown[0]);
              }
              break;
            case "blue":
              if (int.Parse(breakdown[0]) > highestBlue)
              {
                highestBlue = int.Parse(breakdown[0]);
              }
              break;
            case "red":
              if (int.Parse(breakdown[0]) > highestRed)
              {
                highestRed = int.Parse(breakdown[0]);
              }
              break;
            default:
              Console.WriteLine($"Line contained an unexpected color: {line}");
              break;
          }
        }
      }

      return highestRed < 13 && highestGreen < 14 && highestBlue < 15 ? int.Parse(stringWithOutPrefix[0].Split(' ')[1]) : 0;
    }

    //Day 1
    public static int GetLineNumber(string line)
    {
      var dictionary = new List<KeyValuePair<string, int>>();
      var numbersArray = new Dictionary<string, string>
      {
        { "one",   "1" },
        { "two",   "2" },
        { "three", "3" },
        { "four",  "4" },
        { "five",  "5" },
        { "six",   "6" },
        { "seven", "7" },
        { "eight", "8" },
        { "nine",  "9" }
      };

      for (int i = 0; i < line.Length; i++)
      {
        if (Char.IsDigit(line[i]))
        {
          dictionary.Add(new KeyValuePair<string, int>($"{line[i]}", i));
        }
      }

      foreach (var stringNumber in numbersArray)
      {
        var lineToChomp = line;
        while (lineToChomp.ToLower().Contains(stringNumber.Key))
        {
          var index = lineToChomp.IndexOf(stringNumber.Key);
          dictionary.Add(new KeyValuePair<string, int>(stringNumber.Value, index));
          var modifiedNumber = stringNumber.Key.Replace(stringNumber.Key[0], 'z');
          lineToChomp = lineToChomp.Substring(0, index) + modifiedNumber + lineToChomp.Substring(index + stringNumber.Key.Length);
        }
      }
      var stringVersion = $"{dictionary.OrderBy(x => x.Value).First().Key}{dictionary.OrderByDescending(y => y.Value).First().Key}";
      return int.Parse(stringVersion);
    }

  }
}