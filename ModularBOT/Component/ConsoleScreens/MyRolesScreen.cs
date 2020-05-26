﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModularBOT.Entity;
using System.Reflection;
using System.Threading;
using Discord.WebSocket;
using System.Windows;

namespace ModularBOT.Component.ConsoleScreens
{
    public class MyRolesScreen : ConsoleScreen
    {
        #region --- DECLARE ---
        private short page = 1;
        private readonly short max = 1;
        private int index = 0;
        private int selectionIndex = 0;
        private int countOnPage = 0;
        private int ppg = 0;
        private readonly DiscordNET DNet;
        private List<SocketRole> MyRoles = new List<SocketRole>();
        private readonly SocketGuild currentguild;

        #endregion

        public MyRolesScreen(SocketGuild Guild, DiscordNET discord, short startpage=1)
        {
            DNet = discord;
            currentguild = Guild;
            MyRoles = Guild.CurrentUser.Roles.ToList();
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

            Title = $"My Roles in {GetSafeName(Guild)} | ModularBOT v{Assembly.GetExecutingAssembly().GetName().Version}";
            RefreshMeta();
            ShowProgressBar = true;
            ShowMeta = true;

            ProgressVal = page;
            ProgressMax = max;
            BufferHeight = 34;
            WindowHeight = 32;
        }

        #region Screen Methods

        public override bool ProcessInput(ConsoleKeyInfo keyinfo)
        {
            Console.CursorVisible = false;

            if(ActivePrompt) return false;

            if (keyinfo.Key == ConsoleKey.P || keyinfo.Key == ConsoleKey.LeftArrow)
            {
                if (page > 1) page--;

                selectionIndex = 0;
                index = (page * 22) - 22;//0 page 1 = 0; page 2 = 22; etc.

                if (ppg != page)
                {
                    ProgressVal = page;
                    RefreshMeta();
                    UpdateProgressBar();
                    ClearContents();
                    countOnPage = PopulateList(page, max, ref index, selectionIndex, ref ppg, ref MyRoles);
                }
                    
                ppg = page;
            }

            if (keyinfo.Key == ConsoleKey.N || keyinfo.Key == ConsoleKey.RightArrow)
            {
                if (page < max) page++;

                selectionIndex = 0;
                index = (page * 22) - 22;//0 page 1 = 0; page 2 = 22; etc.

                if (ppg != page)
                {
                    ProgressVal = page;
                    RefreshMeta();
                    UpdateProgressBar();
                    ClearContents();
                    countOnPage = PopulateList(page, max, ref index, selectionIndex, ref ppg, ref MyRoles);
                }

                ppg = page;
            }

            if (keyinfo.Key == ConsoleKey.UpArrow)
            {
                selectionIndex--;

                if (selectionIndex < 0) selectionIndex = countOnPage - 1;

                index = (page * 22) - 22;//0 page 1 = 0; page 2 = 22; etc.
                countOnPage = PopulateList(page, max, ref index, selectionIndex, ref ppg, ref MyRoles);
            }

            if (keyinfo.Key == ConsoleKey.DownArrow)
            {
                selectionIndex++;

                if (selectionIndex > countOnPage - 1) selectionIndex = 0;

                index = (page * 22) - 22;//0 page 1 = 0; page 2 = 22; etc.
                countOnPage = PopulateList(page, max, ref index, selectionIndex, ref ppg, ref MyRoles);
            }

            if (keyinfo.Key == ConsoleKey.Enter)
            {
                index = (page * 22) - 22;//0 page 1 = 0; page 2 = 22; etc.
                
                string RoleName = GetSafeName(MyRoles, index + selectionIndex);

                ConsoleColor PRVBG = Console.BackgroundColor;
                ConsoleColor PRVFG = Console.ForegroundColor;

                #region -------------- [Role Properties Sub-Screen] --------------
                UpdateFooter(page, max, true);          //Prompt footer

                int rr = ShowOptionSubScreen($"Properties: {RoleName}", "What do you want to do?", 
                    "-", "Copy ID", "Edit AccessLevel...", "-");

                switch (rr)
                {
                    case (2):
                        index = (page * 22) - 22;//0 page 1 = 0; page 2 = 22; etc.
                        Thread thread = new Thread(() => Clipboard.SetText(MyRoles[selectionIndex + index].Id.ToString()));
                        thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                        thread.Start();
                        thread.Join(); //Wait for the thread to end
                                       //TODO: If success or fail, Display a message prompt
                        break;
                    case (3):
                        //SS_EditAccessLevel();

                        RefreshMeta();
                        RenderScreen();
                        break;
                    default:
                        break;
                }

                UpdateFooter(page, max);                //Restore footer 
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
            countOnPage = PopulateList(page, max, ref index, selectionIndex, ref ppg, ref MyRoles);
        }

        private void RefreshMeta()
        {
            Meta = $"{DNet.Client.CurrentUser.Username}#{DNet.Client.CurrentUser.Discriminator} " +
                $"has {MyRoles.Count} role(s) in {GetSafeName(currentguild)}";
            UpdateMeta();
        }

        private void UpdateFooter(short page, short max, bool prompt = false)
        {
            LayoutUpdating = true;

            if (page > 1 && page < max)
            {
                WriteFooter("[ESC] Exit \u2502 [N/RIGHT] Next Page \u2502 [P/LEFT] Previous Page \u2502 [UP/DOWN] Select \u2502 [ENTER] Properties...");
            }
            if (page == 1 && page < max)
            {
                WriteFooter("[ESC] Exit \u2502 [N/RIGHT] Next Page \u2502 [UP/DOWN] Select \u2502 [ENTER] Properties...");
            }
            if (page == 1 && page == max)
            {
                WriteFooter("[ESC] Exit \u2502 [UP/DOWN] Select \u2502 [ENTER] Properties...");
            }
            if (page > 1 && page == max)
            {
                WriteFooter("[ESC] Exit \u2502 [P/LEFT] Previous Page \u2502 [UP/DOWN] Select \u2502 [ENTER] Properties...");
            }
            if (prompt)
            {
                WriteFooter("[Prompt] Please follow on-prompt instruction");
            }

            LayoutUpdating = false;
        }

        private void WriteFooter(string footer)
        {
            LayoutUpdating = true;
            ScreenBackColor = ConsoleColor.Gray;
            ScreenFontColor = ConsoleColor.Black;
            int CT = Console.CursorTop;
            Console.CursorTop = 31;
            WriteEntry($"\u2502 {footer} \u2502".PadRight(141, '\u2005') + "\u2502", ConsoleColor.Gray, false, ConsoleColor.Gray);
            Console.CursorTop = 0;
            Console.CursorTop = CT;
            ScreenFontColor = ConsoleColor.Cyan;
            ScreenBackColor = ConsoleColor.Black;
            LayoutUpdating = false;
        }

        #endregion

        #region Role Properties Sub-Screen Methods
        //TODO: SS_EditAccessLevel();
        #endregion

        #region Role Listing Methods

        private int PopulateList(short page, short max, ref int index, int selectionIndex, ref int ppg, ref List<SocketRole> roles)
        {
            int countOnPage;

            if (ppg != page)//is page changing?
            {
                LayoutUpdating = true;
                _ = Console.CursorTop;
                Console.CursorTop = 2;
                UpdateProgressBar();
                Console.CursorTop = 2;
                ppg = page;

                LayoutUpdating = false;
                UpdateFooter(page, max);
            }

            countOnPage = 0;

            if (ppg == page)
            {
                Console.SetCursorPosition(0, ContentTop);
            }

            WriteEntry($"\u2502\u2005\u2005\u2005 - {"Role Name".PadRight(39, '\u2005')} {"Role ID".PadRight(22, '\u2005')} {"Is Admin".PadLeft(10, '\u2005')}", ConsoleColor.Blue,false);
            WriteEntry($"\u2502\u2005\u2005\u2005 \u2500 {"".PadRight(39, '\u2500')} {"".PadLeft(22, '\u2500')} {"".PadLeft(10, '\u2500')}", ConsoleColor.Blue,false);
            
            for (int i = index; i < 22 * page; i++)//22 results per page.
            {
                if (i >= roles.Count)
                {
                    break;
                }
                countOnPage++;
                WriteRole(selectionIndex, countOnPage, roles, i);
            }

            Console.SetCursorPosition(0, 0);
            return countOnPage;
        }

        private void WriteRole(int selectionIndex, int countOnPage, List<SocketRole> roles, int i)
        {
            string name = roles[i]?.Name ?? "[Pending...]";

            if (name.Length > 36)
            {
                name = name.Remove(36) + "...";
            }
            string o = Encoding.ASCII.GetString(Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback("?"), new DecoderExceptionFallback()), Encoding.Unicode.GetBytes(name))).Replace(' ', '\u2005').Replace("??", "?");
            string p = $"{o}".PadRight(39, '\u2005');
            string admin = roles[i].Permissions.Administrator ? "Yes".PadLeft(10, '\u2005') : "No".PadLeft(10, '\u2005');
            WriteEntry($"\u2502\u2005\u2005\u2005 - {p} [{roles.ElementAt(i).Id.ToString().PadLeft(20, '0')}] {admin}", (countOnPage - 1) == selectionIndex, ConsoleColor.DarkGreen, false);


        }

        private string GetSafeName(List<SocketRole> roles, int i)
        {
            string userinput = roles.ElementAt(i).Name ?? "[Pending...]";
            string o = Encoding.ASCII.GetString(Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback("?"), new DecoderExceptionFallback()), Encoding.Unicode.GetBytes(userinput))).Replace(' ', '\u2005').Replace("??", "?");
            
            if (o.Length > 17)
            {
                o = o.Remove(13) + "...";
            }
            
            string p = $"{o}";
            
            return p;
        }

        private string GetSafeName(SocketGuild guild)
        {
            string userinput = guild.Name ?? "[Pending...]";
            string o = Encoding.ASCII.GetString(Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback("?"), new DecoderExceptionFallback()), Encoding.Unicode.GetBytes(userinput))).Replace(' ', '\u2005').Replace("??", "?");

            if (o.Length > 17)
            {
                o = o.Remove(13) + "...";
            }

            string p = $"{o}";

            return p;
        }

        #endregion
    }
}
