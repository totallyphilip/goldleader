using System;

public class Textify
{

    #region " Properties "

    public static int LeftEdge
    {
        get { return 0; }
    }

    public static int RightEdge
    {
        get { return Console.WindowWidth; }
    }

    public static int BottomEdge
    {
        get { return Console.WindowHeight - 1; }
    }

    public static int Height
    {
        get { return Console.WindowHeight; }
    }

    public static bool PastEdge(int X, int Offset, string Ascii)
    {
        return X + Offset < Textify.LeftEdge || X + Offset > Textify.RightEdge - Ascii.Length;
    }

    public static bool PastEdge(int X, string Ascii)
    {
        return PastEdge(X, 0, Ascii);
    }

    #endregion

    #region " Screen Manipulation "

    public static bool SetWindowSizeSafely(int setwidth, int setheight)
    {
        return SetWindowSizeSafely(setwidth, setheight, true);
    }

    public static bool SetWindowSizeSafely(int setwidth, int setheight, bool manualadjust)
    {

        try
        {
            Console.SetWindowSize(setwidth, setheight);
        }
        catch
        {

            while ((Console.WindowWidth != setwidth || Console.WindowHeight != setheight) && (!Console.KeyAvailable) && manualadjust)
            {

                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("can't resize window.");
                Console.WriteLine("please adjust manually.");
                Console.WriteLine();

                Console.Write("adjust width: ");
                if (Console.WindowWidth > setwidth) { Console.WriteLine(StringRepeat(Console.WindowWidth - setwidth, '\x2190')); }
                else if (Console.WindowWidth < setwidth) { Console.WriteLine(StringRepeat(setwidth - Console.WindowWidth, '\x2192')); }
                else { Console.WriteLine("Perfect!"); }

                Console.Write("adjust height: ");
                if (Console.WindowHeight > setheight) { Console.Write(StringRepeat(Console.WindowHeight - setheight, '\x2191')); }
                else if (Console.WindowHeight < setheight) { Console.Write(StringRepeat(setheight - Console.WindowHeight, '\x2193')); }
                else { Console.Write("Perfect!"); }

                System.Threading.Thread.Sleep(100); // good enough so the CPU doesn't go crazy

            }

        }

        Console.SetBufferSize(setwidth, setheight);

        return (Console.WindowHeight == setheight && Console.WindowWidth == setwidth);

    }

    #endregion
    public static string StringRepeat(int n, char c)
    {
        return new String(c, n);
    }

    public static void WaitPrompt(string s)
    {
        Console.Write(s);
        Console.ReadKey(true);
    }

}
