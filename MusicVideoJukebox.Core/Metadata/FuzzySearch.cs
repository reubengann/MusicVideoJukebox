using FuzzySharp;
using FuzzySharp.PreProcess;

namespace MusicVideoJukebox.Core.Metadata
{
    public class FuzzySearch
    {
        public static List<Tuple<string, int>> FuzzySearchStrings(string target, List<string> collection, int threshold)
        {
            var results = new List<Tuple<string, int>>();

            foreach (string item in collection)
            {
                int similarity = Fuzz.WeightedRatio(target, item, PreprocessMode.Full);

                if (similarity >= threshold)
                    results.Add(new Tuple<string, int>(item, similarity));
            }

            return results;
        }
    }
}
