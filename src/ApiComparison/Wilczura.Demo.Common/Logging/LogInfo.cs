using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Wilczura.Demo.Common.Consts;

namespace Wilczura.Demo.Common.Logging;

public class LogInfo : Dictionary<string, object>
{
    public static LogInfo From(string message) => new LogInfo(message, CommonConsts.EventCategoryProcess);

    public const string MessageKey = "Message";
    public string Message
    {
        get
        {
            TryGetValue(MessageKey, out object? message);
            return message != null ? (string)message : string.Empty;
        }
        set
        {
            this[MessageKey] = value;
        }
    }

    public const string AdditionalInfoKey = "AdditionalInfo";
    public string AdditionalInfo
    {
        get
        {
            TryGetValue(AdditionalInfoKey, out object? additionalInfo);
            return additionalInfo != null ? (string)additionalInfo : string.Empty;
        }
        set
        {
            this[AdditionalInfoKey] = value;
        }
    }


    private const string eventDurationKey = "EventDuration";
    /// <summary>
    /// Should be nanoseconds according to ECS spec :)
    /// </summary>
    public long? EventDuration
    {
        get
        {
            TryGetValue(eventDurationKey, out object? duration);
            return duration != null ? (long)duration : null;
        }
        set
        {
            if (value != null)
            {
                this[eventDurationKey] = value;
            }
            else
            {
                Remove(eventDurationKey);
            }
        }
    }

    private const string eventActionKey = "EventAction";
    public string? EventAction
    {
        get
        {
            TryGetValue(eventActionKey, out object? eventAction);
            return eventAction != null ? (string)eventAction : string.Empty;
        }
        set
        {
            if (value != null)
            {
                this[eventActionKey] = value;
            }
            else
            {
                Remove(eventActionKey);
            }
        }
    }

    private const string eventReasonKey = "EventReason";
    public string? EventReason
    {
        get
        {
            TryGetValue(eventReasonKey, out object? eventReason);
            return eventReason != null ? (string)eventReason : string.Empty;
        }
        set
        {
            if (value != null)
            {
                this[eventReasonKey] = value;
            }
            else
            {
                Remove(eventReasonKey);
            }
        }
    }

    private const string eventOutcomeKey = "EventOutcome";
    /// <summary>
    /// https://www.elastic.co/guide/en/ecs/current/ecs-allowed-values-event-outcome.html
    /// </summary>
    public string? EventOutcome
    {
        get
        {
            TryGetValue(eventOutcomeKey, out object? eventOutcom);
            return eventOutcom != null ? (string)eventOutcom : string.Empty;
        }
        set
        {
            if (value != null)
            {
                this[eventOutcomeKey] = value;
            }
            else
            {
                Remove(eventOutcomeKey);
            }
        }
    }

    private const string eventCategoryKey = "EventCategory";
    /// <summary>
    /// https://www.elastic.co/guide/en/ecs/current/ecs-allowed-values-event-category.html
    /// </summary>
    public string? EventCategory
    {
        get
        {
            TryGetValue(eventCategoryKey, out object? eventCategory);
            return eventCategory != null ? (string)eventCategory : string.Empty;
        }
        set
        {
            if (value != null)
            {
                this[eventCategoryKey] = value;
            }
            else
            {
                Remove(eventCategoryKey);
            }
        }
    }

    private const string clientUserNameKey = "ClientUserName";
    public string? UserName
    {
        get
        {
            TryGetValue(clientUserNameKey, out object? clientUserName);
            return clientUserName != null ? (string)clientUserName : string.Empty;
        }
        set
        {
            if (value != null)
            {
                this[clientUserNameKey] = value;
            }
            else
            {
                Remove(clientUserNameKey);
            }
        }
    }

    private const string clientUserIdKey = "ClientUserId";
    public string? UserId
    {
        get
        {
            TryGetValue(clientUserIdKey, out object? clientUserId);
            return clientUserId != null ? (string)clientUserId : string.Empty;
        }
        set
        {
            if (value != null)
            {
                this[clientUserIdKey] = value;
            }
            else
            {
                Remove(clientUserIdKey);
            }
        }
    }

    private const string httpRequestMethodKey = "HttpRequestMethod";
    public string? HttpMethod
    {
        get
        {
            TryGetValue(httpRequestMethodKey, out object? httpRequestMethod);
            return httpRequestMethod != null ? (string)httpRequestMethod : string.Empty;
        }
        set
        {
            if (value != null)
            {
                this[httpRequestMethodKey] = value;
            }
            else
            {
                Remove(httpRequestMethodKey);
            }
        }
    }


    private const string urlPathKey = "UrlPath";
    public string? Endpoint
    {
        get
        {
            TryGetValue(urlPathKey, out object? urlPath);
            return urlPath != null ? (string)urlPath : string.Empty;
        }
        set
        {
            if (value != null)
            {
                this[urlPathKey] = value;
            }
            else
            {
                Remove(urlPathKey);
            }
        }
    }

    private const string traceparentKey = "Traceparent";
    public string Traceparent
    {
        get
        {
            TryGetValue(traceparentKey, out object? traceparent);
            return traceparent != null ? (string)traceparent : string.Empty;
        }
        set
        {
            if (value != null)
            {
                this[traceparentKey] = value;
            }
            else
            {
                Remove(traceparentKey);
            }
        }
    }

    private const string traceIdKey = "TraceId";
    public string TraceId
    {
        get
        {
            TryGetValue(traceIdKey, out object? traceId);
            return traceId != null ? (string)traceId : string.Empty;
        }
        set
        {
            if (value != null)
            {
                this[traceIdKey] = value;
            }
            else
            {
                Remove(traceIdKey);
            }
        }
    }

    private const string spanIdKey = "SpanId";
    public string SpanId
    {
        get
        {
            TryGetValue(spanIdKey, out object? spanId);
            return spanId != null ? (string)spanId : string.Empty;
        }
        set
        {
            if (value != null)
            {
                this[spanIdKey] = value;
            }
            else
            {
                Remove(spanIdKey);
            }
        }
    }

    public string? ExceptionMessage { get; set; }

    public LogInfo(string message, string eventCategory)
    {
        Message = message;
        EventCategory = eventCategory;
    }

    public LogInfo(string message) : this(message, CommonConsts.EventCategoryProcess)
    {
    }

    public LogInfo ApplyPrincipal(IPrincipal? principal)
    {
        if (principal?.Identity != null && principal.Identity.IsAuthenticated)
        {
            UserName = principal.Identity.Name;
            UserId = principal.Identity.Name;
        }
        return this;
    }

    public LogInfo ApplyException(Exception exception)
    {
        ExceptionMessage = exception.Message;
        EventReason = exception.Message;
        EventOutcome = CommonConsts.EventOutcomeFailure;
        return this;
    }

    public LogInfo WithAdditionalInfo(string additionalInfo)
    {
        AdditionalInfo = additionalInfo;
        return this;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
