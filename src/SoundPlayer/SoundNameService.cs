using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;


namespace Sound
{
    public class SoundNameService : ISoundNameService
    {
        public static List<string> TicketsFolder = null;
        public static List<string> CashiersFolder = null;




        #region ctor

        public SoundNameService()
        {
            try
            {
                var dir = new DirectoryInfo(Environment.CurrentDirectory + @"\Wav\Sounds\");
                TicketsFolder = new List<string>();
                foreach (FileInfo file in dir.GetFiles("*.wav"))
                    TicketsFolder.Add(Path.GetFileNameWithoutExtension(file.FullName));

                dir = new DirectoryInfo(Environment.CurrentDirectory + @"\Wav\Number of trains\");
                CashiersFolder = new List<string>();
                foreach (FileInfo file in dir.GetFiles("*.wav"))
                    CashiersFolder.Add(Path.GetFileNameWithoutExtension(file.FullName));

            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        #endregion





        #region Methode

        public string GetFileName(string track)
        {
            string path = Environment.CurrentDirectory + @"\";    

            if (TicketsFolder != null && TicketsFolder.Contains(track))
                return path + @"Wav\Sounds\" + track + ".wav";

            if (CashiersFolder != null && CashiersFolder.Contains(track))
                return path + @"Wav\Number of trains\" + track + ".wav";


            return "";
        }

        #endregion
    }
}