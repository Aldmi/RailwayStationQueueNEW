using System.Collections.Generic;

namespace Sound
{
    public class SoundTemplate
    {
        #region prop

        public string Name { get; set; }
        public Queue<string> FileNameQueue { get; set; }

        #endregion





        #region ctor

        public SoundTemplate(string name)
        {
            Name = name;
            var files = Name.Split(' '); // формат: "Талон А 001 Касса 1"
            FileNameQueue = new Queue<string>(files);
        }

        #endregion
    }
}