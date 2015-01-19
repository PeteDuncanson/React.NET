/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.ComponentModel;
using System.Web;
using System.Web.Mvc;

namespace React.Web.Mvc
{
	using AssemblyRegistration = React.AssemblyRegistration;

	/// <summary>
	/// HTML Helpers for utilising React from an ASP.NET MVC application.
	/// </summary>
	public static class HtmlHelperExtensions
	{
		/// <summary>
		/// Gets the React environment
		/// </summary>
		private static IReactEnvironment Environment
		{
			// TODO: Figure out if this can be injected
			get { return AssemblyRegistration.Container.Resolve<IReactEnvironment>(); }
		}

		/// <summary>
		/// Renders the specified React component
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="htmlHelper">HTML helper</param>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to initialise the component with</param>
		/// <param name="htmlTag">HTML tag to wrap the component in. Defaults to &lt;div&gt;</param>
		/// <param name="containerId">ID to use for the container HTML tag. Defaults to an auto-generated ID</param>
		/// <returns>The component's HTML</returns>
		public static IHtmlString React<T>(
			this HtmlHelper htmlHelper, 
			string componentName, 
			T props,
			string htmlTag = null,
			string containerId = null
		)
		{
			var reactComponent = Environment.CreateComponent(componentName, props, containerId);
			if (!string.IsNullOrEmpty(htmlTag))
			{
				reactComponent.ContainerTag = htmlTag;
			}
			var result = reactComponent.RenderHtml();
			return new HtmlString(result);
		}

        /// <summary>
        /// Renders the HTML for correct route for the passed in url
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="props"></param>
        /// <param name="url">The url to match a route for, defaults to current requested path</param>
        /// <param name="htmlTag">The html tag to render as the container for the out putted html, defaults to div</param>
        /// <param name="containerId">The html ID for the container, will auto generate one if not provided</param>
        /// <returns></returns>
   		public static IHtmlString ReactRouter<T>(
			this HtmlHelper htmlHelper, 
			T props,
			string url = null, 
			string htmlTag = null,
			string containerId = null
		)
		{
   		    if (url == null)
   		    {
   		        url = HttpContext.Current.Request.Path;
   		    }

            return new HtmlString(Environment.GetRoutedHtmlForUrl( url, containerId ));
		}


		/// <summary>
		/// Renders the specified React component, along with its client-side initialisation code.
		/// Normally you would use the <see cref="React{T}"/> method, but <see cref="ReactWithInit{T}"/>
		/// is useful when rendering self-contained partial views.
		/// </summary>
		/// <typeparam name="T">Type of the props</typeparam>
		/// <param name="htmlHelper">HTML helper</param>
		/// <param name="componentName">Name of the component</param>
		/// <param name="props">Props to initialise the component with</param>
		/// <param name="htmlTag">HTML tag to wrap the component in. Defaults to &lt;div&gt;</param>
		/// <param name="containerId">ID to use for the container HTML tag. Defaults to an auto-generated ID</param>
		/// <returns>The component's HTML</returns>
		public static IHtmlString ReactWithInit<T>(
			this HtmlHelper htmlHelper,
			string componentName,
			T props,
			string htmlTag = null,
			string containerId = null
		)
		{
			var reactComponent = Environment.CreateComponent(componentName, props, containerId);
			if (!string.IsNullOrEmpty(htmlTag))
			{
				reactComponent.ContainerTag = htmlTag;
			}
			var html = reactComponent.RenderHtml();
			var script = new TagBuilder("script")
			{
				InnerHtml = reactComponent.RenderJavaScript()
			};
			return new HtmlString(html + System.Environment.NewLine + script.ToString());
		}

		/// <summary>
		/// Renders the JavaScript required to initialise all components client-side. This will 
		/// attach event handlers to the server-rendered HTML.
		/// </summary>
		/// <returns>JavaScript for all components</returns>
		public static IHtmlString ReactInitJavaScript(this HtmlHelper htmlHelper)
		{
			var script = Environment.GetInitJavaScript();
			var tag = new TagBuilder("script")
			{
				InnerHtml = script
			};
			return new HtmlString(tag.ToString());
		}
	}
}
