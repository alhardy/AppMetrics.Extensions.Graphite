﻿// <copyright file="MetricSnapshotGraphiteWriter.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using App.Metrics.Serialization;

namespace App.Metrics.Formatters.Graphite
{
    public class MetricSnapshotGraphiteWriter : IMetricSnapshotWriter
    {
        private readonly TextWriter _textWriter;

        private readonly GraphitePayloadBuilder _payloadBuilder;

        public MetricSnapshotGraphiteWriter(
            TextWriter textWriter,
            IGraphiteNameFormatter metricNameFormatter = null,
            Func<string, string> metricTagValueFormatter = null,
            GeneratedMetricNameMapping dataKeys = null)
        {
            _payloadBuilder = new GraphitePayloadBuilder(metricNameFormatter);

            _textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));

            MetricNameMapping = dataKeys ?? new GeneratedMetricNameMapping(
                                    histogram: GraphiteFormatterConstants.GraphiteDefaults.CustomHistogramDataKeys,
                                    meter: GraphiteFormatterConstants.GraphiteDefaults.CustomMeterDataKeys);
        }

        /// <inheritdoc />
        public GeneratedMetricNameMapping MetricNameMapping { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void Write(string context, string name, object value, MetricTags tags, DateTime timestamp)
        {
            _payloadBuilder.Pack(context, name, value, tags);
        }

        /// <inheritdoc />
        public void Write(string context, string name, IEnumerable<string> columns, IEnumerable<object> values, MetricTags tags, DateTime timestamp)
        {
            _payloadBuilder.Pack(context, name, columns, values, tags);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textWriter.Write(_payloadBuilder.PayloadFormatted());
#if !NETSTANDARD1_6
                _textWriter?.Close();
#endif
                _textWriter?.Dispose();
            }
        }
    }
}
