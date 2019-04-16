using System.Collections.Generic;

internal class Messages
{

    public static string[] HelpText()
    {
        return new[] {
            "GAME PAUSED"
            ,"Press Enter to resume."
            ,""
            ,"<< Ship controls >>"
            ,"Blasters        - Space           "
            ,"Torpedo         - Tab             "
            ,"Detonate Shield - Ctrl-Space      "
            ,"Move            - Left/Right Arrow"
            ,"Hyperdrive      - Up Arrow        "
            ,"Toggle S-foils  - Down Arrow      "
            ,""
            ,"<< System controls >>"
            ,"Pause  - Enter    "
            ,"Quit   - Escape   "
            ,"Faster - Page Up  "
            ,"Slower - Page Down"
            ,""
            ,"Lock S-foils in attack position to"
            ,"divert power from thrusters to blasters."
            ,""
            ,"Defeat all enemies for navicomputer bonus."
            ,""
            ,"Each power up is worth " + PowerUp.Step + " more points."
            ,""
            ,"Hit = 1 point X altitude factor"
            ,"Kill = 2 points X altitude factor"
        };

    }

    public static string[] DeadHeroText(int points)
    {
        return new[] {
            ""
            ,""
            ,""
            ,"Wave cleared!"
            ,""
            ,"That"
            ,"was"
            ,"totally"
            ,"awesome."
            ,""
            ,""
            ,""
            ,"+" + points + " Dead Hero bonus."
        };

    }

    public static string[] GameOverText()
    {
        return new[] {
            "G A M E   O V E R"
            ,""
            ,""
            ,""
            ,"They came from behind!"
        };

    }

    public static string[] CropDustingText()
    {
        return new[] {
            "Traveling through hyperspace"
            ,"ain't like dusting crops, boy."
        };

    }

    public static string[] HyperdriveFailText()
    {
        return new[] {
            "It's not my fault!"
            ,"They told me they fixed it!"
        };

    }
    public static string[] BeginText()
    {
        return new[] {
            "Instructions - Enter"
            ,"Quit         - Escape"
            ,""
        };

    }


    public static List<string> DemoText()
    {

        List<string> s = new List<string>();

        s.Add(Easy.Textify.Fluffer(AsciiEngine.Application.Title, " "));
        s.Add("");
        foreach (Enemy.eEnemyType shiptype in (Enemy.eEnemyType[])System.Enum.GetValues(typeof(Enemy.eEnemyType)))
        {
            Enemy bg = new Enemy(shiptype, true);
            s.Add(new string(bg.Text) + " " + System.Enum.GetName(typeof(Enemy.eEnemyType), shiptype) + " (" + bg.HitPoints + " HP)");
        }
        s.Add("");
        s.Add("Press Esc to Quit");
        s.Add("Press Space to Begin");
        s.Add("");
        s.Add("");
        s.Add("");
        s.Add("(c) 2018 " + AsciiEngine.Application.Company);
        s.Add("");

        return s;

    }




}