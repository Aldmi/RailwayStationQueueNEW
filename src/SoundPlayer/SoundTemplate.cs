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
  
            //TODO: распарсить Name на перменные шаблоны.  ТалонА001 Касса2
             
            FileNameQueue = new Queue<string>();
        }

        #endregion
    }
}