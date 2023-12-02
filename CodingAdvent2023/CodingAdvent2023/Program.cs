using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodingAdvent2023
{
  public class Program
  {
    public static void Main()
    {
      var input = GetInput();

      var sum = 0;
      foreach (var line in input)
      {

        sum += GetMinimumGamePowers(line);
      }
      Console.WriteLine($"The sum was {sum}");
    }

    public static string[] GetInput()
    {
      var assembly = Assembly.GetExecutingAssembly();
      var resourceName = "CodingAdvent2023.Resources.input2.txt";

      using (Stream stream = assembly.GetManifestResourceStream(resourceName))
      using (StreamReader reader = new StreamReader(stream))
      {
        string result = reader.ReadToEnd();
        return result.Split('\n');
      }

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