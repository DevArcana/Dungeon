using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using World.Generation;
using Debug = UnityEngine.Debug;

namespace Tests.EditMode
{
  public class DungeonBenchmarker
  {
    [Test]
    public void Benchmark()
    {
      var settings = ScriptableObject.CreateInstance<MapGenerationSettings>();
      settings.width = 32;
      settings.height = 32;
      settings.layers = 5;
      settings.seed = 0;
      settings.iterations = 0;
      settings.fillPercent = 10;
      settings.minRegionSize = 0;
      settings.maxRegionSize = 100;
      settings.r1CellsToDie = 2;
      settings.r1CellsToLive = 5;
      settings.rnCellsToLive = 0;
      settings.rn = 1;

      var generator = new MapGenerator(settings);
      for (var iterations = 0; iterations <= 5; iterations++)
      {
        settings.iterations = iterations;

        for (var fill = 0; fill <= 100; fill+=10)
        {
          settings.fillPercent = fill;
          var open = 0;
          var filled = 0;
          var features = 0;
      
          for (var i = 0; i < 100; i++)
          {
            var map = generator.Generate();
            var heightmap = map.heightmap;
            for (var y = 0; y < settings.height; y++)
            {
              for (var x = 0; x < settings.width; x++)
              {
                var h = heightmap[x, y];
                if (h == settings.layers)
                {
                  filled++;
                }
                else if (h > 0)
                {
                  features++;
                }
                else
                {
                  open++;
                }
              }
            }
          }

          var sum = settings.width * settings.height * 1.0f;
          Debug.Log($"{open / sum}, {filled / sum}, {features / sum}, {fill}, {iterations}");
        }
      }
    }
    
    [Test]
    public void BenchmarkSingle()
    {
      var settings = ScriptableObject.CreateInstance<MapGenerationSettings>();
      settings.width = 48;
      settings.height = 32;
      settings.layers = 5;
      settings.seed = 0;
      settings.iterations = 5;
      settings.fillPercent = 35;
      settings.minRegionSize = 70;
      settings.maxRegionSize = 150;
      settings.r1CellsToDie = 2;
      settings.r1CellsToLive = 5;
      settings.rnCellsToLive = 3;
      settings.rn = 3;

      var generator = new MapGenerator(settings);
      
      var open = 0;
      var filled = 0;
      var features = 0;

      for (var i = 0; i < 100; i++)
      {
        var map = generator.Generate();
        var heightmap = map.heightmap;
        for (var y = 0; y < settings.height; y++)
        {
          for (var x = 0; x < settings.width; x++)
          {
            var h = heightmap[x, y];
            if (h == settings.layers)
            {
              filled++;
            }
            else if (h > 0)
            {
              features++;
            }
            else
            {
              open++;
            }
          }
        }
      }

      var sum = settings.width * settings.height * 1.0f;
      Debug.Log($"{open / sum}, {filled / sum}, {features / sum}, {settings.fillPercent}, {settings.iterations}");
    }
    
    [Test]
    public void BenchmarkSingleTime()
    {
      var settings = ScriptableObject.CreateInstance<MapGenerationSettings>();
      settings.width = 48;
      settings.height = 32;
      settings.layers = 5;
      settings.seed = 0;
      settings.iterations = 5;
      settings.fillPercent = 35;
      settings.minRegionSize = 70;
      settings.maxRegionSize = 150;
      settings.r1CellsToDie = 2;
      settings.r1CellsToLive = 5;
      settings.rnCellsToLive = 3;
      settings.rn = 3;

      var generator = new MapGenerator(settings);

      for (var size = 16; size < 128; size++)
      {
        settings.width = size;
        settings.height = size;
        
        var elapsed = new List<double>(100);
        for (var i = 0; i < 10; i++)
        {
          var stopwatch = Stopwatch.StartNew();
          generator.Generate();
          elapsed.Add(stopwatch.Elapsed.TotalSeconds);
        }

        var avg = elapsed.Average();

        Debug.Log($"{size},{avg}");
      }
    }
  }
}