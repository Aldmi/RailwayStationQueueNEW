using System;
using Library.Convertion;
using System.Collections.Generic;
using System.Linq;

namespace Sound
{
    public class SoundTemplate
    {
        #region prop

        public string Name { get; set; }
        public Queue<string> FileNameQueue { get; set; } = new Queue<string>();

        #endregion





        #region ctor

        public SoundTemplate(string name)
        {
            Name = name;
            var files = Name.Split(' '); // формат: "Талон А 001 Касса 1"
            if (files.Length == 5)
            {
                var numeric2ListStringConverter = new Numeric2ListStringConverter("X");
                foreach (var file in files)
                {
                    int res;
                    if (int.TryParse(file, out res))
                    {
                        var nums = numeric2ListStringConverter.Convert(res.ToString())?.Where(f => f != "0" && f != "0" + "X").ToList();// Where(f => f != "0" && f != "0" + eof)
                        if (nums != null)
                        {
                            foreach (var num in nums)
                            {
                                FileNameQueue.Enqueue(num);
                            }
                        }
                    }
                    else
                    {
                        FileNameQueue.Enqueue(file);
                    }
                }
            }
        }

        #endregion
    }
}