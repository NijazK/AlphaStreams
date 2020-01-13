using QuantConnect.AlphaStream.Infrastructure;
using QuantConnect.AlphaStream.Models;
using RestSharp;
using System.Collections.Generic;

namespace QuantConnect.AlphaStream.Requests
{
    /// <summary>
    /// Fetch Alpha equity curve consisting of both backtest and live performance
    /// </summary>

    [Endpoint(Method.GET, "/alpha/{id}/equity")]

    public class GetAlphaEquityCurveRequest : AttributeRequest<ApiResponse>
    {
        /// <summary>
        /// Unique id hash of an Alpha published to the marketplace.
        /// </summary>
        [PathParameter("id")]
        public string Id { get; set; }

        /// <summary>
        /// Preferred date format
        /// </summary>
        [QueryParameter("date-format")]
        public string DateFormat { get; set; } = "date";

        /// <summary>
        /// Preferred format of returned equity curve
        /// </summary>
        [QueryParameter("format")]
        public string Format { get; set; } = "json";
    }
}