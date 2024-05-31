using WrtChallengeTracker.IO;

namespace WrtChallengeTracker
{
    internal static class Program
    {
        static private frm_main mainWindows = new frm_main();
        static public frm_main MainWindows { get { return mainWindows; } }
        static private StreamWriter writer = new StreamWriter("./out.txt");
        
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            WebReader.Load();
            writer.Close();
           // Application.Run(mainWindows);




        }
        static public void AddText(string text)
        {
            writer.Write(text);
        }
    }
}