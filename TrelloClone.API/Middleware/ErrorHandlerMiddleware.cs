﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TrelloClone.Exceptions;
using TrelloClone.Exceptions.ExceptionFactory;

namespace Application.Middleware
{
	public class ErrorHandlerMiddleware
	{
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                _logger.LogError(">>>>>>>>>>>>>>>" + ex.Message);
                await CreateJSONResponse(context, ex);


            }
        }

        public static async Task CreateJSONResponse(HttpContext context, Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = ExceptionFactory.SetStatusCode(ex);
            var result = JsonSerializer.Serialize(new { message = ex.Message, exceptionName = ex.GetType().Name });
            await response.WriteAsync(result);
        }



    }
}
