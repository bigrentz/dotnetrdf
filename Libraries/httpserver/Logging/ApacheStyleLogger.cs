/*
dotNetRDF is free and open source software licensed under the MIT License

-----------------------------------------------------------------------------

Copyright (c) 2009-2012 dotNetRDF Project (dotnetrdf-developer@lists.sf.net)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VDS.Web.Logging
{
    /// <summary>
    /// An Apache style Logger
    /// </summary>
    public abstract class ApacheStyleLogger
        : IHttpLogger
    {
        private String _formatString;
        private String[] _logParts;

        /// <summary>
        /// Constant for Date Format used in Common Log Format
        /// </summary>
        public const String ClfDateFormat = "[dd/MMM/yyyy:hh:mm:ss zzz]";
        /// <summary>
        /// Constant for Common Log Format
        /// </summary>
        public const String LogCommon = "%h %l %u %t \"%r\" %>s %b";
        /// <summary>
        /// Constant for Combined Log Format
        /// </summary>
        public const String LogCombined = LogCommon + " \"%{Referer}i\" \"%{User-Agent}i\"";
        /// <summary>
        /// Constant for Logging all available fields
        /// </summary>
        public const String LogAll = "%a %A %B %b %h %H %l %m %p %P %q \"%r\" %s %>s %t %U \"%u\" \"%{Referer}i \"%{User}i\" \"%{Accept}i \"%{Content-Type}o";
        /// <summary>
        /// Constant for User Agent Log
        /// </summary>
        public const String LogUserAgent = "\"%{User-Agent}i\"";

        /// <summary>
        /// Creates a new Apache style Logger
        /// </summary>
        /// <param name="logFormatString">Log Format</param>
        public ApacheStyleLogger(String logFormatString)
        {
            if (logFormatString == null || logFormatString.Equals(String.Empty))
            {
                this._formatString = LogCommon;
            }
            else
            {
                this._formatString = logFormatString;
            }
            if (this._formatString.Contains(' '))
            {
                this._logParts = this._formatString.Split(' ');
            }
            else
            {
                this._logParts = new String[] { this._formatString };
            }
        }

        /// <summary>
        /// Creates a new Apache style logger using the Common Log format
        /// </summary>
        public ApacheStyleLogger()
            : this(LogCommon) { }

        /// <summary>
        /// Helper method to convert string format names into Log Formats
        /// </summary>
        /// <param name="format">Log Format Name</param>
        /// <returns></returns>
        public static String GetLogFormat(String format)
        {
            if (format == null) return ApacheStyleLogger.LogCommon;

            switch (format)
            {
                case "common":
                    return ApacheStyleLogger.LogCommon;
                case "combined":
                    return ApacheStyleLogger.LogCombined;
                case "all":
                    return ApacheStyleLogger.LogAll;
                case "user-agent":
                    return ApacheStyleLogger.LogUserAgent;
                case "":
                    return ApacheStyleLogger.LogCommon;
                default:
                    return format;
            }
        }

        /// <summary>
        /// Logs a request
        /// </summary>
        /// <param name="context">Server Context</param>
        public void LogRequest(HttpServerContext context)
        {
            StringBuilder logLine = new StringBuilder();
            String logPart;
            String logItem;
            for (int i = 0; i < this._logParts.Length; i++)
            {
                logPart = this._logParts[i];
                if (logPart.Contains("%"))
                {
                    int pos = logPart.IndexOf('%');
                    logLine.Append(logPart.Substring(0, pos));
                    logItem = logPart.Substring(pos);
                    if (logItem.Length > 1)
                    {
                        logItem = logItem.Substring(0, 2);
                    }

                    switch (logItem)
                    {
                        case "%%":
                            logLine.Append('%');
                            break;
                        case "%a":
                            logLine.Append(context.Request.RemoteEndPoint.Address.ToLogString());
                            break;
                        case "%A":
                            logLine.Append(context.Request.LocalEndPoint.Address.ToLogString());
                            break;
                        case "%B":
                            logLine.Append(context.Response.ContentLength64.ToString());
                            break;
                        case "%b":
                            if (context.Response.ContentLength64 > 0)
                            {
                                logLine.Append(context.Response.ContentLength64.ToString());
                            }
                            else
                            {
                                logLine.Append('-');
                            }
                            break;
                        case "%h":
                            logLine.Append(context.Request.UserHostName.ToLogString());
                            break;
                        case "%H":
                            logLine.Append("HTTP/");
                            logLine.Append(context.Request.ProtocolVersion.Major + "." + context.Request.ProtocolVersion.Minor);
                            break;
                        case "%l":
                            if (context.User != null)
                            {
                                logLine.Append(context.User.Identity.Name);
                            }
                            else
                            {
                                logLine.Append('-');
                            }
                            break;
                        case "%m":
                            logLine.Append(context.Request.HttpMethod);
                            break;
                        case "%p":
                            logLine.Append(context.Request.RemoteEndPoint.Port.ToString());
                            break;
                        case "%P":
                            logLine.Append(Process.GetCurrentProcess().Id.ToString());
                            break;
                        case "%q":
                            if (!context.Request.Url.Query.Equals(String.Empty))
                            {
                                logLine.Append(context.Request.Url.Query.ToString());
                            }
                            break;
                        case "%r":
                            logLine.Append(context.Request.HttpMethod);
                            logLine.Append(' ');
                            logLine.Append(context.Request.RawUrl);
                            logLine.Append(" HTTP/");
                            logLine.Append(context.Request.ProtocolVersion.Major + "." + context.Request.ProtocolVersion.Minor);
                            break;
                        case "%>":
                            if (logPart.Length > pos + logItem.Length)
                            {
                                logItem += logPart[pos + logItem.Length];
                                if (logItem.Equals("%>s"))
                                {
                                    logLine.Append(context.Response.StatusCode.ToString());
                                }
                                else
                                {
                                    logLine.Append(logItem);
                                }
                            }
                            else
                            {
                                logLine.Append(logItem);
                            }
                            break;
                        case "%s":
                            logLine.Append(context.Response.StatusCode.ToString());
                            break;                           
                        case "%t":
                            logLine.Append(DateTime.Now.ToString(ClfDateFormat));
                            break;
                        case "%u":
                            if (context.User != null)
                            {
                                logLine.Append(context.User.Identity.Name);
                            }
                            else
                            {
                                logLine.Append('-');
                            }
                            break;
                        case "%U":
                            logLine.Append(context.Request.RawUrl);
                            break;

                        case "%{":
                            if (logPart.Length > pos + logItem.Length)
                            {
                                logItem = logPart.Substring(pos, logPart.IndexOf('}', pos) + 1 - pos);
                                if (logPart.Length > pos + logItem.Length)
                                {
                                    char next = logPart[pos + logItem.Length];
                                    String header = logItem.Substring(logItem.IndexOf('{') + 1, logItem.IndexOf('}', logItem.IndexOf('{')) - logItem.IndexOf('{') - 1);
                                    if (next == 'i')
                                    {
                                        logItem = logItem + next;
                                        if (context.Request.Headers[header] != null)
                                        {
                                            logLine.Append(context.Request.Headers[header]);
                                        }
                                        else if (header.Equals("User-Agent"))
                                        {
                                            logLine.Append(context.Request.UserAgent);
                                        }
                                        else
                                        {
                                            logLine.Append('-');
                                        }
                                    }
                                    else if (next == 'o')
                                    {
                                        logItem = logItem + next;
                                        if (context.Response.Headers[header] != null)
                                        {
                                            logLine.Append(context.Response.Headers[header]);
                                        }
                                        else
                                        {
                                            logLine.Append('-');
                                        }
                                    }
                                    else
                                    {
                                        logLine.Append(logItem);
                                    }
                                }
                            }
                            else
                            {
                                logLine.Append(logItem);
                            }
                            break;

                        default:
                            logLine.Append(logItem);
                            break;
                    }

                    if (pos + logItem.Length < logPart.Length)
                    {
                        logLine.Append(logPart.Substring(pos + logItem.Length));
                    }
                }
                else
                {
                    logLine.Append(logPart);
                }
                if (i < this._logParts.Length - 1)
                {
                    logLine.Append(' ');
                }
            }

            this.AppendToLog(logLine.ToString());
        }

        /// <summary>
        /// Appends to the log
        /// </summary>
        /// <param name="line">Line to append</param>
        /// <remarks>
        /// Abstract so that derived implementations can log to whatever target they want
        /// </remarks>
        protected abstract void AppendToLog(String line);

        /// <summary>
        /// Disposes of the logger
        /// </summary>
        public virtual void Dispose()
        {

        }
    }
}
