using DodocoTales.SR.Gui.Views.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DodocoTales.SR.Gui
{
    public static class DDCV
    {
        public static readonly Dictionary<string, DDCVSwapableScreen> MainScreens = new Dictionary<string, DDCVSwapableScreen>();
        public static readonly Stack<DDCVSwapableScreen> StackedScreens = new Stack<DDCVSwapableScreen>();

        public static MainWindow MainWindow;
        public static Grid MainNavigater;


        public static void RegisterMainScreens()
        {
            RegisterMainScreen("Home", new DDCVHomeScreen());
            RegisterMainScreen("Version", new DDCVVersionViewScreen());
            RegisterMainScreen("Prediction", new DDCVGachaPredictionScreen());
        }


        
        public static void RegisterMainScreen(string tag, DDCVSwapableScreen screen)
        {
            MainScreens.Add(tag, screen);
            screen.Visibility = Visibility.Collapsed;
            MainNavigater.Children.Add(screen);
            if (StackedScreens.Count == 0)
            {
                StackedScreens.Push(screen);
                screen.Visibility = Visibility.Visible;
            }
        }


        public static void PushScreen(DDCVSwapableScreen screen)
        {
            //DDCLog.Info(DCLN.Gui, String.Format("Push screen: {0}", screen.GetType().Name));
            if (screen.GetType() == StackedScreens.Peek().GetType())
                return;

            StackedScreens.Peek().Visibility = Visibility.Collapsed;
            MainNavigater.Children.Add(screen);
            screen.Visibility = Visibility.Visible;
            StackedScreens.Push(screen);
        }

        public static bool PopScreen()
        {
            if (StackedScreens.Count == 1) return false;
            MainNavigater.Children.Remove(StackedScreens.Pop());
            StackedScreens.Peek().Visibility = Visibility.Visible;
            return true;
        }

        public static void SwapMainScreen(string tag)
        {
            //DDCLog.Info(DCLN.Gui, "Swap main page: " + tag);
            if (!MainScreens.TryGetValue(tag, out DDCVSwapableScreen ns))
            {
                return;
            }
            while (PopScreen()) ;
            var os = StackedScreens.Pop();
            if (ns != os)
            {
                os.Visibility = Visibility.Collapsed;
                ns.Visibility = Visibility.Visible;
            }
            StackedScreens.Push(ns);
        }

        public static void RefreshAll()
        {
            foreach (var scn in StackedScreens.Union(MainScreens.Values))
            {
                scn.Refresh();
            }
        }

        public static void CreateBannerViewScreen(ulong versionid, ulong bannerinternalid)
        {
            
            var scn = new DDCVBannerViewScreen();
            scn.SetBanner(versionid, bannerinternalid);
            PushScreen(scn);
            scn.Refresh();
            
        }
        
        public static bool ShowWindowDialog(Window window)
        {
            window.Owner = MainWindow;
            return window.ShowDialog() ?? false;
        }
    }
}
