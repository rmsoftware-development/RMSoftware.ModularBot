﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModularBOT.Entity;
using System.Reflection;
using System.Threading;
using Discord.WebSocket;
using System.Windows;
using Discord;

namespace ModularBOT.Component.ConsoleScreens
{
    public class GuildsScreen : ConsoleScreen
    {
        private List<SocketGuild> Guildlist = new List<SocketGuild>();
        short page = 1;
        short max = 1;
        int index = 0;
        int selectionIndex = 0;
        int countOnPage = 0;
        int ppg = 0;
        DiscordNET d;
        public GuildsScreen(List<SocketGuild> guilds, DiscordNET discord, short startpage=1)
        {
            d = discord;
            Guildlist = guilds;
            page = startpage;

            max = (short)(Math.Ceiling((double)(discord.Client.Guilds.Count / 22)) + 1);
            index = 0;
            selectionIndex = 0;
            countOnPage = 0;
            ppg = 0;

            ScreenFontColor = ConsoleColor.Cyan;
            ScreenBackColor = ConsoleColor.Black;
            TitlesBackColor = ConsoleColor.Black;
            TitlesFontColor = ConsoleColor.White;
            ProgressColor = ConsoleColor.Cyan;

            Title = $"Guilds | ModularBOT v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
            Meta = $"Present in {Guildlist.Count} guild(s). Connected to {Guildlist.Where(x=>x.IsConnected).LongCount()} guild(s)";
            ShowProgressBar = true;
            ShowMeta = true;

            ProgressVal = page;
            ProgressMax = max;
            BufferHeight = 34;
            WindowHeight = 32;

        }
        
        public override bool ProcessInput(ConsoleKeyInfo keyinfo)
        {
            //TODO: Custom input handling: NOTE -- Base adds exit handler [E] key.
            if(ActivePrompt) { return false; }
            if (keyinfo.Key == ConsoleKey.P)
            {
                
                
                if (page > 1)
                {
                    page--;
                    index = (page * 22) - 22;//0 page 1 = 0; page 2 = 20; etc.
                                             //continue;
                }
                selectionIndex = 0;
                index = (page * 22) - 22;//0 page 1 = 0; page 2 = 20; etc.
                if(ppg!=page) countOnPage = PopulateGuildList(d, page, max, ref index, selectionIndex, ref ppg, ref Guildlist);
                ppg = page;

            }
            if (keyinfo.Key == ConsoleKey.N)
            {
                
                if (page < max)
                {

                    page++;
                }
                selectionIndex = 0;
                index = (page * 22) - 22;//0 page 1 = 0; page 2 = 20; etc.
                if (ppg != page) countOnPage = PopulateGuildList(d, page, max, ref index, selectionIndex, ref ppg, ref Guildlist);
                ppg = page;

            }

            if (keyinfo.Key == ConsoleKey.UpArrow)
            {
                selectionIndex--;
                if (selectionIndex < 0)
                {
                    selectionIndex = countOnPage - 1;
                }
                

                index = (page * 22) - 22;//0 page 1 = 0; page 2 = 20; etc.
                countOnPage = PopulateGuildList(d, page, max, ref index, selectionIndex, ref ppg, ref Guildlist);
            }
            if (keyinfo.Key == ConsoleKey.DownArrow)
            {

               
                selectionIndex++;
                if (selectionIndex > countOnPage - 1)
                {
                    selectionIndex = 0;
                }

               
                index = (page * 22) - 22;//0 page 1 = 0; page 2 = 20; etc.
                countOnPage = PopulateGuildList(d, page, max, ref index, selectionIndex, ref ppg, ref Guildlist);
            }

            if (keyinfo.Key == ConsoleKey.Enter)
            {

                index = (page * 22) - 22;//0 page 1 = 0; page 2 = 22; etc.
                #region SubScreen
                string guildname = SafeName(Guildlist, index + selectionIndex);
                if (!(Guildlist[index + selectionIndex].IsConnected))
                {
                    return false;
                }
                //int left = 71-20;
                //int top = 16 - 7;
                //Console.CursorLeft = left;
                //Console.CursorTop = top;
                ConsoleColor PRVBG = Console.BackgroundColor;
                ConsoleColor PRVFG = Console.ForegroundColor;
                int rr = -1;
                rr = ShowOptionSubScreen($"Manage: {guildname}", "What do you want to do?", "View Users...", "View Channels...", "View Roles...", "More Actions...");

                switch (rr)
                {
                    case (1):
                        break;
                    case (2):
                        
                        break;
                    case (3):
                       
                        break;
                    case (4):
                        int ss = -1;

                        countOnPage = PopulateGuildList(d, page, max, ref index, selectionIndex, ref ppg, ref Guildlist);
                        index = (page * 22) - 22;//0 page 1 = 0; page 2 = 20; etc.
                        ss = ShowOptionSubScreen($"Manage: {guildname}", "What do you want to do?", "View My Roles...", "Copy ID...", "-", "Leave Guild...");
                        switch (ss)
                        {
                            case (1):
                               
                                break;
                            case (2):
                                index = (page * 22) - 22;//0 page 1 = 0; page 2 = 22; etc.
                                Thread thread = new Thread(() => Clipboard.SetText(Guildlist[selectionIndex + index].Id.ToString()));
                                thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                                thread.Start();
                                thread.Join(); //Wait for the thread to end
                                               //TODO: If success or fail, Display a message prompt
                                
                                break;
                            case (4):

                                
                                index = (page * 22) - 22;//0 page 1 = 0; page 2 = 22; etc.
                                int confirmprompt = ShowOptionSubScreen($"Leave {guildname}?", $"Are you sure you want to leave?", "-", "NO", "YES", "-", ConsoleColor.DarkRed);
                                if (confirmprompt == 3)
                                {
                                    index = (page * 22) - 22;
                                    Guildlist[selectionIndex + index].LeaveAsync();
                                    Guildlist.Remove(Guildlist[selectionIndex + index]);
                                    RenderScreen();//re-renderr
                                }
                                break;
                        }

                        break;
                    default:
                        break;

                }
                
                #endregion


                Console.ForegroundColor = PRVFG;
                Console.BackgroundColor = PRVBG;
            }
            
            return base.ProcessInput(keyinfo);
        }

        protected override void RenderContents()
        {
            SpinWait.SpinUntil(() => !LayoutUpdating);
            SpinWait.SpinUntil(() => !ActivePrompt);
            countOnPage = PopulateGuildList(d, page, max, ref index, selectionIndex, ref ppg, ref Guildlist);
        }

        private void WriteFooter(string footer)
        {
            ScreenBackColor = ConsoleColor.Gray;
            ScreenFontColor = ConsoleColor.Black;
            int CT = Console.CursorTop;
            Console.CursorTop = 31;
            WriteEntry($"\u2502 {footer} \u2502".PadRight(141, '\u2005') + "\u2502", ConsoleColor.DarkGray, false, ConsoleColor.Gray);
            Console.CursorTop = 0;
            Console.CursorTop = CT;
            ScreenFontColor = ConsoleColor.Cyan;
            ScreenBackColor = ConsoleColor.Black;
        }

        private int PopulateGuildList(DiscordNET discord, short page, short max, ref int index, int selectionIndex, ref int ppg, ref List<SocketGuild> guilds)
        {
            int countOnPage;
            if (ppg != page)//is page changing?
            {
                LayoutUpdating = true;
                int ct = Console.CursorTop;
                Console.CursorTop = 2;
                UpdateProgressBar(2);
                Console.CursorTop = 2;
                ppg = page;

                LayoutUpdating = false;

            }

            countOnPage = 0;
            if (ppg == page)
            {
                Console.SetCursorPosition(0, ContentTop);
            }
            WriteEntry($"\u2502\u2005\u2005\u2005 - {"Guild Name".PadRight(39, '\u2005')} {"Guild ID".PadRight(22, '\u2005')}", ConsoleColor.Blue,false);
            WriteEntry($"\u2502\u2005\u2005\u2005 \u2500 {"".PadRight(39, '\u2500')} {"".PadLeft(22, '\u2500')}", ConsoleColor.Blue,false);
            for (int i = index; i < 22 * page; i++)//22 results per page.
            {
                if (index >= guilds.Count)
                {
                    break;
                }
                countOnPage++;
                WriteGuild(discord, selectionIndex, countOnPage, guilds, i);

                index++;
            }

            LayoutUpdating = true;
            if (page > 1 && page < max)
            {
                
                WriteFooter("[ESC] Exit \u2502 [N] Next Page \u2502 [P] Previous Page \u2502 [UP/DOWN] Select \u2502 [ENTER] Properties...");
                
            }
            if (page == 1 && page < max)
            {
                WriteFooter("[ESC] Exit \u2502 [N] Next Page \u2502 [UP/DOWN] Select \u2502 [ENTER] Properties...");
            }
            if (page == 1 && page == max)
            {
                WriteFooter("[ESC] Exit \u2502 [UP/DOWN] Select \u2502 [ENTER] Properties...");
            }
            if (page > 1 && page == max)
            {
                WriteFooter("[ESC] Exit \u2502 [P] Previous Page \u2502 [UP/DOWN] Select \u2502 [ENTER] Properties...");
            }
            LayoutUpdating = false;
            return countOnPage;
        }
        private void WriteGuild(DiscordNET discord, int selectionIndex, int countOnPage, List<SocketGuild> guilds, int i)
        {
            if (guilds[i].IsConnected)
            {
                string name = guilds[i]?.Name ?? "[Pending...]";

                if (name.Length > 36)
                {
                    name = name.Remove(36) + "...";
                }
                string o = Encoding.ASCII.GetString(Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback("?"), new DecoderExceptionFallback()), Encoding.Unicode.GetBytes(name))).Replace(' ', '\u2005').Replace("??", "?");
                string p = $"{name}".PadRight(39, '\u2005');
                WriteEntry($"\u2502\u2005\u2005 - {p} [{guilds.ElementAt(i).Id.ToString().PadLeft(20, '0')}]", (countOnPage - 1) == selectionIndex, ConsoleColor.DarkGreen,false);

            }
            else
            {
                string name = "[Guild Unavailable]";

                if (name.Length > 36)
                {
                    name = name.Remove(36) + "...";
                }
                string o = Encoding.ASCII.GetString(Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback("?"), new DecoderExceptionFallback()), Encoding.Unicode.GetBytes(name))).Replace(' ', '\u2005').Replace("??", "?");
                string p = $"{name}".PadRight(39, '\u2005');
                WriteEntry($"\u2502\u2005\u2005 - {p} [{guilds.ElementAt(i).Id.ToString().PadLeft(20, '0')}]", (countOnPage - 1) == selectionIndex, true, ConsoleColor.DarkGreen);

            }
        }

        private string SafeName(List<SocketGuild> guilds, int i)
        {
            string userinput = guilds.ElementAt(i).Name ?? "[Pending...]";
            string o = Encoding.ASCII.GetString(Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback("?"), new DecoderExceptionFallback()), Encoding.Unicode.GetBytes(userinput))).Replace(' ', '\u2005').Replace("??", "?");
            if (o.Length > 17)
            {
                o = o.Remove(13) + "...";
            }
            string p = $"{o}";
            return p;
        }
    }
}