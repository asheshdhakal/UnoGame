using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoGame
{
    using System;
    using System.IO;
    //Provides functionality to log player activities to a file.
    namespace PlayerActivityLogger
    {
        public static class ActivityLogger
        {

            //this file is situated in bin\Debug\player_activity.log
            private const string logFilePath = "player_activity.log";

            /// Logs the activity of a player.

            public static void LogPlayerActivity(string playerName, string action)
            {
                //Logs the activity of a player.
                try
                {
                    using (StreamWriter writer = File.AppendText(logFilePath))
                    {
                        writer.WriteLine($"{DateTime.Now}: {playerName} {action}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while logging player activity: {ex.Message}");
                }
            }

        }
    }

}
