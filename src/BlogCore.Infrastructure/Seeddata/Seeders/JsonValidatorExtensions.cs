using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BlogCore.Infrastructure.Seeddata.Seeders
{
    public static class JsonValidatorExtensions
    {
        public static bool IsValidJson(this string jsonContent)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonContent);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        public static async Task<bool> ValidateJsonFileAsync(this string filePath, ILogger logger)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    logger?.LogWarning("JSON file not found: {FilePath}", filePath);
                    return false;
                }

                var jsonContent = await File.ReadAllTextAsync(filePath);

                if (!jsonContent.IsValidJson())
                {
                    logger?.LogError("Invalid JSON format in file: {FilePath}", filePath);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error validating JSON file: {FilePath}", filePath);
                return false;
            }
        }
    }
}
