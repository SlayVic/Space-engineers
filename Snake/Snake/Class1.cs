using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
public sealed class Program : MyGridProgram
{
  // НАЧАЛО СКРИПТА
  const int wight = 24;
  const int height = 21;
  const int startLenght = 3;
  const int lengthPerApple = 2;

  const float fontSize = 0.51f;
  const string font = "Monospace";

  const string voidCell = "  ";
  const string snakeCell = "██";
  const string appleCell = "◄►";


  string[,] field = new string[height, wight];

  List<Dictionary<string, int>> snake = new List<Dictionary<string, int>>();
  string direction = "";
  Dictionary<string, int> apple = new Dictionary<string, int>();

  IMyTextSurface display;

  #region Standart and input
  public Program()
  {
    Runtime.UpdateFrequency = UpdateFrequency.Update10;

    generateSnake();
    generateField();
    display = Me.GetSurface(0);
    display.FontSize = fontSize;
    display.Font = font;
    display.WriteText(fieldToString());
  }

  public void Main(string args)
  {
    // Echo(args);
    rotate(args);
    move();
    generateApple();
    generateField();
    checkForInside();
    checkForApple();
    display.WriteText(fieldToString());
  }

  public void Save()
  { }
  #endregion

  #region Game

  private void generateSnake()
  {
    snake.Clear();
    direction = "";
    Dictionary<string, int> apple = new Dictionary<string, int>();
    for (int i = 0; i < startLenght; i++)
    {
      snake.Add(new Dictionary<string, int> {
        { "y", (int)height / 2 },
        { "x", (int)wight / 2 },
      });
    }
  }

  private void generateField()
  {
    for (int i = 0; i < height; i++)
    {
      for (int j = 0; j < wight; j++)
      {
        field[i, j] = voidCell;
      }
    }

    foreach (var item in snake)
    {
      field[item["y"], item["x"]] = snakeCell;
    }

    try
    {
      field[apple["y"], apple["x"]] = appleCell;
    }
    catch { }

  }

  private void generateApple()
  {
    Boolean flag = true;
    var r = new Random();
    Boolean appleFlag = false;
    try
    {
      int x = apple["x"];
      appleFlag = true;
    }
    catch (System.Exception)
    { }

    while (!appleFlag && direction != "" && flag)
    {
      flag = false;
      apple["x"] = r.Next(wight);
      apple["y"] = r.Next(height);
      foreach (var item in snake)
      {
        if (apple["x"] == item["x"] && apple["y"] == item["y"])
        {
          flag = true;
          break;
        }
      }
    }
  }

  private void rotate(string route)
  {
    switch (route)
    {
      case "Left":
        switch (direction)
        {
          case "Left":
            direction = "Down";
            break;
          case "Up":
            direction = "Left";
            break;
          case "Right":
            direction = "Up";
            break;
          case "Down":
            direction = "Right";
            break;
          default:
            direction = "Left";
            break;
        }
        break;
      case "Right":
        switch (direction)
        {
          case "Left":
            direction = "Up";
            break;
          case "Up":
            direction = "Right";
            break;
          case "Right":
            direction = "Down";
            break;
          case "Down":
            direction = "Left";
            break;
          default:
            direction = "Right";
            break;
        }
        break;
      default:
        break;
    }
  }

  private void move()
  {
    Dictionary<string, int> oldHead = snake[snake.Count - 1];
    // field[snake[0]["y"], snake[0]["x"]] = voidCell;
    snake.RemoveAt(0);
    int newX = oldHead["x"];
    int newY = oldHead["y"];
    switch (direction)
    {
      case "Left":
        newY = oldHead["y"];
        newX = oldHead["x"] - 1;
        break;
      case "Up":
        newY = oldHead["y"] - 1;
        newX = oldHead["x"];
        break;
      case "Right":
        newY = oldHead["y"];
        newX = oldHead["x"] + 1;
        break;
      case "Down":
        newY = oldHead["y"] + 1;
        newX = oldHead["x"];
        break;
      default:
        break;
    }
    snake.Add(new Dictionary<string, int>{
        {"y", newY},
        {"x", newX},
    });

    Dictionary<string, int> head = snake[snake.Count - 1];

    if (head["x"] >= wight)
    {
      head["x"] = 0;
    }
    if (head["x"] < 0)
    {
      head["x"] = wight - 1;
    }
    if (head["y"] >= height)
    {
      head["y"] = 0;
    }
    if (head["y"] < 0)
    {
      head["y"] = height - 1;
    }

  }

  private void checkForInside()
  {
    Dictionary<string, int> head = snake[snake.Count - 1];
    for (int i = 0; i < snake.Count - 1; i++)
    {
      var item = snake[i];
      if (item["x"] == head["x"] && item["y"] == head["y"])
      {
        generateSnake();
      }
    }
  }

  private void checkForApple()
  {
    try
    {
      Dictionary<string, int> head = snake[snake.Count - 1];
      if (apple["x"] == head["x"] && apple["y"] == head["y"])
      {
        apple = new Dictionary<string, int>();
        for (int i = 0; i < lengthPerApple; i++)
        {
          snake.Add(new Dictionary<string, int> {
        { "y", head["y"]},
        { "x", head["x"]},
      });
        }
      }
    }
    catch { }
  }

  private string fieldToString()
  {
    string fieldStr = "";

    for (int i = 0; i < height; i++)
    {
      for (int j = 0; j < wight; j++)
      {
        fieldStr += field[i, j];
      }
      fieldStr += "\n";
    }

    return fieldStr;
  }

  #endregion
  // КОНЕЦ СКРИПТА
}
