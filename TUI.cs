using System;

public class TUI {

#region " Properties "

    public static int LeftEdge {
        get {return 0;}
    }

    public static int RightEdge {
        get {return Console.WindowWidth;}
    }        

    public static int BottomEdge {
        get {return Console.WindowHeight-1;}
    }

    public static bool PastEdge(int X, int Offset, string Ascii) {
        return X+Offset < TUI.LeftEdge || X+Offset > TUI.RightEdge-Ascii.Length;
    }

    public static bool PastEdge(int X, string Ascii) {
        return PastEdge(X,0,Ascii);
    }

#endregion

#region " Screen Manipulation "

    public static void FailSafeSetWindowSize(int cols, int rows) {

        try {
            Console.SetWindowSize(rows,cols);
        }
        catch {

           while ((Console.WindowWidth != cols || Console.WindowHeight != rows) && (! Console.KeyAvailable)) {

                Console.Clear();
                Console.SetCursorPosition(0,0);
                Console.WriteLine("can't resize window.");
                Console.WriteLine("please adjust manually.");
                Console.WriteLine();

                Console.Write("adjust width: ");
                if (Console.WindowWidth > cols) { Console.WriteLine(StringRepeat(Console.WindowWidth - cols, '\x2190')); }
                else if (Console.WindowWidth < cols) { Console.WriteLine(StringRepeat(cols - Console.WindowWidth, '\x2192')); }
                else { Console.WriteLine("Perfect!"); }

                Console.Write("adjust height: ");
                if (Console.WindowHeight > rows) { Console.Write(StringRepeat(Console.WindowHeight - rows, '\x2191')); }
                else if (Console.WindowHeight < rows) { Console.Write(StringRepeat(rows - Console.WindowHeight, '\x2193')); }
                else { Console.Write("Perfect!"); }

                System.Threading.Thread.Sleep(100); // good enough so the CPU doesn't go crazy
                
            }

        }

    }

#endregion
    public static string StringRepeat(int n, char c) {
        return new String(c, n);
    }

}
