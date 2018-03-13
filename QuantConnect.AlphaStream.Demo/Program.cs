﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using QuantConnect.AlphaStream.Models;
using QuantConnect.AlphaStream.Requests;

namespace QuantConnect.AlphaStream.Demo
{
    class AlphaStreamsDemonstration
    {
        static void Main(string[] args)
        {
            Title("QuantConnect: Alpha Streams Demo Project v0.1");

            //Initialize:
            //Basic credentials for Demo Client
            var credentials = AlphaCredentials.FromConfiguration();

            //Enable tracing within the SDK
            //Trace.Listeners.Add(new ConsoleTraceListener());
            //AlphaStreamRestClient.RequestTracingEnabled = true;
            //AlphaStreamRestClient.ResponseTracingEnabled = true;

            //Alpha Streams REST Client
            // This is the search and subscription manager
            var client = new AlphaStreamRestClient(credentials);

            //1.Search to find the demo alpha.
            var projectId = 830918;
            Title("1. Alpha Search");
            Log($"1. /alpha/search: Searching alphas matching project id: {projectId}...");
            var alphas = client.Execute(new SearchAlphasRequest { ProjectId = projectId }).Result;
            Log($"1. /alpha/search: Located {alphas.Count}.. ");
            foreach (var a in alphas)
            {
                Log($"1. /alpha/search: Alpha.Id: {a.Id} - Alpha.Project: {a.Project.Name}");
            }
            Pause();


            // 3. Search for information on a specific alpha:
            //Title("2. Alpha Detail View");
            var alphaId = "623b06b231eb1cc1aa3643a46";
            Log("2. /alpha/id: Pulling information for specific Alpha...");
            var alpha = client.Execute(new GetAlphaByIdRequest { Id = alphaId }).Result;
            Log($"2. /alpha/{alphaId}: Specific Alpha.Project.Name: {alpha.Project.Name} Fee: {alpha.SubscriptionFee:C} Exclusive Available: {alpha.ExclusiveAvailable:C} Listed: {alpha.ListedDate:u}");
            Pause();


            // 3. List current insights generated by specific alpha:
            Title("3. Alpha Insights Generated");
            Log("3. /alpha/alpha-id/insights: Pulling information for specific Alpha...");
            var insightAlphaId = "623b06b231eb1cc1aa3643a46";
            var insights = client.Execute(new GetAlphaInsightsRequest { Id = insightAlphaId, Start = 200000 }).Result;
            foreach (var i in insights.Take(5))
            {
                Log($"3. /alpha/{insightAlphaId}/insights: Prediction for { (i.Ticker ?? "").ToUpper().PadRight(8, ' ') }\t going {i.Direction}\t by {i.Magnitude ?? 0:P}\t from {i.Reference ?? 0:C}\t created at {i.Created:u} from {i.Source}\t for {i.Period ?? 0} period of seconds.");
            }
            Pause();


            // 3. Search by author information:
            Title("4. Author Search");
            var language = "C#";
            Log($"4. /author/search: Searching authors who code in: '{language}'");
            var authors = client.Execute(new SearchAuthorsRequest
            {
                Languages = new List<string> { "C#" },
                Projects = new NumberRange<int> { Minimum = 5 }
            }).Result;
            foreach (var b in authors.OrderByDescending(c => c.Projects).Take(5))
            {
                Log($"4. /author/search: Author.Id: {b.Id.Substring(0, 5)} \t Projects: {b.Projects} \t Last Online: {b.LastOnlineTime:u} \t Location: {b.Location}");
            }
            Pause();


            // 5. Detailed information about a specific author:
            Title("5. Author Detail View");
            var authorId = "1f48359f6c6cbad65b091232eaae73ce";
            Log($"5. /author/id: Pulling information for specific author: '{authorId}'");
            var author = client.Execute(new GetAuthorByIdRequest { Id = authorId }).Result;
            Log($"5. /author/id: Specific Author Details:" +
                $"\r\n-> Id: \t\t\t {author.Id} " +
                $"\r\n-> Bio: \t\t {author.Biography.Substring(0, 100)}..." +
                $"\r\n-> Backtests: \t\t {author.Backtests}" +
                $"\r\n-> Projects: \t\t {author.Projects}" +
                $"\r\n-> Language: \t\t {author.Language}" +
                $"\r\n-> Signed Up: \t\t {author.SignupTime}");
            Pause();


            // 6. Streaming Real Time Insights
            Title("6. Live Insights Streaming");
            // Credentials for streaming client
            var streamingCredentials = AlphaInsightsStreamCredentials.FromConfiguration();
            var streamingClient = new AlphaInsightsStreamClient(streamingCredentials);

            //Configure client to handle received insights
            streamingClient.InsightReceived += (sender, e) =>
            {
                Log($"6. AlphaId: {e.AlphaId.Substring(0, 5)} \t InsightId: {e.Insight.Id} " +
                    $"Created: {e.Insight.Created:u} \t " +
                    $"Type: {e.Insight.Type} \t " +
                    $"Ticker: {e.Insight.Ticker.PadRight(8, ' ')} \t " +
                    $"Direction: {e.Insight.Direction}... \t " +
                    (e.Insight.Magnitude == null ? "" : $"Magnitude: {e.Insight.Magnitude:P} \t") +
                    (e.Insight.Confidence == null ? "" : $"Confidence: {e.Insight.Confidence:P}"));
            };

            //Request insights from an alpha stream
            streamingClient.AddAlphaStream(new AddInsightsStreamRequest {AlphaId = alphaId});

            // wait 30 seconds while insights stream in
            Thread.Sleep(30000);

            streamingClient.Dispose();
            Pause();
            Environment.Exit(0);
        }

        /// <summary>
        /// Log this snippet with a time date stamp.
        /// </summary>
        private static void Log(string message, params string[] args)
        {
            Console.WriteLine(DateTime.UtcNow.ToString("u") + " " + message);
        }

        /// <summary>
        /// Print a title
        /// </summary>
        private static void Title(string message)
        {
            Log($"-------------------- {message} --------------------");
        }

        /// <summary>
        /// Pause for a moment for the demo
        /// </summary>
        private static void Pause()
        {
            Log("-------------------- Press Key To Continue -------------------- \r\n\r\n");
            Console.ReadKey();
        }
    }
}
