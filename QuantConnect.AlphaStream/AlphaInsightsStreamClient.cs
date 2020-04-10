﻿using QuantConnect.AlphaStream.Requests;

namespace QuantConnect.AlphaStream
{
    /// <summary>
    /// Client used to stream live alpha insights.
    /// </summary>
    /// <remarks>Kept for backwards compatibility <see cref="AlphaStreamClient"/></remarks>
    public class AlphaInsightsStreamClient : AlphaStreamClient, IAlphaInsightsStreamClient
    {
        public AlphaInsightsStreamClient(AlphaStreamCredentials credentials) : base(credentials)
        {
        }

        public bool AddAlphaStream(AddInsightsStreamRequest request)
        {
            return AddAlphaStream(request as AddAlphaStreamRequest);
        }

        public bool RemoveAlphaStream(RemoveInsightsStreamRequest request)
        {
            return RemoveAlphaStream(request as RemoveAlphaStreamRequest);
        }
    }
}