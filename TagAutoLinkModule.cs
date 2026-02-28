/*
 * Author  : Winson
 * Website : https://www.coderblog.in
 * Medium  : https://medium.com/@winsonet
 * * Copyright (c) 2026 Winson. All rights reserved.
 * Licensed under the MIT License. See LICENSE file in the project root for full license information.
 */

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Statiq.Common;

public class TagAutoLinkModule : ParallelModule
{
   protected override async Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
    {
        // 1. Retrieve all tags and their corresponding URLs from the entire context
        // Assuming your tag page path format is /tags/tag-name
        var allTags = context.Outputs
        .SelectMany(doc => doc.GetList<string>("Tags") ?? Enumerable.Empty<string>()) // Directly use the string "Tags"
        .Distinct()
        .OrderByDescending(t => t.Length)
        .ToDictionary(
            tag => tag,
            tag => $"/tag/{tag.ToLower().Replace(" ", "-")}"
        );

        string content = await input.GetContentStringAsync();

        // 2. Iterate through all tags to process the current article
        foreach (var tag in allTags)
        {
            // Prevent in-article tags from linking to themselves (optional)
            // string pattern = $@"(?<!<[^>]*){tag}(?![^<]*</a>)";

            // Complex Regex: Match tags, but exclude cases where they are already inside <a> tags or HTML attributes
            string pattern = $@"\b({Regex.Escape(tag.Key)})\b(?![^<]*>)(?![^<]*</a>)";

            content = Regex.Replace(content, pattern, $"<a href=\"{tag.Value}\" class=\"internal-tag-link\">$1</a>", RegexOptions.IgnoreCase);
        }

        return input.Clone(context.GetContentProvider(content, "text/html")).Yield();
    }
}