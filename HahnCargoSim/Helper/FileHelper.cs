using System.Globalization;
using Microsoft.IdentityModel.Tokens;

namespace HahnCargoSim.Helper
{
  public class FileHelper
  {
    public static async void WriteToFile(string callerId, string content, string fileName = "")
    {
      if (fileName.IsNullOrEmpty() || fileName == "\"\"")
      {
        fileName = "HahnCargoSim" + callerId + "_" + DateTime.UtcNow.ToString(CultureInfo.InvariantCulture).Replace('/', '_').Replace(':', '_') + ".json";
      }
      await using var streamWriter = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName), false);
      await streamWriter.WriteLineAsync(content);
    }
  }
}
