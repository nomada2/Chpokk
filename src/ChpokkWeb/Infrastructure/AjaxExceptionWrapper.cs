﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Elmah;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuCore.Reflection;

namespace ChpokkWeb.Infrastructure {
	//Adapted from http://www.dovetailsoftware.com/blogs/kmiller/archive/2012/04/24/fubumvc-hijacking-behaviors
	public class AjaxExceptionWrapper : IActionBehavior {
		private readonly IFubuRequest _request;
		private readonly IPartialFactory _factory;
		private readonly IJsonWriter _writer;
		private readonly HttpContext _httpContext;

		public AjaxExceptionWrapper(IFubuRequest request, IPartialFactory factory, IJsonWriter writer,
		                            HttpContext httpContext) {
			_request = request;
			_factory = factory;
			_writer = writer;
			_httpContext = httpContext;
		}

		public IActionBehavior InsideBehavior { get; set; }

		public void Invoke() {
			exceptionHandledBehavior(b => b.Invoke());
		}

		public void InvokePartial() {
			exceptionHandledBehavior(b => b.InvokePartial());
		}

		public void exceptionHandledBehavior(Action<IActionBehavior> behaviorAction) {
			if (InsideBehavior == null) return;

			try {
				behaviorAction(InsideBehavior);
			}
			catch (Exception exception) {
				ErrorLog.GetDefault(_httpContext).Log(new Error(exception));



				var continuation = new AjaxContinuation().ForException(exception);
				_request.Set(continuation);

				_writer.Write(continuation, MimeType.Json.ToString());
				//var partial = _factory.BuildPartial(typeof(AjaxContinuation));
				//partial.InvokePartial();

				//_writer.WriteResponseCode(HttpStatusCode.InternalServerError);
			}
		}
	}

	public class AjaxExceptionWrappingConvention : IConfigurationAction {
		public void Configure(BehaviorGraph graph) {
			graph
				.Actions()
				.Where(action => action.OutputType() == typeof (AjaxContinuation) || action.HasAttribute<JsonEndpointAttribute>())
				.Each(action => action.WrapWith<AjaxExceptionWrapper>());
		}
	}
}