using System;
using System.IO;
using System.Linq;

namespace GoogleHashCode
{
    public class Video
    {
        public long Id;
        public long Size;
    }

    public static class NumberExtension
    {
        public static long ToLong(this string source)
        {
            return long.Parse(source);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            foreach (var source in new[] { "me_at_the_zoo.in", "trending_today.in", "videos_worth_spreading.in", "kittens.in"})
            {
                var db = Database.Build($@"Inputs\{source}");
                var result = db.GetOutput();
                var sw = new StringWriter();
                sw.WriteLine(result.Count(r => r.Value.Any()));
                foreach (var entry in result)
                {
                    if (!entry.Value.Any())
                        continue;

                    sw.Write($"{entry.Key} ");
                    sw.WriteLine($"{string.Join(" ", entry.Value.Distinct().ToArray())}");
                }
                File.WriteAllText($"result_{source}", sw.ToString());
            }
            Console.WriteLine("Done");
        }

        
    }
}
