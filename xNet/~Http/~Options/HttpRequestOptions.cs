using System;
using System.Collections.Generic;

namespace xNet
{
    /// <summary>
    /// Represents strongly typed configuration for <see cref="HttpRequest"/> instances.
    /// </summary>
    public sealed class HttpRequestOptions
    {
        /// <summary>
        /// Gets or sets timeout configuration applied to new requests.
        /// </summary>
        public HttpTimeoutOptions Timeouts { get; set; } = new HttpTimeoutOptions();

        /// <summary>
        /// Gets or sets retry configuration applied to new requests.
        /// </summary>
        public HttpRetryOptions Retry { get; set; } = new HttpRetryOptions();

        /// <summary>
        /// Gets or sets default headers appended to every request.
        /// </summary>
        public IDictionary<string, string> DefaultHeaders { get; set; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Creates a deep copy of the current options instance.
        /// </summary>
        /// <returns>The cloned options instance.</returns>
        public HttpRequestOptions Clone()
        {
            return new HttpRequestOptions
            {
                Timeouts = Timeouts?.Clone() ?? new HttpTimeoutOptions(),
                Retry = Retry?.Clone() ?? new HttpRetryOptions(),
                DefaultHeaders = DefaultHeaders != null
                    ? new Dictionary<string, string>(DefaultHeaders, StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            };
        }

        /// <summary>
        /// Creates a default options instance with production ready defaults.
        /// </summary>
        public static HttpRequestOptions CreateDefault()
        {
            return new HttpRequestOptions();
        }
    }

    /// <summary>
    /// Describes timeout behaviour for <see cref="HttpRequest"/>.
    /// </summary>
    public sealed class HttpTimeoutOptions
    {
        /// <summary>
        /// Initializes a new instance with default timeout values.
        /// </summary>
        public HttpTimeoutOptions()
        {
            ConnectTimeout = TimeSpan.FromSeconds(60);
            ReadWriteTimeout = TimeSpan.FromSeconds(60);
            KeepAliveTimeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Gets or sets the connection timeout.
        /// </summary>
        public TimeSpan ConnectTimeout { get; set; }

        /// <summary>
        /// Gets or sets the read/write timeout.
        /// </summary>
        public TimeSpan ReadWriteTimeout { get; set; }

        /// <summary>
        /// Gets or sets the keep-alive timeout.
        /// </summary>
        public TimeSpan KeepAliveTimeout { get; set; }

        /// <summary>
        /// Creates a clone of the current instance.
        /// </summary>
        /// <returns>The cloned timeout options.</returns>
        public HttpTimeoutOptions Clone()
        {
            return new HttpTimeoutOptions
            {
                ConnectTimeout = ConnectTimeout,
                ReadWriteTimeout = ReadWriteTimeout,
                KeepAliveTimeout = KeepAliveTimeout
            };
        }
    }

    /// <summary>
    /// Describes retry strategy for <see cref="HttpRequest"/>.
    /// </summary>
    public sealed class HttpRetryOptions
    {
        private int _maxRetries = 3;

        /// <summary>
        /// Initializes a new instance with default retry settings.
        /// </summary>
        public HttpRetryOptions()
        {
            IsEnabled = true;
            BaseDelay = TimeSpan.FromMilliseconds(100);
        }

        /// <summary>
        /// Gets or sets whether retries are enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of retry attempts.
        /// </summary>
        public int MaxRetries
        {
            get
            {
                return _maxRetries;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("MaxRetries", "Value must be greater than zero.");
                }

                _maxRetries = value;
            }
        }

        /// <summary>
        /// Gets or sets the base delay between retry attempts.
        /// </summary>
        public TimeSpan BaseDelay { get; set; }

        /// <summary>
        /// Gets or sets a custom delay generator.
        /// </summary>
        public Func<int, TimeSpan> DelayGenerator { get; set; }

        /// <summary>
        /// Returns the delay for the provided attempt number.
        /// </summary>
        /// <param name="attempt">Attempt number starting from 1.</param>
        /// <returns>The delay that should be applied.</returns>
        public TimeSpan GetDelay(int attempt)
        {
            if (DelayGenerator != null)
            {
                return DelayGenerator(attempt);
            }

            return BaseDelay;
        }

        /// <summary>
        /// Creates a clone of the retry options.
        /// </summary>
        /// <returns>The cloned retry options.</returns>
        public HttpRetryOptions Clone()
        {
            return new HttpRetryOptions
            {
                IsEnabled = IsEnabled,
                MaxRetries = MaxRetries,
                BaseDelay = BaseDelay,
                DelayGenerator = DelayGenerator
            };
        }
    }
}
