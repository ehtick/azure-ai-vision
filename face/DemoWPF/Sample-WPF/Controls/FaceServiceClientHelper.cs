﻿//
// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Windows;
using System.Windows.Controls;
using Azure;
using Azure.AI.Vision.Face;
using Azure.Core.Pipeline;
using Azure.Core;

namespace Microsoft.ProjectOxford.Face.Controls
{
    internal class SampleUsageTrackingPolicy : HttpPipelineSynchronousPolicy
    {
        public override void OnSendingRequest(HttpMessage message)
        {
            message.Request.Headers.Add("X-MS-AZSDK-Telemetry", "sample=demo-wpf");
        }
    }

    public static class FaceServiceClientHelper
    {
        /// <summary>
        /// Lock, help initializing <see cref="FaceClient"/> instance.
        /// </summary>
        private static readonly object InstanceLock = new object();

        /// <summary>
        /// <see cref="FaceClient"/> instance.
        /// </summary>
        private static FaceClient _instance;

        /// <summary>
        /// <see cref="FaceClient"/> subscription key.
        /// </summary>
        private static string _subscriptionKey;

        /// <summary>
        /// <see cref="FaceClient"/> subscription endpoint.
        /// </summary>
        private static string _subscriptionEndpoint;

        /// <summary>
        /// Gets the instance of the <see cref="FaceClient"/> class.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static FaceClient GetInstance(Page page)
        {
            MainWindow mainWindow = Window.GetWindow(page) as MainWindow;
            string subscriptionKey = mainWindow._scenariosControl.SubscriptionKey;
            string subscriptionEndpoint = mainWindow._scenariosControl.SubscriptionEndpoint;
            if (subscriptionKey == null || subscriptionEndpoint == null)
            {
                throw new ArgumentException("Subscription key or subscription endpoint is null.");
            }

            if (_subscriptionKey != subscriptionKey || _subscriptionEndpoint != subscriptionEndpoint || _instance == null)
            {
                lock (InstanceLock)
                {
                    if (_subscriptionKey != subscriptionKey || _subscriptionEndpoint != subscriptionEndpoint || _instance == null)
                    {
                        var clientOptions = new AzureAIVisionFaceClientOptions();
                        clientOptions.AddPolicy(new SampleUsageTrackingPolicy(), HttpPipelinePosition.PerCall);
                        _instance = new FaceClient(new Uri(subscriptionEndpoint), new AzureKeyCredential(subscriptionKey), clientOptions);

                        _subscriptionKey = subscriptionKey;
                        _subscriptionEndpoint = subscriptionEndpoint;
                    }
                }
            }

            return _instance;
        }
    }
}
