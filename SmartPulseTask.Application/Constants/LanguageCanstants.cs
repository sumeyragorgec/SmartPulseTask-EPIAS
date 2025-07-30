namespace SmartPulseTask.Application;

    public static class LanguageConstants
    {

        public const string Success = "general.success";
        public const string Error = "general.error";
        public const string Warning = "general.warning";
        public const string Info = "general.info";
        public const string Loading = "general.loading";
        public const string Processing = "general.processing";
        public const string Completed = "general.completed";
        public const string Failed = "general.failed";


        public static class Epias
        {
            public const string ApiRequest = "epias.api_request";
            public const string ApiResponse = "epias.api_response";
            public const string TransactionHistory = "epias.transaction_history";
            public const string TransactionAnalysis = "epias.transaction_analysis";
            public const string HourlyAnalysis = "epias.hourly_analysis";
            public const string ContractAnalysis = "epias.contract_analysis";
            public const string PriceTrend = "epias.price_trend";
            public const string VolumeAnalysis = "epias.volume_analysis";
            public const string SummaryStats = "epias.summary_stats";
            public const string DataParsing = "epias.data_parsing";
            public const string RequestStarted = "epias.request_started";
            public const string RequestCompleted = "epias.request_completed";
            public const string ResponseReceived = "epias.response_received";
        }


        public static class EpiasAuth
        {
            public const string AuthenticationStarted = "epias.auth.started";
            public const string AuthenticationSuccess = "epias.auth.success";
            public const string AuthenticationFailed = "epias.auth.failed";
            public const string CredentialsNull = "epias.auth.credentials_null";
            public const string TokenParsing = "epias.auth.token_parsing";
            public const string TokenParseError = "epias.auth.token_parse_error";
            public const string TokenExtracted = "epias.auth.token_extracted";
            public const string HtmlResponseEmpty = "epias.auth.html_response_empty";
            public const string TokenNotFound = "epias.auth.token_not_found";
            public const string InvalidCredentials = "epias.auth.invalid_credentials";
            public const string AccessDenied = "epias.auth.access_denied";
            public const string ServiceNotFound = "epias.auth.service_not_found";
            public const string ServerError = "epias.auth.server_error";
            public const string ServiceUnavailable = "epias.auth.service_unavailable";
            public const string ServiceMaintenance = "epias.auth.service_maintenance";
            public const string ServiceTimeout = "epias.auth.service_timeout";
            public const string UnexpectedResponse = "epias.auth.unexpected_response";
        }


        public static class EpiasData
        {
            public const string RequestDebug = "epias.data.request_debug";
            public const string ResponseDebug = "epias.data.response_debug";
            public const string SendingRequest = "epias.data.sending_request";
            public const string JsonParseStarted = "epias.data.json_parse_started";
            public const string JsonParseCompleted = "epias.data.json_parse_completed";
            public const string JsonParseError = "epias.data.json_parse_error";
            public const string ArrayParsingStarted = "epias.data.array_parsing_started";
            public const string ArrayEmpty = "epias.data.array_empty";
            public const string TransactionParsed = "epias.data.transaction_parsed";
            public const string TransactionSkipped = "epias.data.transaction_skipped";
            public const string BatchProcessing = "epias.data.batch_processing";
            public const string ParsingCompleted = "epias.data.parsing_completed";
            public const string UnknownFormat = "epias.data.unknown_format";
            public const string ItemsFound = "epias.data.items_found";
            public const string DataFound = "epias.data.data_found";
            public const string BodyFound = "epias.data.body_found";
            public const string RootArrayFound = "epias.data.root_array_found";
        }


        public static class ChartData
        {
            public const string HourlyAnalysisStarted = "chart.hourly_analysis_started";
            public const string ContractAnalysisStarted = "chart.contract_analysis_started";
            public const string PriceTrendStarted = "chart.price_trend_started";
            public const string VolumeAnalysisStarted = "chart.volume_analysis_started";
            public const string SummaryStatsStarted = "chart.summary_stats_started";
            public const string AnalysisCompleted = "chart.analysis_completed";
            public const string NoDataAvailable = "chart.no_data_available";
            public const string DataGrouping = "chart.data_grouping";
            public const string CalculatingStats = "chart.calculating_stats";
            public const string TotalTransactions = "chart.total_transactions";
            public const string TotalVolume = "chart.total_volume";
            public const string TotalValue = "chart.total_value";
            public const string AveragePrice = "chart.average_price";
            public const string MinPrice = "chart.min_price";
            public const string MaxPrice = "chart.max_price";
            public const string UniqueContracts = "chart.unique_contracts";
            public const string DateRange = "chart.date_range";
        }


        public static class TokenCache
        {
            public const string TokenCached = "cache.token_cached";
            public const string TokenRetrieved = "cache.token_retrieved";
            public const string TokenExpired = "cache.token_expired";
            public const string CacheInvalidated = "cache.invalidated";
            public const string CacheEmpty = "cache.empty";
            public const string TokenNull = "cache.token_null";
            public const string CacheHit = "cache.hit";
            public const string CacheMiss = "cache.miss";
        }


        public static class ContractAnalysis
        {
            public const string AnalysisStarted = "contract.analysis_started";
            public const string AnalysisCompleted = "contract.analysis_completed";
            public const string GroupingTransactions = "contract.grouping_transactions";
            public const string CalculatingMetrics = "contract.calculating_metrics";
            public const string ContractProcessed = "contract.processed";
            public const string InvalidContractName = "contract.invalid_name";
            public const string DateParsed = "contract.date_parsed";
            public const string DateParseError = "contract.date_parse_error";
        }


        public static class JsonAnalysis
        {
            public const string StructureAnalysisStarted = "json.structure_analysis_started";
            public const string PropertyFound = "json.property_found";
            public const string ArrayFound = "json.array_found";
            public const string ObjectFound = "json.object_found";
            public const string ParsingError = "json.parsing_error";
            public const string EmptyResponse = "json.empty_response";
            public const string OriginalFormatDetected = "json.original_format_detected";
            public const string AlternativeFormatDetected = "json.alternative_format_detected";
            public const string FormatNotRecognized = "json.format_not_recognized";
        }


        public static class TransactionAnalysis
        {
            public const string AnalysisStarted = "transaction.analysis_started";
            public const string TransactionsNull = "transaction.transactions_null";
            public const string GroupingByContract = "transaction.grouping_by_contract";
            public const string CalculatingFormulas = "transaction.calculating_formulas";
            public const string WeightedAverageCalculated = "transaction.weighted_average_calculated";
            public const string ResultCreated = "transaction.result_created";
            public const string AnalysisCompleted = "transaction.analysis_completed";
        }


        public static class Errors
        {
            public const string ApiError = "errors.api_error";
            public const string NetworkError = "errors.network_error";
            public const string ValidationError = "errors.validation_error";
            public const string UnexpectedError = "errors.unexpected_error";
            public const string TimeoutError = "errors.timeout_error";
            public const string JsonParseError = "errors.json_parse_error";
            public const string HttpRequestError = "errors.http_request_error";
            public const string BadRequest = "errors.bad_request";
            public const string Unauthorized = "errors.unauthorized";
            public const string Forbidden = "errors.forbidden";
            public const string NotFound = "errors.not_found";
            public const string ServerError = "errors.server_error";
            public const string ServiceUnavailable = "errors.service_unavailable";
            public const string ArgumentNull = "errors.argument_null";
            public const string InvalidOperation = "errors.invalid_operation";
            public const string FormatError = "errors.format_error";
        }


        public static class HttpStatus
        {
            public const string BadRequest = "http.bad_request";
            public const string Unauthorized = "http.unauthorized";
            public const string Forbidden = "http.forbidden";
            public const string NotFound = "http.not_found";
            public const string InternalServerError = "http.internal_server_error";
            public const string BadGateway = "http.bad_gateway";
            public const string ServiceUnavailable = "http.service_unavailable";
            public const string GatewayTimeout = "http.gateway_timeout";
            public const string Success = "http.success";
        }


        public static class Units
        {
            public const string Mwh = "units.mwh";
            public const string TlPerMwh = "units.tl_per_mwh";
            public const string Tl = "units.tl";
            public const string Million = "units.million";
            public const string Thousand = "units.thousand";
            public const string Piece = "units.piece";
            public const string Percent = "units.percent";
            public const string Hours = "units.hours";
            public const string Minutes = "units.minutes";
            public const string Seconds = "units.seconds";
        }


        public static class Validation
        {
            public const string Required = "validation.required";
            public const string InvalidFormat = "validation.invalid_format";
            public const string InvalidTokenFormat = "validation.invalid_token_format";
            public const string TokenTooShort = "validation.token_too_short";
            public const string InvalidContractFormat = "validation.invalid_contract_format";
            public const string InvalidDateFormat = "validation.invalid_date_format";
            public const string InvalidRange = "validation.invalid_range";
            public const string MustBePositive = "validation.must_be_positive";
        }


        public static class Language
        {
            public const string ChangeLanguage = "language.change_language";
            public const string CurrentLanguage = "language.current_language";
            public const string LanguageChanged = "language.language_changed";
            public const string LanguageNotSupported = "language.not_supported";
            public const string AvailableLanguages = "language.available_languages";
        }


        public static class Debug
        {
            public const string RequestHeaders = "debug.request_headers";
            public const string ResponseHeaders = "debug.response_headers";
            public const string ContentLength = "debug.content_length";
            public const string StatusCode = "debug.status_code";
            public const string ProcessingTime = "debug.processing_time";
            public const string MemoryUsage = "debug.memory_usage";
            public const string ItemCount = "debug.item_count";
            public const string BatchSize = "debug.batch_size";
        }
    }