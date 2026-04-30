namespace BlogCore.Application.Common.Base
{
    public static class BaseGenerateSlug
    {
        public static string GenerateSlug(string title)
        {
            var slug = title.ToLower().Trim();
            slug = slug.Replace(" ", "-");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\-+", "-");
            return slug.Trim('-');
        }
    }
}
