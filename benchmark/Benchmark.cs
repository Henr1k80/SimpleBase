﻿using System;
using System.Diagnostics;

namespace benchmark;

internal class Benchmark
{
    public const int Iterations = 5_000_000;

    // buffer size for average use case scenarios -- needs to be legit Base64 length w/o padding
    public const int EncodeSize = 64;
    public const int DecodeSize = 80;

    public string Name { get; private set; }
    public float Growth { get; private set; }
    public TimeSpan EncodeTime;
    public TimeSpan DecodeTime;
    public Action<byte[]> EncodeFunc { get; private set; }
    public Action<string> DecodeFunc { get; private set; }

    public Benchmark(string name, float growth, Action<byte[]> encodeFunc, Action<string> decodeFunc)
    {
        this.Name = name;
        this.Growth = growth;
        this.EncodeFunc = encodeFunc;
        this.DecodeFunc = decodeFunc;
    }

    private static string compare(TimeSpan bench, TimeSpan baseline)
    {
        bool faster = bench < baseline;
        double factor = faster ? baseline.TotalMilliseconds / bench.TotalMilliseconds
            : bench.TotalMilliseconds / baseline.TotalMilliseconds;
        string formattedFactor = $"{factor:0.#}";
        if (formattedFactor == "1")
        {
            return "about the same";
        }
        string mood = faster ? "faster! YAY!" : "slower";
        return $"{formattedFactor}x {mood}";
    }

    public string GetEncodeText(Benchmark baseline)
    {
        return getPrintable(EncodeTime, baseline.EncodeTime);
    }

    public string GetDecodeText(Benchmark baseline)
    {
        return getPrintable(DecodeTime, baseline.DecodeTime);
    }

    private static string getPrintable(TimeSpan time, TimeSpan baseline)
    {
        string result = $"{time.TotalMilliseconds / 1000.0:F2}μs";
        return time == baseline ? result : $"{result} ({compare(time, baseline)})";
    }

    public void TestEncode()
    {
        byte[] buf = new byte[EncodeSize];
        buf[0] = 1;
        buf[EncodeSize - 1] = 1; // avoid all-zero optimizations of Base58

        EncodeFunc(buf); // warmup
        var w = Stopwatch.StartNew();
        for (int n = 0; n < Iterations; n++)
        {
            EncodeFunc(buf);
        }
        w.Stop();
        this.EncodeTime = w.Elapsed;
    }

    public void TestDecode()
    {
        string str = new('a', DecodeSize);

        DecodeFunc(str); // warmup
        var w = Stopwatch.StartNew();
        for (int n = 0; n < Iterations; n++)
        {
            DecodeFunc(str);
        }
        w.Stop();
        this.DecodeTime = w.Elapsed;
    }
}
