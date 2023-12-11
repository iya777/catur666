using System.IO;
using UnityEngine;
using UtilityLib;
namespace UtilityLib
{
    public class BookCreator : MonoBehaviour
    {

        public int maxPlyToRecord;

        public int minMovePlayCount = 10;

        public TextAsset gamesFile;
        public TextAsset bookFile;
        public bool append;

        public static Book LoadBookFromFile(TextAsset bookFile)
        {
            Book book = new Book();
            var reader = new StringReader(bookFile.text);

            string line;
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                ulong positionKey = ulong.Parse(line.Split(':')[0]);
                string[] moveInfoStrings = line.Split(':')[1].Trim().Split(',');

                for (int i = 0; i < moveInfoStrings.Length; i++)
                {
                    string moveInfoString = moveInfoStrings[i].Trim();
                    if (!string.IsNullOrEmpty(moveInfoString))
                    {

                        ushort moveValue = ushort.Parse(moveInfoString.Split(' ')[0]);
                        string numTimesPlayedString = moveInfoString.Split(' ')[1].Replace("(", "").Replace(")", "");
                        int numTimesPlayed = int.Parse(numTimesPlayedString);
                        book.Add(positionKey, new Move(moveValue), numTimesPlayed);

                    }
                }
            }
            return book;
        }

    }

}
