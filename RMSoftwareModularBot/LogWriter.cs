
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RMSoftware.ModularBot
{
    public class ConsoleLogWriter
    {
        public bool Busy { get; private set; }

        public ConsoleLogWriter()
        {
            Busy = false;
        }

        //Slightly tweaked from: https://stackoverflow.com/questions/20534318/make-console-writeline-wrap-words-instead-of-letters
        //TODO: This is not perfect. I would like to improve.
        public static string WordWrap(string paragraph)
        {
            paragraph = new Regex(@" {2,}").Replace(paragraph.Trim(), @" ");
            var left = Console.CursorLeft; var top = Console.CursorTop; var lines = new List<string>();
            string returnstring = "";
            for (var i = 0; paragraph.Length > 0; i++)
            {
                lines.Add(paragraph.Substring(0, Math.Min(Console.WindowWidth-1, paragraph.Length)));
                var length = lines[i].LastIndexOf(" ");
                if (length > 0 && paragraph.Length > Console.WindowWidth-1) lines[i] = lines[i].Remove(length);
                paragraph = paragraph.Substring(Math.Min(lines[i].Length + 1, paragraph.Length));
                returnstring += (lines[i].Trim())+"\n";
            }
            return returnstring;
        }

        /// <summary>
        /// Write a color cordinated log message to console. Function is intended for full mode. Not '-log_only'.
        /// </summary>
        /// <param name="message">The Discord.NET Log message</param>
        /// <param name="Entrycolor">An optional entry color. If none (or black), the message.LogSeverity is used for color instead.</param>
        public void WriteEntry(LogMessage message,ConsoleColor Entrycolor=ConsoleColor.Black)
        {
            if (Busy)
            {
                SpinWait.SpinUntil(() => !Busy);//This will help prevent the console from being sent into a mess of garbled words.
            }
            Busy = true;
            Console.SetCursorPosition(0, Console.CursorTop);//Reset line position.

            string[] lines = WordWrap(message.ToString()).Split('\n');
            ConsoleColor bglast = Program.ConsoleBackgroundColor;



            
            for (int i = 0; i < lines.Length; i++)
            {

                if (lines[i].Length == 0)
                {
                    continue;
                }
                ConsoleColor bg = ConsoleColor.Black;
                ConsoleColor fg = ConsoleColor.Black;
                #region setup entry color.
                if (Entrycolor == ConsoleColor.Black)
                {
                    switch (message.Severity)
                    {
                        case LogSeverity.Critical:
                            bg = ConsoleColor.DarkRed;
                            fg = ConsoleColor.DarkRed;
                            break;
                        case LogSeverity.Error:
                            fg = ConsoleColor.Red;
                            bg = ConsoleColor.Red;
                            break;
                        case LogSeverity.Warning:
                            fg = ConsoleColor.Yellow;
                            bg = ConsoleColor.Yellow;
                            break;
                        case LogSeverity.Info:
                            fg = ConsoleColor.Black;
                            bg = ConsoleColor.Black;
                            break;
                        case LogSeverity.Verbose:
                            fg = ConsoleColor.DarkGray;
                            bg = ConsoleColor.DarkGray;
                            break;
                        case LogSeverity.Debug:
                            fg = ConsoleColor.DarkMagenta;
                            bg = ConsoleColor.DarkMagenta;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    bg = Entrycolor;
                    fg = Entrycolor;
                }
                #endregion

                Console.BackgroundColor = bg;
                Console.ForegroundColor = fg;
                Console.Write((char)9617);//Write the colored space.
                Console.BackgroundColor = bglast;//restore previous color.
                Console.ForegroundColor = Program.ConsoleForegroundColor;
                Thread.Sleep(1);//safe.
                Console.WriteLine(lines[i]);//write current line in queue.
                
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(">");//Write the input indicator.
            Thread.Sleep(1);//safe.
            Console.BackgroundColor = Program.ConsoleBackgroundColor;
            Console.ForegroundColor = Program.ConsoleForegroundColor;
            Busy = false;
        }
    }
}