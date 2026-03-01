# Statiq .NET Blog Starter 🚀

A modern, high-performance static site generator theme built primarily for **.NET developers**.

This project demonstrates how to leverage [Statiq](https://statiq.dev/), a powerful static generation framework based on .NET Core, to build a fully functional blog with advanced features like client-side search and dynamic taxonomy.

It serves as the companion source code for the Medium article: **[Finally, a Static Site Generator That Speaks C#: Getting Started with Statiq](https://medium.com/@winsonet/finally-a-static-site-generator-that-speaks-c-getting-started-with-statiq-e07c3ba3c5fd)**.

---

## ✨ Key Features

This starter kit comes pre-configured with essential blogging features that are often difficult to implement in static sites:

### 🔍 Client-Side Full-Text Search
Powered by **Lunr.js**, this theme implements a "serverless" search experience.
- **How it works:** A custom Razor pipeline generates a lightweight `search-index.json` during the build process.
- **Performance:** Search happens entirely in the browser—instant results with zero server latency.
- **Privacy:** No external search services or API keys required.

### 🏷️ Intelligent Tagging System
Organize your content effortlessly.
- Automatically generates individual tag pages (e.g., `/tags/dotnet`).
- Displays tag clouds and related post counts.
- Metadata-driven: Just add `Tags: [C#, Statiq]` to your Markdown front matter.

### 🗂️ Date-Based Archives
Keep your history accessible.
- Automatically groups posts by Year and Month.
- Generates dedicated archive pages for easy navigation through older content.

### 📄 Built-in Pagination
Handles large content libraries gracefully.
- Configurable page size (default: 10 posts per page).
- SEO-friendly pagination links (Next/Previous).

---

## 📖 Detailed Tutorial

I have written a comprehensive guide explaining the architecture behind this project, specifically focusing on how the Search Pipeline works.

👉 **Read the full tutorial on Medium:**
[**How to build a Static Site with Statiq & .NET**](https://medium.com/@winsonet/finally-a-static-site-generator-that-speaks-c-getting-started-with-statiq-e07c3ba3c5fd)

---

## 🚀 Quick Start

To get this project running locally on your machine:

### Prerequisites
- .NET SDK (6.0 or later)

### Installation

1. **Clone the repository**
```bash
git clone [https://github.com/coderblog-winson/statiq_theme](https://github.com/coderblog-winson/statiq_theme)
cd statiq_theme
```

2. **Run the preview server**
This command builds the site and hosts it at `http://localhost:5080` with live reload enabled.

```bash
dotnet run -- preview
``` 

3. **Build for deployment**
To generate the static files in the output folder:

```bash
dotnet run
```