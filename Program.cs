/*
 * Author  : Winson
 * Website : https://www.coderblog.in
 * Medium  : https://medium.com/@winsonet
 * * Copyright (c) 2026 Winson. All rights reserved.
 * Licensed under the MIT License. See LICENSE file in the project root for full license information.
 */

using System.Threading.Tasks;
using Statiq.App;
using Statiq.Web;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;

namespace TablewareStatiq
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await Bootstrapper
              .Factory
              .CreateWeb(args)
              .ConfigureSettings(settings =>
              {
                  if (!settings.ContainsKey("Theme"))
                  {
                      settings["Theme"] = "themes/default";
                  }
              })
              .ConfigureEngine(engine =>
                {
                    // Get the default "Content" pipeline
                    // This is the pipeline where Statiq Web processes all Markdown and Razor files
                    var contentPipeline = engine.Pipelines["Content"];

                    // Add a module in the "PostProcess" phase (after all processing is done, but before writing to disk)
                    contentPipeline.PostProcessModules.Add(
                    new SetDestination(Config.FromDocument(doc =>
                    {
                        // 1. Get the source file name
                        var sourceName = doc.Source.FileName.ToString();

                        // 2. Check: If it is our search index file
                        if (sourceName.Contains("search-index.json"))
                        {
                            // [Critical Fix] Use NormalizedPath instead of FilePath
                            return new NormalizedPath("search-index.json");
                        }

                        // 3. Keep other files as they are
                        return doc.Destination;
                    }))
                    );

                    // 2. [New] Tags generation pipeline
                    // We create a new pipeline named "Tags"
                    engine.Pipelines.Add("Tags", new Pipeline
                    {
                        // Depends on the Content pipeline, ensuring articles have been read
                        Dependencies = { "Content" },

                        ProcessModules = {
                            // A. Read all articles processed by the "Content" pipeline
                            new ReplaceDocuments("Content"),

                            // B. Filter out articles without Tags to prevent errors
                            new FilterDocuments(Config.FromDocument(doc => doc.ContainsKey("Tags"))),

                            // C. Core: Group by the "Tags" field
                            // This turns 100 articles into (for example) 10 documents, where each document represents a Tag
                            new GroupDocuments("Tags"),

                            // D. Load the template we just wrote
                            new MergeContent(new ReadFiles("_TagLayout.cshtml")),

                            // E. Render Razor
                            new RenderRazor()
                                .WithModel(Config.FromDocument((doc, ctx) => doc)),

                            // F. Set output path: /tag/tag-name.html
                            new SetDestination(Config.FromDocument(doc =>
                            {
                                var tagName = doc.GetString(Keys.GroupKey);
                                var slug = tagName.ToLower().Replace(" ", "-");
                                return new NormalizedPath($"tag/{slug}.html");
                            })),

                            // [Critical Fix] G. Write files!
                            // This step is mandatory, otherwise the files only exist in memory
                            new WriteFiles()
                        }
                    });

                    engine.Shortcodes.Add("Figure", (KeyValuePair<string, string>[] args,
                        string content, IDocument doc, IExecutionContext ctx) =>
                    {
                        // 1. Get parameters (src, alt, width)
                        var src = args.FirstOrDefault(x => x.Key == "src").Value;
                        var alt = args.FirstOrDefault(x => x.Key == "alt").Value ?? "";

                        // 2. Get the content inside the tags as the caption
                        var caption = content;

                        // 3. Build HTML (using @"" string interpolation)
                        string html = $@"
                        <figure style=""width: 90%; margin: 2rem auto; text-align: center;"">
                            <img src=""{src}"" alt=""{alt}"" style=""width: 100%; height: auto; border-radius: 5px;"" />
                            <figcaption style=""margin-top: 10px; color: #888; font-style: italic; font-size: 0.9em;"">
                            {caption}
                            </figcaption>
                        </figure>";

                        // 4. Return the result
                        return new ShortcodeResult(html);
                    });
                }).
                ModifyPipeline(nameof(Statiq.Web.Pipelines.Content), pipeline =>
                {
                    // Add custom module to PostModules
                    // This ensures it runs after all standard processing (like Markdown rendering) is complete
                    pipeline.ProcessModules.Add(new TagAutoLinkModule());
                })
                .ConfigureFileSystem((fileSystem, settings) =>
                {
                    var themePath = settings.GetString("Theme");
                    if (!string.IsNullOrEmpty(themePath))
                    {
                        // Create a validated path from the setting
                        var themePathNormalized = new NormalizedPath(themePath);

                        // Ensure the path is absolute. If it's relative, combine it with the root path.
                        if (themePathNormalized.IsRelative)
                        {
                            themePathNormalized = fileSystem.RootPath.Combine(themePathNormalized);
                        }

                        // Check if the theme has an 'input' folder (common convention)
                        // If it does, strictly use that as the input path instead of the theme root
                        var themeInputPath = themePathNormalized.Combine("input");

                        if (fileSystem.GetDirectory(themeInputPath).Exists)
                        {
                            fileSystem.InputPaths.Add(themeInputPath);
                        }
                        else
                        {
                            fileSystem.InputPaths.Add(themePathNormalized);
                        }
                    }
                })
                .RunAsync();
        }
    }
}