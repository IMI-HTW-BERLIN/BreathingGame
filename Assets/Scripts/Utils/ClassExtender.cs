using System.IO;
using System.Threading.Tasks;

namespace Utils
{
    public static class ClassExtender
    {
        /// <summary>
        /// BinaryReader's <see cref="BinaryReader.ReadString"/> method will block the current thread until data is received.
        /// Use this for reading data asynchronously
        /// <example>
        /// A example use-case would be the following
        /// <code>
        /// Task&lt;string&gt; task = reader.ReadStringAsync();
        /// yield return new WaitUntil(() => task.IsCompleted)
        /// string data = task.Result;
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>The task of reading a string from the BinaryReader's stream.</returns>
        public static async Task<string> ReadStringAsync(this BinaryReader reader) => await Task.Run(reader.ReadString);
    }
}