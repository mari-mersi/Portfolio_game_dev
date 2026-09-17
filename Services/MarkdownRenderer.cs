using Markdig;

namespace Portfolio_game_dev.Services {
    public static class MarkdownRenderer {
        private static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()   // таблицы, сноски, автоссылки и т.д.
            .UseSoftlineBreakAsHardlineBreak()
            .Build();

        public static string ToHtml(string markdown) {
            if (string.IsNullOrWhiteSpace(markdown))
                return string.Empty;

            return Markdown.ToHtml(markdown, _pipeline);
        }
    }
}