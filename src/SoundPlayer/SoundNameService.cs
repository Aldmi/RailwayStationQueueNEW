using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;


namespace Sound
{
    public class SoundNameService : ISoundNameService
    {
        public static List<string> NumbersFolder = null;
        public static List<string> PhrasesFolder = null;




        #region ctor

        public SoundNameService()
        {
            try
            {
                var dir = new DirectoryInfo(Environment.CurrentDirectory + @"\Wav\Numbers\");
                if (Directory.Exists(dir.FullName))
                {
                    NumbersFolder = new List<string>();
                    foreach (FileInfo file in dir.GetFiles("*.wav"))
                        NumbersFolder.Add(Path.GetFileNameWithoutExtension(file.FullName));
                }

                dir = new DirectoryInfo(Environment.CurrentDirectory + @"\Wav\Phrases\");
                PhrasesFolder = new List<string>();
                foreach (FileInfo file in dir.GetFiles("*.wav"))
                    PhrasesFolder.Add(Path.GetFileNameWithoutExtension(file.FullName));
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

            if (NumbersFolder != null && NumbersFolder.Contains(track))
                return path + @"\Wav\Numbers\" + track + ".wav";

            if (PhrasesFolder != null && PhrasesFolder.Contains(track))
                return path + @"\Wav\Phrases\" + track + ".wav";

            return "";
        }

        #endregion
    }
}