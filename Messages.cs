using AsciiEngine;
using AsciiEngine.Sprites;
using AsciiEngine.Coordinates;
using System.Collections.Generic;

public class MessageScroller : Swarm
{

    public void AddMessage() { AddMessage(""); }

    public void AddMessage(string s)
    {
        int y = Screen.Height;
        foreach (Sprite message in Items)
        {
            if (message.XY.iY >= y) { y = message.XY.iY + 2; }
        }
        this.Items.Add(new Sprite(s.ToCharArray(), new Point(Screen.Width / 2 - s.Length / 2, y), new Trajectory(-.5, 0, y - Screen.Height / 2)));
    }

}