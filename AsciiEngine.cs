using System;

public class AsciiEngine
{

    #region " Static Properties "

    public static int TopEdge { get { return 0; } }

    public static int BottomEdge { get { return Console.WindowHeight - 1; } }

    public static int LeftEdge { get { return 0; } }

    public static int RightEdge { get { return Console.WindowWidth - 1; } }

    public static int Width { get { return Console.WindowWidth; } }

    public static int Height { get { return Console.WindowHeight; } }

    #endregion

    #region " Screen Manipulation "

    public static bool TrySetWindowSize(int setwidth, int setheight)
    {
        return TrySetWindowSize(setwidth, setheight, true);
    }

    public static bool TrySetWindowSize(int setwidth, int setheight, bool manualadjust)
    {

        try
        {
            Console.SetWindowSize(setwidth, setheight);
            Console.SetBufferSize(setwidth + 100, setheight + 100);
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
                if (Console.WindowWidth > setwidth) { Console.WriteLine(new String('\x2190', Console.WindowWidth - setwidth)); }
                else if (Console.WindowWidth < setwidth) { Console.WriteLine(new String('\x2192', setwidth - Console.WindowWidth)); }
                else { Console.WriteLine("Perfect!"); }

                Console.Write("adjust height: ");
                if (Console.WindowHeight > setheight) { Console.Write(new String('\x2191', Console.WindowHeight - setheight)); }
                else if (Console.WindowHeight < setheight) { Console.Write(new String('\x2193', setheight - Console.WindowHeight)); }
                else { Console.Write("Perfect!"); }

                System.Threading.Thread.Sleep(100); // good enough so the CPU doesn't go crazy

            }

        }

        return (Console.WindowHeight == setheight && Console.WindowWidth == setwidth);

    }

    #endregion

    public static char CharPrompt(string s)
    {
        Console.Write(s);
        return Console.ReadKey(true).KeyChar;
    }

    public static bool TryWrite(int x, int y, string s)
    {
        char[] chars = s.ToCharArray();
        for (int c = 0; c < chars.Length; c++) { AsciiEngine.TryWrite(x + c, y, chars[c]); }
        return (x >= AsciiEngine.LeftEdge && x + s.Length - 1 <= AsciiEngine.RightEdge && y >= AsciiEngine.TopEdge && y <= AsciiEngine.BottomEdge);
    }

    public static bool TryWrite(int x, int y, char c)
    {
        // don't write anything past the screen edges, nor in lower right corner
        if (x >= AsciiEngine.LeftEdge && x <= AsciiEngine.RightEdge && y >= AsciiEngine.TopEdge && y <= AsciiEngine.BottomEdge && !(x == AsciiEngine.RightEdge && y == AsciiEngine.BottomEdge))
        {
            Console.SetCursorPosition(x, y);
            Console.Write(c);
            return true;
        }
        else { return false; }
    }

}
