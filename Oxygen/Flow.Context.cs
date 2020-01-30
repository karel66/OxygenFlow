﻿/*
Oxygen Flow library
*/

using System;
using System.Collections.ObjectModel;

using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Oxygen
{
    public partial class Flow
    {
        /// <summary>
        /// Flow context with monadic Bind. 
        /// </summary>
        public struct Context
        {
            /// <summary>
            /// Selenium driver instance
            /// </summary>
            public readonly RemoteWebDriver Driver;

            /// <summary>
            /// Remote web element retrieved by flow step
            /// </summary>
            public readonly RemoteWebElement Element;

            /// <summary>
            /// Collection of elements retrieved by flow step
            /// </summary>
            public readonly ReadOnlyCollection<IWebElement> Collection;

            /// <summary>
            /// Last problem cause
            /// </summary>
            public readonly object ProblemCause;

            public Context(RemoteWebDriver driver, RemoteWebElement element, ReadOnlyCollection<IWebElement> collection, object problemCause = null)
            {
                Driver = driver;
                Element = element;
                Collection = collection;
                ProblemCause = problemCause;
            }

            public bool IsProblem => ProblemCause != null;

            /// <summary>
            /// Indicates that there is an RemoteWebElement instance in the context
            /// </summary>
            public bool HasElement => Element != null;

            /// <summary>
            /// Returns current browser window title.
            /// </summary>
            public string Title => Driver != null ? Driver.Title : null;

            /// <summary>
            /// Returns current context element text.
            /// </summary>
            public string Text => Element != null ? Element.Text : null;

            /// <summary>
            /// Returns current context element value attribute.
            /// </summary>
            public string Value => Element != null ? Element.GetAttribute("value") : null;


            /// <summary>
            /// cretaes new context from the driver instance
            /// </summary>
            /// <param name="driver"></param>
            /// <returns></returns>
            public static Context FromDriver(RemoteWebDriver driver) => new Context(driver, null, null);

            /// <summary>
            /// Set context Element
            /// </summary>
            public Context FromElement(RemoteWebElement element) =>
                new Context(this.Driver, element, this.Collection);

            public Context FromElement(Func<RemoteWebElement> generator)
            {
                if (generator == null)
                {
                    return this.Problem("Missing element generator.");
                }

                try
                {
                    return new Context(this.Driver, generator(), this.Collection);
                }
                catch (Exception x)
                {
                    return this.Problem(x);
                }
            }

            /// <summary>
            /// Set context Collection
            /// </summary>
            public Context FromCollection(ReadOnlyCollection<IWebElement> collection) =>
                new Context(this.Driver, this.Element, collection);

            public Context FromCollection(Func<ReadOnlyCollection<IWebElement>> generator)
            {
                if (generator == null)
                {
                    return this.Problem("Missing collection generator.");
                }

                try
                {
                    return new Context(this.Driver, this.Element, generator());
                }
                catch (Exception x)
                {
                    return this.Problem(x);
                }
            }


            /// <summary>
            /// Set context Problem
            /// </summary>
            public Context Problem(object problemCause)
            {
                System.Diagnostics.Trace.TraceError(problemCause.ToString());

                return new Context(this.Driver, this.Element, this.Collection, problemCause);
            }


            /// <summary>
            /// Monadic bind. 
            /// </summary>
            public Context Bind(FlowStep step)
            {
                if (step == null) return this.Problem("NULL argument in Bind(step).");

                if (IsProblem) return this; // short-circuit problems

                try
                {
                    return step(this);
                }
                catch (Exception x)
                {
                    return this.Problem(x);
                }
            }

            /// <summary>
            /// Action bind.
            /// </summary>
            public Context Use(Action<Context> action)
            {
                if (action == null) return this.Problem("NULL argument in Bind(action).");

                if (IsProblem) return this; // short-circuit problems

                try
                {
                    action(this);
                    return this;
                }
                catch (Exception x)
                {
                    return this.Problem(x);
                }
            }

            public override string ToString() =>
              IsProblem ? ProblemCause.ToString() : Driver != null ? Driver.ToString() : "Uninitialized Context";

            /// <summary>
            /// Overloaded | operator for bind.
            /// </summary>
            public static Context operator |(Context a, FlowStep b) => a.Bind(b);
        }

    }
}